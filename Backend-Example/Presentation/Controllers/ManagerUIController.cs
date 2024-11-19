namespace Backend_Example.Controllers
{
    public static class ManagerUIController
    {
        public static void ManagerUIcontroller(this WebApplication app)
        {
            app.MapGet(
                    "/example",
                    () =>
                    {
                        return "Hello World!";
                    }
                )
                .WithName("example")
                .WithOpenApi();
        }
    }
}
