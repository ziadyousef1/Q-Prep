using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Model
{
    public class SaveQuestions
    {
        [Key]
        public string Id { get; set; }
        [ForeignKey("UserId")]
        public string Question { get; set; }

        public string? Answer { get; set; }
        public string UserId { get; set; }
        [JsonIgnore]
        public AppUser? User { get; set; }
        


    }
}
