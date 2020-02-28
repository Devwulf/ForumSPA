using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSPA.Server.Data.Models
{
    public interface IDateTrackable
    {
        DateTime DateCreated { get; set; }
        DateTime DateModified { get; set; }
    }
}
