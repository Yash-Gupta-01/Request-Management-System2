using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using RMS.Data;
using RMS.Models;
using Microsoft.Extensions.Logging;

namespace RMS.Controllers
{
    public class RequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RequestController> _logger;

        public RequestController(ApplicationDbContext context, ILogger<RequestController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /Request/GuestHome
        public IActionResult GuestHome()
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(username) || role != "Guest")
            {
                return RedirectToAction("Login", "User");
            }
            return View();
        }

        // GET: /Request/Create
        [HttpGet]
        public IActionResult Create()
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(username) || role != "Guest")
            {
                return RedirectToAction("Login", "User");
            }
            ViewBag.RequestTypes = new[] { "Admin", "IT", "HR", "Miscellaneous" };
            return View();
        }

        // POST: /Request/Create
        [HttpPost]
        public async Task<IActionResult> Create(string requestType, string subject, string details)
        {
            _logger.LogInformation($"Create Request - Type: {requestType}, Subject: {subject}, Details: {details}");

            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");
            
            if (string.IsNullOrEmpty(username) || role != "Guest")
            {
                _logger.LogWarning($"Invalid access attempt - Username: {username}, Role: {role}");
                return RedirectToAction("Login", "User");
            }
            
            if (string.IsNullOrWhiteSpace(requestType) || string.IsNullOrWhiteSpace(subject))
            {
                _logger.LogWarning($"Validation failed - Type: {requestType}, Subject: {subject}");
                ViewBag.Error = "Request type and subject are required.";
                ViewBag.RequestTypes = new[] { "Admin", "IT", "HR", "Miscellaneous" };
                return View();
            }

            var latestRequest = await _context.Requests.OrderByDescending(r => r.RequestId).FirstOrDefaultAsync();
            var nextNumber = latestRequest != null ? (int.Parse(latestRequest.RequestNumber[3..]) + 1) : 1;
            var requestNumber = $"REQ{nextNumber:D6}";

            var request = new RequestDetails
            {
                RequestNumber = requestNumber,
                RequestType = requestType,
                Subject = subject,
                Details = details ?? "",
                Username = username,
                DateOfIssue = DateTime.UtcNow,
                CurrentStatus = "Yet to Start"
            };

            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            var statusHistory = new RequestStatusHistory
            {
                RequestId = request.RequestId,
                UpdatedBy = username,
                NewStatus = "Yet to Start",
                Remarks = "Request created",
                Timestamp = DateTime.UtcNow
            };
            _context.RequestStatusHistories.Add(statusHistory);
            await _context.SaveChangesAsync();

            return RedirectToAction("GuestHome");
        }

        // GET: /Request/ActiveRequests
        public async Task<IActionResult> ActiveRequests()
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(username) || role != "Guest")
            {
                return RedirectToAction("Login", "User");
            }

            var requests = await _context.Requests
                .Where(r => r.Username == username && r.CurrentStatus != "Finished")
                .ToListAsync();

            return View(requests);
        }

        // GET: /Request/OldRequests
        public async Task<IActionResult> OldRequests()
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(username) || role != "Guest")
            {
                return RedirectToAction("Login", "User");
            }

            var requests = await _context.Requests
                .Where(r => r.Username == username && r.CurrentStatus == "Finished")
                .ToListAsync();

            return View(requests);
        }

        // POST: /Request/Reactivate/{id}
        [HttpPost]
        public async Task<IActionResult> Reactivate(int id)
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(username) || role != "Guest")
            {
                return RedirectToAction("Login", "User");
            }

            var request = await _context.Requests.FirstOrDefaultAsync(r => r.RequestId == id && r.Username == username);
            if (request == null)
            {
                return NotFound();
            }

            request.CurrentStatus = "Yet to Start";
            _context.Requests.Update(request);

            var statusHistory = new RequestStatusHistory
            {
                RequestId = request.RequestId,
                UpdatedBy = username,
                NewStatus = "Yet to Start",
                Remarks = "Request reactivated",
                Timestamp = DateTime.UtcNow
            };
            _context.RequestStatusHistories.Add(statusHistory);

            await _context.SaveChangesAsync();

            return RedirectToAction("ActiveRequests");
        }

        // GET: /Request/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(username) || role != "Guest")
            {
                return RedirectToAction("Login", "User");
            }

            var request = await _context.Requests
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.RequestId == id && r.Username == username);

            if (request == null)
            {
                return NotFound();
            }

            var statusHistory = await _context.RequestStatusHistories
                .Where(h => h.RequestId == id)
                .OrderBy(h => h.Timestamp)
                .ToListAsync();

            ViewBag.StatusHistory = statusHistory;

            return View(request);
        }
    }
}
