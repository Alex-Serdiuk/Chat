using Microsoft.EntityFrameworkCore;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
	// Клас для роботи з авторизацією та повідомленнями
	public class ChatService
	{
		private ServerContext _context;

		public ChatService()
		{
			_context = new ServerContext();
		}

		public User? Authenticate(string login, string password)
		{
			Console.WriteLine("Try auth: {0} {1}", login, password);
			var user = _context.Users.FirstOrDefault(u => u.Login == login && u.Password == password);

			return user;
		}

		public User? GetUser(int id) {

			return _context.Users.FirstOrDefault(x => x.Id == id);
		}

		public List<User> GetUsers()
		{
			return _context.Users.ToList();
		}

		public void SaveMessage(Message message)
		{
			_context.Messages.Add(message);
			_context.SaveChanges();
		}

		public List<Message> GetMessages(int senderId, int receiverId, int afterId)
		{
			return _context
				.Messages
				.Include(message => message.From)
				.Include(message => message.To)
				.Where(x => x.Id > afterId)
				.Where(x =>
					(x.From.Id == senderId && x.To.Id == receiverId) ||
					(x.From.Id == receiverId && x.To.Id == senderId)
					)
				.OrderBy(message => message.Id)
				.ToList();
		}

        public List<Message> GetMessagesFromAll(int senderId, int afterId)
        {
            return _context
                .Messages
                .Include(message => message.From)
                .Include(message => message.To)
                .Where(x => x.Id > afterId)
                .Where(x =>
                    (x.From.Id == senderId) ||
                    (x.To.Id == senderId)
                    )
                .OrderBy(message => message.Id)
                .ToList();
        }

        public User? RegisterUser(string username, string login, string password)
		{
			Console.WriteLine("Try reg: {0} {1}", login, password);

			try
			{

				// Перевірте, чи ім'я користувача вже існує
				if (_context.Users.Any(u => u.Name == username))
				{
					Console.WriteLine("Username already exists.");
					return null;
				}

				// Перевірте, чи логін користувача вже існує
				if (_context.Users.Any(u => u.Login == login))
				{
					Console.WriteLine("Login already exists.");
					return null;
				}

				// Створіть нового користувача
				var newUser = new User
				{
					Name = username,
					Login = login,
					Password = password
				};

				_context.Users.Add(newUser);
				_context.SaveChanges();

				return newUser;

			}
			catch (Exception ex)
			{
				Console.WriteLine("Error registering user: " + ex.Message);
				return null;
			}
		}

		private bool DeleteMessageById(int messageId)
		{
			try
			{
				using (var dbContext = new ServerContext())
				{
					var message = dbContext.Messages.FirstOrDefault(m => m.Id == messageId);

					if (message != null)
					{
						dbContext.Messages.Remove(message);
						dbContext.SaveChanges();
						return true;
					}
					else
					{
						Console.WriteLine("Message not found.");
						return false;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error deleting message: " + ex.Message);
				return false;
			}
		}
	}
}
