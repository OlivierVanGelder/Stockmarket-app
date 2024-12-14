namespace Backend_Example.Controllers
{
    public static class ManagerUiController
    {
        public static void NewManagerUiController(this WebApplication app)
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
