using NetSSHTunneler.Domain.Responses;
using NetSSHTunneler.Services.Interfaces;
using NetSSHTunneler.Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace NetSSHTunneler.Services.Services
{
    public class FileOperations : IFileOperations
    {
        public void DeleteFile(string fileName)
        {
            try
            {
                string folderName = @".\configuration\";
                string path = Path.Combine(folderName, fileName);
                File.Delete(path);
            }
            catch (Exception)
            {
                // Manage exception
            }
        }

        public string ReadFile(string fileName, string folder)
        {
            try
            {
                string folderName = folder;
                string path = Path.Combine(folderName, fileName);
                string result;
                using StreamReader r = new(path);
                result = r.ReadToEnd();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<DiscoveryConfiguration> GetDiscoveryConfigFiles()
        {
            string folderName = @".\configuration";
            string path = Path.Combine(folderName, "discovery");
            DirectoryInfo di = new DirectoryInfo(path);
            List<DiscoveryConfiguration> result = new List<DiscoveryConfiguration>();
            foreach (FileInfo file in di.GetFiles().Where(s=>s.Extension==".json"))
            {
                File.ReadAllText(file.FullName);
                string configuration = File.ReadAllText(file.FullName);
                var content = JsonSerializer.Deserialize<DiscoveryConfiguration>(configuration);
                result.Add(content);
            }
            return result;
            

        }
        public DiscoveryConfiguration GetDiscoveryConfig(string configfile)
        {
            using StreamReader r = new(configfile);
            string configuration = r.ReadToEnd();
            var result = JsonSerializer.Deserialize<DiscoveryConfiguration>(configuration);
            return result;

        }
        public bool SaveDiscoveryConfig(DiscoveryConfiguration DiscoveryConfiguration)
        {
            string folderName = @".\configuration";
            string path = Path.Combine(folderName, "discovery", DiscoveryConfiguration.configurationName + ".json");
            foreach (FileItem item in DiscoveryConfiguration.files)
            {
                item.path = ".\\" + item.path.Substring(item.path.IndexOf("discovery"));
            }
            var content = JsonSerializer.Serialize<DiscoveryConfiguration>(DiscoveryConfiguration);
            File.WriteAllText(path, content, Encoding.ASCII);
            return true;
        }
        public GeneralConfiguration GetGlobalConfig()
        {
            string folderName = @".\configuration";
            string path = Path.Combine(folderName, "globalConfig.json");
            using StreamReader r = new(path);
            string configuration = r.ReadToEnd();
            var result=JsonSerializer.Deserialize<GeneralConfiguration>(configuration);
            return result;
            
        }
        public bool SaveGlobalConfig(GeneralConfiguration configuration)
        {
            string folderName = @".\configuration";
            string path = Path.Combine(folderName, "globalConfig.json");
            using FileStream fs = File.Create(path);
            var content = JsonSerializer.Serialize<GeneralConfiguration>(configuration);
            fs.Write(Encoding.ASCII.GetBytes(content), 0, Encoding.ASCII.GetBytes(content).Count());
            return true;
        }
        public bool SaveConfigFile(HostInfo config)
        {
            string folderName = config.conectionInfo.TargetIp.Replace(".", "_");
            string path = Path.Combine("hosts", folderName);
            if (!(new DirectoryInfo(path).Exists))
            {
                new DirectoryInfo(path).Create();
            }
            path = Path.Combine(path, folderName + ".json"); 
            using FileStream fs = File.Create(path);
            var content = JsonSerializer.Serialize<HostInfo>(config);
            fs.Write(Encoding.ASCII.GetBytes(content), 0, Encoding.ASCII.GetBytes(content).Count());
            return true;
        }
        public string FindAndReadConfigFile(string host)
        {
            try
            {
                string folderName = host.Replace(".","_");
                string path = Path.Combine("hosts",folderName, host.Replace(".", "_")+".json");
                string result;
                using StreamReader r = new(path);
                result = r.ReadToEnd();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void WriteFile(string filename, string folderName, string content)
        {
            Directory.CreateDirectory(folderName);
            string fileName = filename;
            string path = Path.Combine(folderName, fileName);
            Console.WriteLine("Path to my file: {0}\n", path);
            using FileStream fs = File.Create(path);
            fs.Write(Encoding.ASCII.GetBytes(content), 0, Encoding.ASCII.GetBytes(content).Count());
        }

        public bool SaveDiscoveryFile(string path, string content, string name)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    path = Path.Combine(@".\discovery\", name);
                }
                else
                {
                    path = Path.Combine("."+path.Substring(path.IndexOf("\\discovery")));
                }
                using FileStream fs = File.Create(path);
                fs.Write(Encoding.ASCII.GetBytes(content), 0, Encoding.ASCII.GetBytes(content).Count());
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public List<DiscoveryScriptFolder> GetDiscoveryScripts()
        {
            try
            {
                var result = new List<DiscoveryScriptFolder>();
                string folderName = @".\discovery\";
                DirectoryInfo directory = new DirectoryInfo(folderName);
                directory.GetDirectories().ToList().ForEach(d =>
                {
                    var folder = new DiscoveryScriptFolder
                    {
                        Name = d.Name,
                        Scripts = new List<DiscoveryScriptFile>()
                    };
                    DirectoryInfo scriptFolder = new DirectoryInfo(d.FullName);
                    scriptFolder.GetFiles().ToList().ForEach(file =>
                    {
                        var scriptFile = new DiscoveryScriptFile
                        {
                            Name = file.Name,
                            Path = file.FullName,
                            Content = JsonSerializer.Deserialize<CommandContainer>(this.ReadFile(file.Name, Path.Combine(folderName, folder.Name)))
                        };
                        folder.Scripts.Add(scriptFile);
                    });
                    result.Add(folder);
                });
                directory.GetFiles().ToList().ForEach(file =>
                {
                    var folder = new DiscoveryScriptFolder
                    {
                        Name = directory.Name,
                        Scripts = new List<DiscoveryScriptFile>()
                    };
                    var scriptFile = new DiscoveryScriptFile
                    {
                        Name = file.Name,
                        Path = file.FullName,
                        Content = JsonSerializer.Deserialize<CommandContainer>(this.ReadFile(file.Name, folderName))
                    };
                    folder.Scripts.Add(scriptFile);
                    result.Add(folder);
                });
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
