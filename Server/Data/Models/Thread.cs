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
    [Table("Threads", Schema = "Forum")]
    public class Thread : IUserInfo, IHubInfo, IDateTrackable, IValidation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(300, MinimumLength = 6, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.")]
        public string Name { get; set; }

        [ForeignKey("Hub")]
        public int HubId { get; set; }
        [JsonIgnore]
        public Hub Hub { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        [JsonIgnore]
        public ApplicationUser User { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        [JsonIgnore]
        public List<Post> Posts { get; set; }

        public bool IsValid()
        {
            if (Name.IsNullOrWhiteSpace())
                return false;

            return true;
        }
    }
}
