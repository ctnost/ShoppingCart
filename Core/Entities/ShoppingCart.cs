using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Enums;
using Core.Interfaces;

namespace Core.Entities
{
    public class ShoppingCart
    {
        public Dictionary<Product, int> ShoppingCartItems { get; set;}
        public List<Campaign> Campaigns {get; set;}
        public Coupon Coupon {get; set;}
        private IDeliveryCostCalculator _deliveryCostCalculator;
        public ShoppingCart(IDeliveryCostCalculator deliveryCostCalculator)
        {
            _deliveryCostCalculator = deliveryCostCalculator;
            ShoppingCartItems = new Dictionary<Product, int>();
            Campaigns = new List<Campaign>();
        }
    
        public void addItem(Product product, int quantity)
        {
            if(product == null)
                throw new ArgumentNullException("Product is Null");

            if(quantity > 0)
            {
                if(ShoppingCartItems.ContainsKey(product))
                {
                    ShoppingCartItems[product] += quantity;
                }
                else
                {
                    ShoppingCartItems.Add(product, quantity);
                }
            }
        }
        public int getNumberOfDeliveries()
        {
            if(ShoppingCartItems == null)
                throw new NullReferenceException("ShoppingCartItems is Null");
            return ShoppingCartItems.GroupBy(it => it.Key.Category.Title).Count();;
        }

        public int getNumberOfProducts()
        {
            return ShoppingCartItems.Count;   
        }

        public void applyDiscounts(params Campaign[] campaigns)
        {
            if(campaigns == null)
                throw new ArgumentNullException("Campaign is Null");
            Campaigns.AddRange(campaigns);
        }
        public void applyCoupon(Coupon coupon)
        {
            if(coupon == null)
                throw new ArgumentNullException("Campaign is Null");
            Coupon = coupon;
        }

        public double getTotalAmount()
        {
            double totalAmount = 0;
            foreach (var item in ShoppingCartItems)
            {
                totalAmount += (item.Key.Price*item.Value);
            }
            return totalAmount;
        }

        public double getTotalAmountAfterDiscounts()
        {
            double totalAmount = getTotalAmount();
            double totalAmountAfterDiscounts = totalAmount;
            totalAmountAfterDiscounts -= getCampaignDiscount();
            totalAmountAfterDiscounts -= getCouponDiscount();
            return totalAmountAfterDiscounts;
        }

        public double getCouponDiscount()
        {
            double totalAmount = getTotalAmount();
            double campaignDiscounts = getCampaignDiscount();
            double totalAfterCampaignDiscounts = totalAmount - campaignDiscounts;
            double couponDiscount = 0;
            if(Coupon == null)
                return couponDiscount;

            if(totalAfterCampaignDiscounts < Coupon.MinimumAmount)
                return couponDiscount;

            if(Coupon.DiscountType == DiscountType.Rate)
            {
                couponDiscount = totalAfterCampaignDiscounts * (Coupon.DiscountAmount / 100);
            }
            else
            {
                couponDiscount = Coupon.DiscountAmount;
            }

            return couponDiscount;
        }

        private Tuple<Campaign, double> getAvailableCampaign()
        {
            double maksCampaignDiscount = 0;
            Campaign campaign = null;
            foreach (var item in Campaigns)
            {
                double campaignDiscount = calculateCampaignDiscount(item);
                if(maksCampaignDiscount < campaignDiscount){
                    maksCampaignDiscount = campaignDiscount;
                    campaign = item;
                }
            }

            return Tuple.Create(campaign, maksCampaignDiscount);
        }
        public double getCampaignDiscount()
        {
            double maksCampaignDiscount = 0;
            foreach (var item in Campaigns)
            {
                double campaignDiscount = calculateCampaignDiscount(item);
                if(maksCampaignDiscount < campaignDiscount)
                    maksCampaignDiscount = campaignDiscount;
            }

            return maksCampaignDiscount;
        }

        private double calculateCampaignDiscount(Campaign campaign)
        {
            if (campaign == null)
                throw new ArgumentNullException("campaign is Null");

            double totalAmount = 0;
            int totalNumberOfProduct = 0;
            double campaignDiscount = 0;
            var selectedShoppingCartItemsByCategory = ShoppingCartItems.Where(it => it.Key.Category == campaign.Category).ToDictionary(it => it.Key, it => it.Value);
            foreach (var item in selectedShoppingCartItemsByCategory)
            {
                totalNumberOfProduct += item.Value;
                totalAmount += (item.Value * item.Key.Price);
            }
            if(totalNumberOfProduct <= campaign.MinimumAmount)
                return campaignDiscount;
            
            if(campaign.DiscountType == DiscountType.Rate)
            {
                campaignDiscount = totalAmount * (campaign.DiscountAmount / 100);
            }
            else
            {
                campaignDiscount = campaign.DiscountAmount;
            }
            return campaignDiscount;
        }

        public double getDeliveryCost()
        {
            return _deliveryCostCalculator.calculateFor(this);
        }
        private double getAvailableCampaignDiscountByProduct(Dictionary<string, double> calculatedDiscounts, Tuple<Campaign,double> campaignWithDiscount, Product product)
        {
            double productDiscount = 0.0;
            if(campaignWithDiscount.Item1 == null)
                return productDiscount;
            if(campaignWithDiscount.Item1.Category != product.Category)
                return productDiscount;
            if(calculatedDiscounts != null)
            {
                if(calculatedDiscounts.TryGetValue(product.Category.Title, out double discount))
                {
                    return discount;
                }
            }

            var selectedShoppingCartItemsByCategory = ShoppingCartItems.Where(it => it.Key.Category == campaignWithDiscount.Item1.Category).ToDictionary(it => it.Key, it => it.Value);
            if(selectedShoppingCartItemsByCategory.Count > 0)
            {
                productDiscount = campaignWithDiscount.Item2 / selectedShoppingCartItemsByCategory.Count;
                calculatedDiscounts.Add(product.Category.Title, productDiscount);
            }
            return productDiscount;
        }
        public string print()
        {
            StringBuilder stringBuilder = new StringBuilder();
            var campaignDiscount = getAvailableCampaign();
            double couponDiscountPerProduct = getCouponDiscount();
            if(couponDiscountPerProduct.CompareTo(0.0) != 0)
            {
                couponDiscountPerProduct = getCouponDiscount() / getNumberOfProducts();
            }
            var calculatedDiscounts = new Dictionary<string, double>();
            var products = ShoppingCartItems.GroupBy(p => p.Key.Category.Title).ToDictionary(it => it.Key, it => it.ToList());
            stringBuilder.AppendLine($"{"Category ",15}  {"Product ",15}  {"Quantity",15}  {"Unit Price",15}  {"Total Price",15} {"Total Discount For Campaign", 15} {"Total Discount For Coupon", 15}");
            foreach (var item in products)
            {
                foreach (var p in item.Value)
                {
                    stringBuilder.AppendLine($"{item.Key,15} {p.Key.Title,15} {p.Value,15} {p.Key.Price,15} {p.Value * p.Key.Price,15} {getAvailableCampaignDiscountByProduct(calculatedDiscounts,campaignDiscount,p.Key),15}  {couponDiscountPerProduct, 15}\t");
                }
            }
            stringBuilder.AppendLine($"Total Amount: {getTotalAmount()}");
            stringBuilder.AppendLine($"Total Amount After Discounts: {getTotalAmountAfterDiscounts()}");
            stringBuilder.AppendLine($"Delivery Cost: {getDeliveryCost()}");
            return stringBuilder.ToString();
        }
    }
}