namespace Backend_Example
{
    public static class OrderAPI
    {
        public static void SetupOrders(this WebApplication app)
        {
            app.MapGet("/orders", (int restaurant) =>
            {
                return new Order[]
                {
                    new Order(1, 0, new List<string> { "Pizza", "Coke" }),
                    new Order(2, 1, new List<string> { "Burger", "Fries" }),
                    new Order(3, 2, new List<string> { "Salad", "Water" })
                };
            })
            .WithName("GetOrders")
            .WithOpenApi();
        }
        class Order
        {
            public int id { get; set; }
            public int status { get; set; }
            public List<string> items { get; set; }

            public Order(int id, int status, List<string> items)
            {
                this.id = id;
                this.status = status;
                this.items = items;
            }
        }
    }
}
