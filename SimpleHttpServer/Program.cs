using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimpleHttpServer
{
    class Program
    {
        const int port = 8080;

        static void Main(string[] args)
        {
            Console.Title = "SimpleHttpServer";
            string listenerPrefix = $"http://+:{port}/";

            try
            {
                if (!NetshHelper.AddressIsRegisteredForCurrentUser(listenerPrefix))
                {
                    Console.WriteLine($"Press any key to allow the current user to access {listenerPrefix}");
                    Console.ReadKey();
                    NetshHelper.RegisterAddressForCurrentUser(listenerPrefix);
                }

                HttpListener httpListener = new HttpListener();
                httpListener.Prefixes.Add(listenerPrefix);
                httpListener.Start();

                Console.WriteLine($"Server now listening on port {port}");

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
            catch (HttpListenerException ex) when (ex.ErrorCode == 5)
            {
                string[] allowedUsers = NetshHelper.GetUserRegistrationForAddress(listenerPrefix);

                Console.WriteLine("Access denied.");
                if (allowedUsers.Length == 0)
                {
                    Console.WriteLine($"There are no users registered for the address {listenerPrefix}");
                }
                else
                {
                    string allowedUsersString = allowedUsers.Select(x => $" - {x}").Aggregate((a, b) => $"{a}\n{b}");
                    Console.WriteLine($"Allowed users for the address {listenerPrefix} are:\n{allowedUsersString}");
                }
                
                Console.ReadKey();
            }
        }
    }
}
