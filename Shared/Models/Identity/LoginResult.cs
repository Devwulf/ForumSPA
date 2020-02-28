using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSPA.Shared.Models.Identity
{
    public class LoginResult
    {
        public bool Succeeded { get; set; }
        public string Error { get; set; }
        public string Token { get; set; }
    }
}
