using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Homework1.Data;
using Homework1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Homework1.Models.CustomModels;

namespace Homework1.Controllers
{
    [Authorize(Roles = "Админ")]
    public class ServicesController : Controller
    {
        private readonly SPaPSContext _context;

        public ServicesController(SPaPSContext context)
        {
            _context = context;
        }

        #region Index/Details
        // GET: Services
        public async Task<IActionResult> Index()
        {
            List<vm_Service> model = await _context.Services.Include(x => x.ServiceActivities)
            .Select(x => new vm_Service()
            {
            ServiceId = x.ServiceId,
            Description = x.Description,
            Activities = String.Join("; ", x.ServiceActivities.Select(a => a.Activity.Name).ToList())
            }).ToListAsync();

            return View(model);
        }

        // GET: Services/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Services == null)
            {
                return NotFound();
            }

            var service = await _context.Services
                .FirstOrDefaultAsync(m => m.ServiceId == id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }
        #endregion

        #region Create
        // GET: Services/Create
        public IActionResult Create()
        {
            ViewBag.Activities = new SelectList(_context.Activities.ToList(), "ActivityId", "Name");
            return View();
        }

        // POST: Services/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(vm_Service model)
        {
            if (ModelState.IsValid)
            {
                Service service = new Service()
                {
                    Description = model.Description,
                    CreatedOn = DateTime.Now,
                    CreatedBy = 1
                };

                _context.Services.Add(service);
                await _context.SaveChangesAsync();

                List<ServiceActivity> serviceActivities = new List<ServiceActivity>();

                foreach (var item in model.ActivityIds)
                {
                    ServiceActivity sa = new ServiceActivity()
                    {
                        ServiceId = service.ServiceId,
                        ActivityId = item
                    };

                    serviceActivities.Add(sa);
                }

                await _context.ServiceActivities.AddRangeAsync(serviceActivities);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        #endregion

        #region Edit
        // GET: Services/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Services == null)
            {
                return NotFound();
            }

            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }
            return View(service);
        }

        // POST: Services/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("ServiceId,Description,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy,IsActive")] Service service)
        {
            if (id != service.ServiceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(service);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceExists(service.ServiceId))
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
            return View(service);
        }
        #endregion

        #region Delete
        // GET: Services/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.Services == null)
            {
                return NotFound();
            }

            var service = await _context.Services
                .FirstOrDefaultAsync(m => m.ServiceId == id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        // POST: Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.Services == null)
            {
                return Problem("Entity set 'SPaPSContext.Services'  is null.");
            }
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                _context.Services.Remove(service);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        private bool ServiceExists(long id)
        {
            return _context.Services.Any(e => e.ServiceId == id);
        }
    }
}