namespace Backend_Example.Controllers
{
    public static class ManagerUiController
    {
        public static void NewManagerUiController(this WebApplication app, IConfiguration configuration)
        {
            app.MapGet(
                    "/example",
                    () =>
                    {
                        return () => "Hello World!";
                    }
                )
                .WithName("example")
                .WithOpenApi();
        }
    }
}
