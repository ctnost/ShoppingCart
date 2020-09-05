using Core.Entities;

namespace Core.Interfaces
{
    public interface IDeliveryCostCalculator
    {
        public double calculateFor(ShoppingCart cart);
    }
}