using System.Data;
using System.Linq;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace BKabanApi.Models
{
    public interface IUserRepository
    {
        int? GetUserIdFullMatch(UserModel user);
        int? GetUserIdByUsername(string username);
        int? Create(UserModel user);
    }

    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        private readonly SHA1 _sha1 = SHA1.Create();
        private readonly RNGCryptoServiceProvider _secureRandom = new RNGCryptoServiceProvider();

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private byte[] GetSha1FromString(string str, byte[] salt)
        {
            return _sha1.ComputeHash(Encoding.Unicode.GetBytes(str).Concat(salt).ToArray());
        }

        public int? GetUserIdFullMatch(UserModel user)
        {
            using IDbConnection db = new SqlConnection(_connectionString);

            byte[] salt = db.Query<byte[]>(@"SELECT Salt FROM UserTable WHERE Username = @username", new { username = user.Username }).FirstOrDefault();

            if (salt == null)
            {
                return null;
            }

            return db.Query<int?>(@"SELECT Id
                FROM UserTable
                WHERE PasswordSha1 = @pwdHash AND Username = @username", new { username = user.Username, pwdHash = GetSha1FromString(user.Password, salt) }).FirstOrDefault();
        }

        public int? GetUserIdByUsername(string username)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            return db.Query<int?>(@"SELECT Id
                FROM UserTable
                WHERE Username = @username", new { username }).FirstOrDefault();
        }

        public int? Create(UserModel user)
        {
            using IDbConnection db = new SqlConnection(_connectionString);

            var salt = new byte[8];
            _secureRandom.GetBytes(salt);

            return db.Query<int?>(@"INSERT INTO UserTable (Username, PasswordSha1, Salt) 
                OUTPUT INSERTED.Id
                VALUES (@username, @pwdHash, @salt)", new { username = user.Username, pwdHash = GetSha1FromString(user.Password, salt), salt }).FirstOrDefault();
        }
    }
}
