using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSPA.Shared.Models.Identity
{
    public class RegisterResult
    {
        public bool Succeeded { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
