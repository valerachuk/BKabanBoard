package edu.bkaban.repositories;

import java.sql.Connection;
import java.sql.SQLException;

public class ColumnRepository {
    private static final String isAllowedSql =
            "SELECT C.Id\n" +
                    "FROM UserTable AS U\n" +
                    "JOIN BoardTable AS B ON B.UserId=U.Id\n" +
                    "JOIN ColumnTable AS C ON C.BoardId=B.Id\n" +
                    "WHERE U.Id = ? AND C.Id = ?";

    public static boolean isAllowed(Connection conn, int userId, int columnId) throws SQLException {
        try (var stmt = conn.prepareStatement(isAllowedSql)){
            stmt.setInt(1, userId);
            stmt.setInt(2, columnId);

            return stmt.executeQuery().next();
        }
    }
}
