using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using RMS.Data;
using RMS.Models;

namespace RMS.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/AdminHome
        public async Task<IActionResult> AdminHome()
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(username) || role != "Admin")
            {
                return RedirectToAction("Login", "User");
            }

            var requests = await _context.Requests
                .Where(r => r.CurrentStatus != "Finished")
                .Include(r => r.User)
                .ToListAsync();

            return View(requests);
        }

        // GET: /Admin/CompletedRequests
        public async Task<IActionResult> CompletedRequests()
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(username) || role != "Admin")
            {
                return RedirectToAction("Login", "User");
            }

            var requests = await _context.Requests
                .Where(r => r.CurrentStatus == "Finished")
                .Include(r => r.User)
                .ToListAsync();

            return View(requests);
        }

        // GET: /Admin/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(username) || role != "Admin")
            {
                return RedirectToAction("Login", "User");
            }

            var request = await _context.Requests
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.RequestId == id);

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

        // POST: /Admin/UpdateStatus/{id}
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string newStatus, string remarks)
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(username) || role != "Admin")
            {
                return RedirectToAction("Login", "User");
            }

            var request = await _context.Requests.FirstOrDefaultAsync(r => r.RequestId == id);
            if (request == null)
            {
                return NotFound();
            }

            request.CurrentStatus = newStatus;
            _context.Requests.Update(request);

            var statusHistory = new RequestStatusHistory
            {
                RequestId = id,
                UpdatedBy = username,
                NewStatus = newStatus,
                Remarks = remarks,
                Timestamp = DateTime.UtcNow
            };
            _context.RequestStatusHistories.Add(statusHistory);

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = id });
        }
    }
}
