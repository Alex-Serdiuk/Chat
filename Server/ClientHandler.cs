using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ClientHandler
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private ChatService _chatService;
        private List<User> users;
        

        public ClientHandler(TcpClient client, ChatService chatService)
        {
            _client = client;
            _stream = client.GetStream();
            _chatService = chatService;
            users = chatService.GetUsers();
        }

        public void HandleClient()
        {
            try
            {
                byte[] data = new byte[1024];
                int bytesRead;

                while (true)
                {
                    bytesRead = _stream.Read(data, 0, data.Length);
                    string message = Encoding.ASCII.GetString(data, 0, bytesRead);

                    // Розділити отримане повідомлення на частини
                    string[] parts = message.Split('|');

                    // Перший елемент має вказувати на дію
                    string action = parts[0];

                    // Обробка різних дій в залежності від значення action
                    switch (action)
                    {
                        case "LOGIN":
                            HandleLogin(parts);
                            break;
                        case "SEND":
                            HandleSendMessage(parts);
                            break;
                        case "GET_USERS":
                            // Реалізуйте обробку запиту на список користувачів
                            SendUserList();
                            break;
                        // Додаткові дії можна обробляти тут
                        default:
                            //Console.WriteLine("Unknown action: " + action);
                            break;
                    }
                    if (message == "exit")
                    {
                        break;
                    }

                    // Очистити буфер для наступного повідомлення
                    data = new byte[1024];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error handling client: " + ex.Message);
            }
            finally
            {
                // Закриття з'єднання та звільнення ресурсів
                _stream.Close();
                _client.Close();
            }
        }

        private void HandleLogin(string[] parts)
        {
            // Отримання даних для авторизації
            string username = parts[1];
            string password = parts[2];

            // Перевірка авторизації через чат-сервіс
            bool isAuthenticated = _chatService.Authenticate(username, password);

            // Відправка відповіді клієнту
            string response = isAuthenticated ? "AUTHENTICATED" : "AUTH_FAILED";
            byte[] responseData = Encoding.ASCII.GetBytes(response);
            _stream.Write(responseData, 0, responseData.Length);
        }

        private void HandleSendMessage(string[] parts)
        {
            
            // Отримання даних повідомлення
            string content = parts[1];

            // Отримання інформації про користувача-відправника
            int senderId = GetCurrentUserId(); // Ваш спосіб отримання ID користувача

            // Отримання ID отримувача повідомлення (це може бути вибір зі списку, чи інше)
            int receiverId = GetReceiverId(parts[2]); // Ваш спосіб отримання ID отримувача

            // Створення об'єкта повідомлення
            Message message = new Message
            {
                From = users.FirstOrDefault(u => u.Id == senderId),
                To = users.FirstOrDefault(u => u.Id == receiverId),
                Text = content,
                CreatedAt = DateTime.Now
            };

            // Збереження повідомлення через чат-сервіс
            _chatService.SaveMessage(message);

            // Опціонально: Відправити підтвердження клієнту
            string response = "MESSAGE_SENT";
            byte[] responseData = Encoding.ASCII.GetBytes(response);
            _stream.Write(responseData, 0, responseData.Length);
        }

        private void SendUserList()
        {
            // Реалізуйте логіку відправки списку користувачів на _stream
            // Отримайте список користувачів з бази даних або деінде
            List<string> users = GetUsersFromDatabase();

            // Створіть рядок зі списком користувачів, розділених "|"
            string userListString = string.Join("|", users);

            // Відправте рядок зі списком користувачів на _stream
            byte[] data = Encoding.ASCII.GetBytes(userListString);
            _stream.Write(data, 0, data.Length);
        }

        private List<string> GetUsersFromDatabase()
        {
            List<string> users = new List<string>();

            try
            {
                using (var dbContext = new ServerContext())
                {
                    users = dbContext.Users.Select(user => user.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting users from the database: " + ex.Message);
            }

            return users;
        }

        private int GetCurrentUserId()
        {
            // Отримайте ID поточного авторизованого користувача зі свого механізму аутентифікації.
            // Наприклад, якщо ви використовуєте аутентифікацію на основі JWT, тут ви можете отримати ID з JWT токену.
            // Це буде залежати від вашої конкретної реалізації аутентифікації.
            // Для цілей прикладу, повертаємо просто фіксований ID 1.
            return 1;
        }

        private int GetReceiverId(string receiverName)
        {
            // Отримайте ID отримувача на основі його імені (логіну) з бази даних чи іншого місця збереження.
            // Це приклад та спрощений метод, реально ви повинні виконати запит до бази даних для отримання ID по імені.
            // Для цілей прикладу, повертаємо просто фіксований ID 2.
            return 2;
        }
    }

}
