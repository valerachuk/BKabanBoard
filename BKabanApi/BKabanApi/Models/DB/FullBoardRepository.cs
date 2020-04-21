using System.Data;
using System.Linq;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BKabanApi.Models.DB
{
    public interface IFullBoardRepository
    {
        BoardModel GetUserBoard(int userId);
        int? UpdateBoardName(int userId, BoardModel board);
    }

    public class FullBoardRepository : IFullBoardRepository
    {
        private readonly string _connectionString;

        public FullBoardRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public BoardModel GetUserBoard(int userId)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            var board = db.Query<BoardModel>(@"SELECT B.Id, B.Name FROM UserTable AS U
                    JOIN BoardTable AS B ON B.UserId = U.Id
                    WHERE U.Id = @userId", new { userId }).FirstOrDefault();

            board ??= db.Query<BoardModel>(@"INSERT INTO BoardTable (Name, UserId) 
                    OUTPUT INSERTED.Id, INSERTED.Name VALUES
                    ('Your Board', @userId);", new { userId }).FirstOrDefault();

            FillBoardTables(db, board);

            return board;
        }

        public int? UpdateBoardName(int userId, BoardModel board)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            return db.Query<int?>(@"UPDATE B
                SET B.Name = @newName
                OUTPUT DELETED.Id
                FROM BoardTable B
                JOIN UserTable AS U ON U.Id = B.UserId
                WHERE U.Id = @userId", new {userId, newName = board.Name}).FirstOrDefault();
        }

        public void FillBoardTables(IDbConnection db, BoardModel board)
        {
            board.Columns =
                db.Query<ColumnModel>(@"SELECT Id, Name
                    FROM ColumnTable
                    WHERE BoardID = @boardID", new { boardId = board.Id });

            foreach (var boardColumn in board.Columns)
            {
                FillColumnTasks(db, boardColumn);
            }
        }

        public void FillColumnTasks(IDbConnection db, ColumnModel column)
        {
            column.Tasks = db.Query<TaskModel>(@"SELECT Id, Name, Description
                FROM TaskTable
                WHERE ColumnId = @columnId", new { columnId = column.Id });
        }

    }
}
