﻿using DataLayer.Models;
using DataLayer.Security;
using DataLayer.Utilities;
using DataLayer.ViewModels;
using GoogleReCaptcha.V3.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GhasreMobile.Controllers
{
    public class AccountController : Controller
    {
        private Core db = new Core();
        private readonly ICaptchaValidator _captchaValidator;

        public AccountController(ICaptchaValidator captchaValidator)
        {
            _captchaValidator = captchaValidator;
        }

        // Login
        [Route("Login")]
        public async Task<IActionResult> Login()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    if (User.Claims.Last().Value == "user")
                    {
                        return Redirect("/User/Profile");
                    }
                    else if (User.Claims.Last().Value == "employee" || User.Claims.Last().Value == "admin")
                    {
                        return Redirect("/Admin");
                    }
                }
                return await Task.FromResult(View());
            }
            catch (Exception)
            {
                return await Task.FromResult(Redirect("404.html"));
            }

        }
        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginVm login, string ReturnUrl = "/")
        {
            try
            {
                if (!await _captchaValidator.IsCaptchaPassedAsync(login.Captcha))
                {
                    ModelState.AddModelError("TellNo", "لطفا دوباره امتحان کنید");
                    return View(login);
                }
                if (ModelState.IsValid)
                {
                    string password = PasswordHelper.EncodePasswordMd5(login.Password);
                    if (db.Client.Get().Any(i => i.TellNo == login.TellNo && i.Password == password))
                    {
                        TblClient user = db.Client.Get().FirstOrDefault(i => i.TellNo == login.TellNo);
                        if (user.IsActive)
                        {
                            var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier,user.ClientId.ToString()),
                        new Claim(ClaimTypes.Name,user.TellNo),
                        new Claim(ClaimTypes.Role,db.Role.GetById(user.RoleId).Name.Trim()),
                    };
                            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var principal = new ClaimsPrincipal(identity);

                            var properties = new AuthenticationProperties
                            {
                                IsPersistent = login.RememberMe
                            };
                            await HttpContext.SignInAsync(principal, properties);
                            ViewBag.IsSuccess = true;
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            ModelState.AddModelError("TellNo", "حساب کاربری شما فعال نیست");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("TellNo", "شماره تلفن  یا رمز اشتباه است");
                    }
                }
                return await Task.FromResult(View(login));
            }
            catch
            {
                return await Task.FromResult(Redirect("404.html"));
            }

        }
        // Register
        [Route("Register")]
        public async Task<IActionResult> Register()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    if (User.Claims.Last().Value == "user")
                    {
                        return Redirect("/User/Profile");
                    }
                    else if (User.Claims.Last().Value == "employee" || User.Claims.Last().Value == "admin")
                    {
                        return Redirect("/Admin");
                    }
                }
                return await Task.FromResult(View());
            }
            catch (Exception)
            {
                return await Task.FromResult(Redirect("404.html"));
            }
        }
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> RegisterAsync(RegisterVm register)
        {
            try
            {
                if (!await _captchaValidator.IsCaptchaPassedAsync(register.Captcha))
                {
                    ModelState.AddModelError("TellNo", "لطفا دوباره امتحان کنید");
                    return View(register);
                }
                if (ModelState.IsValid)
                {
                    if (!db.Client.Get().Any(i => i.TellNo == register.TellNo))
                    {
                        var CodeCreator = Guid.NewGuid().ToString();
                        string Code = CodeCreator.Substring(CodeCreator.Length - 5);
                        TblClient addUser = new TblClient();
                        addUser.DateCreated = DateTime.Now;
                        addUser.Auth = Code;
                        addUser.RoleId = 1;
                        addUser.Balance = 0;
                        addUser.IsActive = false;
                        addUser.Password = PasswordHelper.EncodePasswordMd5(register.Password);
                        addUser.TellNo = register.TellNo;
                        addUser.Name = register.Name;
                        db.Client.Add(addUser);
                        db.Save();
                        string message = addUser.Auth;
                        await Sms.SendSms(addUser.TellNo, message, "GhasrMobileRegister");
                        return await Task.FromResult(Redirect("/Verify/" + addUser.TellNo));
                    }
                    else
                    {
                        ModelState.AddModelError("TellNo", "شماره تلفن تکراریست");
                    }
                }
                return await Task.FromResult(View(register));
            }
            catch
            {
                return await Task.FromResult(Redirect("404.html"));
            }

        }
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword()
        {
            try
            {
                return await Task.FromResult(View());
            }
            catch (Exception)
            {
                return await Task.FromResult(Redirect("404.html"));
            }

        }
        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordVm forget)
        {
            try
            {
                if (!await _captchaValidator.IsCaptchaPassedAsync(forget.Captcha))
                {
                    ModelState.AddModelError("TellNo", "لطفا دوباره امتحان کنید");
                    return View(forget);
                }
                if (ModelState.IsValid)
                {
                    if (db.Client.Get().Any(i => i.TellNo == forget.TellNo))
                    {
                        TblClient forgotPassword = db.Client.Get().FirstOrDefault(i => i.TellNo == forget.TellNo);
                        string message = forgotPassword.Auth;
                        await Sms.SendSms(forgotPassword.TellNo, message, "GhasrMobileForget");
                        return await Task.FromResult(Redirect("/RestorePassword/" + forgotPassword.TellNo));
                    }
                    else
                    {
                        ModelState.AddModelError("TellNo", "شماره تلفن یافت نشد");
                    }
                }
                return await Task.FromResult(View(forget));
            }
            catch
            {
                return await Task.FromResult(Redirect("404.html"));
            }
        }
        [Route("RestorePassword/{tell}")]
        public async Task<IActionResult> RestorePassword(string tell)
        {
            try
            {
                return await Task.FromResult(View(new ActiveVm()
                {
                    Tell = tell
                }));
            }
            catch (Exception)
            {
                return await Task.FromResult(Redirect("404.html"));
            }
        }
        [HttpPost]
        [Route("RestorePassword/{tell}")]
        public async Task<IActionResult> RestorePasswordAsync(ActiveVm active)
        {
            try
            {
                if (!await _captchaValidator.IsCaptchaPassedAsync(active.Captcha))
                {
                    ModelState.AddModelError("Auth", "ورود غیر مجاز");
                    return View(active);
                }
                if (ModelState.IsValid)
                {
                    if (db.Client.Get().Any(i => i.TellNo == active.Tell))
                    {
                        TblClient selectedUser = db.Client.Get().FirstOrDefault(i => i.TellNo == active.Tell);
                        if (selectedUser.Auth == active.Auth)
                        {
                            return await Task.FromResult(Redirect("/ChangePassword/" + selectedUser.TellNo + "/" + selectedUser.Auth));
                        }
                        else
                        {
                            ModelState.AddModelError("Auth", "کد تغیر رمز  اشتباه است");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Auth", "شماره تلفن یافت نشد");
                    }
                }
                return await Task.FromResult(View(active));
            }
            catch
            {
                return await Task.FromResult(Redirect("404.html"));
            }
        }
        [Route("ChangePassword/{tell}/{auth}")]
        public async Task<IActionResult> ChangePassword(string tell, string auth)
        {
            try
            {
                return await Task.FromResult(View(new ChangePasswordVm()
                {
                    Tell = tell,
                    Auth = auth
                }));
            }
            catch (Exception)
            {
                return await Task.FromResult(Redirect("404.html"));
            }
           
        }
        [Route("ChangePassword/{tell}/{auth}")]
        [HttpPost]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordVm change)
        {
            try
            {

                if (!await _captchaValidator.IsCaptchaPassedAsync(change.Captcha))
                {
                    ModelState.AddModelError("Password", "ورود غیر مجاز");
                    return View(change);
                }
                if (ModelState.IsValid)
                {
                    if (db.Client.Get().Any(i => i.TellNo == change.Tell && i.Auth == change.Auth))
                    {
                        TblClient selectedUser = db.Client.Get().FirstOrDefault(i => i.TellNo == change.Tell);
                        selectedUser.Password = PasswordHelper.EncodePasswordMd5(change.Password);
                        var CodeCreator = Guid.NewGuid().ToString();
                        string Code = CodeCreator.Substring(CodeCreator.Length - 5);
                        selectedUser.Auth = Code;
                        db.Client.Update(selectedUser);
                        db.Save();
                        return await Task.FromResult(Redirect("/Login?ChangePassword=true"));
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "شماره تلفن یا کد فعال سازی اشتباه است");
                    }
                }
                return await Task.FromResult(View(change));
            }
            catch
            {
                return await Task.FromResult(Redirect("404.html"));
            }

        }
        [Route("Verify/{tellNo}")]
        public async Task<IActionResult> Verify(string tellNo)
        {
            try
            {
                return await Task.FromResult(View(new ActiveVm()
                {
                    Tell = tellNo
                }));
            }
            catch (Exception)
            {
                return await Task.FromResult(Redirect("404.html"));
            }
        }
        [Route("Verify/{tellNo}")]
        [HttpPost]
        public async Task<IActionResult> VerifyAsync(ActiveVm active)
        {
            try
            {

                if (!await _captchaValidator.IsCaptchaPassedAsync(active.Captcha))
                {
                    ModelState.AddModelError("Auth", "ورود غیر مجاز");
                    return View(active);
                }
                if (ModelState.IsValid)
                {
                    if (db.Client.Get().Any(i => i.TellNo == active.Tell))
                    {
                        TblClient selectedUser = db.Client.Get().FirstOrDefault(i => i.TellNo == active.Tell);
                        if (selectedUser.Auth == active.Auth)
                        {
                            selectedUser.IsActive = true;
                            var CodeCreator = Guid.NewGuid().ToString();
                            string Code = CodeCreator.Substring(CodeCreator.Length - 5);
                            selectedUser.Auth = Code;
                            db.Save();
                            return await Task.FromResult(Redirect("/Login?Active=true"));
                        }
                        else
                        {
                            ModelState.AddModelError("Auth", "کد فعال سازی اشتباه است");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Auth", "شماره تلفن یافت نشد");
                    }
                }
                return await Task.FromResult(View(active));
            }
            catch
            {
                return await Task.FromResult(Redirect("404.html"));
            }
        }
        [Route("LogOut")]
        public async Task<IActionResult> LogOut()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return await Task.FromResult(Redirect("/"));
            }
            catch
            {
                return await Task.FromResult(Redirect("404.html"));
            }
        }
    }
}
