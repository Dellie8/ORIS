using MyHttpServer;
using System.Net;
using System.Text;
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
        this._listener.Prefixes.Add(_url); //префиксес это список адресов
    }
    
    public async Task Start()
    {
        _listener.Start();
        Console.WriteLine($"Сервер запущен: {_url}");
        _isWorking = true; //запускаем!! прям в космос
        while (_isWorking)
        {
            var context = await _listener.GetContextAsync(); // ждём гостя
            await ProcessRequest(context);
        }
    }

    private async Task ProcessRequest(HttpListenerContext context)
    {
        HttpListenerResponse response = context.Response; //спросить = смотрим в context.Response 
        string requestedPath = context.Request.Url.AbsolutePath; //спрашиваем путь
        string relativePath = requestedPath.TrimStart('/');
        if (string.IsNullOrEmpty(relativePath))
        {
            relativePath = "search-engine.html";
        }
        string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "static");
        string filePath = Path.Combine(basePath, relativePath);
        FileInfo fileInfo = new FileInfo(filePath);
 
        if (!fileInfo.Exists)
        {
            response.StatusCode = 404;
            filePath = Directory.GetCurrentDirectory() + "/static/404.html";
            fileInfo = new FileInfo(filePath);
        }

        byte[] buffer = await File.ReadAllBytesAsync(filePath);

        response.ContentLength64 = buffer.Length;

        switch (fileInfo.Extension)
        {
            case ".html":
                response.ContentType = "text/html; charset=utf-8";
                break;

            case ".css":
                response.ContentType = "text/css; charset=utf-8";
                break;

            case ".js":
                response.ContentType = "text/javascript; charset=utf-8";
                break;

            case ".png":
                response.ContentType = "image/png";
                break;

            case ".jpg":
                response.ContentType = "image/jpeg";
                break;

            case ".svg":
                response.ContentType = "image/svg+xml";
                break;

            case ".ico":
                response.ContentType = "image/x-icon";
                break;
        }

        using (Stream output = response.OutputStream)
        {
            await output.WriteAsync(buffer, 0, buffer.Length);
        }
        
    }

    public void Stop()
    {
        _listener.Stop();
        _isWorking = false;
    }
}