﻿using System;
using System.Collections.Generic;
using System.IO;
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
using POYA.Areas.EduHub.Controllers;
using POYA.Areas.XAd.Models;
using POYA.Areas.XUserFile.Controllers;
using POYA.Data;
using POYA.Unities.Helpers;

namespace POYA.Areas.XAd.Controllers
{
    [Area("XAd")]
    [Authorize]
    public class XAdCustomersController : Controller
    {

        #region     DI
        private readonly IHostingEnvironment _hostingEnv;
        private readonly IStringLocalizer<Program> _localizer;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly X_DOVEHelper _x_DOVEHelper;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<XAdCustomersController> _logger;
        private readonly HtmlSanitizer _htmlSanitizer;
        //  private readonly MimeHelper _mimeHelper;
        private readonly XUserFileHelper _xUserFileHelper;
        public XAdCustomersController(
            //  MimeHelper mimeHelper,
            HtmlSanitizer htmlSanitizer,
            ILogger<XAdCustomersController> logger,
            SignInManager<IdentityUser> signInManager,
            X_DOVEHelper x_DOVEHelper,
            RoleManager<IdentityRole> roleManager,
           IEmailSender emailSender,
           UserManager<IdentityUser> userManager,
           ApplicationDbContext context,
           IHostingEnvironment hostingEnv,
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

        #region 

        // GET: XAd/XAdCustomers
        #endregion
        public async Task<IActionResult> Index()
        {
            return View(await _context.XAdCustomer.ToListAsync());
        }

        #region 

        // GET: XAd/XAdCustomers/Details/5
        #endregion
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var xAdCustomer = await _context.XAdCustomer
                .FirstOrDefaultAsync(m => m.Id == id);
            if (xAdCustomer == null)
            {
                return NotFound();
            }

            return View(xAdCustomer);
        }

        #region 

        // GET: XAd/XAdCustomers/Create
        #endregion
        public IActionResult Create()
        {
            return View();
        }

        #region 

        // POST: XAd/XAdCustomers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        #endregion
        public async Task<IActionResult> Create([Bind("Id,Name,UserId,DORegistering,LicenseImgFiles,Address")] XAdCustomer xAdCustomer)
        {
            if (ModelState.IsValid)
            {
                if (xAdCustomer.LicenseImgFiles.Count() < 3)
                {
                    return View(xAdCustomer);
                }

                #region CONTENTTYPE_VALIDATE
                var IsContentTypeValid = true;
                xAdCustomer.LicenseImgFiles.ForEach(p =>
                {
                    if (p.ContentType.StartsWith("image/") || !p.FileName.Contains('.'))
                    {
                        IsContentTypeValid = false;
                    }
                });
                if (!IsContentTypeValid)
                {
                    return View(xAdCustomer);
                }

                #endregion

                xAdCustomer.Id = Guid.NewGuid();
                _context.Add(xAdCustomer);
                var _XAdCustomerLicenses = new List<XAdCustomerLicense>();
                foreach (var f in xAdCustomer.LicenseImgFiles)
                {
                    var _ = new MemoryStream();
                    if (f.Length > 0)
                    {
                        var _memoryStream = new MemoryStream();
                        await f.CopyToAsync(_memoryStream);
                        var _bytes = _memoryStream.ToArray();
                        var _md5 = new XUserFileHelper().GetFileMD5(_bytes);
                        var _FileStream = new FileStream(XAdCustomerHelper.XAdCustomerLicenseImgFilePath(_hostingEnv) + $"/{_md5}.{f.FileName.Split('.').Last()}", FileMode.Create);
                        await f.CopyToAsync(_FileStream);
                        _XAdCustomerLicenses.Add(new XAdCustomerLicense { });
                    }
                }
                await _context.XAdCustomerLicenses.AddRangeAsync(_XAdCustomerLicenses);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(xAdCustomer);
        }

        #region 

        // GET: XAd/XAdCustomers/Edit/5
        #endregion
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var xAdCustomer = await _context.XAdCustomer.FindAsync(id);
            if (xAdCustomer == null)
            {
                return NotFound();
            }
            return View(xAdCustomer);
        }

        #region 

        // POST: XAd/XAdCustomers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        #endregion
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,UserId,DORegistering,Address")] XAdCustomer xAdCustomer)
        {
            if (id != xAdCustomer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(xAdCustomer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!XAdCustomerExists(xAdCustomer.Id))
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
            return View(xAdCustomer);
        }

        #region 

        // GET: XAd/XAdCustomers/Delete/5
        #endregion
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var xAdCustomer = await _context.XAdCustomer
                .FirstOrDefaultAsync(m => m.Id == id);
            if (xAdCustomer == null)
            {
                return NotFound();
            }

            return View(xAdCustomer);
        }


        #region 

        // POST: XAd/XAdCustomers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        #endregion
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var xAdCustomer = await _context.XAdCustomer.FindAsync(id);
            _context.XAdCustomer.Remove(xAdCustomer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool XAdCustomerExists(Guid id)
        {
            return _context.XAdCustomer.Any(e => e.Id == id);
        }
    }
}
