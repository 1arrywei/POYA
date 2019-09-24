using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ganss.XSS;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using POYA.Areas.XAd.Models;
using POYA.Data;
using POYA.Unities.Helpers;

namespace POYA.Areas.XAd.Controllers
{
    [Area("XAd")]
    public class UserFeedbackController : Controller
    {
        
        #region     DI
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly IStringLocalizer<Program> _localizer;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly X_DOVEHelper _x_DOVEHelper;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly HtmlSanitizer _htmlSanitizer;
      
        public UserFeedbackController(
            HtmlSanitizer htmlSanitizer,
            SignInManager<IdentityUser> signInManager,
            X_DOVEHelper x_DOVEHelper,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context,
            IWebHostEnvironment hostingEnv,
            IStringLocalizer<Program> localizer)
        {
            _htmlSanitizer = htmlSanitizer;
            _hostingEnv = hostingEnv;
            _localizer = localizer;
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _x_DOVEHelper = x_DOVEHelper;
            _signInManager = signInManager;
        }
        #endregion


        // GET: XAd/UserFeedback
        public async Task<IActionResult> Index()
        {
            return View(await _context.UserFeedbacks.ToListAsync());
        }

        // GET: XAd/UserFeedback/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userFeedback = await _context.UserFeedbacks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userFeedback == null)
            {
                return NotFound();
            }

            return View(userFeedback);
        }

        // GET: XAd/UserFeedback/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: XAd/UserFeedback/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,DOFeedback,Comment")] UserFeedback userFeedback)
        {
            if (ModelState.IsValid)
            {
                userFeedback.Id = Guid.NewGuid();
                _context.Add(userFeedback);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userFeedback);
        }

        // GET: XAd/UserFeedback/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userFeedback = await _context.UserFeedbacks.FindAsync(id);
            if (userFeedback == null)
            {
                return NotFound();
            }
            return View(userFeedback);
        }

        // POST: XAd/UserFeedback/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,UserId,DOFeedback,Comment")] UserFeedback userFeedback)
        {
            if (id != userFeedback.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userFeedback);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserFeedbackExists(userFeedback.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(userFeedback);
        }

        // GET: XAd/UserFeedback/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userFeedback = await _context.UserFeedbacks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userFeedback == null)
            {
                return NotFound();
            }

            return View(userFeedback);
        }

        // POST: XAd/UserFeedback/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var userFeedback = await _context.UserFeedbacks.FindAsync(id);
            _context.UserFeedbacks.Remove(userFeedback);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserFeedbackExists(Guid id)
        {
            return _context.UserFeedbacks.Any(e => e.Id == id);
        }
    }
}
