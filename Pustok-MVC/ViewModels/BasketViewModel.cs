namespace Pustok_MVC.ViewModels
{
    public class BasketViewModel
    {
        public List<BookBasketItem> BookItems { get; set; }
        public double TotalPrice { get; set; }
    }
    public class BookBasketItem
    {
        public int Id { get; set; }
        public int Count { get; set; }
        public string UserId { get; set; }
    }
}
