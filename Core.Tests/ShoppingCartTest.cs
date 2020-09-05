using System;
using Core.Entities;
using Core.Services;
using Xunit;
using Moq;
using Core.Interfaces;
using Core.Enums;

namespace Core.Tests
{
    public class ShoppingCartTest
    {
        ShoppingCart shoppingCart;
        Mock<IDeliveryCostCalculator> deliveryCostCalculator;
        public ShoppingCartTest()
        {
            deliveryCostCalculator = new Mock<IDeliveryCostCalculator>();
            shoppingCart = new ShoppingCart(deliveryCostCalculator.Object);
        }
        [Fact]
        public void getDeliveryCost_ShouldReturnDeliveryCost_WhencalculateForReturnValue()
        {            
            double expected = 5;
            deliveryCostCalculator.Setup(c => c.calculateFor(shoppingCart)).Returns(expected);
            Assert.Equal(shoppingCart.getDeliveryCost(), expected);
        }
        [Fact]
        public void addItem_ShouldThrowArgumentNullException_WhenProductIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => shoppingCart.addItem(null,1));
        }
        [Fact]
        public void addItem_ShouldAddProduct_WhenShoppingCartItemsContainsKey()
        {
            //Arrange
            Category foodCategory = new Category("food");
            Product apple = new Product("Apple", 100.0, foodCategory);
            shoppingCart.addItem(apple, 3);
            var expected = 1;
            //Act
            shoppingCart.addItem(apple, 2);
            var actual = shoppingCart.getNumberOfProducts();
            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void addItem_ShouldAddProduct_WhenShoppingCartItemsDontContainKey()
        {
            //Arrange
            Category foodCategory = new Category("food");
            Product apple = new Product("Apple", 100.0, foodCategory);
            shoppingCart.addItem(apple, 3);
            var expected = 1;
            //Act
            var actual = shoppingCart.getNumberOfProducts();
            //Assert
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void addItem_ShouldNotAddProduct_WhenQuantityLessThanOne()
        {
            //Arrange
            Category foodCategory = new Category("food");
            Product apple = new Product("Apple", 100.0, foodCategory);
            shoppingCart.addItem(apple, -1);
            var expected = 0;
            //Act
            var actual = shoppingCart.getNumberOfProducts();
            //Assert
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void getNumberOfDeliveries_ShouldReturnNumberOfDeliveries_WhenShoppingCartItemNotNull()
        {
            //Arrange
            Category foodCategory = new Category("food");
            Product apple = new Product("Apple", 100.0, foodCategory);
            Product almond = new Product("Almonds", 150.0, foodCategory);
            shoppingCart.addItem(apple, 2);
            shoppingCart.addItem(almond, 3);
            var expected = 1;
            //Act
            var actual = shoppingCart.getNumberOfDeliveries();
            //Assert
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void getNumberOfProducts_ShouldReturnNumberOfProducts_WhenShoppingCartItemsAdded()
        {
            //Arrange
            Category foodCategory = new Category("food");
            Product apple = new Product("Apple", 100.0, foodCategory);
            Product almond = new Product("Almonds", 150.0, foodCategory);
            shoppingCart.addItem(apple, 2);
            shoppingCart.addItem(almond, 3);
            var expected = 2;
            //Act
            var actual = shoppingCart.getNumberOfProducts();
            //Assert
            Assert.Equal(expected, actual);
        }
           
        [Fact]
        public void applyCoupon_ShouldThrowArgumentNullException_WhenCouponIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => shoppingCart.applyCoupon(null));
        }

        [Fact]
        public void getTotalAmount_ShouldReturnZero_WhenNotAddedProduct()
        {
            //Arrange
            var expected = 0;
            //Act
            var actual = shoppingCart.getTotalAmount();
            //Assert
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void getTotalAmount_ShouldReturnFive_WhenAddedFiveProduct()
        {
            //Arrange
            Category foodCategory = new Category("food");
            Product apple = new Product("Apple", 1.0, foodCategory);
            shoppingCart.addItem(apple, 5);
            var expected = 5;
            //Act
            var actual = shoppingCart.getTotalAmount();
            //Assert
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void getTotalAmount_ShouldReturnTen_WhenAddedTwoSameProductsAdded()
        {
            //Arrange
            Category foodCategory = new Category("food");
            Product apple = new Product("Apple", 1.0, foodCategory);
            shoppingCart.addItem(apple, 5);
            shoppingCart.addItem(apple, 5);
            var expected = 10;
            //Act
            var actual = shoppingCart.getTotalAmount();
            //Assert
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void getTotalAmount_ShouldReturnTen_WhenAddedTwoDifferentProductsAdded()
        {
            //Arrange
            Category foodCategory = new Category("food");
            Product apple = new Product("Apple", 1.0, foodCategory);
            Product almond = new Product("Almonds", 1.0, foodCategory);
            shoppingCart.addItem(apple, 5);
            shoppingCart.addItem(almond, 5);
            var expected = 10;
            //Act
            var actual = shoppingCart.getTotalAmount();
            //Assert
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void getTotalAmountAfterDiscounts_ShouldTotalAmountAfterDiscountsEqualTotalAmount_WhenZeroCampaignZeroCoupon()
        {
            //Arrange
            Category foodCategory = new Category("food");
            Product apple = new Product("Apple", 10.0, foodCategory);
            Product almond = new Product("Almonds", 10.0, foodCategory);
            Category dressCategory = new Category("dress");
            Product pant = new Product("pant", 100.0, dressCategory);
            shoppingCart.addItem(apple, 5);
            shoppingCart.addItem(almond, 5);
            shoppingCart.addItem(pant, 2);
            var expected = shoppingCart.getTotalAmount();
            //Act
            var actual = shoppingCart.getTotalAmountAfterDiscounts();
            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void getTotalAmountAfterDiscounts_ShouldReturnTotalAmountAfterDiscounts_WhenGivenOneCampaignAndOneCoupon()
        {
            //Arrange
            Category foodCategory = new Category("food");
            Product apple = new Product("Apple", 10.0, foodCategory);
            Product almond = new Product("Almonds", 10.0, foodCategory);
            Category dressCategory = new Category("dress");
            Product pant = new Product("pant", 100.0, dressCategory);
            shoppingCart.addItem(apple, 5);
            shoppingCart.addItem(almond, 5);
            shoppingCart.addItem(pant, 2);
            double campaignDiscount = 50.0;
            Campaign campaign = new Campaign(dressCategory, DiscountType.Amount, 1, campaignDiscount);
            Coupon coupon = new Coupon(DiscountType.Rate, 1, 50.0);
            shoppingCart.applyCoupon(coupon);
            shoppingCart.applyDiscounts(campaign);
            var expected = (shoppingCart.getTotalAmount() - campaignDiscount)*0.5;
            //Act
            var actual = shoppingCart.getTotalAmountAfterDiscounts();
            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void getCouponDiscount_ShouldReturnZero_WhenNotAddedCoupon()
        {
            //Arrange
            var expected = 0;
            //Act
            var actual = shoppingCart.getCouponDiscount();
            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void getCouponDiscount_ShouldReturnZero_WhenTotalAmountLessThanMinimumAmount()
        {
            //Arrange
            Coupon coupon = new Coupon(DiscountType.Amount, 1, 1.0);
            shoppingCart.applyCoupon(coupon);
            var expected = 0;
            //Act
            var actual = shoppingCart.getCouponDiscount();
            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void getCouponDiscount_ShouldReturnHalfOfTheTotalAmount_WhenGivenRateFifty()
        {
            //Arrange
            Category dressCategory = new Category("dress");
            Product pant = new Product("pant", 100.0, dressCategory);
            shoppingCart.addItem(pant, 5);
            Coupon coupon = new Coupon(DiscountType.Rate, 1, 50.0);
            shoppingCart.applyCoupon(coupon);
            var expected = shoppingCart.getTotalAmount() * 0.5;
            //Act
            var actual = shoppingCart.getCouponDiscount();
            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void getCouponDiscount_ShouldReturnZero_WhenGivenRateZero()
        {
            //Arrange
            Category dressCategory = new Category("dress");
            Product pant = new Product("pant", 100.0, dressCategory);
            shoppingCart.addItem(pant, 5);
            Coupon coupon = new Coupon(DiscountType.Rate, 1, 0.0);
            shoppingCart.applyCoupon(coupon);
            var expected = 0;
            //Act
            var actual = shoppingCart.getCouponDiscount();
            //Assert
            Assert.Equal(expected, actual);
        }

         

        [Fact]
        public void getCouponDiscount_ShouldReturnDiscountedAmount_WhenGivenDiscountAmount()
        {
            //Arrange
            Category dressCategory = new Category("dress");
            Product pant = new Product("pant", 100.0, dressCategory);
            shoppingCart.addItem(pant, 5);
            double discountAmount = 50.0;
            Coupon coupon = new Coupon(DiscountType.Amount, 1, discountAmount);
            shoppingCart.applyCoupon(coupon);
            var expected = discountAmount;
            //Act
            var actual = shoppingCart.getCouponDiscount();
            //Assert
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void getCampaignDiscount_ShouldReturnZero_WhenNotAddedCampaign()
        {
            //Arrange
            var expected = 0;
            //Act
            var actual = shoppingCart.getCampaignDiscount();
            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void getCampaignDiscount_ShouldReturnHalfOfTheTotalAmount_WhenGivenRateFifty()
        {
            //Arrange
            Category dressCategory = new Category("dress");
            Product pant = new Product("pant", 100.0, dressCategory);
            shoppingCart.addItem(pant, 5);
            Campaign campaign = new Campaign(dressCategory, DiscountType.Rate, 1, 50.0);
            shoppingCart.applyDiscounts(campaign);
            var expected = shoppingCart.getTotalAmount() * 0.5;
            //Act
            var actual = shoppingCart.getCampaignDiscount();
            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void getCampaignDiscount_ShouldReturnDiscountedAmount_WhenGivenDiscountAmount()
        {
            //Arrange
            Category dressCategory = new Category("dress");
            Product pant = new Product("pant", 100.0, dressCategory);
            shoppingCart.addItem(pant, 5);
            double discountAmount = 50.0;
            Campaign campaign = new Campaign(dressCategory, DiscountType.Amount, 1, discountAmount);
            shoppingCart.applyDiscounts(campaign);
            var expected = discountAmount;
            //Act
            var actual = shoppingCart.getCampaignDiscount();
            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void getCampaignDiscount_ShouldReturnCampaignDiscount_WhenAddedOneCampaign()
        {
            //Arrange
            Category dressCategory = new Category("dress");
            Product pant = new Product("pant", 100.0, dressCategory);
            shoppingCart.addItem(pant, 5);
            double discountAmount = 50.0;
            Campaign campaign = new Campaign(dressCategory, DiscountType.Amount, 1, discountAmount);
            shoppingCart.applyDiscounts(campaign);
            var expected = discountAmount;
            //Act
            var actual = shoppingCart.getCampaignDiscount();
            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void getCampaignDiscount_ShouldReturnMaksimumCampaignDiscount_WhenAddedTwoCampaign()
        {
            //Arrange
            Category dressCategory = new Category("dress");
            Product pant = new Product("pant", 100.0, dressCategory);
            shoppingCart.addItem(pant, 5);
            double discountAmount1 = 50.0;
            double discountAmount2 = 100.0;
            Campaign campaign1 = new Campaign(dressCategory, DiscountType.Amount, 1, discountAmount1);
            Campaign campaign2 = new Campaign(dressCategory, DiscountType.Amount, 1, discountAmount2);
            shoppingCart.applyDiscounts(campaign1);
            shoppingCart.applyDiscounts(campaign2);
            var expected = Math.Max(discountAmount1, discountAmount2) ;
            //Act
            var actual = shoppingCart.getCampaignDiscount();
            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void getCampaignDiscount_ShouldReturnZero_WhenAddedCampaignsProductCategoryIsNotInShoppingCart()
        {
            //Arrange
            Category foodCategory = new Category("food");
            Category dressCategory = new Category("dress");
            Product pant = new Product("pant", 100.0, dressCategory);
            shoppingCart.addItem(pant, 5);
            double discountAmount = 50.0;
            Campaign campaign = new Campaign(foodCategory, DiscountType.Amount, 1, discountAmount);
            shoppingCart.applyDiscounts(campaign);
            var expected = 0 ;
            //Act
            var actual = shoppingCart.getCampaignDiscount();
            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void getCampaignDiscount_ShouldReturnZero_WhenShoppingCartProductIsLessThanCampaignMinimumAmount()
        {
            //Arrange
            Category dressCategory = new Category("dress");
            Product pant = new Product("pant", 100.0, dressCategory);
            shoppingCart.addItem(pant, 5);
            Campaign campaign = new Campaign(dressCategory, DiscountType.Amount, 6, 50.0);
            shoppingCart.applyDiscounts(campaign);
            var expected = 0 ;
            //Act
            var actual = shoppingCart.getCampaignDiscount();
            //Assert
            Assert.Equal(expected, actual);
        }
    }
}
