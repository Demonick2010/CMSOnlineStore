using CMSOnlineStore.Models.Data;
using CMSOnlineStore.Models.WievModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMSOnlineStore.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            //Объявляем список для представления (PageVM) (Урок 3)
            List<PageVM> pageList;

            //Инициализируем список (Урок 3)
            using (Db db = new Db())
            {
                pageList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }
            //Возвращаем список в представление (Урок 3)
            return View(pageList);
        }

        //Метод добавления страниц (Урок 3)
        // GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }

        //Добавляем пост метод (Урок 3)
        // POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            //Проверка модели на валидность (Урок 3)
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {

                //Объявляем краткий заголовок (Урок 3)
                string slug;

                //Инициализируем PageDTO (Урок 3)
                PagesDTO dto = new PagesDTO();

                //DTO заголовок (Урок 3)
                dto.Title = model.Title;

                //Проверяем заголовок и присваеваем его, если надо (Урок 3)
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                //Убеждаемся, что заголовок и краткое описание - уникальны (Урок 3)
                if (db.Pages.Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "That title already exist.");
                    return View(model);
                }
                else if (db.Pages.Any(x => x.Slug == model.Slug))
                {
                    ModelState.AddModelError("", "That slug already exist.");
                    return View(model);
                }

                //Присваиваем остальные значение в DTO (Урок 3)
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100; //Последний в списке

                //Сохраняем в базу данных (Урок 3)
                db.Pages.Add(dto);
                db.SaveChanges();

            }

            //Передаём во ViewBag сообщение (Урок 3)
            TempData["SM"] = "You have added a new page!";

            //Переадресовываем (Урок 3)
            return RedirectToAction("Index");
        }

        //Добавлаем метод редактирования страниц (Урок 4)
        // GET: Admin/Pages/EditPage/id
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            //Объявляем модель pageVM (Урок 4)
            PageVM model;

            using (Db db = new Db())
            {
                //Получаем страницу (Урок 4)
                PagesDTO dto = db.Pages.Find(id);

                //Подтверждаем, что страница доступна (Урок 4)
                if (dto == null)
                {
                    return Content("The page does not exist.");
                }

                //Инициализируем модель данных (Урок 4)
                model = new PageVM(dto);
            }
            //Возвращаем представление с моделью (Урок 4)
            return View(model);
        }

        //Добавлаем метод редактирования страниц (Урок 4)
        // POST: Admin/Pages/EditPage/id
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            //Проверяем модель на валидность (Урок 4)
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                //Получаем ID страницы (Урок 4)
                int id = model.Id;

                //Объявляем краткий заголовок (Урок 4)
                string slug = "home";

                //Получаем страницу
                PagesDTO dto = db.Pages.Find(id);

                //присваиваем название в DTO (Урок 4)
                dto.Title = model.Title;

                //Проверяем краткий заголовок и присваеваем его, если это необходимо (Урок 4)
                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }

                //Проверяем заголовок и краткий заголовок на уникальность (Урок 4)
                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "That title alredy exist.");
                    return View(model);
                }
                else if (db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "That slug alredy exist.");
                    return View(model);
                }

                //Записываем остальные значения в DTO (Урок 4)
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;

                //Сохраняем в базу (Урок 4)
                db.SaveChanges();

            }
            //Устанавливаем сообщение в TemData (Урок 4)
            TempData["SM"] = "You have edited the page!";

            //Переадресовываем пользователя (Урок 4)
            return RedirectToAction("EditPage");
        }

        //Создаём метод страницы деталей (Урок 4)
        // GET: Admin/Pages/PageDetails/id
        public ActionResult PageDetails(int id)
        {
            //Объявляем модель PageVM
            PageVM model;

            using (Db db = new Db())
            {
                //Получаем страницу
                PagesDTO dto = db.Pages.Find(id);

                //Подтверждаем, что страница доступна
                if (dto == null)
                {
                    return Content("The page does not exist.");
                }

                //Присваиваем модели данные из базы
                model = new PageVM(dto);
            }
                //Возвращаем модель в представление
                return View(model);
        }

        //Создаём метод удаления (Урок 5)
        // GET: Admin/Pages/DeletePage/id
        public ActionResult DeletePage(int id)
        {
            using (Db db = new Db())
            {
                //Получаем страницу
                PagesDTO dto = db.Pages.Find(id);

                //Удаляем страницу
                db.Pages.Remove(dto);

                //Сохраняем изменения в базе
                db.SaveChanges();
            }

            //Добавляем сообщение о удачном удалении страницы
            TempData["SM"] = "You have deleted a page!";

            //Переадресовываем пользователя
            return RedirectToAction("Index");
        }

        //Создаём метод сортировки
        // GET: Admin/Pages/ReorderPages
        [HttpPost]
        public void ReorderPages(int [] id)
        {
            using (Db db = new Db())
            {
                //Реализуем начальный счётчик
                int count = 1;

                //Инициализируем модель данных
                PagesDTO dto;

                //Устанавливаем сортировку для каждой страницы
                foreach (var pageId in id)
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;

                    db.SaveChanges();

                    count++;
                }
            }
        }

        //Создаём метод редактирования боковой панели (Урок 6)
        // GET: Admin/Pages/EditSidebar
        [HttpGet]
        public ActionResult EditSidebar()
        {
            //Объявить модель
            SidebarVM model;

            using (Db db = new Db())
            {
                //Получить данные из DTO
                SidebarDTO dto = db.Sidebars.Find(1); //Говнокодим, не желательно в коде задавать жёсткие значения!

                //Заполнить модель данными
                model = new SidebarVM(dto);
            }

            //Вернуть представление с моделью
            return View(model);
        }

        //Создаём POST метод редактирования боковой панели (Урок 6)
        // POST: Admin/Pages/EditSidebar
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            using (Db db = new Db())
            {
                //Получаем данные (DTO)
                SidebarDTO dto = db.Sidebars.Find(1); //Ещё говнокод, по той же причине

                //Присваиваем данные в тело 
                dto.Body = model.Body;

                //Сохраняем
                db.SaveChanges();
            }

            //Присваиваем сообщение в TempData 
            TempData["SM"] = "You have edited the sidebar!";

            //Переадресовываем
            return RedirectToAction("EditSidebar");
        }
    }
}