using System.Data;
using System.Diagnostics;
using System.Linq;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BKabanApi.Models.DB
{
    public interface IColumnRepository
    {
        int? CreateColumn(int userId, ColumnModelBoardLink column);
        bool UpdateColumn(int userId, ColumnModel column);
        bool UpdateColumnPosition(int userId, ColumnModelWithPosition column);
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

        private static int GetColumnsCount(IDbConnection db, int boardId)
        {
            return db.Query<int>(
                @"SELECT COUNT(C.Id)
                FROM BoardTable AS B
                JOIN ColumnTable AS C ON B.Id = C.BoardId
                WHERE B.Id = @boardId",
                new { boardId }).FirstOrDefault();
        }

        public int? CreateColumn(int userId, ColumnModelBoardLink column)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            Debug.Assert(column.BoardId != null, "column.BoardId != null");
            if (!BoardRepository.IsAllowed(db, userId, (int)column.BoardId))
            {
                return null;
            }

            return db.Query<int?>(
                @"INSERT INTO ColumnTable(Name, BoardId, Position) 
                OUTPUT INSERTED.Id VALUES
                (@columnName, @boardId, @position);",
                new { boardId = column.BoardId, columnName = column.Name, position = GetColumnsCount(db, (int)column.BoardId) }).FirstOrDefault();
        }

        public bool UpdateColumn(int userId, ColumnModel column)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            Debug.Assert(column.Id != null, "column.Id != null");
            if (!IsAllowed(db, userId, (int)column.Id))
            {
                return false;
            }

            return db.Query(
                @"UPDATE ColumnTable
                SET Name = @newName
                OUTPUT DELETED.Id
                WHERE Id = @id", new { newName = column.Name, id = column.Id }).Any();
        }

        public bool UpdateColumnPosition(int userId, ColumnModelWithPosition column)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            Debug.Assert(column.Id != null, "column.Id != null");
            if (!IsAllowed(db, userId, (int)column.Id))
            {
                return false;
            }

            int? boardId = db.Query<int?>(
                @"SELECT B.Id 
	            FROM ColumnTable AS C
	            JOIN BoardTable AS B ON B.Id = C.BoardId 
	            WHERE C.Id = @columnId;",
                new { columnId = column.Id }).FirstOrDefault();

            if (boardId == null)
            {
                return false;
            }

            int maxColumnPosition = GetColumnsCount(db, (int) boardId) - 1;

            if (column.Position < 0 || column.Position > maxColumnPosition)
            {
                return false;
            }

            return db.Query(
                @"DECLARE @OldColumnPosition INT;
                SET @OldColumnPosition = 
	                (SELECT Position 
	                FROM ColumnTable 
	                WHERE Id = @columnId);

                UPDATE ColumnTable
                SET Position = C.Position - 1
                FROM ColumnTable AS C
                JOIN BoardTable AS B ON B.Id = C.BoardId
                WHERE B.Id = @boardId
                AND C.Position > @OldColumnPosition;

                UPDATE ColumnTable
                SET Position = C.Position + 1
                FROM ColumnTable AS C
                JOIN BoardTable AS B ON B.Id = C.BoardId
                WHERE B.Id = @boardId
                AND C.Position >= @newPosition;

                UPDATE ColumnTable
                SET Position = @newPosition
                OUTPUT DELETED.Id
                WHERE Id = @columnId;",
                new { boardId, columnId = column.Id, newPosition = column.Position}).Any();
        }

        public bool DeleteColumn(int userId, int id)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            if (!IsAllowed(db, userId, id))
            {
                return false;
            }

            return db.Query(
                @"DECLARE @BoardId INT, @ColumnPosition INT;
                SET @BoardId =
	                (SELECT B.Id 
	                FROM ColumnTable AS C
	                JOIN BoardTable AS B ON B.Id = C.BoardId 
	                WHERE C.Id = @columnId);
                SET @ColumnPosition = 
	                (SELECT Position 
	                FROM ColumnTable 
	                WHERE Id = @columnId);

                UPDATE ColumnTable
                SET Position = C.Position - 1
                FROM ColumnTable AS C
                JOIN BoardTable AS B ON B.Id = C.BoardId
                WHERE B.Id = @BoardId
                AND C.Position > @ColumnPosition

                DELETE ColumnTable
                OUTPUT DELETED.Id
                WHERE Id = @columnId",
                new { columnId = id }).Any();
        }
    }
}
