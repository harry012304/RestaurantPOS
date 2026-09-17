using RestaurantPOS.Models;
using RestaurantPOS.Services;

namespace RestaurantPOS.Tests
{
    public class OrderCalculationServiceTests
    {
        private readonly OrderCalculationService
            _service = new();

        [Fact]
        public void Calculate_EmptyCart_ReturnsZero()
        {
            // Arrange
            var items = new List<CartItem>();

            // Act
            var result =
                _service.Calculate(items);

            // Assert
            Assert.Equal(0m, result.Subtotal);
            Assert.Equal(0m, result.Tax);
            Assert.Equal(0m, result.Total);
        }

        [Fact]
        public void Calculate_SingleProduct_ReturnsCorrectTotal()
        {
            var items = new List<CartItem>
            {
                new CartItem
                {
                    ProductId = 1,
                    ProductName = "Chicken Chop",
                    UnitPrice = 15.90m,
                    Quantity = 1
                }
            };

            var result =
                _service.Calculate(items);

            Assert.Equal(15.90m, result.Subtotal);
            Assert.Equal(0.954m, result.Tax);
            Assert.Equal(16.854m, result.Total);
        }

        [Fact]
        public void Calculate_MultipleProducts_ReturnsCorrectTotal()
        {
            var items = new List<CartItem>
            {
                new CartItem
                {
                    ProductId = 1,
                    ProductName = "Chicken Chop",
                    UnitPrice = 15.90m,
                    Quantity = 2
                },

                new CartItem
                {
                    ProductId = 2,
                    ProductName = "Iced Coffee",
                    UnitPrice = 6.50m,
                    Quantity = 1
                }
            };

            var result =
                _service.Calculate(items);

            Assert.Equal(38.30m, result.Subtotal);
            Assert.Equal(2.298m, result.Tax);
            Assert.Equal(40.598m, result.Total);
        }

        [Fact]
        public void Calculate_QuantityChanges_TotalChanges()
        {
            var item = new CartItem
            {
                ProductId = 1,
                ProductName = "Burger",
                UnitPrice = 10m,
                Quantity = 3
            };

            var result =
                _service.Calculate(
                    new List<CartItem> { item });

            Assert.Equal(30m, result.Subtotal);
            Assert.Equal(1.80m, result.Tax);
            Assert.Equal(31.80m, result.Total);
        }
    }
}