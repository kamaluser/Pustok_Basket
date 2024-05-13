using Newtonsoft.Json;
using Pustok_MVC.Data;
using Pustok_MVC.Models;
using Pustok_MVC.ViewModels;

namespace Pustok_MVC.Services
{
    public class LayoutService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LayoutService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public List<Genre> GetGenres()
        {
            return _context.Genres.ToList();
        }

        public Dictionary<String, String> GetSettings()
        {
            return _context.Settings.ToDictionary(x=>x.Key, x => x.Value);
        }

        public List<BasketItemViewModel> GetBasket()
        {
            List<BasketItemViewModel> basketItems = new List<BasketItemViewModel>();
            var basketItemsStr = _httpContextAccessor.HttpContext.Request.Cookies["basket"];

            if (basketItemsStr != null)
            {
                basketItems = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemsStr);
            }

            foreach (var item in basketItems)
            {
                Book bk = _context.Books.FirstOrDefault(x => x.Id == item.Id);
                double salePrice = bk.DiscountPercent > 0 ? bk.SalePrice * (100 - bk.DiscountPercent) / 100 : bk.SalePrice;
                item.TotalPrice = salePrice * item.Count;
            }

            return basketItems;
        }
    }
}
