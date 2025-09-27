using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillSwapApp.Models
{
    public class SwapRequest
    {
        [Key]
        public int Id { get; set; }

        public string FromUserId { get; set; }
        [ForeignKey("FromUserId")]
        public ApplicationUser FromUser { get; set; }

        public string ToUserId { get; set; }
        [ForeignKey("ToUserId")]
        public ApplicationUser ToUser { get; set; }

        // Add these two properties
        public string OfferedSkill { get; set; }
        public string NeededSkill { get; set; }

        public string Status { get; set; } = "Pending"; // Default: Pending

        // When true, the conversation tied to this request is read-only
        public bool ReadOnly { get; set; } = false;
    }
}