using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pustok_MVC.Data;
using Pustok_MVC.Models;
using Pustok_MVC.ViewModels;

namespace Pustok_MVC.Controllers
{
    public class BookController:Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BookController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult GetBookById(int id)
        {
            Book book = _context.Books.Include(x => x.Genre).Include(x => x.BookImages.Where(x => x.PosterStatus==true)).FirstOrDefault(x=>x.Id == id);
            return PartialView("_BookModalPartial", book);
        }

        public IActionResult BookDetail(int id)
        {
            Book book = _context.Books
                .Include(x => x.BookImages)
                .Include(x => x.BookTags).ThenInclude(x => x.Tag)
                .Include(x => x.Author).Include(x => x.Genre)
                .FirstOrDefault(x => x.Id == id);

            if (book == null) return RedirectToAction("index");

            return View(book);
        }

        public IActionResult AddBasket(int id)
        {
            Book book = _context.Books.FirstOrDefault(x => x.Id == id);

            if (book == null)
                return Json(new { isSucceeded = false });

            List<BookBasketItem> bookBasketItems = new List<BookBasketItem>();
            BookBasketItem bookBasketItem = new BookBasketItem
            {
                Id = book.Id,
                Count = 1
            };

            if (User.Identity.IsAuthenticated)
            {
                string userId = User.Identity.Name;

                if (HttpContext.Request.Cookies["basket"] != null)
                {
                    bookBasketItems = JsonConvert.DeserializeObject<List<BookBasketItem>>(HttpContext.Request.Cookies["basket"]);

                    var existBasketItem = bookBasketItems.FirstOrDefault(x => x.Id == book.Id && x.UserId == userId);

                    if (existBasketItem != null)
                    {
                        existBasketItem.Count++;
                    }
                    else
                    {
                        bookBasketItem.UserId = userId;
                        bookBasketItems.Add(bookBasketItem);
                    }
                }
                else
                {
                    bookBasketItem.UserId = userId;
                    bookBasketItems.Add(bookBasketItem);
                }
            }
            else 
            {
                if (HttpContext.Request.Cookies["basket"] != null)
                {
                    bookBasketItems = JsonConvert.DeserializeObject<List<BookBasketItem>>(HttpContext.Request.Cookies["basket"]);

                    var existBasketItem = bookBasketItems.FirstOrDefault(x => x.Id == book.Id);

                    if (existBasketItem != null)
                    {
                        existBasketItem.Count++;
                    }
                    else
                    {
                        bookBasketItems.Add(bookBasketItem);
                    }
                }
                else
                {
                    bookBasketItems.Add(bookBasketItem);
                }
            }

            double totalPrice = 0;
            foreach (var item in bookBasketItems)
            {
                Book bookItem = _context.Books.FirstOrDefault(x => x.Id == item.Id);
                double price = bookItem.DiscountPercent > 0 ? bookItem.SalePrice * (100 - bookItem.DiscountPercent) / 100 : bookItem.SalePrice;
                totalPrice += price * item.Count;
            }

            var json = JsonConvert.SerializeObject(bookBasketItems, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            HttpContext.Response.Cookies.Append("basket", json);

            return Json(new { totalPrice, totalCount = bookBasketItems.Count() });
        }

        public IActionResult GetBasket()
        {
            var bookStr = HttpContext.Request.Cookies["basket"];
            List<BasketItemViewModel> book = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(bookStr);

            return Ok(book);
        }
    }
}   