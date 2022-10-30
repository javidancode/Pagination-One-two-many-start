using Entity_Frame_Work_Project.Data;
using Entity_Frame_Work_Project.Helpers;
using Entity_Frame_Work_Project.Models;
using Entity_Frame_Work_Project.ViewModels.ProductViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entity_Frame_Work_Project.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, int take = 3)
        {
            List<Product> products = await _context.Products
                .Include(m => m.ProductImages)
                .Include(m => m.Category)
                .Skip((page * take) - take)
                .Take(take)
                //.OrderByDescending(m => m.Id)
                .ToListAsync();

            List<ProductListVM> mapDatas = GetDatasMap(products);

            int count = await GetPageCount(take);

            Paginate<ProductListVM> result = new Paginate<ProductListVM>(mapDatas, page, count);

            return View(result);
        }

        private async Task<int> GetPageCount(int take)
        {
            int productCount = await _context.Products.Where(m => !m.IsDeleted).CountAsync();

            return (int)Math.Ceiling((decimal)productCount / take);
        }

        private List<ProductListVM> GetDatasMap(List<Product> products)
        {
            List<ProductListVM> productList = new List<ProductListVM>();

            foreach (var producs in products)
            {
                ProductListVM newProduct = new ProductListVM
                {
                    Id = producs.Id,
                    Title = producs.Title,
                    Desc = producs.Desc,
                    Price = producs.Price,
                    MainImage = producs.ProductImages.Where(m=> m.IsMain).FirstOrDefault()?.Image,
                    CategoryName = producs.Category.Name
                };

                productList.Add(newProduct);
            }

            return productList;
        }
    }
}
