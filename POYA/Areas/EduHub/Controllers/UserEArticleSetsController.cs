﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ganss.XSS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using POYA.Areas.EduHub.Models;
using POYA.Areas.XUserFile.Controllers;
using POYA.Data;
using POYA.Unities.Helpers;

namespace POYA.Areas.EduHub.Controllers
{
    [Authorize]
    [Area("EduHub")]
    public class UserEArticleSetsController : Controller
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
        private readonly XUserFileHelper _xUserFileHelper;
        //  private readonly MimeHelper _mimeHelper;
        public UserEArticleSetsController(
            //  MimeHelper mimeHelper,
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
            //  _mimeHelper = mimeHelper;
            _xUserFileHelper = new XUserFileHelper();
        }

        #endregion

        // GET: EduHub/UserEArticleSets
        public async Task<IActionResult> Index()
        {
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var _UserEArticleSet = await _context.UserEArticleSet.Where(p => p.UserId == _UserId).ToListAsync();

            #region INITIALIZE_THE_DEFAULT

            _UserEArticleSet.Add(
                new UserEArticleSet
                {
                    Id = X_DOVEValues.DefaultEArticleSetId,
                    Comment = _localizer["The default"],
                    Name = _localizer["default"],
                    Label = _localizer["default"]
                }
            );

            _UserEArticleSet.Add(
                new UserEArticleSet
                {
                    Id = X_DOVEValues.AddEArticleSetId,
                    Label = _localizer["Click to add"],
                    Name = _localizer["Add"],
                    Comment = _localizer["Click to add"]
                }
            );

            #endregion

            ViewData[nameof(UserEArticleHomeInfo)] = 
                (await _context.UserEArticleHomeInfos.Where(p => p.UserId == _UserId).FirstOrDefaultAsync()) ?? 
                new UserEArticleHomeInfo { UserId = _UserId, Comment = _localizer["No set yet"] + "!" };

            return View(_UserEArticleSet.OrderByDescending(p => p.DOCreating));
        }

