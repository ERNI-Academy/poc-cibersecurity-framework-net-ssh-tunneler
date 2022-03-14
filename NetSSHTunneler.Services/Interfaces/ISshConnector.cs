using NetSSHTunneler.Domain.DTOs;
using NetSSHTunneler.Domain.Responses;
using NetSSHTunneler.Services.Models;
using System.Collections.Generic;

namespace NetSSHTunneler.Services.Interfaces
{
    public interface ISshConnector
    {
        ConnectionStatusResponse CheckConnection(SshConnectionDto sshConnection);

        CommandResponse SendCommand(SshConnectionDto sshConnection, CommandContainer command);
        DiscoveryResults ProcessDiscovery(SshConnectionDto sshConnection, List<string> files);
        bool RedirectPort(SshConnectionDto sshConnection, string originHost, uint originPort, uint destinationPort, string destinationHost);
    }
}
