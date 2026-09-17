using RestaurantPOS.Models;

namespace RestaurantPOS.Services
{
    public class OrderCalculationService
    {
        public const decimal TaxRate = 0.06m;

        public OrderTotals Calculate(
            IEnumerable<CartItem> items)
        {
            decimal subtotal =
                items.Sum(item => item.Total);

            decimal tax =
                subtotal * TaxRate;

            decimal total =
                subtotal + tax;

            return new OrderTotals
            {
                Subtotal = subtotal,
                Tax = tax,
                Total = total
            };
        }
    }

    public class OrderTotals
    {
        public decimal Subtotal { get; set; }

        public decimal Tax { get; set; }

        public decimal Total { get; set; }
    }
}