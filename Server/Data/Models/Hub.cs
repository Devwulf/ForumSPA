using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;
using ForumSPA.Shared.Models;
using ForumSPA.Shared.Utils;

namespace ForumSPA.Server.Data.Models
{
    [Table("Hubs", Schema = "Forum")]
    public class Hub : IValidation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Rules { get; set; }

        [JsonIgnore]
        public List<Thread> Threads { get; set; }

        public bool IsValid()
        {
            if (Name.IsNullOrWhiteSpace())
                return false;

            return true;
        }
    }
}
