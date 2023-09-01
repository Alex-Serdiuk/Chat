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

        public List<User> GetUsers()
        {
            return _context.Users.ToList();
        }

        public void SaveMessage(Message message)
        {
            _context.Messages.Add(message);
            _context.SaveChanges();
        }

        public User GetUserById(int userId)
        {
            User user = new User();

            try
            {
                using (var dbContext = new ServerContext())
                {
                    user = dbContext.Users.FirstOrDefault(u => u.Id == userId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting user from the database: " + ex.Message);
            }

            return user;
        }

        public List<Message> GetMessages(int senderId, int receiverId)
        {
            
            List<Message> messages = new List<Message>();
            User sender = GetUserById(senderId);
            User receiver = GetUserById(receiverId);
                try
                {
                    using (var dbContext = new ServerContext())
                    {
                    messages = dbContext.Messages
                            .Include(message => message.From)
                            .Include(message => message.To)
                            .Where(message =>
                    (message.From.Id == senderId && message.To.Id == receiverId) ||
                    (message.From.Id == receiverId && message.To.Id == senderId))
                            .OrderBy(message => message.CreatedAt)
                            .ToList();
                    //messages = dbContext.Messages
                    //    .Where(message =>
                    //        (message.From == sender && message.To == receiver) ||
                    //        (message.From == receiver && message.To == sender))
                    //    .OrderBy(message => message.CreatedAt)
                    //    .ToList();
                }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error getting messages from the database: " + ex.Message);
                }

                return messages;
            
        }

        private bool RegisterUser(string username, string login, string password)
        {
            try
            {
                using (var dbContext = new ServerContext())
                {
                    // Перевірте, чи ім'я користувача вже існує
                    if (dbContext.Users.Any(u => u.Name == username))
                    {
                        Console.WriteLine("Username already exists.");
                        return false;
                    }

                    // Перевірте, чи логін користувача вже існує
                    if (dbContext.Users.Any(u => u.Login == login))
                    {
                        Console.WriteLine("Login already exists.");
                        return false;
                    }

                    // Створіть нового користувача
                    var newUser = new User
                    {
                        Name = username,
                        Login = login,
                        Password = password
                    };

                    dbContext.Users.Add(newUser);
                    dbContext.SaveChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error registering user: " + ex.Message);
                return false;
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
