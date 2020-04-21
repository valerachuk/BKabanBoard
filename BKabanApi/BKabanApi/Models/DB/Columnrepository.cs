using System.Data;
using System.Linq;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BKabanApi.Models.DB
{
    public interface IColumnRepository
    {
        int? CreateColumn(int userId, ColumnModel column);
        bool UpdateColumn(int userId, ColumnModel column);
        bool DeleteColumn(int userId, int id);
    }

    public class ColumnRepository : IColumnRepository
    {
        private readonly string _connectionString;

        public ColumnRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public static bool IsAllowed(IDbConnection db, int userId, int columnId)
        {
            return db.Query(@"SELECT C.Id
                FROM UserTable AS U
                JOIN BoardTable AS B ON B.UserId=U.Id
                JOIN ColumnTable AS C ON C.BoardId=B.Id
                WHERE U.Id = @userId AND C.Id=@columnId", new { userId, columnId }).Any();
        }

        public int? CreateColumn(int userId, ColumnModel column)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            int? usersBoardId = db.Query<int?>(@"SELECT B.Id
                    FROM UserTable AS U
                    JOIN BoardTable AS B ON B.UserId=U.Id
                    WHERE U.Id = @id", new { id = userId }).FirstOrDefault();

            if (usersBoardId == null)
            {
                return null;
            }

            return db.Query<int?>(@"INSERT INTO ColumnTable(Name, BoardId) 
                    OUTPUT INSERTED.Id VALUES
                    (@columnName, @boardId)", new { boardId = usersBoardId, columnName = column.Name }).FirstOrDefault();
        }

        public bool UpdateColumn(int userId, ColumnModel column)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            if (column.Id != null && !IsAllowed(db, userId, (int)column.Id))
            {
                return false;
            }

            return db.Query(@"UPDATE ColumnTable
                    SET Name = @newName
                    OUTPUT DELETED.Id
                    WHERE Id = @id", new { newName = column.Name, id = column.Id }).Any();
        }

        public bool DeleteColumn(int userId, int id)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            if (!IsAllowed(db, userId, id))
            {
                return false;
            }

            return db.Query(@"DELETE ColumnTable
                    OUTPUT DELETED.Id
                    WHERE Id = @id", new { id }).Any();
        }
    }
}
