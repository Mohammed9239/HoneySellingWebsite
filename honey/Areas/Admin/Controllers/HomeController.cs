using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using honey.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace honey.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            await All();
            return View();
        }

        // To Get Last Three Product In Footer
        public async Task All()
        {
            ViewData["Last3Product"] = await _context.Products.Take(3).ToListAsync();
        }
    }
}