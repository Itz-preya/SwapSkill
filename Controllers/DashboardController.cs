using Microsoft.AspNetCore.Mvc;
using SkillSwapApp.Data;
using SkillSwapApp.Models;
using SkillSwapApp.Repositories;
using SkillSwapApp.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace SkillSwapApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISkillRepository _skillRepo;

        public DashboardController(ApplicationDbContext context, ISkillRepository skillRepo)
        {
            _context = context;
            _skillRepo = skillRepo;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            // Get all skills for all users
            var allSkills = _context.Skills.ToList();
            var skills = allSkills.Where(s => s.UserId == userId).ToList();
            var otherUsers = _context.Users.Where(u => u.Id != userId).ToList();

            // Prioritize users whose NeededSkill matches the current user's OfferedSkill
            var offeredSkill = user?.OfferedSkill?.Trim().ToLower();
            if (!string.IsNullOrWhiteSpace(offeredSkill))
            {
                otherUsers = otherUsers
                    .OrderByDescending(u => (u.NeededSkill ?? string.Empty).Trim().ToLower() == offeredSkill)
                    .ThenBy(u => u.FullName)
                    .ToList();
            }

            var incomingRequests = _context.SwapRequests
                .Where(r => r.ToUserId == userId && r.Status == "Pending")
                .ToList();

            var sentRequests = _context.SwapRequests
                .Where(r => r.FromUserId == userId)
                .ToList();

            // Build set of users that already have a pending/accepted request with current user
            var blockedIds = _context.SwapRequests
                .Where(r => (r.FromUserId == userId || r.ToUserId == userId)
                            && (r.Status == "Pending" || r.Status == "Accepted"))
                .Select(r => r.FromUserId == userId ? r.ToUserId : r.FromUserId)
                .ToHashSet();

            var vm = new DashboardViewModel
            {
                User = user,
                Skills = allSkills, // Pass all skills for all users
                OtherUsers = otherUsers,
                IncomingRequests = incomingRequests,
                SentRequests = sentRequests,
                BlockedUserIds = blockedIds
            };

            return View(vm);
        }

        // Send a swap request
        [HttpGet]
        public IActionResult RequestSwap(string userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var fromUser = _context.Users.FirstOrDefault(u => u.Id == currentUserId);
            var toUser = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (fromUser == null || toUser == null)
                return RedirectToAction("Index");

            // Guard: prevent duplicate requests between the same two users
            var existing = _context.SwapRequests
                .FirstOrDefault(r =>
                    (
                        (r.FromUserId == fromUser.Id && r.ToUserId == toUser.Id) ||
                        (r.FromUserId == toUser.Id && r.ToUserId == fromUser.Id)
                    )
                    && (r.Status == "Pending" || r.Status == "Accepted")
                );

            if (existing != null)
            {
                TempData["InfoMessage"] = "A swap request already exists between you and this user.";
                return RedirectToAction("Index");
            }

            var request = new SwapRequest
            {
                FromUserId = fromUser.Id,
                ToUserId = toUser.Id,
                OfferedSkill = fromUser.OfferedSkill,
                NeededSkill = fromUser.NeededSkill,
                Status = "Pending"
            };

            _context.SwapRequests.Add(request);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Edit profile
        // Inside AccountController
        [HttpGet]
        public IActionResult Edit()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return NotFound();

            return View(user); // looks for Views/Dashboard/Edit.cshtml
        }


        [HttpPost]
        public IActionResult Edit(ApplicationUser model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return NotFound();

            // Capture old skills before updating
            var oldOffered = user.OfferedSkill;
            var oldNeeded = user.NeededSkill;

            user.FullName = model.FullName;
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.OfferedSkill = model.OfferedSkill;
            user.NeededSkill = model.NeededSkill;

            _context.SaveChanges();

            // If skills changed, mark related accepted chats as read-only
            bool offeredChanged = !string.Equals(oldOffered?.Trim(), model.OfferedSkill?.Trim(), System.StringComparison.OrdinalIgnoreCase);
            bool neededChanged  = !string.Equals(oldNeeded?.Trim(),  model.NeededSkill?.Trim(),  System.StringComparison.OrdinalIgnoreCase);

            if (offeredChanged || neededChanged)
            {
                var toLock = _context.SwapRequests
                    .Where(r => (r.FromUserId == userId || r.ToUserId == userId)
                                && r.Status == "Accepted"
                                && !r.ReadOnly
                                && ((offeredChanged && r.OfferedSkill == oldOffered) || (neededChanged && r.NeededSkill == oldNeeded)))
                    .ToList();

                foreach (var req in toLock)
                {
                    req.ReadOnly = true;
                }

                if (toLock.Count > 0)
                {
                    _context.SaveChanges();
                }
            }

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("Edit");
        }

        // Accept request
        [HttpGet]
        public IActionResult AcceptRequest(int requestId)
        {
            var request = _context.SwapRequests.FirstOrDefault(r => r.Id == requestId);
            if (request != null)
            {
                request.Status = "Accepted";
                _context.SaveChanges();

                return RedirectToAction("Chat", new { requestId = request.Id });
            }

            return RedirectToAction("Index");
        }

        // Decline request
        [HttpGet]
        public IActionResult DeclineRequest(int requestId)
        {
            var request = _context.SwapRequests.FirstOrDefault(r => r.Id == requestId);
            if (request != null)
            {
                request.Status = "Declined";
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public IActionResult ChatList()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Distinct partners from accepted swap requests
            var partners = _context.SwapRequests
                .Where(r => (r.FromUserId == currentUserId || r.ToUserId == currentUserId)
                            && r.Status == "Accepted")
                .Select(r => r.FromUserId == currentUserId ? r.ToUser : r.FromUser)
                .ToList()
                .GroupBy(u => u.Id)
                .Select(g => g.First())
                .ToList();

            // Build view model items with last message preview
            var items = partners.Select(p =>
            {
                var lastMsg = _context.Messages
                    .Where(m => (m.FromUserId == currentUserId && m.ToUserId == p.Id) ||
                                (m.FromUserId == p.Id && m.ToUserId == currentUserId))
                    .OrderByDescending(m => m.SentAt)
                    .FirstOrDefault();

                return new ViewModels.ChatListItemViewModel
                {
                    PartnerId = p.Id,
                    PartnerName = p.FullName,
                    LastMessage = lastMsg?.Content,
                    LastMessageAt = lastMsg?.SentAt
                };
            })
            .OrderByDescending(i => i.LastMessageAt ?? System.DateTime.MinValue)
            .ToList();

            return View(items);
        }

        public IActionResult Chat(int requestId)
        {
            var request = _context.SwapRequests.FirstOrDefault(r => r.Id == requestId);
            if (request == null || request.Status != "Accepted")
                return RedirectToAction("Index");

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var otherUserId = request.FromUserId == currentUserId ? request.ToUserId : request.FromUserId;

            var messages = _context.Messages
                .Where(m => (m.FromUserId == currentUserId && m.ToUserId == otherUserId) ||
                            (m.FromUserId == otherUserId && m.ToUserId == currentUserId))
                .OrderBy(m => m.SentAt)
                .ToList();

            ViewBag.CurrentUserId = currentUserId;
            ViewBag.OtherUserId = otherUserId;
            ViewBag.RequestId = request.Id;
            ViewBag.IsReadOnly = request.ReadOnly;

            return View("~/Views/Chat/Chat.cshtml", messages);
        }

        

        // Home page for logged-in users
        [HttpGet]
        public IActionResult Home()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            return View(); // Views/Dashboard/Home.cshtml
        }

        // TEMP: Development-only cleanup to remove all swap requests
        // Call: GET /Dashboard/WipeSwapRequests
        // Remove this method after use.
        [HttpGet]
        public IActionResult WipeSwapRequests()
        {
            var all = _context.SwapRequests.ToList();
            if (all.Any())
            {
                _context.SwapRequests.RemoveRange(all);
                _context.SaveChanges();
            }

            TempData["SuccessMessage"] = "All swap requests have been deleted.";
            return RedirectToAction("Index");
        }

        // TEMP: Development-only cleanup to remove all chat messages
        // Call: GET /Dashboard/WipeMessages
        // Remove this method after use.
        [HttpGet]
        public IActionResult WipeMessages()
        {
            var allMsgs = _context.Messages.ToList();
            if (allMsgs.Any())
            {
                _context.Messages.RemoveRange(allMsgs);
                _context.SaveChanges();
            }

            TempData["SuccessMessage"] = "All chat messages have been deleted.";
            return RedirectToAction("Index");
        }

        // TEMP: Development-only cleanup to remove all chats and swap requests together
        // Call: GET /Dashboard/WipeAllChats
        // Remove this method after use.
        [HttpGet]
        public IActionResult WipeAllChats()
        {
            var allMsgs = _context.Messages.ToList();
            if (allMsgs.Any())
            {
                _context.Messages.RemoveRange(allMsgs);
            }

            var allReqs = _context.SwapRequests.ToList();
            if (allReqs.Any())
            {
                _context.SwapRequests.RemoveRange(allReqs);
            }

            _context.SaveChanges();
            TempData["SuccessMessage"] = "All chats and swap requests have been deleted.";
            return RedirectToAction("Index");
        }
    }
}
