using CMSOnlineStore.Models.Data;
using CMSOnlineStore.Models.WievModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace CMSOnlineStore.Controllers
{
    // Урок 20
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            // Объявляем Cart List
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            // Проверяем, пустая ли корзина
            if (cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "Your cart is empty.";
                return View();
            }

            // Складываем сумму и записываем во ViewBag
            decimal total = 0m;

            foreach (var item in cart)
            {
                total += item.Total;
            }

            ViewBag.GrandTotal = total;

            // Возвращаем list и предсталение
            return View(cart);
        }

        public ActionResult CartPartial()
        {
            // Объявляем CartVM
            CartVM model = new CartVM();

            // Объявляем количество
            int qty = 0;

            // Объявляем цену
            decimal price = 0m;

            // Проверяем сессию корзины
            if (Session["cart"] != null)
            {
                // Получаем общее количество и цену
                var list = (List<CartVM>)Session["cart"];

                foreach (var item in list)
                {
                    qty += item.Quantity;
                    price += item.Quantity * item.Price;
                }

                // Урок 21 КОД КОРЗИНЫ
                model.Quantity = qty;
                model.Price = price;
                //*********************
            }
            else
            {
                // Или устанавливаем количество и цену на 0
                model.Quantity = 0;
                model.Price = 0m;
            }

            // Возвращаем частичное представление с моделью
            return PartialView("_CartPartial", model);
        }

        // Урок 21
        public ActionResult AddToCartPartial(int id)
        {
            // Объявляем лист параметризированный типом CartVM
            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            // Объявляем модель CartVM
            CartVM model = new CartVM();

            using (Db db = new Db())
            {
                // Получаем продукт
                ProductDTO product = db.Products.Find(id);

                // Проверяем, находится ли товар уже в корзине
                var productInCart = cart.FirstOrDefault(x => x.ProductId == id);

                // Если нет, добавляем новый
                if (productInCart == null)
                {
                    cart.Add(new CartVM()
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = 1,
                        Price = product.Price,
                        Image = product.ImageName
                    });
                }
                else
                {
                    // Если да, добавляем еденицу
                    productInCart.Quantity++;
                }
            }
            // Получаем общее количество, цену и добавляем в модель
            int qty = 0;
            decimal price = 0m;

            foreach (var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }

            model.Quantity = qty;
            model.Price = price;

            // Сохраняем состояние корзины в сессию
            Session["cart"] = cart;

            // Возвращаем частичное представление с моделью
            return PartialView("_AddToCartPartial", model);
        }

        // Урок 21
        // GET: /Cart/IncrementProduct
        public JsonResult IncrementProduct (int productId)
        {
            // Объявляем лист cart
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                // Получаем cartVM из листа
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                // добавляем количество
                model.Quantity++;

                // Сохраняем необходимые данные
                var result = new { qty = model.Quantity, price = model.Price };

                // Возвращаем json ответ с данными
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        // Урок 22
        // GET: /Cart/DecrementProduct
        public ActionResult DecrementProduct(int productId)
        {
            // Объявляем лист cart
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                // Получаем данные модели из листа
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                // Декрементируем (отнимаем) количество
                if (model.Quantity > 1)
                    model.Quantity--;
                else
                {
                    model.Quantity = 0;
                    cart.Remove(model);
                }

                // Сохраняем необходимые данные
                var result = new { qty = model.Quantity, price = model.Price };

                // Возвращаем JSON ответ с данными
                return Json(result, JsonRequestBehavior.AllowGet);

            }
        }

        // Урок 22
        // GET: /Cart/RemoveProduct
        public void RemoveProduct(int productId)
        {
            // Объявляем лист cart
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                // Получаем данные модели из листа
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                // Удаляем модель из листа
                cart.Remove(model);
            }
        }

        // Урок 26
        public ActionResult PaypalPartial()
        {
            // Получаем лист товаров в корзине
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            // Возвращаем частичное представление с листом товаров
            return PartialView(cart);
        }

        // Урок 26
        // POST: /cart/PlaceOrder
        [HttpPost]
        public void PlaceOrder()
        {
            // Получаем лист товаров в корзине
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            // Получаем имя пользователя
            string userName = User.Identity.Name;

            // Объявляем переменную orderId
            int orderId = 0;

            using (Db db = new Db())
            {
                // Инициализируем модель OrderDTO
                OrderDTO orderDto = new OrderDTO();

                // Получаем ID пользователя
                var q = db.Users.FirstOrDefault(x => x.Username == userName);
                int userId = q.Id;

                // Заполняем модель OrderDTO и сохраняем
                orderDto.UserId = userId;
                orderDto.CreatedAt = DateTime.Now;

                db.Orders.Add(orderDto);
                db.SaveChanges();

                // Получаем встроенный ID 
                orderId = orderDto.OrderId;

                // Инициализируем модель OrderDetailsDTO
                OrderDetailsDTO orderDetailsDto = new OrderDetailsDTO();

                // Добавляем данные в OrderDetailsDTO
                foreach (var item in cart)
                {
                    orderDetailsDto.OrderId = orderId;
                    orderDetailsDto.UserId = userId;
                    orderDetailsDto.ProductId = item.ProductId;
                    orderDetailsDto.Quantity = item.Quantity;

                    db.OrderDetails.Add(orderDetailsDto);
                    db.SaveChanges();
                }
            }

            // Отправляем письмо админу на почту
            var client = new SmtpClient("smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("ee053ae36467a9", "8bf25933605f02"),
                EnableSsl = true
            };
            client.Send("admin@example.com", "admin@example.com", "New order", $"You have a new order. Order number: {orderId}");

            // Обноляем сессию
            Session["cart"] = null;
        }
    }
}