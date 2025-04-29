using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RMS.Data;
using RMS.Models;
using RMS.Utils;

namespace RMS.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserController> _logger;

        public UserController(ApplicationDbContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /User/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /User/Register
        [HttpPost]
        public async Task<IActionResult> Register(string fullName, string password)
        {
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Full Name and Password are required.";
                return View();
            }

            // Generate unique username (e.g., first letter of first name + last name + random number)
            string baseUsername = GenerateUsername(fullName);
            string username = baseUsername;
            int suffix = 1;
            while (await _context.Users.AnyAsync(u => u.Username == username))
            {
                username = baseUsername + suffix;
                suffix++;
            }

            // Hash password using common utility
            string passwordHash = PasswordHasher.HashPassword(password);

            var user = new UserDetails
            {
                Username = username,
                FullName = fullName,
                PasswordHash = passwordHash,
                Role = "Guest"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            ViewBag.Message = $"Registration successful! Your username is {username}. Please login.";
            return View("Login");
        }

        // GET: /User/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /User/Login
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Username and Password are required.";
                return View();
            }

            _logger.LogInformation("Login attempt for username: {Username}", username);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                _logger.LogWarning("Login failed: user not found for username: {Username}", username);
                ViewBag.Error = "Invalid username or password.";
                return View();
            }

            bool passwordValid = PasswordHasher.VerifyPassword(password, user.PasswordHash);
            _logger.LogInformation("Password verification result for username {Username}: {Result}", username, passwordValid);

            if (!passwordValid)
            {
                _logger.LogWarning("Login failed: invalid password for username: {Username}", username);
                ViewBag.Error = "Invalid username or password.";
                return View();
            }

            // Store username and role in session
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role);

            if (user.Role == "Admin")
            {
                return RedirectToAction("AdminHome", "Admin");
            }
            else
            {
                return RedirectToAction("GuestHome", "Request");
            }
        }

        // GET: /User/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        private string GenerateUsername(string fullName)
        {
            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return "user" + new Random().Next(1000, 9999);
            string username = parts[0].Substring(0, 1).ToLower();
            if (parts.Length > 1)
            {
                username += parts[parts.Length - 1].ToLower();
            }
            else
            {
                username += parts[0].Substring(1).ToLower();
            }
            return username;
        }
    }
}
