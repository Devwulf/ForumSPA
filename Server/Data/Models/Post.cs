using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ForumSPA.Shared.Models;
using ForumSPA.Shared.Utils;

namespace ForumSPA.Server.Data.Models
{
    [Table("Posts", Schema = "Forum")]
    public class Post : IThreadInfo, IUserInfo, IDateTrackable, IValidation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "ntext")]
        public string Body { get; set; }

        public bool IsMainPost { get; set; } = false;

        [ForeignKey("Thread")]
        public int ThreadId { get; set; }
        [JsonIgnore]
        public Thread Thread { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        [JsonIgnore]
        public ApplicationUser User { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        public bool IsValid()
        {
            if (Body.IsNullOrWhiteSpace())
                return false;

            return true;
        }
    }
}
