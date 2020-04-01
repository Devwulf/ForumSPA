using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ForumSPA.Shared.Models
{
    public class ThreadEditModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(300, MinimumLength = 6, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.")]
        [Display(Name = "Title")]
        public string Name { get; set; }
    }
}
