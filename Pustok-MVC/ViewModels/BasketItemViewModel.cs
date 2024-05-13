namespace Pustok_MVC.ViewModels
{
    public class BasketItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
        public double TotalPrice { get; set; }
        public string PosterImg { get; set; }
    }
}
