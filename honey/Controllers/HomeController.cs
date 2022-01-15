using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using honey.Models;
using honey.Data;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace honey.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger,ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            await All();

            var Last12Product = await _context.Products.Include(i=>i.Images).Take(12).ToListAsync();
            
            return View(Last12Product);
        }

        public async Task<IActionResult> Products(int? page)
        {
            await All();

            var Products = _context.Products.Include(i => i.Images);

            return View(Products.OrderByDescending(i => i.Date).ToPagedList(page ?? 1, 12));
        }

        public async Task<IActionResult> Details(int? id)
        {
            await All();
            var product = await _context.Products
                .Include(p => p.Currency)
                .FirstOrDefaultAsync(m => m.ID == id);
            product.Images = await _context.Images.Where(i => i.ProductID == id).ToListAsync();
            product.Phones = await _context.Phones.Where(i => i.ProductID == id).ToListAsync();



            return View(product);
        }

        // To Get Last Three Product In Footer
        public async Task All()
        {
            ViewData["Last3Product"] = await _context.Products.Take(3).ToListAsync();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
