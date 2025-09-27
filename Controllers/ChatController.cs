using Microsoft.AspNetCore.Mvc;
using SkillSwapApp.Data;
using SkillSwapApp.Models;
using System.Linq;
using System.Security.Claims;

namespace SkillSwapApp.Controllers
{
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChatController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Individual chat page
        public IActionResult Chat(string userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var messages = _context.Messages
                .Where(m => (m.FromUserId == currentUserId && m.ToUserId == userId) ||
                            (m.FromUserId == userId && m.ToUserId == currentUserId))
                .OrderBy(m => m.SentAt)
                .ToList();

            ViewBag.CurrentUserId = currentUserId;
            ViewBag.OtherUserId = userId;

            // Try to find an accepted swap request between these two users to determine read-only state
            var req = _context.SwapRequests
                .Where(r => (r.FromUserId == currentUserId && r.ToUserId == userId) ||
                            (r.FromUserId == userId && r.ToUserId == currentUserId))
                .OrderByDescending(r => r.Id)
                .FirstOrDefault(r => r.Status == "Accepted");

            ViewBag.RequestId = req?.Id ?? 0;
            ViewBag.IsReadOnly = req?.ReadOnly ?? false;

            return View(messages); // uses your Chat.cshtml
        }
    }
}
