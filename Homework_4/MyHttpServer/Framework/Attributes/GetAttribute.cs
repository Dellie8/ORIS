namespace MyHttpServer.Framework.Attributes;

[AttributeUsage(AttributeTargets.Method)]
internal class GetAttribute : Attribute
{
    public string Route { get; }

    public GetAttribute(string route)
    {
        Route = route;
    }
}

