﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using POYA.Data;
using POYA.Models;
using POYA.Unities.Helpers;

namespace POYA.Controllers
{
    [Authorize]
    public class LUserFilesController : Controller
    {
        #region
        private readonly IHostingEnvironment _hostingEnv;
        private readonly IStringLocalizer<Program> _localizer;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly X_DOVEHelper _x_DOVEHelper;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LUserFilesController> _logger;
        private readonly IAntiforgery _antiforgery;
        public LUserFilesController(
            IAntiforgery antiforgery,
            ILogger<LUserFilesController> logger,
            SignInManager<IdentityUser> signInManager,
            X_DOVEHelper x_DOVEHelper,
            RoleManager<IdentityRole> roleManager,
           IEmailSender emailSender,
           UserManager<IdentityUser> userManager,
           ApplicationDbContext context,
           IHostingEnvironment hostingEnv,
           IStringLocalizer<Program> localizer)
        {
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

        // GET: LUserFiles
        public async Task<IActionResult> Index(Guid? InDirId)
        {
            InDirId = InDirId ?? Guid.Empty;
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;

            var _FileNames = new DirectoryInfo(_x_DOVEHelper.FileStoragePath(_hostingEnv)).GetFiles().Select(p => p.Name);
            //  Console.WriteLine("FileName >> "+JsonConvert.SerializeObject(_FileNames));
            var _LFiles = await _context.LFile.ToListAsync();
            var _LUserFiles = await _context.LUserFile.ToListAsync();
            //  Console.WriteLine("File >> "+JsonConvert.SerializeObject(_LFiles));
            _LFiles.ForEach(f=> {
                if (!_FileNames.Contains(f.MD5))
                {
                    _context.LFile.Remove(f);
                }
            });
            _LUserFiles.ForEach(f => {
                if (!_FileNames.Contains(f.MD5))
                {
                    _context.LUserFile.Remove(f);
                }
            });
            await _context.SaveChangesAsync();

            var LUserFile_ = await _context.LUserFile.Where(p => p.UserId == UserId_ && p.InDirId == InDirId).OrderBy(p => p.DOGenerating).ToListAsync();
            var InDirName = (await _context.LDir.Where(p => p.Id == InDirId).Select(p => p.Name).FirstOrDefaultAsync()) ?? "root";
            //  LUserFile_.ForEach(m => { m.InDirName = InDirName; });
            ViewData[nameof(InDirName)] = InDirName;
            ViewData[nameof(InDirId)] = InDirId;
            ViewData["LastDirId"] = InDirId == Guid.Empty ? InDirId
                : await _context.LDir.Where(p => p.Id == InDirId).Select(p => p.InDirId).FirstOrDefaultAsync();
            var LDirs = await _context.LDir.Where(p => p.InDirId == InDirId && p.UserId == UserId_).ToListAsync();
            ViewData[nameof(LDirs)] = LDirs;
            return View(LUserFile_);
        }

        // GET: LUserFiles/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lUserFile = await _context.LUserFile.FirstOrDefaultAsync(m => m.Id == id);
            if (lUserFile == null)
            {
                return NotFound();
            }

            return View(lUserFile);
        }

        public async Task<IActionResult> GetFile(Guid? id)
        {
            if (id == null)
            {
                return NoContent();
            }
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var _LUserFile = await _context.LUserFile.Select(p => new { p.MD5, p.Id, p.SharedCode, p.Name, p.UserId, p.ContentType })
                .FirstOrDefaultAsync(p => (p.Id == id && p.UserId == _UserId) || p.SharedCode == id);
            if (_LUserFile == null )
            {
                return NoContent();
            }
            var _FilePath = _x_DOVEHelper.FileStoragePath(_hostingEnv) + _LUserFile.MD5;
            if (!System.IO.File.Exists(_FilePath))
            {
                return NoContent();
            }
            var FileBytes = await System.IO.File.ReadAllBytesAsync(_FilePath);
            return File(FileBytes, _LUserFile.ContentType ,_LUserFile.Name);
        }

