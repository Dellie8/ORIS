using MyHttpServer;
using System.Net;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        string settingsText = File.ReadAllText("settings.json");
        Settings settings = JsonSerializer.Deserialize<Settings>(settingsText); //создаем объект Settings и заполняем значениями из JSON
        HttpServer httpServer = new HttpServer(settings.Serverr.Host, settings.Serverr.Port, settings.Serverr.Path);
        httpServer.Start();  
        Console.WriteLine("Введите 'stop'");
        string command = Console.ReadLine();
        if (command?.ToLower() == "stop")
        {
            httpServer.Stop();
        } 
        //await task; 
    }
}

/*//чтение настроек
var settingsText = File.ReadAllText("settings.json"); //программа получает из файла текст
Settings settings = JsonSerializer.Deserialize<Settings>(settingsText);// текст превращаем в объект settings

//создание и запуск сервера
HttpServer httpServer = new HttpServer(settings.Serverr.Host, settings.Serverr.Port, settings.Serverr.Path);
HttpListener server = new HttpListener(); //server стоит у двери и ждет гостей 
// установка адресов прослушки
server.Prefixes.Add($"http://{settings.Serverr.Host}:{settings.Serverr.Port}/{settings.Serverr.Path}"); //вешаем на дверь таблицу с адресом
server.Start(); // начинаем прослушивать входящие подключения или открываем ресторан
// получаем контекст
HttpListenerContext context = await server.GetContextAsync(); //щвейцар щамираети смотрит в окно ничего не делаем пока не прийдет первый гость (браузер)

//обработка запроса
var response = context.Response; //швейцар берет чистую тарелку (response) чтобы положить еду для гостя
// отправляемый в ответ код htmlвозвращает
string responseText = //начинаем готовить
    @"<!DOCTYPE html> 
    <html>
        <head>
            <meta charset='utf8'>
            <title>METANIT.COM</title>
        </head>
        <body>
            <h2>Hello METANIT.COM</h2>
        </body>
    </html>";
byte[] buffer = Encoding.UTF8.GetBytes(responseText);//упакуй текст в коробку с байтами
// получаем поток ответа и пишем в него ответ
response.ContentLength64 = buffer.Length; //.ContentLength64 - наклейка с весом посылки
using Stream output = response.OutputStream; //поток - труба но которой идет посылка response.OutputStream-труба которая ведет напрямую к гостю (в браузер) using - когда закончишь работу с потоком закрой его
// отправляем данные
await output.WriteAsync(buffer); //WriteAsync(buffer) - щвейцар кладет коробку в трубу и отправляет ее
await output.FlushAsync(); //FlushAsync() - гарантирет что гость вс получил
Console.WriteLine("Запрос обработан");
 
//остановка
server.Stop(); //закрываем ресторан */