using CMSOnlineStore.Models.Data;
using CMSOnlineStore.Models.WievModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.IO;

namespace CMSOnlineStore.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop (Урок 18)
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages");
        }


        public ActionResult CategoryMenuPartial()
        {
            // Объявляем модель типа лист CategoryVM
            List<CategoryVM> categoryVMList;

            // Инициализируем модель данными
            using (Db db = new Db())
            {
                categoryVMList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();
            }

            // Возвращаем частичное представление с моделью
            return PartialView("_CategoryMenuPartial", categoryVMList);
        }

        // GET: Shop/category/name (Урок 19)
        [HttpGet]
        public ActionResult Category (string name)
        {
            // Объявляем список типа List<ProductVM>
            List<ProductVM> productVMList;

            using (Db db = new Db())
            {
                // Получаем ID категории
                CategoryDTO categoryDTO = db.Categories.Where(x => x.Slug == name).FirstOrDefault();

                int catId = categoryDTO.Id;

                // инициализируем список данными
                productVMList = db.Products.ToArray().Where(x => x.CategoryId == catId).Select(x => new ProductVM(x)).ToList();


                // Получаем имя категории
                var productCat = db.Products.Where(x => x.CategoryId == catId).FirstOrDefault();

                // Делаем проверку категории на NULL
                if (productCat == null)
                {
                    var catName = db.Categories.Where(x => x.Slug == name).Select(x => x.Name).FirstOrDefault().ToString();
                    ViewBag.CategoryName = catName;
                }
                else
                {
                    ViewBag.CategoryName = productCat.CategoryName;
                }
            }
            // Возвращаем представление с моделью
            return View(productVMList);
        }

        // GET: Shop/product-details/name (Урок 19)
        [HttpGet]
        // Добавляем другое имя контроллера
        [ActionName("product-details")]
        public ActionResult ProductDetails (string name)
        {
            // Объявляем модели VM и DTO
            ProductVM model;
            ProductDTO dto;

            // Инициализируем ID продукта
            int id = 0;

            using (Db db = new Db())
            {
                // Проверяем, доступен ли продукт
                if (! db.Products.Any(x => x.Slug.Equals(name)))
                {
                    return RedirectToAction("Index", "Shop");
                }

                // Инициализируем модель productDTO
                dto = db.Products.Where(x => x.Slug == name).FirstOrDefault();

                // Получаем ID
                id = dto.Id;

                // Инициализируем модель данными
                model = new ProductVM(dto);
            }
            // Получаем изображения из галереи
            model.GalleryImages = Directory
                    .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                    .Select(fn => Path.GetFileName(fn));

            // Возвращаем модель и представление
            return View("ProductDetails", model);
        }
    }
}