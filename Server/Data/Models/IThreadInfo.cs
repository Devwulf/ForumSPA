using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSPA.Server.Data.Models
{
    public interface IThreadInfo
    {
        int ThreadId { get; set; }
        Thread Thread { get; set; }
    }
}
