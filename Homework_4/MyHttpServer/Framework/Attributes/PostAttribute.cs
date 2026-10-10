namespace MyHttpServer.Framework.Attributes;

[AttributeUsage(AttributeTargets.Method)]
internal class PostAttribute : Attribute
{
    public string Route { get; }

    public PostAttribute(string route)
    {
        Route = route;
    }
}
