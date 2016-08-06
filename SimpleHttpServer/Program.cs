using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHttpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "SimpleHttpServer";

            HttpListener httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://*:8080/");
            httpListener.Start();

            Console.WriteLine("Server listening on Port 8080");

            while (true)
            {
                HttpListenerContext context = httpListener.GetContext(); // waiting for connections
                HttpListenerRequest req = context.Request;
                HttpListenerResponse res = context.Response;

                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {req.RemoteEndPoint} {req.HttpMethod} {req.Url}");

                string payload = "Hello World!\n";

                foreach (var queryItem in req.QueryString.AllKeys.Select(key => new { Key = key, Value = req.QueryString[key] }))
                {
                    Console.WriteLine($"{queryItem.Key}: {queryItem.Value}");
                }

                byte[] buffer = Encoding.UTF8.GetBytes(payload);
                res.ContentLength64 = buffer.Length;
                res.OutputStream.Write(buffer, 0, buffer.Length);

                res.OutputStream.Close();
            }
        }
    }
}
