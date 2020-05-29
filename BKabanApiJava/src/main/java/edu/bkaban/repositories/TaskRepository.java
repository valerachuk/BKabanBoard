package edu.bkaban.repositories;

import edu.bkaban.models.task.TaskModel;
import edu.bkaban.models.task.TaskModelColumnLink;
import edu.bkaban.models.task.TaskModelWithPositionAndNewColumn;
import edu.bkaban.services.DbService;
import org.springframework.stereotype.Service;

import java.sql.Connection;
import java.sql.SQLException;

@Service
public class TaskRepository {
    private static final String isAllowedSql =
            "SELECT T.Id\n" +
                    "FROM UserTable AS U\n" +
                    "JOIN BoardTable AS B ON B.UserId=U.Id\n" +
                    "JOIN ColumnTable AS C ON C.BoardId=B.Id\n" +
                    "JOIN TaskTable AS T ON T.ColumnId=C.Id\n" +
                    "WHERE U.Id = ? AND T.Id = ?";

    private static boolean isAllowed(Connection conn, int userId, int TaskId) throws SQLException {
        try (var stmt = conn.prepareStatement(isAllowedSql)) {
            stmt.setInt(1, userId);
            stmt.setInt(2, TaskId);

            return stmt.executeQuery().next();
        }
    }

    private static final String getTaskCountSql =
            "SELECT COUNT(T.Id)\n" +
                    "FROM ColumnTable AS C\n" +
                    "JOIN TaskTable AS T ON T.ColumnId = C.Id\n" +
                    "WHERE C.Id = ?";

    private static int getTaskCount(Connection conn, int columnId) throws SQLException {
        try (var stmt = conn.prepareStatement(getTaskCountSql)) {
            stmt.setInt(1, columnId);

            var rset = stmt.executeQuery();
            rset.next();
            return rset.getInt(1);
        }
    }

    private static final String createTaskSql =
            "INSERT INTO TaskTable(Name, Description, ColumnId, Position)\n" +
                    "OUTPUT INSERTED.Id VALUES\n" +
                    "(?, ?, ?, ?);";

    public Integer createTask(int userId, TaskModelColumnLink task) throws SQLException {
        try (var conn = DbService.getConnection(); var stmt = conn.prepareStatement(createTaskSql)) {
            if (!ColumnRepository.isAllowed(conn, userId, task.getColumnId())) {
                return null;
            }

            stmt.setString(1, task.getName());
            stmt.setString(2, task.getDescription());
            stmt.setInt(3, task.getColumnId());
            stmt.setInt(4, getTaskCount(conn, task.getColumnId()));

            var rset = stmt.executeQuery();
            if (!rset.next()) {
                return null;
            }
            return rset.getInt(1);
        }
    }

    private String updateTaskSql(TaskModel task) {
        return "UPDATE TaskTable SET\n" +
                (task.getName() != null ? ("Name = '" + task.getName() + "'") : "") +
                (task.getName() != null && task.getDescription() != null ? ",\n" : "") +
                (task.getDescription() != null ? ("Description = '" + task.getDescription() + "'\n") : "") +
                "OUTPUT DELETED.Id\n" +
                "WHERE Id = " + task.getId();
    }

    public boolean updateTask(int userId, TaskModel task) throws SQLException {
        try (var conn = DbService.getConnection(); var stmt = conn.prepareStatement(updateTaskSql(task))) {
            if (!isAllowed(conn, userId, task.getId())) {
                return false;
            }
            return stmt.executeQuery().next();
        }
    }

    private final String updateTaskPositionAndColumnSql =
            "DECLARE @OldColumnId INT, @OldTaskPosition INT;\n" +
                    "SET @OldColumnId =\n" +
                    "(SELECT C.Id\n" +
                    "FROM TaskTable AS T\n" +
                    "JOIN ColumnTable AS C ON C.Id = T.ColumnId\n" +
                    "WHERE T.Id = ?);\n" +
                    "SET @OldTaskPosition = \n" +
                    "(SELECT Position \n" +
                    "FROM TaskTable \n" +
                    "WHERE Id = ?);\n" +
                    "UPDATE TaskTable\n" +
                    "SET Position = T.Position - 1\n" +
                    "FROM TaskTable AS T\n" +
                    "JOIN ColumnTable AS C ON C.Id = T.ColumnId\n" +
                    "WHERE C.Id = @OldColumnId\n" +
                    "AND T.Position > @OldTaskPosition;\n" +
                    "UPDATE TaskTable\n" +
                    "SET Position = T.Position + 1\n" +
                    "FROM TaskTable AS T\n" +
                    "JOIN ColumnTable AS C ON C.Id = T.ColumnId\n" +
                    "WHERE C.Id = ?\n" +
                    "AND T.Position >= ?;\n" +
                    "UPDATE TaskTable\n" +
                    "SET Position = ?, ColumnId = ?\n" +
                    "OUTPUT DELETED.Id\n" +
                    "WHERE Id = ?;";

    public boolean updateTaskPositionAndColumn(int userId, TaskModelWithPositionAndNewColumn task) throws SQLException {
        try (var conn = DbService.getConnection(); var stmt = conn.prepareStatement(updateTaskPositionAndColumnSql)) {
            if (!isAllowed(conn, userId, task.getId())) {
                return false;
            }

            if (!ColumnRepository.isAllowed(conn, userId, task.getNewColumnId())) {
                return false;
            }

            int maxTaskPosition = getTaskCount(conn, task.getNewColumnId());
            if (task.getPosition() < 0 || task.getPosition() > maxTaskPosition) {
                return false;
            }

            stmt.setInt(1, task.getId());
            stmt.setInt(2, task.getId());
            stmt.setInt(3, task.getNewColumnId());
            stmt.setInt(4, task.getPosition());
            stmt.setInt(5, task.getPosition());
            stmt.setInt(6, task.getNewColumnId());
            stmt.setInt(7, task.getId());

            return stmt.executeQuery().next();
        }
    }

    private final String deleteTaskSql =
            "DECLARE @ColumnId INT, @TaskPosition INT;\n" +
                    "SET @ColumnId = \n" +
                    "(SELECT C.Id\n" +
                    "FROM ColumnTable AS C\n" +
                    "JOIN TaskTable AS T ON T.ColumnId = C.Id\n" +
                    "WHERE T.Id = ?);\n" +
                    "SET @TaskPosition = \n" +
                    "(SELECT Position \n" +
                    "FROM TaskTable \n" +
                    "WHERE Id = ?);\n" +
                    "UPDATE TaskTable\n" +
                    "SET Position = T.Position - 1\n" +
                    "FROM TaskTable AS T\n" +
                    "JOIN ColumnTable AS C ON C.Id = T.ColumnId\n" +
                    "WHERE C.Id = @ColumnId\n" +
                    "AND T.Position > @TaskPosition;\n" +
                    "DELETE TaskTable\n" +
                    "OUTPUT DELETED.Id\n" +
                    "WHERE Id = ?";

    public boolean deleteTask(int userId, int id) throws SQLException {
        try (var conn = DbService.getConnection(); var stmt = conn.prepareStatement(deleteTaskSql)) {
            if (!isAllowed(conn, userId, id)) {
                return false;
            }

            stmt.setInt(1, id);
            stmt.setInt(2, id);
            stmt.setInt(3, id);

            return stmt.executeQuery().next();
        }
    }

}
