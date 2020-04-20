using System.Data;
using System.Linq;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BKabanApi.Models
{
    public interface IUserRepository
    {
        int? GetUserIdFullMatch(string email, string password);
        int? GetIdByEmail(string email);
        void Create(UserCredentials user);
    }

    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int? GetUserIdFullMatch(string email, string password)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return db.Query<int?>("SELECT Id FROM UserTable WHERE Email = @email AND Password = @password", new {email, password}).FirstOrDefault();
            }
        }

        public int? GetIdByEmail(string email)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return db.Query<int?>("SELECT Id FROM UserTable WHERE Email = @email", new { email }).FirstOrDefault();
            }
        }

        public void Create(UserCredentials user)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Execute("INSERT INTO UserTable (Email, Password) VALUES (@Email, @Password)", user);
            }
        }
    }
}
