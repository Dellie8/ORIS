using MyHttpServer;
using System.Net;
using System.Text;
using System.Net.Mail;
using MyHttpServer.Framework.Handlers;

namespace MyHttpServer;

public class HttpServer
{
    private HttpListener _listener;
    private string _url;
    private bool _isWorking;
    public HttpServer(string host, int port, string path)
    {
        _listener = new HttpListener();
        _url = $"http://{host}:{port}/{path}";
        this._listener.Prefixes.Add(_url); 
    }
    
    public async Task Start()
    {
        _listener.Start();
        Console.WriteLine($"Сервер запущен: {_url}");
        _isWorking = true; 
        while (_isWorking)
        {
            var context = await _listener.GetContextAsync(); 
            await ProcessRequest(context);
        }
    }

    private async Task ProcessRequest(HttpListenerContext context)
    {
        Handler h1 = new StaticFilesHandler();
        Handler h2 = new ControllerHandler();
        h1.Successor = h2;
        await h1.HandleRequest(context);
        Console.WriteLine($"Обработан запрос: {context.Request.Url}");
        string path = context.Request.Url!.AbsolutePath;
        
        try
        {
            Console.WriteLine($"Обработан запрос: {context.Request.Url}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);

            context.Response.StatusCode = 500;
        }
        finally
        {
            context.Response.Close();
        }
    }

    public void Stop()
    {
        _listener.Stop();
        _isWorking = false;
    }
}