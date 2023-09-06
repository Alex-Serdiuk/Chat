using CommonLibrary;
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
		//private List<User> users;


		public ClientHandler(TcpClient client, ChatService chatService)
		{
			_client = client;
			_stream = client.GetStream();
			_chatService = chatService;
			//users = chatService.GetUsers();
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
					} while (bytesRead > 1023);

					Console.WriteLine(strData);

					var wrapper = JsonSerializer.Deserialize<DataWrapper>(strData);

					if (wrapper.Type == DataType.Login)
					{
						HandleLogin(JsonSerializer.Deserialize<LoginData>(wrapper.Content));
					}
					else if (wrapper.Type == DataType.Register)
					{
						HandleRegister(JsonSerializer.Deserialize<RegisterData>(wrapper.Content));
					}
					else if (wrapper.Type == DataType.GetUsers)
					{
						SendUserList(JsonSerializer.Deserialize<GetDataRequest>(wrapper.Content));

					}
					else if (wrapper.Type == DataType.SendMessage)
					{
						HandleSendMessage(JsonSerializer.Deserialize<MessageData>(wrapper.Content));
					}
					else if (wrapper.Type == DataType.GetMessages)
					{
						HandleGetMessages(JsonSerializer.Deserialize<GetMessagesRequest>(wrapper.Content));
					}
                    else if (wrapper.Type == DataType.GetAll)
                    {
                        HandleGetUsersAndMessages(JsonSerializer.Deserialize<GetDataRequest>(wrapper.Content));
                    }

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

        private void HandleGetUsersAndMessages(ChatUser? chatUser)
        {
            throw new NotImplementedException();
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

		private void HandleRegister(RegisterData registerData)
		{
			// Отримання даних для регістрації
			string username = registerData.Name;
			string login = registerData.Login;
			string password = registerData.Password;

			// Перевірка авторизації через чат-сервіс
			var user = _chatService.RegisterUser(username, login, password);

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
				Type = DataType.Register,
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
			Message? message = null;

			message = new Message
			{
				From = _chatService.GetUser(messageData.From.Id),
				To = _chatService.GetUser(messageData.To.Id),
				Text = messageData.Text,
				CreatedAt = DateTime.Now
			};

			// Збереження повідомлення через чат-сервіс
			_chatService.SaveMessage(message);

			var response = new MessageResponse
			{
				IsSaveMessage = message != null,
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
		}

		private List<ChatUser> GetUsersFromDatabase(GetDataRequest request)
		{
            List<ChatUser> users = _chatService
                .GetUsers()
                .Select(user => new ChatUser
                {
                    Id = user.Id,
                    Name = user.Name,
                    Login = user.Login
                })
                .ToList();

            foreach (var item in users)
            {
				item.Messages = _chatService
                .GetMessages(
                request.From.Id, item.Id, request.AfterId == null ? 0 : (int)request.AfterId)
                .Select(FromDbMessage)
                .ToList();
            }

            return users;
		}

		private ChatUser GetChatUserById(int id)
		{
			return FromDbUser(_chatService.GetUser(id));
		}

		private void SendUserList(GetDataRequest request)
		{
			// Реалізуйте логіку відправки списку користувачів на _stream
			// Отримайте список користувачів з бази даних або деінде
			List<ChatUser> users = GetUsersFromDatabase(request);

			var wrapper = new DataWrapper
			{
				Type = DataType.GetUsers,
				Content = JsonSerializer.Serialize(users)
			};

			// Відправте рядок зі списком користувачів на _stream
			// Відправка відповіді клієнту

			var json = JsonSerializer.Serialize(wrapper);
			Console.WriteLine(json);
			byte[] responseData = Encoding.UTF8.GetBytes(json);
			_stream.Write(responseData, 0, responseData.Length);
		}

		private ChatUser FromDbUser(User user)
		{
			return new ChatUser
			{
				Id = user.Id,
				Name = user.Name,
				Login = user.Login
			};
		}

		private MessageData FromDbMessage(Message message)
		{
			return new MessageData
			{
				From = FromDbUser(message.From),
				To = FromDbUser(message.To),
				CreatedAt = message.CreatedAt,
				Text = message.Text
			};
		}

		private void HandleGetMessages(GetMessagesRequest request)
		{
			var messages = _chatService
				.GetMessages(
				request.From.Id, request.To.Id, request.AfterId == null ? 0 : (int)request.AfterId)
				.Select(FromDbMessage)
				.ToList();

			var wrapper = new DataWrapper
			{
				Type = DataType.GetMessages,
				Content = JsonSerializer.Serialize(messages)
			};

			var json = JsonSerializer.Serialize(wrapper);
			Console.WriteLine(json);
			byte[] responseData = Encoding.UTF8.GetBytes(json);
			_stream.Write(responseData, 0, responseData.Length);
		}
        private void HandleGetUsersAndMessages(GetDataRequest request)
        {
            var messages = _chatService
                 .GetMessagesFromAll(
                 request.From.Id,  request.AfterId == null ? 0 : (int)request.AfterId)
                 .Select(FromDbMessage)
                 .ToList();

            List<ChatUser> users = GetUsersFromDatabase(request);

			GetDataResponse data = new GetDataResponse();
			data.users = users;
			data.Messages = messages;

            var wrapper = new DataWrapper
            {
                Type = DataType.GetUsers,
                Content = JsonSerializer.Serialize(data)
            };

            // Відправте рядок зі списком користувачів на _stream
            // Відправка відповіді клієнту

            var json = JsonSerializer.Serialize(wrapper);
            Console.WriteLine(json);
            byte[] responseData = Encoding.UTF8.GetBytes(json);
            _stream.Write(responseData, 0, responseData.Length);
        }
    }

}
