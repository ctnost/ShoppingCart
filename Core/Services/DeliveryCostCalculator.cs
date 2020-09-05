using System;
using Core.Entities;
using Core.Interfaces;

namespace Core.Services
{
    public class DeliveryCostCalculator : IDeliveryCostCalculator
    {
        private double _costPerDelivery;
        private double _costPerProduct;
        private double _fixedCost;

        public DeliveryCostCalculator(double costPerDelivery, 
                            double costPerProduct, 
                            double fixedCost)
        {
            _costPerDelivery = costPerDelivery;
            _costPerProduct = costPerProduct;
            _fixedCost = fixedCost;
        }

        public double calculateFor(ShoppingCart cart)
        {
            if (cart == null)
            {
                throw new ArgumentNullException("ShoppingCart is Null");
            }
            int numberOfDeliveries = cart.getNumberOfDeliveries();
            int numberOfProducts = cart.getNumberOfProducts();
            return (_costPerDelivery * numberOfDeliveries) + (_costPerProduct * numberOfProducts) + _fixedCost;
        }
    }
}