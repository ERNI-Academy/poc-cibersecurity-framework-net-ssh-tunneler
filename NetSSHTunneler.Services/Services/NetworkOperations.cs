using NetSSHTunneler.Services.Models;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;
using NetSSHTunneler.Services.Interfaces;

namespace NetSSHTunneler.Services.Services
{
    public class NetworkOperations : INetworkOperations
    {
        public List<NetWorkPrint> PrintNetMap()
        {
            var ordered = BuildMap();
            List<NetWorkPrint> result = new List<NetWorkPrint>();
            NetWorkPrint register = new NetWorkPrint();
            foreach (Host host in ordered)
            {
                register = PrepareRegisters(host,false);
                result.Add(register);
            }
            return result;
        }

        private NetWorkPrint PrepareRegisters(Host ordered,bool insertParent)
        {
            NetWorkPrint register = new NetWorkPrint();

            register.Name = ordered.HostName;
            register.Type = 1;
            bool hasPorts = false;
            foreach (int port in ordered.Ports)
            {
                hasPorts = true;
                NetWorkPrint ports = new NetWorkPrint();
                ports.Name = port.ToString();
                ports.Parent = ordered.HostName;
                ports.Type = 2;
                register.NetWork.Add(ports);
            }
            if (insertParent && !hasPorts)
                register.Parent = "1";
            else
                register.Parent = "";
            foreach (Network net in ordered.Network)
            {
                NetWorkPrint nets = new NetWorkPrint();
                nets.Name = net.NetworkName;
                nets.Parent = "";
                nets.Type = 3;
                
                foreach (Host host in net.ChildHost)
                {
                    nets.NetWork.Add(PrepareRegisters(host,true));
                }
                register.NetWork.Add(nets);
            }


            return register;
        }

        private List<Host> BuildMap()
        {
            List<Host> netmaps = new List<Host>();
            List<Host> hosts = new List<Host>();
            DirectoryInfo di = new DirectoryInfo(@".\hosts\");
            if (di.Exists)
            {
                foreach (DirectoryInfo folder in di.GetDirectories())
                {
                    var path = Path.Combine(folder.FullName, folder.Name + ".json");
                    if (File.Exists(path))
                    {
                        var json = File.ReadAllText(path);
                        HostInfo hostInfo = JsonSerializer.Deserialize<HostInfo>(json);
                        Host current = new Host();
                        current.HostName = folder.Name.Replace("_", ".");
                        if (hostInfo.Ports==null)
                        {
                            hostInfo.Ports = new List<int>();
                        }
                        current.Ports.AddRange(hostInfo.Ports);
                        current.Parent = hostInfo.Parent;
                        current.NetworkName = hostInfo.Network;
                        hosts.Add(current);
                    }
                }
                foreach (Host host in hosts)
                {
                    if (host.Parent == "")
                    {
                        Host padre = FindChild(hosts, host);
                        netmaps.Add(padre);
                    }
                }

            }

            return netmaps;
        }
        private Host FindChild(List<Host> final, Host padre)
        {
            Host result = new Host();
          
            foreach (Host host in final)
            {
                if (host.Parent == padre.HostName)
                {
                    Host newpadre = FindChild(final, host);
                   
                    if (!padre.Network.Any(s => s.NetworkName == newpadre.NetworkName))
                    {
                        Network net = new Network();
                        net.NetworkName = newpadre.NetworkName;
                        net.ChildHost.Add(newpadre);
                        padre.Network.Add(net);
                    }
                    else
                    {
                        Network net = padre.Network.FirstOrDefault(s => s.NetworkName == newpadre.NetworkName);
                        net.ChildHost.Add(newpadre);
                    }

                }
            }
            return padre;
        }
    }
}
