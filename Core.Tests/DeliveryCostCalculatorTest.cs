using System;
using Core.Entities;
using Core.Services;
using Xunit;

namespace Core.Tests
{
    public class DeliveryCostCalculatorTest
    {
        [Fact]
        public void CalculateFor_ShouldReturnDeliveryCost_WhenShoppingCartIsNotNull()
        {
            DeliveryCostCalculator deliveryCostCalculator = new DeliveryCostCalculator(5,4,7);
            ShoppingCart shoppingCart = new ShoppingCart(deliveryCostCalculator);
            Category foodCategory = new Category("food");
            Product apple = new Product("Apple", 100.0, foodCategory);
            Product almond = new Product("Almonds", 150.0, foodCategory);
            shoppingCart.addItem(apple, 3);
            shoppingCart.addItem(almond, 1);
            var actual = deliveryCostCalculator.calculateFor(shoppingCart);
            var expected = 20;
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void CalculateFor_ShouldThrowArgumentNullException_WhenShoppingCartIsNull()
        {
            DeliveryCostCalculator deliveryCostCalculator = new DeliveryCostCalculator(1,2,3);
            Assert.Throws<ArgumentNullException>(() => deliveryCostCalculator.calculateFor(null));
        }
    }
}
