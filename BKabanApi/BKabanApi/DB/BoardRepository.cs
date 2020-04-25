using System.Data;
using System.Linq;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BKabanApi.Models.DB
{
    public interface IBoardRepository
    {
        int? CreateBoard(int userId, BoardModel  board);
        BoardModel GetFullBoard(int userId, int boardId);
        bool UpdateBoardName(int userId, BoardModel board);
        bool DeleteBoard(int userId, int boardId);
    }

    public class BoardRepository : IBoardRepository
    {
        private readonly string _connectionString;

        public BoardRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public static bool IsAllowed(IDbConnection db, int userId, int boardId)
        {
            return db.Query(
                @"SELECT B.Id
                FROM UserTable AS U
                JOIN BoardTable AS B ON B.UserId = U.Id
                WHERE U.Id = @userId AND B.Id = @boardId", new {userId, boardId}).Any();
        }

        public int? CreateBoard(int userId, BoardModel board)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            return db.Query<int?>(
                @"INSERT INTO BoardTable(Name, UserId)
                OUTPUT INSERTED.Id
                VALUES(@boardName, @userId)", new {boardName = board.Name, userId}).FirstOrDefault();
        }

        public BoardModel GetFullBoard(int userId, int boardId)
        {
            using IDbConnection db = new SqlConnection(_connectionString);

            if (!IsAllowed(db, userId, boardId))
            {
                return null;
            }

            var board = db.Query<BoardModel>(
                @"SELECT Id, Name
                FROM BoardTable
                WHERE Id = @boardId", new { boardId }).FirstOrDefault();

            FillBoardTables(db, board);

            return board;
        }

        public bool UpdateBoardName(int userId, BoardModel board)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            if (!IsAllowed(db, userId, (int)board.Id))
            {
                return false;
            }

            return db.Query(
                @"UPDATE BoardTable
                SET Name = @boardName
                OUTPUT DELETED.Id
                WHERE Id = @boardId", new {boardId = board.Id, boardName = board.Name}).Any();
        }

        public bool DeleteBoard(int userId, int boardId)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            if (!IsAllowed(db, userId, boardId))
            {
                return false;
            }

            return db.Query(
                @"DELETE BoardTable
                OUTPUT DELETED.Id
                WHERE Id = @boardId", new { boardId, userId }).Any();
        }

        public void FillBoardTables(IDbConnection db, BoardModel board)
        {
            board.Columns =
                db.Query<ColumnModel>(
                    @"SELECT Id, Name
                    FROM ColumnTable
                    WHERE BoardID = @boardID", new { boardId = board.Id });

            foreach (var boardColumn in board.Columns)
            {
                FillColumnTasks(db, boardColumn);
            }
        }

        public void FillColumnTasks(IDbConnection db, ColumnModel column)
        {
            column.Tasks = db.Query<TaskModel>(
                @"SELECT Id, Name, Description
                FROM TaskTable
                WHERE ColumnId = @columnId", new { columnId = column.Id });
        }

    }
}
