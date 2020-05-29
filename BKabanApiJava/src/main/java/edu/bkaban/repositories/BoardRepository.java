package edu.bkaban.repositories;

import edu.bkaban.models.board.BoardModel;
import edu.bkaban.models.board.BoardModelWithPosition;
import edu.bkaban.models.column.ColumnModel;
import edu.bkaban.models.task.TaskModel;
import edu.bkaban.services.DbService;
import org.springframework.stereotype.Service;

import java.sql.Connection;
import java.sql.SQLException;
import java.util.LinkedList;

@Service
public class BoardRepository {
    private static final String isAllowedSql =
            "SELECT B.Id\n" +
                    "FROM UserTable AS U\n" +
                    "JOIN BoardTable AS B ON B.UserId = U.Id\n" +
                    "WHERE U.Id = ? AND B.Id = ?";

    public static boolean isAllowed(Connection conn, int userId, int boardId) throws SQLException {
        try (var stmt = conn.prepareStatement(isAllowedSql)) {
            stmt.setInt(1, userId);
            stmt.setInt(2, boardId);

            return stmt.executeQuery().next();
        }
    }

    private static final String getBoardsCountSql =
            "SELECT COUNT(B.Id)\n" +
                    "FROM UserTable AS U\n" +
                    "JOIN BoardTable AS B ON U.Id = B.UserId\n" +
                    "WHERE U.Id = ?";

    private static int getBoardsCount(Connection conn, int userId) throws SQLException {
        try (var stmt = conn.prepareStatement(getBoardsCountSql)) {

            stmt.setInt(1, userId);

            var rset = stmt.executeQuery();
            rset.next();
            return rset.getInt(1);
        }
    }

    private final String createBoardSql =
            "INSERT INTO BoardTable (Name, UserId, Position) \n" +
                    "OUTPUT INSERTED.Id VALUES\n" +
                    "(?, ?, ?);";

    public Integer createBoard(int userId, BoardModel board) throws SQLException {
        try (var conn = DbService.getConnection(); var stmt = conn.prepareStatement(createBoardSql)) {

            stmt.setString(1, board.getName());
            stmt.setInt(2, userId);
            stmt.setInt(3, getBoardsCount(conn, userId));

            var rset = stmt.executeQuery();
            if (!rset.next()) {
                return null;
            }
            return rset.getInt(1);
        }
    }

    private final String updateBoardNameSql =
            "UPDATE BoardTable\n" +
                    "SET Name = ?\n" +
                    "OUTPUT DELETED.Id\n" +
                    "WHERE Id = ?";

    public boolean updateBoardName(int userId, BoardModel board) throws SQLException {
        try (var conn = DbService.getConnection(); var stmt = conn.prepareStatement(updateBoardNameSql)) {

            if (!isAllowed(conn, userId, board.getId())) {
                return false;
            }

            stmt.setString(1, board.getName());
            stmt.setInt(2, board.getId());

            return stmt.executeQuery().next();
        }
    }

    private final String updateBoardPositionSql =
            "DECLARE @OldBoardPosition INT;\n" +
                    "SET @OldBoardPosition = \n" +
                    "(SELECT Position \n" +
                    "FROM BoardTable \n" +
                    "WHERE Id = ?);\n" +
                    "UPDATE BoardTable\n" +
                    "SET Position = B.Position - 1\n" +
                    "FROM BoardTable AS B\n" +
                    "JOIN UserTable AS U ON U.Id = B.UserId\n" +
                    "WHERE U.Id = ?\n" +
                    "AND B.Position > @OldBoardPosition;\n" +
                    "UPDATE BoardTable\n" +
                    "SET Position = B.Position + 1\n" +
                    "FROM BoardTable AS B\n" +
                    "JOIN UserTable AS U ON U.Id = B.UserId\n" +
                    "WHERE U.Id = ?\n" +
                    "AND B.Position >= ?;\n" +
                    "UPDATE BoardTable\n" +
                    "SET Position = ?\n" +
                    "OUTPUT DELETED.Id\n" +
                    "WHERE Id = ?;";

