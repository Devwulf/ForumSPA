using ForumSPA.Shared.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ForumSPA.Shared.Models
{
    public class HubModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Hub Title")]
        public string Name { get; set; }

        [Display(Name = "Hub Description")]
        public string Description { get; set; }

        [Display(Name = "Hub Rules")]
        public string Rules { get; set; }

        [Display(Name = "Thread Count")]
        public int ThreadCount { get; set; }
    }
}
