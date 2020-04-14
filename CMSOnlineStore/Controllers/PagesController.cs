using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSOnlineStore.Models.Data;
using CMSOnlineStore.Models.WievModels.Pages;

namespace CMSOnlineStore.Controllers
{
    public class PagesController : Controller
    {
        // Создаём метод Index (урок 17)
        // GET: Index/{page}
        public ActionResult Index(string page = "")
        {
            // Получаем/устанавливаем краткий заголовок (slug)
            if (page == "")
                page = "home";

            // Инициализируем модель и DTO
            PageVM model;
            PagesDTO dto;

            // Проверяем, доступна ли страница
            using (Db db = new Db())
            {
                if (!db.Pages.Any(x => x.Slug.Equals(page)))
                {
                    return RedirectToAction("Index", new { page = "" });
                }
            }

            // Получаем DTO страницы
            using (Db db = new Db())
            {
                dto = db.Pages.Where(x => x.Slug == page).FirstOrDefault();
            }

            // Устанавливаем заголовок страницы (Title)
            ViewBag.PageTitle = dto.Title;

            // Проверяем боковую панель
            if (dto.HasSidebar == true)
            {
                ViewBag.Sidebar = "Yes";
            }
            else
            {
                ViewBag.Sidebar = "No";
            }
            // Заполняем модель данными
            model = new PageVM(dto);

            // Возвращаем представление с моделью
            return View(model);
        }

        public ActionResult PagesMenuPartial()
        {
            // Инициализируем лист PageVM
            List<PageVM> pageVMList;

            // Получаем все страницы, кроме HOME
            using (Db db = new Db())
            {
                pageVMList = db.Pages.ToArray().OrderBy(x => x.Sorting).Where(x => x.Slug != "home").Select(x => new PageVM(x)).ToList();
            }

            // Возвращаем частичное представление и лист с данными
            return PartialView(pageVMList);
        }


        public ActionResult SidebarPartial()
        {
            // Объявляем модель
            SidebarVM model;

            // Инициализируем модель
            using (Db db = new Db())
            {
                SidebarDTO dto = db.Sidebars.Find(1);

                model = new SidebarVM(dto);
            }

            // Возвращаем модель в частичное представление
            return PartialView("_SidebarPartial", model);
        }
    }
}