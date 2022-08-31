using NetSSHTunneler.Domain.DTOs;
using NetSSHTunneler.Domain.Responses;
using NetSSHTunneler.Services.Interfaces;
using Renci.SshNet;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using NetSSHTunneler.Services.Models;
using System.Text.Json;
using System.Diagnostics;
using System.Threading;

namespace NetSSHTunneler.Services.Services
{
    public class SshConnector : ISshConnector
    {
        public static Dictionary<string, SshClient> SSHConnections = new Dictionary<string, SshClient>();
        public static Dictionary<string, ScpClient> SCPConnections = new Dictionary<string, ScpClient>();
        public static Dictionary<string, SshClient> ForwardedPorts = new Dictionary<string, SshClient>();
        public static Dictionary<string, ShellStream> Consoles = new Dictionary<string, ShellStream>();
        public ConnectionStatusResponse CheckConnection(SshConnectionDto sshConnection)
        {
            SshClient client = null;
            bool result = false;
            int intPort = int.Parse(sshConnection.TargetPort);
            try
            {
                PrivateKeyFile file = null;
                ConnectionInfo ConnectionParams = null;
                if (!SSHConnections.ContainsKey(sshConnection.TargetIp + ":" + intPort))
                {
                    if (sshConnection.Certificate != "")
                    {
                        file = new PrivateKeyFile(@".\certificates\" + sshConnection.Certificate, sshConnection.Password);
                        ConnectionParams = new ConnectionInfo(sshConnection.TargetIp, intPort, sshConnection.UserName, new AuthenticationMethod[] { new PrivateKeyAuthenticationMethod(sshConnection.UserName, file) });
                    }
                    else
                    {
                        ConnectionParams = new ConnectionInfo(sshConnection.TargetIp, intPort, sshConnection.UserName, new AuthenticationMethod[] { new PasswordAuthenticationMethod(sshConnection.UserName, sshConnection.Password) });
                    }
                    if (ConnectionParams != null)
                    {
                        client = new SshClient(ConnectionParams);
                        client.Connect();

                        SSHConnections.Add(sshConnection.TargetIp + ":" + intPort, client);
                    }
                }
                else
                {
                    if (!SSHConnections[sshConnection.TargetIp + ":" + intPort].IsConnected)
                    {
                        SSHConnections[sshConnection.TargetIp + ":" + intPort].Connect();
                    }
                }
            }
            catch (Exception ex)
            {
                return new ConnectionStatusResponse
                {
                    Message = ex.Message,
                    Status = false
                };
            }
            finally
            {
                if (client != null && client.IsConnected)
                {
                    result = client.IsConnected;
                    client.Disconnect();
                }
            }

            return new ConnectionStatusResponse
            {
                Message = "Connection succeded",
                Status = result,
                ConnectionName = sshConnection.TargetIp + ":" + intPort
            };
        }
        public DiscoveryResults ProcessDiscovery(SshConnectionDto sshConnection, List<string> files)
        {
                DiscoveryResults result =new DiscoveryResults();
                foreach (string file in files)
                {
                    string content=File.ReadAllText(file);
                    CommandContainer command = JsonSerializer.Deserialize<CommandContainer>(content);
                    if (string.IsNullOrEmpty(command.CommandConfig.Discovery.NetworkCommand))
                    {
                        foreach (string execution in command.Commands)
                        {
                        CommandContainer newCommand = new CommandContainer();
                        newCommand.Commands.Add(execution);
                        newCommand.CommandConfig.Timeout = command.CommandConfig.Timeout;
                        var response = SendCommand(sshConnection, newCommand);
                            result.commandResponses.Add(response);
                            if (command.CommandConfig.Output.SaveOutput)
                            {
                                File.WriteAllLines(command.CommandConfig.Output.Filename, response.Results);
                            }
                            if (command.CommandConfig.Crack.DoCrack)
                            {
                                Crack(command);
                            }
                        }
                    }
                    else
                    {
                        result.commandResponses.AddRange(discoverNetwork(command, sshConnection).commandResponses);
                     }
                }
            return result;
        }

        private void Crack(CommandContainer command)
        {
            var configuration= GetGlobalConfig();
            string hashtype = "500";
            string attackmode = "3";
            string dictionary = "";
            FileInfo fi = new FileInfo(command.CommandConfig.Crack.HashFile);
            var hashfile = fi.FullName;
            string filetocrack = File.ReadAllText(hashfile);
            if (configuration.cracker == 0)
            {
                attackmode = "--incremental";
                hashtype = "";
                if (!string.IsNullOrEmpty(command.CommandConfig.Crack.Dictionary))
                {
                    attackmode = "";
                    dictionary = "--wordlist="+command.CommandConfig.Crack.Dictionary;
                }
                else
                {
                    attackmode = "--incremental";
                }
                
                string modifiers = "";
                string outputfile = "--pot=" + hashfile;
                LaunchProcess(hashtype, attackmode, dictionary, hashfile, outputfile,modifiers);
            }
            if (configuration.cracker == 1)
            {
                if (filetocrack.Contains("$5$"))
                {
                    hashtype = "-m 7400";
                }
                if (filetocrack.Contains("$6$"))
                {
                    hashtype = "-m 1800";
                }
                if (!string.IsNullOrEmpty(command.CommandConfig.Crack.Dictionary))
                {
                    attackmode = "-a 0";
                    dictionary = command.CommandConfig.Crack.Dictionary;
                }
                hashfile = "-o " + hashfile;
                string modifiers = "-O -d 1";
                LaunchProcess(hashtype, attackmode, dictionary, hashfile, hashfile, modifiers);
            }
        }

        private void LaunchProcess(string hashtype, string attackmode, string dictionary, string hashfile, string outputfile, string modifiers)
        {
            var process = new Process();
            process.StartInfo.FileName = "cmd"; // relative path. absolute path works too.
            process.StartInfo.WorkingDirectory = new FileInfo($"{GetGlobalConfig().crackerPath}").DirectoryName;
            process.StartInfo.Arguments = $"/c start {GetGlobalConfig().crackerPath} {attackmode} {hashtype} {modifiers} {outputfile}_cracked.txt {hashfile} {dictionary}";
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.RedirectStandardOutput = false;
            process.StartInfo.RedirectStandardError = false;
            process.Start();
        }

        private GeneralConfiguration GetGlobalConfig()
        {
            string folderName = @".\configuration";
            string path = Path.Combine(folderName, "globalConfig.json");
            using StreamReader r = new(path);
            string configuration = r.ReadToEnd();
            var result = JsonSerializer.Deserialize<GeneralConfiguration>(configuration);
            return result;

        }

        private DiscoveryResults discoverNetwork(CommandContainer command, SshConnectionDto stream)
        {
            DiscoveryResults result = new DiscoveryResults();
            CommandContainer newCommand = new CommandContainer();
            newCommand.Commands.Add(command.CommandConfig.Discovery.NetworkCommand);
            newCommand.CommandConfig.Timeout = command.CommandConfig.Timeout;
            var response = SendCommand(stream, newCommand);
            result.commandResponses.Add(response);
            foreach (string network in response.Results)
            {
                result.commandResponses.AddRange(HostScan(command, stream,network).commandResponses);
            }
            return result;
        }
       
        private DiscoveryResults HostScan(CommandContainer command, SshConnectionDto stream, string network)
        {
            DiscoveryResults result = new DiscoveryResults();
            if (Regex.IsMatch(network, "([0-9]{1,3}[\\.]){3}[0-9]{1,3}/[0-9]{1,2}") && !network.Contains("127.0.0.1"))
            {
                var net = network.Substring(0, network.LastIndexOf(".") + 1);
                CommandContainer newCommand = new CommandContainer();
                newCommand.Commands.Add(command.CommandConfig.Discovery.HostCommand.Replace("{network}", net));
                newCommand.CommandConfig.Timeout = command.CommandConfig.Timeout;
                var response = SendCommand(stream, newCommand);
                result.commandResponses.Add(response);
                foreach (string host in response.Results)
                {
                    result.commandResponses.AddRange(PortScan(command, stream, host, network).commandResponses);
                }
            }
            return result;
        }

        private DiscoveryResults PortScan(CommandContainer command, SshConnectionDto stream, string host, string network)
        {
            DiscoveryResults result = new DiscoveryResults();
            if (Regex.IsMatch(host, "([0-9]{1,3}[\\.]){3}[0-9]{1,3}") && !network.Contains("127.0.0.1"))
            {
                HostInfo hostFile = new HostInfo();
                hostFile.Network = network;
                hostFile.Parent = stream.TargetIp;
                CommandContainer newCommand = new CommandContainer();
                newCommand.Commands.Add(command.CommandConfig.Discovery.PortCommand.Replace("{host}", host));
                newCommand.CommandConfig.Timeout = command.CommandConfig.Timeout;
                var response = SendCommand(stream, newCommand);
                result.commandResponses.Add(response);
                foreach (string port in response.Results)
                {   
                    if (port.Contains("open") && !port.Contains("/"))
                    {
                        if (hostFile.Ports==null)
                        {
                            hostFile.Ports = new List<int>();
                        }
                        hostFile.Ports.Add(int.Parse(port.Substring(0, port.IndexOf(" "))));
                    }
                }
                GenerateHostConfig(host, hostFile);
            }
            return result;
        }

        private static void GenerateHostConfig(string host, HostInfo hostFile)
        {
            if (!Directory.Exists(@".\hosts"))
            {
                Directory.CreateDirectory(@".\hosts");
            }
            if (!Directory.Exists(@".\hosts\" + host.Replace('.', '_')))
            {
                Directory.CreateDirectory(@".\hosts\" + host.Replace('.', '_'));
                string filename = Path.Combine(@".\hosts\", host.Replace('.', '_'), host.Replace('.', '_') + ".json");
                if (!File.Exists(filename))
                {
                    File.WriteAllText(filename, JsonSerializer.Serialize<HostInfo>(hostFile));
                }
                else
                {
                    var current = File.ReadAllText(filename);
                    HostInfo currentHost = JsonSerializer.Deserialize<HostInfo>(current);
                    foreach (int port in hostFile.Ports)
                    {
                        if (!currentHost.Ports.Contains(port))
                        {
                            currentHost.Ports.Add(port);
                        }
                    }
                }
            }
        }
        public bool checkOrCreateConnection(SshConnectionDto sshConnection)
        {
            SshClient client = null;
            int intPort = int.Parse(sshConnection.TargetPort);
            try
            {
                PrivateKeyFile file = null;
                ConnectionInfo ConnectionParams = null;
                if (!SSHConnections.ContainsKey(sshConnection.TargetIp + ":" + intPort))
                {
                    if (!string.IsNullOrEmpty(sshConnection.Certificate))
                    {
                        file = new PrivateKeyFile(@".\certificates\" + sshConnection.Certificate, sshConnection.Password);
                        ConnectionParams = new ConnectionInfo(sshConnection.TargetIp, intPort, sshConnection.UserName, new AuthenticationMethod[] { new PrivateKeyAuthenticationMethod(sshConnection.UserName, file) });
                    }
                    else
                    {
                        ConnectionParams = new ConnectionInfo(sshConnection.TargetIp, intPort, sshConnection.UserName, new AuthenticationMethod[] { new PasswordAuthenticationMethod(sshConnection.UserName, sshConnection.Password) });
                    }
                    if (ConnectionParams != null)
                    {
                        client = new SshClient(ConnectionParams);
                        client.Connect();
                        SSHConnections.Add(sshConnection.TargetIp + ":" + intPort, client);
                    }
                }
                else
                {
                    if (!SSHConnections[sshConnection.TargetIp + ":" + intPort].IsConnected)
                    {
                        SSHConnections[sshConnection.TargetIp + ":" + intPort].Connect();
                    }
                }
            }
            catch(Exception)
            {
                return false;
            }
            return true;
        }
        public CommandResponse SendCommand(SshConnectionDto sshConnection, CommandContainer command)
        {
            var connected=checkOrCreateConnection(sshConnection);
            if (connected)
            {
                int intPort = int.Parse(sshConnection.TargetPort);

                if (SSHConnections[sshConnection.TargetIp + ":" + intPort] != null && SSHConnections[sshConnection.TargetIp + ":" + intPort].IsConnected)
                {
                    if (!Consoles.ContainsKey(sshConnection.TargetIp + ":" + intPort))
                    {
                        var stream = SSHConnections[sshConnection.TargetIp + ":" + intPort].CreateShellStream("", 80, 24, 800, 600, 1024 * 8);
                        Consoles.Add(sshConnection.TargetIp + ":" + intPort, stream);

                    }
                    try
                    {
                        var basura = Consoles[sshConnection.TargetIp + ":" + intPort].Read();
                        Consoles[sshConnection.TargetIp + ":" + intPort].WriteLine(command.Commands[0].Replace('\n', ' ').Replace("{target}", sshConnection.AttackedIp));
                        
                        var result = Consoles[sshConnection.TargetIp + ":" + intPort].Expect(new Regex(@"\#|\$|\~"), TimeSpan.FromSeconds(command.CommandConfig.Timeout));
                        var oldresult = "";
                        while (oldresult!=result)
                        {
                            oldresult = result;
                            result+= Consoles[sshConnection.TargetIp + ":" + intPort].Read();
                            
                        }
                       // result = Consoles[sshConnection.TargetIp + ":" + intPort].Read();
                        Thread.Sleep(500);
                        if (command.Commands[0].Count(f=>f=='$')>0)
                        {
                            result += Consoles[sshConnection.TargetIp + ":" + intPort].Expect(new Regex(@"\#|\$|\~"), TimeSpan.FromSeconds(command.CommandConfig.Timeout));
                            Consoles[sshConnection.TargetIp + ":" + intPort].Read();
                        }
                        var tempresult = "new";
                        while (!string.IsNullOrEmpty(tempresult))
                        {
                            tempresult = Consoles[sshConnection.TargetIp + ":" + intPort].Read();
                            result += tempresult;
                        }
                        if (result!=null && result.Contains(command.Commands[0].Replace('\n', ' ').Replace("{target}", sshConnection.AttackedIp)))
                        {
                            result=result.Replace(command.Commands[0], "");
                        }
                        if (string.IsNullOrEmpty(result))
                        {
                            result = "Not processed or timeout";
                        }
                       
                        if (result != null)
                        {
                            return new CommandResponse
                            {
                                Results = result.Trim().Split("\r\n").Where(x => x != "$" && !x.Contains("#")).ToList(),
                                Error = this.IsError(result),
                                Path = this.GetPath(sshConnection, intPort),
                                Command = command.Commands[0].Replace('\n', ' ')
                            };
                        }
                        else
                        {
                            return new CommandResponse { Results = new List<string>() { "No response received" }, Error = true, Path = "$" };
                        }
                    }
                    catch (Exception)
                    {
                        if (!SSHConnections[sshConnection.TargetIp + ":" + intPort].IsConnected)
                            SSHConnections[sshConnection.TargetIp + ":" + intPort].Connect();
                        var stream = SSHConnections[sshConnection.TargetIp + ":" + intPort].CreateShellStream("", 80, 24, 800, 600, 1024 * 8);
                        Consoles[sshConnection.TargetIp + ":" + intPort] = stream;
                        return new CommandResponse { Results = new List<string>() { "Not connected-Reconnected try again" }, Error = true, Path = "$",
                            Command = command.Commands[0].Replace('\n', ' ')
                        };
                    }
                }
                else
                {
                    return new CommandResponse { Results = new List<string>() { "Not connected" }, Error = true, Path = "$",
                        Command = command.Commands[0].Replace('\n', ' ')
                    };
                }
            }
            else
            {
                return new CommandResponse{Results = new List<string>() { "Not connected" },Error = true,Path ="X",
                    Command = command.Commands[0].Replace('\n', ' ')
                };
            }
        }

        private string GetPath(SshConnectionDto sshConnection, int intPort)
        {
            Consoles[sshConnection.TargetIp + ":" + intPort].Read();
            Consoles[sshConnection.TargetIp + ":" + intPort].WriteLine("pwd");
            var result = Consoles[sshConnection.TargetIp + ":" + intPort].Expect(new Regex(@"\#|\$"), TimeSpan.FromMilliseconds(10));
            Consoles[sshConnection.TargetIp + ":" + intPort].Read();
            if (result != null)
            {
                var pathArray = result.Trim().Split("\r\n");
                if (pathArray.Length > 2 && pathArray[2].StartsWith("root"))
                {
                    return pathArray[2];
                }
                else if (pathArray.Length > 1)
                {
                    return pathArray[1] + "$";
                }
            }
            else { return "$"; }
            return "$";
        }
        private bool IsError(string message)
        {
            return message.Contains(" not found\r\n");
        }
        public bool RedirectPort(SshConnectionDto sshConnection, string originHost, uint originPort, uint destinationPort,string destinationHost)
        {
            var connected = checkOrCreateConnection(sshConnection);
            int intPort = int.Parse(sshConnection.TargetPort);
            if (connected)
            {
                if (!ForwardedPorts.ContainsKey(originHost + ":" + originPort.ToString() + "=>" + destinationHost + ":" + destinationPort.ToString()))
                {
                    var fwport = new ForwardedPortLocal(destinationHost, destinationPort, originHost, originPort);
                    SSHConnections[sshConnection.TargetIp + ":" + intPort].AddForwardedPort(fwport);
                    fwport.Start();
                    ForwardedPorts.Add(originHost + ":" + originPort.ToString() + "=>" + destinationHost + ":" + destinationPort.ToString(), SSHConnections[sshConnection.TargetIp + ":" + intPort]);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
