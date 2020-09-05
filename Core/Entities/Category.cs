namespace Core.Entities
{
    public class Category
    {
        public Category(string title)
        {
            this.Title = title;
        }
        public string Title { get; set; }
    }
}