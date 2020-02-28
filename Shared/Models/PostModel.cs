using ForumSPA.Shared.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ForumSPA.Shared.Models
{
    public class PostModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Post Body")]
        public string Body { get; set; }

        public bool IsMainPost { get; set; } = false;

        [Display(Name = "Thread Id")]
        public int ThreadId { get; set; }

        public string UserId { get; set; }

        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Display(Name = "Created")]
        public DateTime DateCreated { get; set; }
        [Display(Name = "Last Modified")]
        public DateTime DateModified { get; set; }
    }
}
