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

    //private string ReceiveData()
    //{
    //    //using (var memoryStream = new MemoryStream())
    //    //{
    //    //    var buffer = new byte[1024];
    //    //    int bytesRead = 0;

    //    //    do
    //    //    {
    //    //        bytesRead = _stream.Read(buffer, 0, buffer.Length);
    //    //        memoryStream.Write(buffer, 0, bytesRead);
    //    //    } while (bytesRead > 1024);

    //    //    // Перетворюємо MemoryStream у рядок
    //    //    return Encoding.UTF8.GetString(memoryStream.ToArray());
    //    //}


    //    var buffer = new byte[4096];
    //    int bytesRead = 0;

    //    string strData = "";
    //    do
    //    {
    //        bytesRead = _stream.Read(buffer, 0, buffer.Length);
    //        strData += Encoding.UTF8.GetString(buffer, 0, bytesRead);

    //    } while (bytesRead > 4096);

    //    return strData;
    //}



    //private string ReceiveData()
    //{
    //    var data = new byte[4096];
    //    int bytesRead = _stream.Read(data, 0, data.Length);
    //    return Encoding.UTF8.GetString(data, 0, bytesRead);
    //}

    private async Task<string> ReceiveData()
    {
        byte[] buffer = new byte[4096];
        int bytesRead;
        MemoryStream memoryStream = new MemoryStream();

        try
        {
            do
            {
                bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    memoryStream.Write(buffer, 0, bytesRead);
                }
            } while (bytesRead > 0);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error receiving data: " + ex.Message);
            return string.Empty;
        }

        // Повертаємо рядок UTF-8 з MemoryStream
        return Encoding.UTF8.GetString(memoryStream.ToArray());
    }

    public async Task<ChatUser?> LoginAsync(string login, string password)
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
		string responseData = await ReceiveData();

		var responseWrapper = JsonSerializer.Deserialize<DataWrapper>(responseData);
		var result = JsonSerializer.Deserialize<LoginResponse>(responseWrapper.Content);
		return result.User;
	}

    public async Task<List<ChatUser>?> GetUsersFromServerAsync()
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
            string responseData = await ReceiveData();

            var responseWrapper = JsonSerializer.Deserialize<DataWrapper>(responseData);
            users = JsonSerializer.Deserialize<List<ChatUser>>(responseWrapper.Content);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error getting users from server: " + ex.Message);
        }

        return users;
    }

    public async Task<List<MessageData>?> LoadMessagesAsync(GetMessagesRequest request)
    {
        List<MessageData>? messages = new List<MessageData>();
        try
        {
            var requestWrapper = new DataWrapper
            {
                Type = DataType.GetMessages,
                Content = JsonSerializer.Serialize(request)
            };

            SendData(JsonSerializer.Serialize(requestWrapper));
            string responseData = await ReceiveData();
            var responseWrapper = JsonSerializer.Deserialize<DataWrapper>(responseData);
            messages = JsonSerializer.Deserialize<List<MessageData>>(responseWrapper.Content);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error getting messages from server: " + ex.Message);
        }

        return messages;
    }

    public async Task<bool> SendMessage(ChatUser Me, ChatUser receiver, string message)
    {
        MessageData messageData = new MessageData();
        messageData.From = Me;
        messageData.To = receiver;
        messageData.Text = message;

        var requestWrapper = new DataWrapper
        {
            Type = DataType.SendMessage,
            Content = JsonSerializer.Serialize(messageData)
        };


        SendData(JsonSerializer.Serialize(requestWrapper));
        string responseData =await ReceiveData();

        var responseWrapper = JsonSerializer.Deserialize<DataWrapper>(responseData);
        var result = JsonSerializer.Deserialize<MessageResponse>(responseWrapper.Content);
        return result.IsSaveMessage;
    }

    

    
}
