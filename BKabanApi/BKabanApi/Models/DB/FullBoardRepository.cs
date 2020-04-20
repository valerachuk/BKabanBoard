using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BKabanApi.Models.DB
{
    public interface IFullBoardRepository
    {
        BoardModel getUserBoard(int userId);
    }

    public class FullBoardRepository : IFullBoardRepository
    {
        private readonly string _connectionString;

        public FullBoardRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public BoardModel getUserBoard(int userId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var board = db.Query<BoardModel>(@"SELECT B.Id, B.Name FROM UserTable AS U
                    JOIN BoardTable AS B ON B.UserId = U.Id
                    WHERE U.Id = @UserId", new { userId }).FirstOrDefault();

                board ??= db.Query<BoardModel>(@"INSERT INTO BoardTable (Name, UserId) 
                    OUTPUT INSERTED.Id, INSERTED.Name VALUES
                    ('Your Board', @UserId);", new { userId }).FirstOrDefault();

                FillBoardTables(db, board);

                return board;
            }
        }

        public void FillBoardTables(IDbConnection db, BoardModel board)
        {
            board.Columns =
                db.Query<ColumnModel>(@"SELECT Id, Name
                    FROM ColumnTable
                    WHERE BoardID = @BoardID", new {boardId = board.Id});

            foreach (var boardColumn in board.Columns)
            {
                FillColumnTasks(db, boardColumn);
            }
        }

        public void FillColumnTasks(IDbConnection db, ColumnModel column)
        {
            column.Tasks = db.Query<TaskModel>(@"SELECT Id, Name, Description
                FROM TaskTable
                WHERE ColumnId = @ColumnId", new { ColumnId = column.Id });
        }

    }
}
