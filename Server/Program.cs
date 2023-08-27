
using System.Net.Sockets;
using System.Net;
using Server;
using Server.Models;

IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
int port = 12345;

TcpListener listener = new TcpListener(ipAddress, port);
listener.Start();
Console.WriteLine("Server started...");

// Створення об'єкту для роботи з базою даних та чат-сервісу
ChatService chatService = new ChatService();

var db = new ServerContext();
if (db.Users.FirstOrDefault(x => x.Login == "admin") == null)
{
    Console.WriteLine("Add admin user");
    db.Users.Add(new User
    {
        Login = "admin",
        Password = "admin",
    });
    db.SaveChanges();
} else
{
    Console.WriteLine("Admin exist");
}


while (true)
{
    // Очікування підключення клієнта
    TcpClient client = listener.AcceptTcpClient();
    Console.WriteLine("Client connected...");

    // Створення обробника клієнта для обробки його дій
    var clientHandler = new ClientHandler(client, chatService);
    var handlerThread = new Thread(clientHandler.HandleClient);
    handlerThread.Start();
    //client.Close();
}