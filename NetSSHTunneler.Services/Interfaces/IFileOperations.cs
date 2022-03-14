using NetSSHTunneler.Domain.Responses;
using NetSSHTunneler.Services.Models;
using System.Collections.Generic;

namespace NetSSHTunneler.Services.Interfaces
{
    public interface IFileOperations
    {
        void WriteFile(string filename, string folderName, string content);

        string ReadFile(string fileName, string folder);
        string FindAndReadConfigFile(string host);
        bool SaveConfigFile(HostInfo HostInfo);
        bool SaveGlobalConfig(GeneralConfiguration configuration);
        GeneralConfiguration GetGlobalConfig();
        void DeleteFile(string fileName);

        bool SaveDiscoveryFile(string path, string content, string name);

        List<DiscoveryScriptFolder> GetDiscoveryScripts();
        List<DiscoveryConfiguration> GetDiscoveryConfigFiles();

        DiscoveryConfiguration GetDiscoveryConfig(string configfile);

        bool SaveDiscoveryConfig(DiscoveryConfiguration DiscoveryConfiguration);
    }
}
