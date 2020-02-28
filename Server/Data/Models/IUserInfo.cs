using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSPA.Server.Data.Models
{
    public interface IUserInfo
    {
        string UserId { get; set; }
        ApplicationUser User { get; set; }
    }
}
