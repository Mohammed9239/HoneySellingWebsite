using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using honey.Areas.Admin.Models;
using honey.Data;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace honey.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index()
        {
            await All();

            var applicationDbContext = _context.Products.Include(p => p.Currency);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            await All();

            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Currency)
                .FirstOrDefaultAsync(m => m.ID == id);
            product.Images = await _context.Images.Where(i => i.ProductID == id).ToListAsync();
            product.Phones = await _context.Phones.Where(i => i.ProductID == id).ToListAsync();

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Products/Create
        public async Task<IActionResult> Create()
        {
            await All();

            ViewData["CurrencyID"] = new SelectList(_context.Currencies, "ID", "Name");
            return View();
        }

        // POST: Admin/Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product , List<int> Phones , IFormFile fav , List<IFormFile> imgs)
        {
            await All();

            if (ModelState.IsValid)
            {
                product.Date = DateTime.Now;
                _context.Add(product);
                await _context.SaveChangesAsync();
                //add phones
                await AddPhones(Phones,product.ID);
                //add fav img
                await FavImg(fav, product.ID);
                //add multi img
                await MultiImg(imgs, product.ID);

                return RedirectToAction(nameof(Index));
            }
            ViewData["CurrencyID"] = new SelectList(_context.Currencies, "ID", "Name", product.CurrencyID);
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            await All();

            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.Include(i=>i.Currency).FirstOrDefaultAsync(i=>i.ID==id);
            product.Images = await _context.Images.Where(i => i.ProductID == id).ToListAsync();
            product.Phones = await _context.Phones.Where(i => i.ProductID == id).ToListAsync();
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CurrencyID"] = new SelectList(_context.Currencies, "ID", "Name", product.CurrencyID);
            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, List<int> Phones, IFormFile fav, List<IFormFile> imgs)
        {
            await All();

            var oldimgs = _context.Images.Where(i => i.ProductID == id).ToList();
            product.Images = await _context.Images.Where(i => i.ProductID == id).ToListAsync();
            product.Phones = await _context.Phones.Where(i => i.ProductID == id).ToListAsync();

            if (id != product.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //update phone
                    if (Phones != null)
                    {
                        var old = await _context.Phones.Where(i=>i.ProductID==id).ToListAsync();

                        for (int i = 0; i < Phones.Count; i++)
                        {
                            if (old.Count>0 && old.Count>i )
                            {
                                old[i].Number = Phones[i];
                                _context.Update(old[i]);
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                var _Phone = new Phone
                                {
                                    ProductID = product.ID,
                                    Number = Phones[i]
                                };
                                _context.Add(_Phone);
                                await _context.SaveChangesAsync();
                            }

                        }
                    }

                    //update fav img
                    if (fav != null)
                    {
                        //remove old img
                        var oldimg = oldimgs.FirstOrDefault(i => i.Url.StartsWith("fav"));
                        string oldpath = string.Format("{0}/wwwroot/Uploads/{1}",
                         Directory.GetCurrentDirectory(), oldimg.Url);
                        System.IO.File.Delete(oldpath);
                        //add new img
                        await FavImgUp(fav,oldimg);
                    }

                    //update other img
                    if (imgs.Count>0)
                    {
                       var oldimg = oldimgs.Where(i => !i.Url.StartsWith("fav")).ToList();
                        //remove old img
                        foreach (var item in oldimg)
                        {
                            string oldpath = string.Format("{0}/wwwroot/uploads/{1}",
                             Directory.GetCurrentDirectory(), item.Url);
                            System.IO.File.Delete(oldpath);
                            _context.Images.Remove(item);
                            await _context.SaveChangesAsync();
                        }
                        //add new img
                       await MultiImg(imgs, product.ID);

                    }

                     _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ID))
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
            ViewData["CurrencyID"] = new SelectList(_context.Currencies, "ID", "Name", product.CurrencyID);
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            await All();
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Currency)
                .FirstOrDefaultAsync(m => m.ID == id);
            product.Images = await _context.Images.Where(i => i.ProductID == id).ToListAsync();
            product.Phones = await _context.Phones.Where(i => i.ProductID == id).ToListAsync();
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await All();
            var product = await _context.Products.FindAsync(id);
            var imgs = _context.Images.Where(i => i.ProductID == id).ToList();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            //remove img from server
            foreach (var item in imgs)
            {
                string oldpaths = string.Format("{0}/wwwroot/Uploads/{1}",
                 Directory.GetCurrentDirectory(), item.Url);
                System.IO.File.Delete(oldpaths);

            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ID == id);
        }



        //Add Phones
        public async Task AddPhones(List<int> Phones, int ID)
        {
            if (Phones != null)
            {
                foreach (var Phone in Phones)
                {
                    var _Phone = new Phone
                    {
                        ProductID =ID,
                        Number = Phone
                    };
                     _context.Phones.Add(_Phone);
                    await _context.SaveChangesAsync();
                }
            }
        }
        // To Get Last Three Product In Footer
        public async Task All()
        {
            ViewData["Last3Product"] = await _context.Products.Take(3).ToListAsync();
        }
        //Add FavImg
        public async Task FavImg(IFormFile fav,int ID)
        {
            if (Request.Form.Files.Count > 0 && Request.Form.Files[0].Length > 0)
            {
                var file = fav;
                string[] str = file.FileName.Split('.');
                string ext = str[str.Length - 1];
                string filename = string.Format("{0}.{1}", "fav" + Guid.NewGuid().ToString(), ext);
                string path = string.Format("{0}/wwwroot/Uploads/{1}",
                    Directory.GetCurrentDirectory(), filename);
                using (var streem = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(streem);
                }
                var img = new Image
                {
                    ProductID = ID,
                    Url = filename
                };
                 _context.Images.Add(img);
                await _context.SaveChangesAsync();
            }
        }
        //update FavImg
        public async Task FavImgUp(IFormFile fav,Image img)
        {
            if (Request.Form.Files.Count > 0 && Request.Form.Files[0].Length > 0)
            {
                var file = fav;
                string[] str = file.FileName.Split('.');
                string ext = str[str.Length - 1];
                string filename = string.Format("{0}.{1}", "fav" + Guid.NewGuid().ToString(), ext);
                string path = string.Format("{0}/wwwroot/Uploads/{1}",
                    Directory.GetCurrentDirectory(), filename);
                using (var streem = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(streem);
                }
                img.Url = filename;
                _context.Images.Update(img);
                await _context.SaveChangesAsync();
            }
        }
        //UploadMultiImg
        public async Task MultiImg(List<IFormFile> Imgs,int ID)
        {
            if (Request.Form.Files.Count > 0 && Request.Form.Files[0].Length > 0)
            {
                if (Imgs != null)
                {
                    foreach (var Img in Imgs)
                    {
                        if (Img.Length > 0)
                        {
                            var files = Img;
                            string[] strs = files.FileName.Split('.');
                            string exts = strs[strs.Length - 1];
                            string filenames = string.Format("{0}.{1}", Guid.NewGuid().ToString(), exts);
                            string paths = string.Format("{0}/wwwroot/uploads/{1}",
                                Directory.GetCurrentDirectory(), filenames);
                            using (var streem = new FileStream(paths, FileMode.Create))
                            {
                                files.CopyTo(streem);
                            }
                            var _imgs = new Image
                            {
                                ProductID = ID,
                                Url = filenames
                            };
                             _context.Images.Add(_imgs);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }
        }
    }
}
