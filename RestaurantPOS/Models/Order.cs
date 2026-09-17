namespace RestaurantPOS.Models
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public string OrderType { get; set; } = string.Empty;

        public int? RestaurantTableId { get; set; }

        public RestaurantTable? RestaurantTable { get; set; }

        public decimal Subtotal { get; set; }

        public decimal Tax { get; set; }

        public decimal Total { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public decimal AmountReceived { get; set; }

        public decimal ChangeAmount { get; set; }

        public string Status { get; set; } = "Completed";

        public ICollection<OrderItem> OrderItems { get; set; }
            = new List<OrderItem>();
    }
}