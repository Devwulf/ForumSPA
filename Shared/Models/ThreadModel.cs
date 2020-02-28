using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ForumSPA.Shared.Utils;

namespace ForumSPA.Shared.Models
{
    public class ThreadModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(300, MinimumLength = 6, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.")]
        [Display(Name = "Title")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Body")]
        public string Body { get; set; }

        [Display(Name = "Hub Id")]
        public int HubId { get; set; }

        public string UserId { get; set; }

        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Display(Name = "Reply Count")]
        public int ReplyCount { get; set; }

        [Display(Name = "Last Modified")]
        public DateTime DateModified { get; set; }
    }
}
