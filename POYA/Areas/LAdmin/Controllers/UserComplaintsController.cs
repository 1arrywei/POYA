using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using POYA.Areas.LAdmin.Models;
using POYA.Data;
using POYA.Unities.Helpers;

namespace POYA.Areas.LAdmin.Controllers
{
    [Area("LAdmin")]
    [Authorize]
    public class UserComplaintsController : Controller
    {

        #region DI
        private readonly IHostingEnvironment _hostingEnv;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly X_DOVEHelper _x_DOVEHelper;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<Program> _localizer;
        private readonly LAdminHelper _lAdminHelper;
        public UserComplaintsController(
            IConfiguration configuration,
            SignInManager<IdentityUser> signInManager,
            X_DOVEHelper x_DOVEHelper,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context,
            IHostingEnvironment hostingEnv,
            IStringLocalizer<Program> localizer)
        {
            _configuration = configuration;
            _hostingEnv = hostingEnv;
            _localizer = localizer;
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _x_DOVEHelper = x_DOVEHelper;
            _signInManager = signInManager;
            _lAdminHelper=new  LAdminHelper(_localizer);
        }

        #endregion

        // GET: UserComplaints
        [Authorize(Roles="ADMINISTRATOR")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.UserComplaint.ToListAsync());
        }

        // GET: UserComplaints/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userComplaint = await _context.UserComplaint
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userComplaint == null)
            {
                return NotFound();
            }

            return View(userComplaint);
        }

        // GET: UserComplaints/Create
        public IActionResult Create(Guid _ContentId)
        {
            var _UserComplaint=new UserComplaint{
                ContentId=_ContentId,
                IllegalityTypeSelectListItems=_lAdminHelper.GetIllegalityTypeSelectListItems()
            };
            return View(_UserComplaint);
        }

        // POST: UserComplaints/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,ContentId,DOComplaint,ReceptionistId,AuditResultId,Description,IllegalityType")] UserComplaint userComplaint)
        {
            if (ModelState.IsValid)
            {
                var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;

                userComplaint.Id = Guid.NewGuid();
                userComplaint.UserId=UserId_;
                userComplaint.DOComplaint=DateTimeOffset.Now;

                _context.Add(userComplaint);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userComplaint);
        }

        // GET: UserComplaints/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userComplaint = await _context.UserComplaint.FindAsync(id);
            if (userComplaint == null)
            {
                return NotFound();
            }
            return View(userComplaint);
        }

        // POST: UserComplaints/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,UserId,ContentId,DOComplaint,ReceptionistId,AuditResultId,Description,IllegalityType")] UserComplaint userComplaint)
        {
            if (id != userComplaint.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;

                    _context.Update(userComplaint);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserComplaintExists(userComplaint.Id))
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
            return View(userComplaint);
        }

        // GET: UserComplaints/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            
            var userComplaint = await _context.UserComplaint
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userComplaint == null)
            {
                return NotFound();
            }

            return View(userComplaint);
        }

        // POST: UserComplaints/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var userComplaint = await _context.UserComplaint.FindAsync(id);
            _context.UserComplaint.Remove(userComplaint);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        private bool UserComplaintExists(Guid id)
        {
            return _context.UserComplaint.Any(e => e.Id == id);
        }

    }
}