        // GET: EduHub/UserEArticleSets/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var userEArticleSet = await _context.UserEArticleSet
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userEArticleSet == null)
            {
                return NotFound();
            }
            return View(userEArticleSet);
        }


        // GET: EduHub/UserEArticleSets/Create
        public IActionResult Create()
        {
            return View();
        }


        // POST: EduHub/UserEArticleSets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Label,Comment")] UserEArticleSet userEArticleSet)
        {
            if (ModelState.IsValid)
            {
                var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
                userEArticleSet.Id = Guid.NewGuid();
                userEArticleSet.UserId = _UserId;
                _context.Add(userEArticleSet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userEArticleSet);
        }


        // GET: EduHub/UserEArticleSets/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;

            var userEArticleSet = await _context.UserEArticleSet.FirstOrDefaultAsync(p => p.Id == id && p.UserId == _UserId);

            if (userEArticleSet == null)
            {
                return NotFound();
            }
            return View(userEArticleSet);
        }


        // POST: EduHub/UserEArticleSets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Label,Comment")] UserEArticleSet userEArticleSet)
        {
            if (id != userEArticleSet.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
                    var _UserEArticleSet = await _context.UserEArticleSet.FirstOrDefaultAsync(p => p.UserId == _UserId && p.Id == userEArticleSet.Id);
                    if (_UserEArticleSet == null) return NotFound();

                    #region UPDATE

                    _UserEArticleSet.Name = userEArticleSet.Name;
                    _UserEArticleSet.Label = userEArticleSet.Label;
                    _UserEArticleSet.Comment = userEArticleSet.Comment;

                    #endregion

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserEArticleSetExists(userEArticleSet.Id))
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
            return View(userEArticleSet);
        }


        // GET: EduHub/UserEArticleSets/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var userEArticleSet = await _context.UserEArticleSet
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == _UserId);
            if (userEArticleSet == null)
            {
                return NotFound();
            }
            return View(userEArticleSet);
        }


        // POST: EduHub/UserEArticleSets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var userEArticleSet = await _context.UserEArticleSet
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == _UserId);

            if (userEArticleSet == null) return NotFound();

            _context.UserEArticleSet.Remove(userEArticleSet);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserEArticleSetExists(Guid id)
        {
            return _context.UserEArticleSet.Any(e => e.Id == id);
        }

        #region DEPOLLUTION

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userEArticleHomeInfo"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadEArticleHomeInfo([FromForm]UserEArticleHomeInfo userEArticleHomeInfo)
        {
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var _userEArticleHomeInfo = await _context.UserEArticleHomeInfos.FirstOrDefaultAsync(p => p.UserId == UserId_);
            var _MD5 = string.Empty;

            if (userEArticleHomeInfo?.CoverFile != null)
            {
                _MD5 = await _xUserFileHelper.LWriteBufferToFileAsync(_hostingEnv, userEArticleHomeInfo.CoverFile);
            }

            if (_userEArticleHomeInfo == null)
            {
                _userEArticleHomeInfo = new UserEArticleHomeInfo
                {
                    UserId = UserId_,
                    Comment = userEArticleHomeInfo.Comment,
                    CoverFileMD5 = _MD5,
                    Id = Guid.NewGuid(),
                    CoverFileContentType = string.IsNullOrWhiteSpace(_MD5) ?
                    "" : userEArticleHomeInfo.CoverFile.ContentType
                };

                await _context.UserEArticleHomeInfos.AddAsync(_userEArticleHomeInfo);
            }
            else
            {
                _userEArticleHomeInfo.Comment = string.IsNullOrWhiteSpace(userEArticleHomeInfo.Comment) ?
                    _userEArticleHomeInfo.Comment : userEArticleHomeInfo.Comment;

                _userEArticleHomeInfo.CoverFileMD5 = string.IsNullOrWhiteSpace(_MD5) ?
                    _userEArticleHomeInfo.CoverFileMD5 : _MD5;

                _userEArticleHomeInfo.CoverFileContentType = string.IsNullOrWhiteSpace(_MD5) ?
                    _userEArticleHomeInfo.CoverFileContentType : userEArticleHomeInfo.CoverFile.ContentType;
            }
            await _context.SaveChangesAsync();

            return Ok();
        }



        /// <summary>
        /// Get a file, it is a user's file or a file is shared
        /// </summary>
        /// <param name="id">The <see cref="LUserFile"/> id or the sharing id of <see cref="LSharing"/> </param>
        /// <param name="LSharingId">The <see cref="LSharing"/> id should be passed if you get a file in shared directory</param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<IActionResult> GetEArticleHomeCover(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var _userEArticleHomeInfo = await _context.UserEArticleHomeInfos.FirstOrDefaultAsync(p => p.Id == id);

            var _FileBytes = new byte[0];
            var _ContentType = "image/webp";  
            
            if (_userEArticleHomeInfo == null ||
                !System.IO.File.Exists(X_DOVEValues.FileStoragePath(_hostingEnv)
                + _userEArticleHomeInfo.CoverFileMD5))
            {
                _FileBytes = await System.IO.File.ReadAllBytesAsync(_hostingEnv.WebRootPath + "/img/earticle_home_default_img.webp");
            }
            else
            {
                var _FilePath_ = X_DOVEValues.FileStoragePath(_hostingEnv) + _userEArticleHomeInfo.CoverFileMD5;
                _FileBytes = await System.IO.File.ReadAllBytesAsync(_FilePath_);
                _ContentType = _userEArticleHomeInfo.CoverFileContentType;
            }
            return File(_FileBytes, _ContentType, $"EARTICLE_HOME_COVER_{_userEArticleHomeInfo?.UserId ?? "In_nI"}", true);
        }

        #endregion
    }
}
