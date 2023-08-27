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
		var data = new byte[1024];
		int bytesRead = _stream.Read(data, 0, data.Length);
		return Encoding.UTF8.GetString(data, 0, bytesRead);
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
}
