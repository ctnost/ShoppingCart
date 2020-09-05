using Core.Enums;

namespace Core.Entities
{
    public class Coupon : Discount
    {
        public Coupon(DiscountType discountType, int quantity, double discountAmount) : base(discountType, quantity, discountAmount)
        {
        }
    }
}