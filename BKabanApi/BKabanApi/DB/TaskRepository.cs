using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BKabanApi.Models.DB
{
    public interface ITaskRepository
    {
        int? CreateTask(int userId, TaskModelColumnLink task);
        bool UpdateTask(int userId, TaskModel task);
        bool UpdateTaskPositionAndColumn(int userId, TaskModelWithPositionAndNewColumn task);
        bool DeleteTask(int userId, int id);
    }

    public class TaskRepository : ITaskRepository
    {
        private readonly string _connectionString;

        public TaskRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private static bool IsAllowed(IDbConnection db, int userId, int taskId)
        {
            return db.Query(
                @"SELECT T.Id
                FROM UserTable AS U
                JOIN BoardTable AS B ON B.UserId=U.Id
                JOIN ColumnTable AS C ON C.BoardId=B.Id
                JOIN TaskTable AS T ON T.ColumnId=C.Id
                WHERE U.Id = @userId AND T.Id=@taskId", new { userId, taskId }).Any();
        }

        private static int GetTasksCount(IDbConnection db, int columnId)
        {
            return db.Query<int>(
                @"SELECT COUNT(T.Id)
                FROM ColumnTable AS C
                JOIN TaskTable AS T ON T.ColumnId = C.Id
                WHERE C.Id = @columnId",
                new { columnId }).FirstOrDefault();
        }

        public int? CreateTask(int userId, TaskModelColumnLink task)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            Debug.Assert(task.ColumnId != null, "task.ColumnId != null");
            if (!ColumnRepository.IsAllowed(db, userId, (int)task.ColumnId))
            {
                return null;
            }

            return db.Query<int?>(@"INSERT INTO TaskTable(Name, Description, ColumnId, Position) 
                OUTPUT INSERTED.Id VALUES
                (@taskName, @taskDescription, @columnId, @position);",
                new { taskName = task.Name, taskDescription = task.Description, columnId = task.ColumnId, position = GetTasksCount(db, (int)task.ColumnId) }).FirstOrDefault();
        }

        public bool UpdateTask(int userId, TaskModel task)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            Debug.Assert(task.Id != null, "task.Id != null");
            if (!IsAllowed(db, userId, (int)task.Id))
            {
                return false;
            }

            return db.Query($@"UPDATE TaskTable SET
                    {(task.Name != null ? "Name = @newName" : String.Empty)}
                    {(task.Name != null && task.Description != null ? "," : String.Empty)}
                    {(task.Description != null ? "Description = @newDescription" : String.Empty)}
                    OUTPUT DELETED.Id
                    WHERE Id = @id", new { newName = task.Name, newDescription = task.Description, id = task.Id }).Any();
        }

        public bool UpdateTaskPositionAndColumn(int userId, TaskModelWithPositionAndNewColumn task)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            Debug.Assert(task.Id != null, "task.Id != null");
            if (!IsAllowed(db, userId, (int)task.Id))
            {
                return false;
            }

            Debug.Assert(task.NewColumnId != null, "task.NewColumnId != null");
            if (!ColumnRepository.IsAllowed(db, userId, (int)task.NewColumnId))
            {
                return false;
            }

            int maxTaskPosition = GetTasksCount(db, (int)task.NewColumnId);
            if (task.Position < 0 || task.Position > maxTaskPosition)
            {
                return false;
            }

            return db.Query(
                @"DECLARE @OldColumnId INT, @OldTaskPosition INT;
                SET @OldColumnId =
	                (SELECT C.Id
	                FROM TaskTable AS T
	                JOIN ColumnTable AS C ON C.Id = T.ColumnId
	                WHERE T.Id = @taskId);
                SET @OldTaskPosition = 
	                (SELECT Position 
	                FROM TaskTable 
	                WHERE Id = @taskId);

                UPDATE TaskTable
                SET Position = T.Position - 1
                FROM TaskTable AS T
                JOIN ColumnTable AS C ON C.Id = T.ColumnId
                WHERE C.Id = @OldColumnId
                AND T.Position > @OldTaskPosition;

                UPDATE TaskTable
                SET Position = T.Position + 1
                FROM TaskTable AS T
                JOIN ColumnTable AS C ON C.Id = T.ColumnId
                WHERE C.Id = @newColumnId
                AND T.Position >= @newTaskPosition;

                UPDATE TaskTable
                SET Position = @newTaskPosition, ColumnId = @newColumnId
                OUTPUT DELETED.Id
                WHERE Id = @taskId;",
                new { taskId = task.Id, newTaskPosition = task.Position, newColumnId = task.NewColumnId }).Any();


        }

        public bool DeleteTask(int userId, int id)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            if (!IsAllowed(db, userId, id))
            {
                return false;
            }

            return db.Query(
                @"DECLARE @ColumnId INT, @TaskPosition INT;
                SET @ColumnId = 
	                (SELECT C.Id
	                FROM ColumnTable AS C
	                JOIN TaskTable AS T ON T.ColumnId = C.Id
	                WHERE T.Id = @taskId);
                SET @TaskPosition = 
	                (SELECT Position 
	                FROM TaskTable 
	                WHERE Id = @taskId);

                UPDATE TaskTable
                SET Position = T.Position - 1
                FROM TaskTable AS T
                JOIN ColumnTable AS C ON C.Id = T.ColumnId
                WHERE C.Id = @ColumnId
                AND T.Position > @TaskPosition;

                DELETE TaskTable
                OUTPUT DELETED.Id
                WHERE Id = @taskId",
                new { taskId = id }).Any();
        }
    }
}
