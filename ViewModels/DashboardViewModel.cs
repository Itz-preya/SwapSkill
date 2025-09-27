using SkillSwapApp.Models;
using System.Collections.Generic;

namespace SkillSwapApp.ViewModels
{
    public class DashboardViewModel
    {
        public ApplicationUser User { get; set; }
        public IEnumerable<Skill> Skills { get; set; }


        public IEnumerable<ApplicationUser> OtherUsers { get; set; }
        // New properties
        public IEnumerable<SwapRequest> IncomingRequests { get; set; }
        public IEnumerable<SwapRequest> SentRequests { get; set; }
        public HashSet<string> BlockedUserIds { get; set; }
    }
}