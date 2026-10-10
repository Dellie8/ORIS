using System.Net;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Text;
using MyHttpServer.Framework.Attributes;

namespace MyHttpServer.Framework.Handlers;

internal class ControllerHandler : Handler
{

    public override async Task HandleRequest(HttpListenerContext context)
    {
        var request = context.Request;
        var response = context.Response;

        string[] segments = request.Url.Segments
            .Skip(1)
            .Select(s => s.Replace("/", ""))
            .ToArray();
        Console.WriteLine(string.Join(", ", segments));

        if (segments.Length < 2)
        {
            if (Successor != null)
                await Successor.HandleRequest(context);

            return;
        }

        string controllerRoute = segments[0];
        string methodName = segments[1];
        
        Type? controllerType = null;

        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            var attribute = type.GetCustomAttribute<ControllerAttribute>();;

            if (attribute != null && attribute.Route == controllerRoute)
            {
                controllerType = type;
                break;
            }
        }
        
        if (controllerType == null)
        {
            if (Successor != null)
                await Successor.HandleRequest(context);

            return;
        }
        
        MethodInfo? controllerMethod = null;

        foreach (MethodInfo method in controllerType.GetMethods())
        {
            if (request.HttpMethod == "GET")
            {
                var attribute = (GetAttribute?)Attribute.GetCustomAttribute(method, typeof(GetAttribute));

                if (attribute != null && attribute.Route == methodName)
                {
                    controllerMethod = method;
                    break;
                }
            }
            else if (request.HttpMethod == "POST")
            {
                var attribute = (PostAttribute?)Attribute.GetCustomAttribute(method, typeof(PostAttribute));

                if (attribute != null && attribute.Route == methodName)
                {
                    controllerMethod = method;
                    break;
                }
            }
        }
        
        if (controllerMethod == null)
        {
            if (Successor != null)
                await Successor.HandleRequest(context);

            return;
        }
       
        object? controller = Activator.CreateInstance(controllerType);

        object? result;

        if (request.HttpMethod == "POST")
        {
            using var reader = new StreamReader(request.InputStream, request.ContentEncoding);
            string body = await reader.ReadToEndAsync();

            var parameters = System.Web.HttpUtility.ParseQueryString(body);

            string login = parameters["login"];
            string password = parameters["password"];

            result = controllerMethod.Invoke(controller, new object[] { login, password });
        }
        else
        {
            result = controllerMethod.Invoke(controller, null);
        }
        
        string html = result?.ToString() ?? "";
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(html);

        response.ContentType = "text/html; charset=utf-8";
        response.ContentLength64 = buffer.Length;

        await response.OutputStream.WriteAsync(buffer);
    }
}