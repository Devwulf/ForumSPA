using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSPA.Server.Data.Models
{
    interface IHubInfo
    {
        int HubId { get; set; }
        Hub Hub { get; set; }
    }
}
