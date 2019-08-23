using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using POYA.Areas.FunFiles.Models;
using POYA.Data;
using POYA.Unities.Helpers;

namespace POYA.Areas.FunFiles.Controllers
{
    [Area("FunFiles")]
    [Authorize]
    public class FunYourFilesController : Controller
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
        private readonly FunFilesHelper _funFilesHelper;
        public FunYourFilesController(
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
            _funFilesHelper=new  FunFilesHelper();
        }

        #endregion


        // GET: FunFiles/FunYourFiles
        public async Task<IActionResult> Index()
        {
            return View(await _context.FunYourFile.ToListAsync());
        }

        // GET: FunFiles/FunYourFiles/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funYourFile = await _context.FunYourFile
                .FirstOrDefaultAsync(m => m.Id == id);
            if (funYourFile == null)
            {
                return NotFound();
            }

            return View(funYourFile);
        }

        // GET: FunFiles/FunYourFiles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FunFiles/FunYourFiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FileByteId,ParentDirId,UserId,Name,DOUploading")] FunYourFile funYourFile)
        {
            if (ModelState.IsValid)
            {
                funYourFile.Id = Guid.NewGuid();
                _context.Add(funYourFile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(funYourFile);
        }

        // GET: FunFiles/FunYourFiles/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funYourFile = await _context.FunYourFile.FindAsync(id);
            if (funYourFile == null)
            {
                return NotFound();
            }
            return View(funYourFile);
        }

        // POST: FunFiles/FunYourFiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,FileByteId,ParentDirId,UserId,Name,DOUploading")] FunYourFile funYourFile)
        {
            if (id != funYourFile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(funYourFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FunYourFileExists(funYourFile.Id))
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
            return View(funYourFile);
        }

        // GET: FunFiles/FunYourFiles/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funYourFile = await _context.FunYourFile
                .FirstOrDefaultAsync(m => m.Id == id);
            if (funYourFile == null)
            {
                return NotFound();
            }

            return View(funYourFile);
        }

        // POST: FunFiles/FunYourFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var funYourFile = await _context.FunYourFile.FindAsync(id);
            _context.FunYourFile.Remove(funYourFile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FunYourFileExists(Guid id)
        {
            return _context.FunYourFile.Any(e => e.Id == id);
        }

        #region DEPOLLUTION

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompareSHA256(SHA256Compare sHA256Compare)
        {
            var _FunFileByteFileSHA256s=await _context.FunFileByte.Select(p=>p.FileSHA256).ToListAsync();
            var UserId_= _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var _FunYourFiles=new List<FunYourFile>();

            sHA256Compare.FileSHA256s
                .Select(
                    p=>new{
                        p.Name,
                        SHA256=_funFilesHelper.ConvertSHA256StringToBytes(p.SHA256)})
                .Where(
                    p=>_FunFileByteFileSHA256s.Contains(p.SHA256))
                .ToList()
                .ForEach(
                    p=>{
                        _FunYourFiles.Add(
                            new FunYourFile{
                                DOUploading=DateTimeOffset.Now,
                                Id=Guid.NewGuid(),
                                UserId=UserId_,
                                Name=p.Name,
                                ParentDirId=sHA256Compare.ParentDirId,

                                FileByteId=_context.FunFileByte
                                    .Where(o=>o.FileSHA256==p.SHA256)
                                    .Select(m=>m.Id)
                                    .FirstOrDefaultAsync()
                                    .GetAwaiter()
                                    .GetResult(),
                                    
                        });
                });
            if(_FunYourFiles.Count>0)
            { 
                await _context.FunYourFile.AddRangeAsync(_FunYourFiles);
                await _context.SaveChangesAsync();   
            }
            
            return Json( 
                sHA256Compare.FileSHA256s.Where(
                    p=>
                        _FunFileByteFileSHA256s.Contains(
                            _funFilesHelper.ConvertSHA256StringToBytes(p.SHA256)
                        )
                ).Select(p=>p.Id).ToArray()
            );
        }

        [HttpGet]
        public IActionResult FunUploadFiles(Guid? ParentDirId)
        {
            var _ParentDirId=ParentDirId??_funFilesHelper.RootDirId;
            ViewData["ParentDirId"]=_ParentDirId;
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FunUploadFiles(FunUploadFile funUploadFile)
        {
            var ParentDirId=Guid.Parse(TempData["ParentDirId"].ToString());
            return RedirectToAction(
                nameof(POYA.Areas.FunFiles.Controllers.FunYourFilesController.Index),
                "FunYourFiles",
                new{ParentDirId}
            );
        }

        #endregion
    }
}
