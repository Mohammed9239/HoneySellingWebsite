using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using honey.Areas.Admin.Models;
using honey.Data;
using Microsoft.AspNetCore.Authorization;

namespace honey.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CurrenciesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CurrenciesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Currencies
        public async Task<IActionResult> Index()
        {
            await All();

            return View(await _context.Currencies.ToListAsync());
        }

        // GET: Admin/Currencies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            await All();

            if (id == null)
            {
                return NotFound();
            }

            var currency = await _context.Currencies
                .FirstOrDefaultAsync(m => m.ID == id);
            if (currency == null)
            {
                return NotFound();
            }

            return View(currency);
        }

        // GET: Admin/Currencies/Create
        public async Task<IActionResult> Create()
        {
            await All();

            return View();
        }

        // POST: Admin/Currencies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name")] Currency currency)
        {
            await All();

            if (ModelState.IsValid)
            {
                _context.Add(currency);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(currency);
        }

        // GET: Admin/Currencies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            await All();

            if (id == null)
            {
                return NotFound();
            }

            var currency = await _context.Currencies.FindAsync(id);
            if (currency == null)
            {
                return NotFound();
            }
            return View(currency);
        }

        // POST: Admin/Currencies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name")] Currency currency)
        {
            await All();

            if (id != currency.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(currency);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CurrencyExists(currency.ID))
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
            return View(currency);
        }

        // GET: Admin/Currencies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            await All();

            if (id == null)
            {
                return NotFound();
            }

            var currency = await _context.Currencies
                .FirstOrDefaultAsync(m => m.ID == id);
            if (currency == null)
            {
                return NotFound();
            }

            return View(currency);
        }

        // POST: Admin/Currencies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await All();
            var currency = await _context.Currencies.FindAsync(id);
            _context.Currencies.Remove(currency);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CurrencyExists(int id)
        {
            return _context.Currencies.Any(e => e.ID == id);
        }

        // To Get Last Three Product In Footer
        public async Task All()
        {
            ViewData["Last3Product"] = await _context.Products.Take(3).ToListAsync();
        }
    }
}
