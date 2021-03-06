﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using POYA.Areas.FunFiles.Controllers;
using POYA.Data;
using POYA.Models;
using POYA.Unities.Attributes;
using POYA.Unities.Helpers;


namespace POYA.Controllers
{
    public class HomeController : Controller
    {
        #region DI
        private readonly IWebHostEnvironment _webHostEnv;
        private readonly IStringLocalizer<Program> _localizer;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly X_DOVEHelper _x_DOVEHelper;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        public HomeController(
            IConfiguration configuration,
            ILogger<HomeController> logger,
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

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Donate()
        {
            return View();
        }

        #region DEPOLLUTION

     
        public async Task<IActionResult> GetAvatar(string UserId = "")
        {
            var CurrentUserId = _userManager.GetUserAsync(User)?.GetAwaiter().GetResult()?.Id;  //    User.FindFirstValue(ClaimTypes.NameIdentifier);
            UserId = string.IsNullOrWhiteSpace(UserId) ? CurrentUserId : UserId;
            var AvatarFilePath = X_DOVEValues.AvatarStoragePath(_webHostEnv) + UserId;
            if (System.IO.File.Exists(AvatarFilePath))
            {
                var AvatarBytes = await System.IO.File.ReadAllBytesAsync(AvatarFilePath);
                if (AvatarBytes != null || AvatarBytes.Length > 1)
                {
                    return File(AvatarBytes, "image/jpg");
                }
            }
            var DefauleAvatar = await System.IO.File.ReadAllBytesAsync(_webHostEnv.WebRootPath + @"/img/article_publish_ico.webp");
            return File(DefauleAvatar, "image/webp");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> UploadAvatar([FromForm]IFormFile avatarFile)
        {
            if (avatarFile.Length > 1024 * 1024)
            {
                return Json(new { status = false, msg = "ExceedSize" });
            }

            var _allowedAvatarFileExtensions = new string[] { "image/jpg", "image/jpeg", "image/png" };
            var _isExtensionNeedChecking = false;
            if (_isExtensionNeedChecking && (!_allowedAvatarFileExtensions.Contains(avatarFile.ContentType.ToLower())))
            {
                return Json(new { status = false, msg = "RefuseExtension" });
            }
            else if (!avatarFile.ContentType.StartsWith("image/"))
            {
                return Json(new { status = false, msg = "RefuseExtension" });
            }
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var memoryStream = new MemoryStream();
            await avatarFile.CopyToAsync(memoryStream);
            var AvatarBytes = memoryStream.ToArray();   //  MakeCircleImage(memoryStream);//  
            var AvatarFilePath = X_DOVEValues.AvatarStoragePath(_webHostEnv) + _UserId;
            if (System.IO.File.Exists(AvatarFilePath))
            {
                var AvatarFileBytes = await System.IO.File.ReadAllBytesAsync(AvatarFilePath);
                var _X_doveUserInfo = await _context.X_DoveUserInfos.FirstOrDefaultAsync(p => p.UserId == _UserId);
                if (_X_doveUserInfo != null && AvatarBytes == AvatarFileBytes)
                {
                    return Json(new { status = true });  //  , X_DOVE_XSRF_TOKEN 
                }
            }
            await System.IO.File.WriteAllBytesAsync(X_DOVEValues.AvatarStoragePath(_webHostEnv) + _UserId, AvatarBytes);
            return Json(new { status = true });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public IActionResult KeepLogin()
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }


        #endregion

    }
}
