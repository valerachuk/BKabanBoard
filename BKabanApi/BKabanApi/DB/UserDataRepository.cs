using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BKabanApi.Models
{
    public interface IUserDataRepository
    {
        UserDataModel GetUserData(int userId);
    }

    public class UserDataRepository : IUserDataRepository
    {
        private readonly string _connectionString;

        public UserDataRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public UserDataModel GetUserData(int userId)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            var userData = new UserDataModel
            {
                Boards = db.Query<BoardModel>(
                    @"SELECT B.Id, B.Name
                    FROM BoardTable AS B
                    JOIN UserTable AS U ON U.Id = B.UserId
                    WHERE U.Id = @userId
                    ORDER BY Position", new {userId})
            };

            return userData;
        }
    }
}
