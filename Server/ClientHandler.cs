﻿using CommonLibrary;
using Server.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
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
                var buffer = new byte[1024];
                int bytesRead;

				while (true)
				{
                    bytesRead = 0;
                    string strData = "";
                    do
                    {
                        bytesRead = _stream.Read(buffer, 0, buffer.Length);
                        strData += Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    } while (bytesRead > 1024);

                    Console.WriteLine(strData);

					var wrapper = JsonSerializer.Deserialize<DataWrapper>(strData);

					if (wrapper.Type == DataType.Login)
					{
						HandleLogin(JsonSerializer.Deserialize<LoginData>(wrapper.Content));
					}
                    else if (wrapper.Type == DataType.GetUsers)
                    {
                        SendUserList();

                    }else if (wrapper.Type == DataType.SendMessage)
                    {
                        HandleSendMessage(JsonSerializer.Deserialize<MessageData>(wrapper.Content));
                    }
                    else if (wrapper.Type == DataType.GetMessages)
                    {
                        HandleGetMessages(JsonSerializer.Deserialize<GetMessagesRequest>(wrapper.Content));
                    }



                    // Обробка різних дій в залежності від значення action
                    //switch (wrapper.Type)
                    //{
                    //    case DataType.Login:
                    //        HandleLogin(parts);
                    //        break;
                    //    case "SEND":
                    //        HandleSendMessage(parts);
                    //        break;
                    //    case "GET_USERS":
                    //        // Реалізуйте обробку запиту на список користувачів
                    //        SendUserList();
                    //        break;
                    //    // Додаткові дії можна обробляти тут
                    //    default:
                    //        //Console.WriteLine("Unknown action: " + action);
                    //        break;
                    //}
                    //if (message == "exit")
                    //{
                    //    break;
                    //}

                    // Очистити буфер для наступного повідомлення
                    buffer = new byte[1024];
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

		private void HandleLogin(LoginData loginData)
		{
			// Отримання даних для авторизації
			string username = loginData.Login;
			string password = loginData.Password;

			// Перевірка авторизації через чат-сервіс
			var user = _chatService.Authenticate(username, password);

			var response = new LoginResponse
			{
				IsLoggedIn = user != null,
				User = user != null ? new ChatUser
				{
					Id = user.Id,
					Name = user.Name,
					Login = user.Login,
				} : null
			};

			var wrapper = new DataWrapper
			{
				Type = DataType.Login,
				Content = JsonSerializer.Serialize(response)
			};

			// Відправка відповіді клієнту

			var json = JsonSerializer.Serialize(wrapper);
			Console.WriteLine(json);
			byte[] responseData = Encoding.UTF8.GetBytes(json);
			_stream.Write(responseData, 0, responseData.Length);
		}

		private void HandleSendMessage(MessageData messageData)
		{

            //// Отримання даних повідомлення
            //string content = parts[1];

            //// Отримання інформації про користувача-відправника
            //int senderId = GetCurrentUserId(); // Ваш спосіб отримання ID користувача

            //// Отримання ID отримувача повідомлення (це може бути вибір зі списку, чи інше)
            //int receiverId = GetReceiverId(parts[2]); // Ваш спосіб отримання ID отримувача

            // Створення об'єкта повідомлення
            Message? message = null;

            message = new Message
			{
				From = users.FirstOrDefault(u => u.Id == messageData.From.Id),
				To = users.FirstOrDefault(u => u.Id == messageData.To.Id),
				Text = messageData.Text,
				CreatedAt = DateTime.Now
			};

			// Збереження повідомлення через чат-сервіс
			_chatService.SaveMessage(message);

            var response = new MessageResponse
            {
                IsSaveMessage = message!= null,
            };

            var wrapper = new DataWrapper
            {
                Type = DataType.SendMessage,
                Content = JsonSerializer.Serialize(response)
            };

            // Відправка відповіді клієнту

            var json = JsonSerializer.Serialize(wrapper);
            Console.WriteLine(json);
            byte[] responseData = Encoding.UTF8.GetBytes(json);
            _stream.Write(responseData, 0, responseData.Length);

            //// Опціонально: Відправити підтвердження клієнту
            //string response = "MESSAGE_SENT";
            //byte[] responseData = Encoding.UTF8.GetBytes(response);
            //_stream.Write(responseData, 0, responseData.Length);
        }

        //private void HandleSendMessage(string[] parts)
        //{

        //    // Отримання даних повідомлення
        //    string content = parts[1];

        //    // Отримання інформації про користувача-відправника
        //    int senderId = GetCurrentUserId(); // Ваш спосіб отримання ID користувача

        //    // Отримання ID отримувача повідомлення (це може бути вибір зі списку, чи інше)
        //    int receiverId = GetReceiverId(parts[2]); // Ваш спосіб отримання ID отримувача

        //    // Створення об'єкта повідомлення
        //    Message message = new Message
        //    {
        //        From = users.FirstOrDefault(u => u.Id == senderId),
        //        To = users.FirstOrDefault(u => u.Id == receiverId),
        //        Text = content,
        //        CreatedAt = DateTime.Now
        //    };

        //    // Збереження повідомлення через чат-сервіс
        //    _chatService.SaveMessage(message);

        //    // Опціонально: Відправити підтвердження клієнту
        //    string response = "MESSAGE_SENT";
        //    byte[] responseData = Encoding.UTF8.GetBytes(response);
        //    _stream.Write(responseData, 0, responseData.Length);
        //}

        
		private List<ChatUser> GetUsersFromDatabase()
		{
            List<ChatUser> chatUsers = new List<ChatUser>();

            try
            {
                using (var dbContext = new ServerContext())
                {
                    chatUsers = dbContext.Users
                        .Select(user => new ChatUser
                        {
                            Id = user.Id,
                            Name = user.Name,
                            Login = user.Login
                        })
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting chat users from the database: " + ex.Message);
            }

            return chatUsers;
        }
        private ChatUser GetChatUserById(int id)
        {
            ChatUser chatUser = new ChatUser();
            try
            {
                User user = _chatService.GetUserById(id);

                chatUser.Id = user.Id;
                chatUser.Name = user.Name;
                chatUser.Login = user.Login;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting chat users from the database: " + ex.Message);
            }

            return chatUser;
        }

        private void SendUserList()
        {
            // Реалізуйте логіку відправки списку користувачів на _stream
            // Отримайте список користувачів з бази даних або деінде
            List<ChatUser> users = GetUsersFromDatabase();


            var wrapper = new DataWrapper
            {
                Type = DataType.GetUsers,
                Content = JsonSerializer.Serialize(users)
            };
            // Створіть рядок зі списком користувачів, розділених "|"
            //string userListString = string.Join("|", users);

            // Відправте рядок зі списком користувачів на _stream
            // Відправка відповіді клієнту

            var json = JsonSerializer.Serialize(wrapper);
            Console.WriteLine(json);
            byte[] responseData = Encoding.UTF8.GetBytes(json);
            _stream.Write(responseData, 0, responseData.Length);
        }

        private void HandleGetMessages(GetMessagesRequest request)
        {
            List<MessageData> sendMessages = new List<MessageData>();

            List<Message> messages = _chatService.GetMessages(request.From.Id, request.To.Id);

            for (int i = (int)request.AfterId; i < messages.Count; i++)
            {
                Message message = messages[i];
                MessageData messageData = new MessageData
                {
                    From = GetChatUserById(message.From.Id),
                    To = GetChatUserById(message.To.Id),
                    CreatedAt = message.CreatedAt,
                    Text = message.Text
                };

                sendMessages.Add(messageData);

               
            }
            var wrapper = new DataWrapper
            {
                Type = DataType.GetUsers,
                Content = JsonSerializer.Serialize(sendMessages)
            };

            var json = JsonSerializer.Serialize(wrapper);
            Console.WriteLine(json);
            byte[] responseData = Encoding.UTF8.GetBytes(json);
            _stream.Write(responseData, 0, responseData.Length);
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