        // GET: LUserFiles/Create
        public IActionResult Create(Guid? InDirId)
        {
            ViewData[nameof(InDirId)] = InDirId?? Guid.Empty;
            return View();
        }

        // POST: LUserFiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm]LFilePost _LFilePost)
        {
            //  [Bind("Id,MD5,UserId,SharedCode,DOGenerating,Name,InDirId")] LUserFile lUserFile)
            if (_LFilePost._LFile.Length > 0)
            {
                var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
                var MemoryStream_ = new MemoryStream();
                await _LFilePost._LFile.CopyToAsync(MemoryStream_);
                var FileBytes = MemoryStream_.ToArray();
                var MD5_ = _x_DOVEHelper.GetFileMD5(FileBytes);
                var FilePath = _x_DOVEHelper.FileStoragePath(_hostingEnv) + MD5_;
                //  System.IO.File.Create(FilePath);
                await System.IO.File.WriteAllBytesAsync(FilePath, FileBytes);
                await _context.LFile.AddAsync(new Models.LFile
                {
                    MD5 = MD5_,
                    UserId = UserId_
                });
                await _context.LUserFile.AddAsync(new LUserFile
                {
                    UserId = UserId_,
                    MD5 = MD5_,
                    InDirId = _LFilePost.InDirId,
                    Name = _LFilePost._LFile.FileName, 
                     ContentType=_LFilePost._LFile.ContentType
                });
                await _context.SaveChangesAsync();
            }
            // process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(_LFilePost.Id);
            #region
            /*
            if (ModelState.IsValid)
            {
                lUserFile.Id = Guid.NewGuid();
                _context.Add(lUserFile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            */
            //      return Ok();     //   View(lUserFile);
            #endregion
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContrastMD5(ContrastMD5 _ContrastMD5)
        {
            Console.WriteLine(">>>>"+JsonConvert.SerializeObject(_ContrastMD5));
            //  System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(_ContrastMD5));
            var ContrastResult = new List<int>();
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            foreach (var i in _ContrastMD5.File8MD5s)
            {
                if (await _context.LFile.AnyAsync(p => p.MD5 == i.MD5))
                {
                    await _context.LUserFile.AddAsync(
                        new LUserFile { InDirId = _ContrastMD5.InDirId, MD5 = i.MD5, Name = i.FileName, UserId = UserId_ }
                        );
                    ContrastResult.Add(i.Id);
                }
                await _context.SaveChangesAsync();
            }
            return Json(ContrastResult);
        }

        // GET: LUserFiles/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lUserFile = await _context.LUserFile.FindAsync(id);
            if (lUserFile == null)
            {
                return NotFound();
            }
            return View(lUserFile);
        }

        // POST: LUserFiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,MD5,UserId,SharedCode,DOGenerating,Name,InDirId")] LUserFile lUserFile)
        {
            if (id != lUserFile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lUserFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LUserFileExists(lUserFile.Id))
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
            return View(lUserFile);
        }

        // GET: LUserFiles/Delete/5 //
        //  [HttpPost]
        //  [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;

            var lUserFile = await _context.LUserFile.Where(p => p.Id == id && p.UserId == UserId_).FirstOrDefaultAsync();
            if (lUserFile == null)
            {
                return NotFound();
            }

            return PartialView(lUserFile);
        }

        // POST: LUserFiles/Delete/5
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var lUserFile = await _context.LUserFile.Where(p => p.Id == id && p.UserId == UserId_).FirstOrDefaultAsync();
            if (lUserFile == null)
            {
                return NotFound();
            }
            _context.LUserFile.Remove(lUserFile);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));   //  Ok();     // 
        }

        private bool LUserFileExists(Guid id)
        {
            return _context.LUserFile.Any(e => e.Id == id);
        }
    }
}