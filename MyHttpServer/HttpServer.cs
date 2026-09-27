namespace MyHttpServer;
using MyHttpServer;
using System.Net;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

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
        string relativePath = requestedPath.Replace("/connection/", "").TrimStart('/');
        if (string.IsNullOrEmpty(relativePath))
        {
            relativePath = "search-engine.html";
        }
        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        string filePath = Path.Combine(basePath, relativePath);
        if (File.Exists(filePath))
        {
            byte[] buffer = File.ReadAllBytes(filePath); //передаем байты так как по сети передаются только байты не текст
            response.ContentLength64 = buffer.Length; //указываем длину ответа гость знает сколько ждать
            string ext = Path.GetExtension(filePath).ToLower();
            response.ContentType = ext switch
            {
                ".html" => "text/html; charset=utf-8",
                ".svg"  => "image/svg+xml"
            }; //дает указание как отобразить файл*/
            using (Stream output = response.OutputStream) //OutputStream труба между сервером и браузером кладем туда письмо и он уезжает к гостю но она толкьо открывает трубу
            {
                await output.WriteAsync(buffer, 0, buffer.Length); //вносим данные
            }
        }
        else
        {
            response.StatusCode = 404;
            byte[] buffer = Encoding.UTF8.GetBytes("<h1>404 — Файл не найден</h1>");
            response.ContentLength64 = buffer.Length;
            response.ContentType = "text/html, chars=utf-8";
            using (Stream output = response.OutputStream)
            {
                await output.WriteAsync(buffer,0, buffer.Length);
            }
        }
        
    }

    public void Stop()
    {
        _listener.Stop();
        _isWorking = false;
    }
}