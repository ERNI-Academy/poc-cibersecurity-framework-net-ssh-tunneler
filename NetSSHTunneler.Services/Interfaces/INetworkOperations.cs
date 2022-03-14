using NetSSHTunneler.Services.Models;
using System.Collections.Generic;

namespace NetSSHTunneler.Services.Interfaces
{
    public interface INetworkOperations
    {
        List<NetWorkPrint> PrintNetMap();
    }
}
