using System;

namespace SkillSwapApp.ViewModels
{
    public class ChatListItemViewModel
    {
        public string PartnerId { get; set; }
        public string PartnerName { get; set; }
        public string LastMessage { get; set; }
        public DateTime? LastMessageAt { get; set; }
    }
}