    public boolean updateBoardPosition(int userId, BoardModelWithPosition board) throws SQLException {
        try (var conn = DbService.getConnection(); var stmt = conn.prepareStatement(updateBoardPositionSql)) {

            if (!isAllowed(conn, userId, board.getId())) {
                return false;
            }

            int maxBoardPosition = getBoardsCount(conn, userId) - 1;

            if (board.getPosition() < 0 || board.getPosition() > maxBoardPosition) {
                return false;
            }

            stmt.setInt(1, board.getId());
            stmt.setInt(2, userId);
            stmt.setInt(3, userId);
            stmt.setInt(4, board.getPosition());
            stmt.setInt(5, board.getPosition());
            stmt.setInt(6, board.getId());

            return stmt.executeQuery().next();
        }
    }

    private final String deleteBoardSql =
            "DECLARE @BoardPosition INT;\n" +
                    "SET @BoardPosition = \n" +
                    "(SELECT Position \n" +
                    "FROM BoardTable \n" +
                    "WHERE Id = ?);\n" +
                    "UPDATE BoardTable\n" +
                    "SET Position = B.Position - 1\n" +
                    "FROM BoardTable AS B\n" +
                    "JOIN UserTable AS U ON U.Id = B.UserId\n" +
                    "WHERE U.Id = ?\n" +
                    "AND B.Position > @BoardPosition\n" +
                    "DELETE BoardTable\n" +
                    "OUTPUT DELETED.Id\n" +
                    "WHERE Id = ?";

    public boolean deleteBoard(int userId, int boardId) throws SQLException {
        try (var conn = DbService.getConnection(); var stmt = conn.prepareStatement(deleteBoardSql)) {

            if (!isAllowed(conn, userId, boardId)) {
                return false;
            }

            stmt.setInt(1, boardId);
            stmt.setInt(2, userId);
            stmt.setInt(3, boardId);

            return stmt.executeQuery().next();
        }
    }

    private final String getFullBoardSql =
            "SELECT Id, Name\n" +
                    "FROM BoardTable\n" +
                    "WHERE Id = ?";

    public BoardModel getFullBoard(int userId, int boardId) throws SQLException {
        try (var conn = DbService.getConnection(); var stmt = conn.prepareStatement(getFullBoardSql)) {

            if (!isAllowed(conn, userId, boardId)) {
                return null;
            }

            stmt.setInt(1, boardId);
            var rset = stmt.executeQuery();
            if (!rset.next()) {
                return null;
            }

            var boardModel = new BoardModel(boardId, rset.getString("Name"));

            fillBoardColumns(conn, boardModel);

            return boardModel;
        }
    }

    private final String fillBoardColumnsSql =
            "SELECT Id, Name\n" +
                    "FROM ColumnTable\n" +
                    "WHERE BoardID = ?\n" +
                    "ORDER BY Position";

    private void fillBoardColumns(Connection conn, BoardModel board) throws SQLException {
        try (var stmt = conn.prepareStatement(fillBoardColumnsSql)) {
            stmt.setInt(1, board.getId());

            board.setColumns(new LinkedList<ColumnModel>());
            var rset = stmt.executeQuery();

            while (rset.next()) {
                var column = new ColumnModel(rset.getInt("Id"), rset.getString("Name"));
                board.getColumns().add(column);
                fillColumnTasks(conn, column);
            }
        }
    }

    private final String fillColumnTasksSql =
            "SELECT Id, Name, Description\n" +
                    "FROM TaskTable\n" +
                    "WHERE ColumnId = ?\n" +
                    "ORDER BY Position";

    private void fillColumnTasks(Connection conn, ColumnModel column) throws SQLException {
        try (var stmt = conn.prepareStatement(fillColumnTasksSql)) {
            stmt.setInt(1, column.getId());

            column.setTasks(new LinkedList<TaskModel>());
            var rset = stmt.executeQuery();

            while (rset.next()) {
                var task = new TaskModel(rset.getInt("Id"), rset.getString("Name"), rset.getString("Description"));
                column.getTasks().add(task);
            }
        }
    }

}
