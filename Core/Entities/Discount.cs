using Core.Enums;

namespace Core.Entities
{
    public class Discount
    {
        public Discount(DiscountType discountType, int minimumAmount, double discountAmount)
        {
            this.DiscountType = discountType;
            this.MinimumAmount = minimumAmount;
            this.DiscountAmount = discountAmount;
        }
        public DiscountType DiscountType { get; set; }
        public int MinimumAmount { get; set; }
        public double DiscountAmount { get; set; }
    }
}