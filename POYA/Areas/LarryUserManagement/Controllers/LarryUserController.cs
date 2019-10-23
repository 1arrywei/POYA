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
using POYA.Areas.FunFiles.Controllers;
using POYA.Areas.LarryUserManagement.Models;
using POYA.Data;
using POYA.Unities.Helpers;

namespace POYA.Areas.LarryUserManagement.Controllers
{
    
    [Area("LarryUserManagement")]
    [Authorize(Roles="ADMINISTRATOR")]
    public class LarryUsersController : Controller
    {
        
        
        #region DI
        private readonly IWebHostEnvironment _webHostEnv;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly X_DOVEHelper _x_DOVEHelper;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<Program> _localizer;
        private readonly FunFilesHelper _funFilesHelper;
        public LarryUsersController(
            IConfiguration configuration,
            SignInManager<IdentityUser> signInManager,
            X_DOVEHelper x_DOVEHelper,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context,
            IWebHostEnvironment webHostEnv,
            IStringLocalizer<Program> localizer)
        {
            _configuration = configuration;
            _webHostEnv = webHostEnv;
            _localizer = localizer;
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _x_DOVEHelper = x_DOVEHelper;
            _signInManager = signInManager;
        }

        #endregion


        // GET: LarryUserManagement/LarryUser
        public async Task<IActionResult> Index()
        {
            return View(await _context.LarryUsers.ToListAsync());
        }

        // GET: LarryUserManagement/LarryUser/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var larryUser = await _context.LarryUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (larryUser == null)
            {
                return NotFound();
            }

            return View(larryUser);
        }

        // GET: LarryUserManagement/LarryUser/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LarryUserManagement/LarryUser/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,UserId,Comment,TelphoneNumber,Password,Email,UserName,RoleId,")] 
            LarryUser larryUser
        )
        {
            if (ModelState.IsValid)
            {
                larryUser.Id = Guid.NewGuid();
                var _CreatedUser = new IdentityUser{
                        Email=larryUser.Email,
                        EmailConfirmed = larryUser.IsEmailConfirmed, 
                        UserName = larryUser.UserName,
                    };

                await _userManager.CreateAsync(
                    _CreatedUser,
                    larryUser.Password
                );

                larryUser.UserId = _CreatedUser.Id;
                await _context.AddAsync(larryUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(larryUser);
        }

        // GET: LarryUserManagement/LarryUser/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var larryUser = await _context.LarryUsers.FindAsync(id);
            if (larryUser == null)
            {
                return NotFound();
            }
            return View(larryUser);
        }

        // POST: LarryUserManagement/LarryUser/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, 
            [Bind("Id,UserId,Comment,TelphoneNumber,Password,Email,UserName,RoleId,")] 
            LarryUser larryUser)
        {
            if (id != larryUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(larryUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LarryUserExists(larryUser.Id))
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
            return View(larryUser);
        }

        // GET: LarryUserManagement/LarryUser/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var larryUser = await _context.LarryUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (larryUser == null)
            {
                return NotFound();
            }

            return View(larryUser);
        }

        // POST: LarryUserManagement/LarryUser/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var larryUser = await _context.LarryUsers.FindAsync(id);
            _context.LarryUsers.Remove(larryUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LarryUserExists(Guid id)
        {
            return _context.LarryUsers.Any(e => e.Id == id);
        }
    }
}
