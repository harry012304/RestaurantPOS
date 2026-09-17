namespace RestaurantPOS.Models
{
    public class RestaurantTable
    {
        public int Id { get; set; }
        public string TableNumber { get; set; }
        public int Capacity { get; set; }
        public string Status { get; set; } = "Available";

    }
}