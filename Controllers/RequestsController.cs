using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Homework1.Data;
using Homework1.Models;
using DataAccess.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using NuGet.Common;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using NuGet.Protocol.Plugins;

namespace Homework1.Controllers
{
    [Authorize]
    public class RequestsController : Controller
    {
        private readonly SPaPSContext _context;
        private readonly IEmailSenderEnhance _emailService;
        private readonly UserManager<IdentityUser> _userManager;

        public RequestsController(SPaPSContext context, IEmailSenderEnhance emailService, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _emailService = emailService;
            _userManager = userManager;
        }

        #region Index/Details
        // GET: Requests
        public async Task<IActionResult> Index()
        {
            var sPaPSContext = _context.Requests.Include(r => r.Service);
            return View(await sPaPSContext.ToListAsync());
        }

        // GET: Requests/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Requests == null)
            {
                return NotFound();
            }

            var request = await _context.Requests
                .Include(r => r.Service)
                .FirstOrDefaultAsync(m => m.RequestId == id);
            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }
        #endregion

        #region Create
        // GET: Requests/Create
        
        public IActionResult Create()
        {
            ViewData["Services"] = new SelectList(_context.Services.ToList(), "ServiceId", "Description");
            ViewData["BuildingTypes"] = new SelectList(_context.References.Where(x => x.ReferenceTypeId == 5).ToList(), "ReferenceId", "Description");



            Models.Request model = new Request()
            {
                RequestDate = DateTime.Now
            };


            return View(model);
        }

        // POST: Requests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Корисник, Админ")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Request request)
        {

            if (!ModelState.IsValid)
            {
                ViewData["Services"] = new SelectList(_context.Services.ToList(), "ServiceId", "Description");
                ViewData["BuildingTypes"] = new SelectList(_context.References.Where(x => x.ReferenceTypeId == 5).ToList(), "ReferenceId", "Description");

                return View(request);
            }

            request.RequestDate = DateTime.Now;
            request.CreatedOn = DateTime.Now;
            request.CreatedBy = 1;
            request.IsActive = true;


            _context.Add(request);
            await _context.SaveChangesAsync();

            var service = _context.Services.Find(request.ServiceId);

            var serviceActivitiesIds = _context.ServiceActivities.Where(x => x.ServiceId == request.ServiceId).Select(x => x.ActivityId).ToList();

            var clientIds = _context.ClientActivities
                                    .Where(x => serviceActivitiesIds.Contains(x.ActivityId))
                                    .Select(x => x.ClientId)
                                    .Distinct()
                                    .ToList();




            foreach (var item in clientIds)
            {
                var client = _context.Clients.Find(item);
                var user = await _userManager.FindByIdAsync(client.UserId);

                if (await _userManager.IsInRoleAsync(user, "Изведувач"))
                {

                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var callback = Url.Action(action: "Edit", controller: "Requests", values: new { id = request.RequestId }, HttpContext.Request.Scheme);

                    EmailSetUp emailSetUp = new EmailSetUp()
                    {
                        To = user.Email,
                        Template = "NewRequest",
                        RequestPath = _emailService.PostalRequest(Request),
                        Callback = callback
                    };

                    await _emailService.SendEmailAsync(emailSetUp);
                }
            }
            return RedirectToAction(nameof(Index));
        }
        #endregion
       
        #region Edit
        // GET: Requests/Edit/5
        [HttpGet]
        [Authorize(Roles = "Изведувач")]
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Requests == null)
            {
                return NotFound();
            }

            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceId", request.ServiceId);
            return View(request);
            ViewData["BuildingTypeId"] = new SelectList(_context.References.Where(x => x.ReferenceTypeId == 5).ToList(),"ReferenceId", "Description");

            var iDs = request.ServiceId;
            ViewBag.iDs = iDs;
            return View(request);

        }

        // POST: Requests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Изведувач")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("RequestId,RequestDate,ServiceId,BuildingTypeId,BuildingSize,FromDate,ToDate,Color,NoOfWindows,NoOfDoors,Note,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy,IsActive")] Request request)
        {
            if (id != request.RequestId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    /* var updateReq = _context.Requests.Find(request.RequestId);
                   if (updateReq.ContractorId != null) 
                   {
                       ViewData["ServiceId"] = new SelectList(_context.ServiceId,"ServiceId", "Description");
                       ViewData["BuildingTypeId"] = new SelectList(_context.References.Where(x => x.ReferenceTypeId == 5).ToList(), "ReferenceId", "Description");
                   }*/
                    _context.Update(request);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequestExists(request.RequestId))
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
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceId", request.ServiceId);
            return View(request);
        }
        #endregion

        #region Delete
        // GET: Requests/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.Requests == null)
            {
                return NotFound();
            }

            var request = await _context.Requests
                .Include(r => r.Service)
                .FirstOrDefaultAsync(m => m.RequestId == id);
            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        // POST: Requests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.Requests == null)
            {
                return Problem("Entity set 'SPaPSContext.Requests'  is null.");
            }
            var request = await _context.Requests.FindAsync(id);
            if (request != null)
            {
                _context.Requests.Remove(request);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        private bool RequestExists(long id)
        {
          return _context.Requests.Any(e => e.RequestId == id);
        }
    }

}
