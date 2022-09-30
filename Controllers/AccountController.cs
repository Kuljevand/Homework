using Homework1.Models.AccModels;
using Microsoft.AspNetCore.Mvc;
using Homework1.Models;
using Microsoft.AspNetCore.Identity;
using Homework1.Data;
using DataAccess.Services;
using Homework1.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Build.Framework;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol;
using System.Security.Cryptography.Pkcs;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Microsoft.Data.SqlClient;

namespace Homework1.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly SPaPSContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSenderEnhance _emailService;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, SPaPSContext context, IEmailSenderEnhance emailService, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _roleManager = roleManager;
            _emailService = emailService;
            _roleManager = roleManager;
            
        }

        [HttpGet]
        public IActionResult Login()
        {

            if (TempData["Success"] != null)
                ModelState.AddModelError("Success", Convert.ToString(TempData["Success"]));

            return View();
        }

        [HttpPost]
        
        public async Task<IActionResult> Login(LoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(userName: model.Email, password: model.Password, isPersistent: false, lockoutOnFailure: true);

            if (!result.Succeeded || result.IsLockedOut || result.IsNotAllowed)
            {
                ModelState.AddModelError("Error", "Погрешно корисничко име или лозинка!");
                return View(model);
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }






        
        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.ClientTypes = new SelectList(_context.References.Where(x => x.ReferenceTypeId == 1).ToList(), "ReferenceId", "Description");
            ViewBag.Cities = new SelectList(_context.References.Where(x => x.ReferenceTypeId == 2).ToList(), "ReferenceId", "Description");
            ViewBag.Countries = new SelectList(_context.References.Where(x => x.ReferenceTypeId == 3).ToList(), "ReferenceId", "Description");

            ViewBag.Roles = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");

            ViewBag.Activities = new SelectList(_context.Activities.ToList(), "ActivityId", "Name");





            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            

            if (userExists != null)
            {
                ModelState.AddModelError("Error", "Корисникот веќе постои!");
                return View(model);
            }


            IdentityUser user = new IdentityUser()
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            var newPassword = Shared.GeneratePassword(8);

            var createUser = await _userManager.CreateAsync(user, newPassword);

            if (!createUser.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            await _userManager.AddToRoleAsync(user, model.Role);


            Client client = new Client()
            {
                UserId = user.Id,
                Name = model.Name,
                Address = model.Address,
                IdNo = model.IdNo,
                ClientTypeId = model.ClientTypeId,
                CityId = model.CityId,
                CountryId = model.CountryId,
                DateEstablished = DateTime.Now,
               /* NumberOfEmployees = model.NumberOfEmployees */
                                
                
            };

            await _context.Clients.AddAsync(client);

            await _context.SaveChangesAsync();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var callback = Url.Action(action: "ResetPassword", controller: "Account", values: new { token, email = user.Email }, HttpContext.Request.Scheme);

            EmailSetUp emailSetUp = new EmailSetUp()
            {
                To = user.Email,
                Template = "Register",
                Username = user.Email,
                Callback = callback,
                Token = token,
                RequestPath = _emailService.PostalRequest(Request),
            };

            await _emailService.SendEmailAsync(emailSetUp);

            TempData["Success"] = "Успешно креиран корисник!";

            return RedirectToAction(nameof(Login));
        }



     
        
        
        
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email); 

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var callback = Url.Action(action: "ResetPassword", controller: "Account", values: new { token, email = user.Email }, HttpContext.Request.Scheme);

            EmailSetUp emailSetUp = new EmailSetUp()
            {
                To = user.Email,
                Template = "ResetPassword",
                Username = user.Email,
                Callback = callback,
                Token = token,
                RequestPath = _emailService.PostalRequest(Request),
            };

            await _emailService.SendEmailAsync(emailSetUp);

            TempData["Success"] = "Ве молиме проверете го вашето влезно сандаче!";

            return RedirectToAction(nameof(Login));
        }

        
        
        
        
        
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {

            ResetPasswordModel model = new ResetPasswordModel()
            {
                Email = email,
                Token = token
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {

            var user = await _userManager.FindByEmailAsync(model.Email);

            var resetPassword = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (!resetPassword.Succeeded)
            {
                ModelState.AddModelError("Error", "Се случи грешка. Обидете се повторно!");
                return View();
            }

            TempData["Success"] = "Успешно промената лозинка!";

            return RedirectToAction(nameof(Login));
        }

       
        
        
        
        
        
        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            var loggedInUserEmail = User.Identity.Name;

            var user = await _userManager.FindByEmailAsync(loggedInUserEmail);

            var changePassword = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (!changePassword.Succeeded)
            {
                ModelState.AddModelError("Error", "Се случи грешка. Обидете се повторно!");
                return View();
            }

            ModelState.AddModelError("Success", "Успешно променета лозинка!");

            return View();
        }





        [Authorize]
        [HttpGet]

        public async Task<IActionResult> ChangeUserInfo()
        {
            var loggedInUserEmail = User.Identity.Name;

            var applicationUser = await _userManager.FindByEmailAsync(loggedInUserEmail);

            var clientUser = await _context.Clients.Where(x => x.UserId == applicationUser.Id).FirstOrDefaultAsync();

            ViewBag.ClientTypes = new SelectList(_context.References.Where(x => x.ReferenceTypeId == 1).ToList(), "ReferenceId", "Description");


            ViewBag.Cities = new SelectList(_context.References.Where(x => x.ReferenceTypeId == 2).ToList(), "ReferenceId", "Description");


            ViewBag.Countries = new SelectList(_context.References.Where(x => x.ReferenceTypeId == 3).ToList(), "ReferenceId", "Description");

            
            ViewBag.Activities = new SelectList(_context.Activities.ToList(), "ActivityId", "Name");


            ChangeUserInfo model = new ChangeUserInfo()
            {

            Name = clientUser.Name,
            Address = clientUser.Address,
            IdNo = clientUser.IdNo,
            CountryId = (int)clientUser.CountryId,
            CityId = clientUser.CityId,
            ClientTypeId = clientUser.ClientTypeId,
            PhoneNumber = applicationUser.PhoneNumber,
          /*DateEstablished = DateTime.Now, 
            NumberOfEmployees = model.NumberOfEmployees,
            Activities = model.Activities*/


            };     

            return View(model);
        }


        [Authorize]
        [HttPost]
        public async Task<IActionResult> ChangeUserInfo(ChangeUserInfo model)
        {

            ViewBag.ClientTypes = new SelectList(_context.References.Where(x => x.ReferenceTypeId == 1).ToList(), "ReferenceId", "Description");
            ViewBag.Cities = new SelectList(_context.References.Where(x => x.ReferenceTypeId == 2).ToList(), "ReferenceId", "Description");
            ViewBag.Countries = new SelectList(_context.References.Where(x => x.ReferenceTypeId == 3).ToList(), "ReferenceId", "Description");
            ViewBag.Activities = new SelectList(_context.Activities.ToList(), "ActivityId", "Name");


            if (!ModelState.IsValid)
            {

                ModelState.AddModelError("Error", "Неуспешно променети информации, обидете се повторно!");

                return View(model);
            }

            var loggedInUserEmail = User.Identity.Name;

            var applicationUser = await _userManager.FindByEmailAsync(loggedInUserEmail);

            var clientUser = await _context.Clients.Where(x => x.UserId == applicationUser.Id).FirstOrDefaultAsync();

            applicationUser.PhoneNumber = model.PhoneNumber;

            var appUserResult = await _userManager.UpdateAsync(applicationUser);
            
            if (!appUserResult.Succeeded)
            {
                ModelState.AddModelError("Error", "Се случи грешка обидете се повторно!");

                return View(model);
            }

            clientUser.ClientId = 0;
            clientUser.Name = model.Name;
            clientUser.Address = model.Address;
            clientUser.CountryId = model.CountryId;
            clientUser.CityId = model.CityId;
            clientUser.IdNo = model.IdNo;
            clientUser.UpdatedOn = DateTime.Now;
          /*clientUser.DateEstablished = model.DateOfEstablishment,
            clientUser.NumberOfEmployees = model.NumberOfEmployees
            clientUser.ClientActivities = model */

            try
            {

                _context.Update(clientUser);
                await _context.SaveChangesAsync();
            
            }
            catch (Exception e)
            {

                ModelState.AddModelError("Error", "Се случи грешка обидете се повторно!");

                return View(model);

            }

            ModelState.AddModelError("Succeeded", "Успешно променети информации!");
            return View(model);
        }
        
        
        
        
        
        
        
        
        
        
        
        
        
        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }

    internal class HttPostAttribute : Attribute
    {
    }
}