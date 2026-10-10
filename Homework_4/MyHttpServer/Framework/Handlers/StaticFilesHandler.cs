using System.Net;
using System.Text;

namespace MyHttpServer.Framework.Handlers;

internal class StaticFilesHandler : Handler
{
    public async override Task HandleRequest(HttpListenerContext context)
    {
        HttpListenerResponse response = context.Response;

        string requestedPath = context.Request.Url.LocalPath;

        bool isFile = requestedPath.Contains(".");
        if (isFile)
        {
            Console.WriteLine("Пришел запрос");
            try
            {
                string relativePath = requestedPath.TrimStart('/');

                string filePath = Directory.GetCurrentDirectory() + "/static/" + relativePath;

                FileInfo fileInfo = new FileInfo(filePath);

                if (!fileInfo.Exists)
                {
                    response.StatusCode = 404;

                    filePath = Directory.GetCurrentDirectory() + "/static/404.html";
                }

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

                byte[] buffer = await File.ReadAllBytesAsync(filePath);
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                return;
            }

            catch (Exception ex)
            {
                response.StatusCode = 505;
                await WriteResponseAsync(response, $"Ошибка при обработке запроса: {ex.Message}");
            }

            
        }
        // передача запроса дальше по цепи при наличии в ней обработчиков
        else if (Successor != null)
        {
           await Successor.HandleRequest(context);
        }

        async Task WriteResponseAsync(HttpListenerResponse response, string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer);
            //response.OutputStream.Close();
        }
    }
}
