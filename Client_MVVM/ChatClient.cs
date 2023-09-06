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

namespace Client_MVVM;

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
            // bytesRead = _stream.Socket.Receive(buffer);
            bytesRead = _stream.Read(buffer, 0, buffer.Length);
            strData += Encoding.UTF8.GetString(buffer, 0, bytesRead);

        } while (bytesRead > 1023);

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

    public ChatUser? Register(string name, string login, string password)
    {
        var registerData = new RegisterData
        {
            Name = name,
            Login = login,
            Password = password
        };

        var requestWrapper = new DataWrapper
        {
            Type = DataType.Register,
            Content = JsonSerializer.Serialize(registerData)
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
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error getting users from server: " + ex.Message);
        }

        return users;
    }

    public List<MessageData>? GetMessagesFromServer(GetMessagesRequest request)
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
            string responseData = ReceiveData();
            var responseWrapper = JsonSerializer.Deserialize<DataWrapper>(responseData);
            messages = JsonSerializer.Deserialize<List<MessageData>>(responseWrapper.Content);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error getting messages from server: " + ex.Message);
        }

        return messages;
    }

    public bool SendMessage(ChatUser Me, ChatUser receiver, string message)
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
        string responseData = ReceiveData();

        var responseWrapper = JsonSerializer.Deserialize<DataWrapper>(responseData);
        var result = JsonSerializer.Deserialize<MessageResponse>(responseWrapper.Content);
        return result.IsSaveMessage;
    }

    public GetDataResponse GetUsersAndMessages(ChatUser Me, int startId)
    {
        GetDataResponse data = new GetDataResponse();
        try
        {
            GetMessagesRequest request = new GetMessagesRequest();
            request = new GetMessagesRequest()
            {
                From = Me,
                AfterId = startId
            };
            var requestWrapper = new DataWrapper
            {
                Type = DataType.GetAll,
                Content = JsonSerializer.Serialize(request)
            };

            SendData(JsonSerializer.Serialize(requestWrapper));
            string responseData = ReceiveData();
            var responseWrapper = JsonSerializer.Deserialize<DataWrapper>(responseData);
            data = JsonSerializer.Deserialize<GetDataResponse>(responseWrapper.Content);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error getting users messages from server: " + ex.Message);
        }

        return data;
    }


}
