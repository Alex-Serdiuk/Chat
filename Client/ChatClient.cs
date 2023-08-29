using CommonLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Client;

public class ChatClient
{
	private TcpClient _client;
	private NetworkStream _stream;

	public ChatClient(string ip, int port)
	{
		_client = new TcpClient(ip, port);
		_stream = _client.GetStream();
	}

	private void SendData(string data)
	{
		_stream.Write(Encoding.UTF8.GetBytes(data), 0, data.Length);
	}

	private string ReceiveData()
	{
		var buffer = new byte[1024];
        int bytesRead = 0;
		string strData = "";
		do
		{
            bytesRead = _stream.Read(buffer, 0, buffer.Length);
			strData += Encoding.UTF8.GetString(buffer, 0, bytesRead);
        } while (bytesRead > 1024);

		return strData;
	}

	public ChatUser? Login(string login, string password)
	{
		var loginData = new LoginData
		{
			Login = login,
			Password = password
		};

		var requestWrapper = new DataWrapper
		{
			Type = DataType.Login,
			Content = JsonSerializer.Serialize(loginData)
		};

		SendData(JsonSerializer.Serialize(requestWrapper));
		string responseData = ReceiveData();

		var responseWrapper = JsonSerializer.Deserialize<DataWrapper>(responseData);
		var result = JsonSerializer.Deserialize<LoginResponse>(responseWrapper.Content);
		return result.User;
	}

    public List<ChatUser>? GetUsersFromServer()
    {

        List<ChatUser>? users = new List<ChatUser>();
        try
        {

            var requestWrapper = new DataWrapper
            {
                Type = DataType.GetUsers,
                Content = ""
            };

            SendData(JsonSerializer.Serialize(requestWrapper));
            string responseData = ReceiveData();

            var responseWrapper = JsonSerializer.Deserialize<DataWrapper>(responseData);
            users = JsonSerializer.Deserialize<List<ChatUser>>(responseWrapper.Content);
            //// Встановіть з'єднання з сервером
            //TcpClient client = new TcpClient("127.0.0.1", 12345);
            //NetworkStream stream = client.GetStream();

            //// Відправте запит на отримання списку користувачів
            //byte[] data = Encoding.UTF8.GetBytes("GET_USERS");
            //stream.Write(data, 0, data.Length);

            //// Отримайте відповідь від сервера
            //data = new byte[1024];
            //int bytesRead = stream.Read(data, 0, data.Length);
            //string response = Encoding.UTF8.GetString(data, 0, bytesRead);

            //// Розібрати отриману відповідь і додати користувачів до списку
            //if (!string.IsNullOrEmpty(response))
            //{
            //    string[] userArray = response.Split('|');
            //    users.AddRange(userArray);
            //}

            //// Закрийте з'єднання
            //stream.Close();
            //client.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error getting users from server: " + ex.Message);
        }

        return users;
    }
}
