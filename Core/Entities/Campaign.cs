using Core.Enums;

namespace Core.Entities
{
    public class Campaign : Discount
    {
        public Campaign(Category category, DiscountType discountType, int quantity, double discountAmount) : base(discountType, quantity, discountAmount)
        {
            Category = category;
        }

        public Category Category { get; set; }
    }
}