package edu.bkaban.repositories;

import edu.bkaban.services.DbService;
import org.springframework.stereotype.Service;

import java.sql.Connection;
import java.sql.SQLException;

@Service
public class BoardRepository {
    private static final String isAllowedSql =
            "SELECT B.Id\n" +
                    "FROM UserTable AS U\n" +
                    "JOIN BoardTable AS B ON B.UserId = U.Id\n" +
                    "WHERE U.Id = ? AND B.Id = ?";

    public static boolean isAllowed(Connection conn, int userId, int boardId) throws SQLException {
        try(var stmt = conn.prepareStatement(isAllowedSql)){
            stmt.setInt(1, userId);
            stmt.setInt(2, boardId);

            return stmt.executeQuery().next();
        }
    }
}
