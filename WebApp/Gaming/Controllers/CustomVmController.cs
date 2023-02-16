using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Gaming.Data;
using Gaming.Models;
using Gaming.Services;

namespace Gaming.Controllers
{
    public class CustomVmController : Controller
    {
        private readonly ApplicationDbContext _context;
        AzureVmService azureVmService = new();

        public CustomVmController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CustomVm
        public async Task<IActionResult> Index()
        {
              return _context.CustomVms != null ? 
                          View(await _context.CustomVms.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.CustomVms'  is null.");
        }

        // GET: CustomVm/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.CustomVms == null)
            {
                return NotFound();
            }

            var customVm = await _context.CustomVms
                .FirstOrDefaultAsync(m => m.Login == id);
            if (customVm == null)
            {
                return NotFound();
            }

            return View(customVm);
        }

        // GET: CustomVm/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CustomVm/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Login,Password")] CustomVm customVm)
        {
            if (!ModelState.IsValid) return View(customVm);
            
            await azureVmService.CreateAzureVm(customVm);
            _context.Add(customVm);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: CustomVm/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.CustomVms == null)
            {
                return NotFound();
            }

            var customVm = await _context.CustomVms.FindAsync(id);
            if (customVm == null)
            {
                return NotFound();
            }
            return View(customVm);
        }

        // POST: CustomVm/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Login,Password,Name,Ip,IsStarted")] CustomVm customVm)
        {
            if (id != customVm.Login)
            {
                return NotFound();
            }

            if (!ModelState.IsValid) return View(customVm);
            try
            {
                _context.Update(customVm);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomVmExists(customVm.Login))
                {
                    return NotFound();
                }

                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: CustomVm/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.CustomVms == null)
            {
                return NotFound();
            }

            var customVm = await _context.CustomVms
                .FirstOrDefaultAsync(m => m.Login == id);
            if (customVm == null)
            {
                return NotFound();
            }

            return View(customVm);
        }

        // POST: CustomVm/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.CustomVms == null)
            {
                return Problem("Entity set 'ApplicationDbContext.CustomVms'  is null.");
            }
            var customVm = await _context.CustomVms.FindAsync(id);
            await azureVmService.DeleteAzureVm(id);
            if (customVm != null)
            {
                _context.CustomVms.Remove(customVm);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        /// <summary>
        /// Start Azure VM
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> StartVm(string id)
        {
            var customVm = await _context.CustomVms
                .FirstOrDefaultAsync(m => m.Name == id);
            if (customVm == null)
            {
                return NotFound();
            }
            
            azureVmService.StartAzureVm(id);
            customVm.IsStarted = true;
            _context.Update(customVm);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> StopVm(string id)
        {
            var customVm = await _context.CustomVms
                .FirstOrDefaultAsync(m => m.Name == id);
            if (customVm == null)
            {
                return NotFound();
            }
            
            azureVmService.StopAzureVm(id);
            customVm.IsStarted = false;
            _context.Update(customVm);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool CustomVmExists(string id)
        {
          return (_context.CustomVms.Any(e => e.Login == id));
        }
    }
}
