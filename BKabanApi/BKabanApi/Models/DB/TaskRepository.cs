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
        int? CreateTask(int userId, TaskModel task, int columnId);
        bool UpdateTask(int userId, TaskModel task);
        bool DeleteTask(int userId, int id);
    }

    public class TaskRepository : ITaskRepository
    {
        private readonly string _connectionString;

        public TaskRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private bool IsAllowed(IDbConnection db, int userId, int taskId)
        {
            return db.Query(@"SELECT T.Id
                FROM UserTable AS U
                JOIN BoardTable AS B ON B.UserId=U.Id
                JOIN ColumnTable AS C ON C.BoardId=B.Id
                JOIN TaskTable AS T ON T.ColumnId=C.Id
                WHERE U.Id = @userId AND T.Id=@taskId", new {userId, taskId}).Any();
        }

        public int? CreateTask(int userId, TaskModel task, int columnId)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            if (!ColumnRepository.IsAllowed(db, userId, columnId))
            {
                return null;
            }

            return db.Query<int?>(@"INSERT INTO TaskTable(Name, Description, ColumnId) 
                    OUTPUT INSERTED.Id VALUES
                    (@taskName, @taskDescription, @columnId);", new { taskName = task.Name, taskDescription = task.Description, columnId}).FirstOrDefault();
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
                    {( task.Name != null ? "Name = @newName" : String.Empty)}
                    {( task.Name != null && task.Description != null ? "," : String.Empty)}
                    {( task.Description != null ? "Description = @newDescription" : String.Empty)}
                    OUTPUT DELETED.Id
                    WHERE Id = @id", new { newName = task.Name, newDescription = task.Description, id = task.Id }).Any();
        }

        public bool DeleteTask(int userId, int id)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            if (!IsAllowed(db, userId, id))
            {
                return false;
            }

            return db.Query(@"DELETE TaskTable
                    OUTPUT DELETED.Id
                    WHERE Id = @id;", new { id }).Any();
        }
    }
}
