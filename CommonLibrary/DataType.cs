using System.ComponentModel.DataAnnotations;

namespace CommonLibrary;

public static class DataType
{
	public static readonly string Login = "LOGIN";
	public static readonly string Register = "REGISTER";
	public static readonly string GetUsers = "GET_USERS";
	
	public static readonly string GetMessages = "GET_MESSAGES";
	public static readonly string SendMessage = "SEND_MESSAGE";
	public static readonly string GetAll = "GET_ALL";
}