namespace Core.Entities
{
    public class Product
    {
        public Product(string title, double price, Category category)
        {
            this.Title = title;
            this.Price = price;
            this.Category = category;

        }
        public string Title { get; set; }
        public double Price { get; set; }
        public Category Category { get; set; }
    }
}