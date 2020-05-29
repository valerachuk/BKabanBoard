package edu.bkaban.repositories;

import edu.bkaban.models.column.ColumnModel;
import edu.bkaban.models.column.ColumnModelBoardLink;
import edu.bkaban.models.column.ColumnModelWithPosition;
import edu.bkaban.services.DbService;
import org.springframework.stereotype.Service;

import java.sql.Connection;
import java.sql.SQLException;

@Service
public class ColumnRepository {
    private static final String isAllowedSql =
            "SELECT C.Id\n" +
                    "FROM UserTable AS U\n" +
                    "JOIN BoardTable AS B ON B.UserId=U.Id\n" +
                    "JOIN ColumnTable AS C ON C.BoardId=B.Id\n" +
                    "WHERE U.Id = ? AND C.Id = ?";

    public static boolean isAllowed(Connection conn, int userId, int columnId) throws SQLException {
        try (var stmt = conn.prepareStatement(isAllowedSql)) {
            stmt.setInt(1, userId);
            stmt.setInt(2, columnId);

            return stmt.executeQuery().next();
        }
    }

    private static final String getColumnsCountSql =
            "SELECT COUNT(C.Id)\n" +
                    "FROM BoardTable AS B\n" +
                    "JOIN ColumnTable AS C ON B.Id = C.BoardId\n" +
                    "WHERE B.Id = ?";

    private static int getColumnsCount(Connection conn, int boardId) throws SQLException {
        try (var stmt = conn.prepareStatement(getColumnsCountSql)) {
            stmt.setInt(1, boardId);

            var rset = stmt.executeQuery();
            rset.next();
            return rset.getInt(1);
        }
    }

    private final String createColumnSql =
            "INSERT INTO ColumnTable(Name, BoardId, Position) \n" +
                    "OUTPUT INSERTED.Id VALUES\n" +
                    "(?, ?, ?)";

    public Integer createColumn(int userId, ColumnModelBoardLink column) throws SQLException {
        try (var conn = DbService.getConnection(); var stmt = conn.prepareStatement(createColumnSql)) {
            if (!BoardRepository.isAllowed(conn, userId, column.getBoardId())) {
                return null;
            }

            stmt.setString(1, column.getName());
            stmt.setInt(2, column.getBoardId());
            stmt.setInt(3, getColumnsCount(conn, column.getBoardId()));

            var rset = stmt.executeQuery();
            if (!rset.next()) {
                return null;
            }
            return rset.getInt(1);
        }
    }

    private final String updateColumnSql =
            "UPDATE ColumnTable\n" +
                    "SET Name = ?\n" +
                    "OUTPUT DELETED.Id\n" +
                    "WHERE Id = ?";

    public boolean updateColumn(int userId, ColumnModel column) throws SQLException {
        try (var conn = DbService.getConnection(); var stmt = conn.prepareStatement(updateColumnSql)) {
            if (!isAllowed(conn, userId, column.getId())) {
                return false;
            }

            stmt.setString(1, column.getName());
            stmt.setInt(2, column.getId());

            return stmt.executeQuery().next();
        }
    }

    private final String getParentBoardSql =
            "SELECT B.Id \n" +
                    "FROM ColumnTable AS C\n" +
                    "JOIN BoardTable AS B ON B.Id = C.BoardId\n" +
                    "WHERE C.Id = ?;";

    private final String updateColumnPositionSql =
            "DECLARE @OldColumnPosition INT;\n" +
                    "SET @OldColumnPosition = \n" +
                    "(SELECT Position \n" +
                    "FROM ColumnTable \n" +
                    "WHERE Id = ?);\n" +
                    "UPDATE ColumnTable\n" +
                    "SET Position = C.Position - 1\n" +
                    "FROM ColumnTable AS C\n" +
                    "JOIN BoardTable AS B ON B.Id = C.BoardId\n" +
                    "WHERE B.Id = ?\n" +
                    "AND C.Position > @OldColumnPosition;\n" +
                    "UPDATE ColumnTable\n" +
                    "SET Position = C.Position + 1\n" +
                    "FROM ColumnTable AS C\n" +
                    "JOIN BoardTable AS B ON B.Id = C.BoardId\n" +
                    "WHERE B.Id = ?\n" +
                    "AND C.Position >= ?;\n" +
                    "UPDATE ColumnTable\n" +
                    "SET Position = ?\n" +
                    "OUTPUT DELETED.Id\n" +
                    "WHERE Id = ?;";

    public boolean updateColumnPosition(int userId, ColumnModelWithPosition column) throws SQLException {
        try (var conn = DbService.getConnection(); var stmt = conn.prepareStatement(updateColumnPositionSql); var boardIdStmt = conn.prepareStatement(getParentBoardSql)) {
            if (!isAllowed(conn, userId, column.getId())) {
                return false;
            }

            boardIdStmt.setInt(1, column.getId());
            var boardIdRset = boardIdStmt.executeQuery();

            if (!boardIdRset.next()) {
                return false;
            }

            int boardId = boardIdRset.getInt(1);

            int maxColumnPosition = getColumnsCount(conn, boardId) - 1;

            if (column.getPosition() < 0 || column.getPosition() > maxColumnPosition) {
                return false;
            }

            stmt.setInt(1, column.getId());
            stmt.setInt(2, boardId);
            stmt.setInt(3, boardId);
            stmt.setInt(4, column.getPosition());
            stmt.setInt(5, column.getPosition());
            stmt.setInt(6, column.getId());


            return stmt.executeQuery().next();
        }
    }

    private final String deleteColumnSql =
            "DECLARE @BoardId INT, @ColumnPosition INT;\n" +
                    "SET @BoardId =\n" +
                    "(SELECT B.Id \n" +
                    "FROM ColumnTable AS C\n" +
                    "JOIN BoardTable AS B ON B.Id = C.BoardId \n" +
                    "WHERE C.Id = ?);\n" +
                    "SET @ColumnPosition = \n" +
                    "(SELECT Position \n" +
                    "FROM ColumnTable \n" +
                    "WHERE Id = ?);\n" +
                    "UPDATE ColumnTable\n" +
                    "SET Position = C.Position - 1\n" +
                    "FROM ColumnTable AS C\n" +
                    "JOIN BoardTable AS B ON B.Id = C.BoardId\n" +
                    "WHERE B.Id = @BoardId\n" +
                    "AND C.Position > @ColumnPosition\n" +
                    "DELETE ColumnTable\n" +
                    "OUTPUT DELETED.Id\n" +
                    "WHERE Id = ?";

    public boolean deleteColumn(int userId, int id) throws SQLException {
        try (var conn = DbService.getConnection(); var stmt = conn.prepareStatement(deleteColumnSql)) {
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
