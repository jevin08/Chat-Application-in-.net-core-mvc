using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MvcChat.Models;
using MvcChat.Data;
using NuGet.Versioning;
using NuGet.Protocol;

namespace MvcChat.Controllers
{
    [Authorize(Roles = "Basic")]
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Chat(string id)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return NotFound();
            }
            ApplicationUser friend;
            try { 
                friend = _context.Users.First(i => i.Id == id);
                ViewBag.friend = friend;
            }
            catch (Exception ex)
            {
                return NotFound();
            }
            ViewBag.currentUser = user;

            var chat = await _context.Comment.Where(c => (c.CommenterId == user.Id && c.UserId==friend.Id) | (c.CommenterId == friend.Id && c.UserId == user.Id)).OrderBy(e=>e.SendTime).ToListAsync();
            ViewBag.chat = chat;

            user = _context.Users.Include(i => i.Follow).First(u => u.Id == user.Id);
            var applicationDbContext = user.Follow.ToList();
            return View("Index", applicationDbContext);
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.currentUser = user;
            user = _context.Users.Include(i => i.Follow).First(u => u.Id == user.Id);
            var applicationDbContext = user.Follow.ToList();
            return View(applicationDbContext);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage([Bind("Text,CommenterId,UserId")] Comment comment)
        {
            comment.Id = Guid.NewGuid().ToString();
            comment.ApplicationUser = _context.Users.First(i => i.Id == comment.UserId);
            Console.WriteLine(comment.ToJson());
            if (comment.Text!=null)
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
            }
            return await Chat(comment.UserId);
        }

        public async Task<IActionResult> Connection()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.currentUser = user;
            var applicationDbContext = _context.Users.Include(c => c.Follow);
            return View(await applicationDbContext.ToListAsync());
        }
        [HttpPost, ActionName("ConnectionAdd")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConnectionAdd(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            if (ModelState.IsValid)
            {

                try
                {
                    ApplicationUser? friend = await _context.Users.FirstOrDefaultAsync(i => i.Id == id);
                    if (friend == null)
                    {
                        return NotFound();
                    }
                    user.Follow.Add(friend);
                    _context.Update(user);
                    friend.Follow.Add(user);
                    _context.Update(friend);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(Connection));
            }
            return RedirectToAction(nameof(Connection));
        }
        [HttpPost, ActionName("ConnectionRemove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConnectionRemove(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            if (ModelState.IsValid)
            {

                try
                {
                    ApplicationUser? friend = await _context.Users.FirstOrDefaultAsync(i => i.Id == id);
                    if (friend == null)
                    {
                        return NotFound();
                    }
                    user.Follow.Remove(friend);
                    _context.Update(user);
                    friend.Follow.Remove(user);
                    _context.Update(friend);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(Connection));
            }
            return RedirectToAction(nameof(Connection));
        }
        public async Task<IActionResult> GetImage(byte[] val)
        {
            var file = File(val, "image/png");
            return file;
        } 
    }
}