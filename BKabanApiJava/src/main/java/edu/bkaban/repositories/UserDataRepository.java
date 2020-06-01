package edu.bkaban.repositories;

import edu.bkaban.models.UserDataModel;
import edu.bkaban.models.board.BoardModel;
import edu.bkaban.services.DbService;
import org.springframework.stereotype.Service;

import java.sql.SQLException;
import java.util.LinkedList;

@Service
public class UserDataRepository {
    private final String getUserDataSql =
            "SELECT B.Id, B.Name\n" +
                    "FROM BoardTable AS B\n" +
                    "JOIN UserTable AS U ON U.Id = B.UserId\n" +
                    "WHERE U.Id = ?\n" +
                    "ORDER BY Position";

    public UserDataModel getUserData(int userId) throws SQLException {
        try (var conn = DbService.getConnection(); var stmt = conn.prepareStatement(getUserDataSql)) {
            stmt.setInt(1, userId);

            var userData = new UserDataModel();
            userData.setBoards(new LinkedList<BoardModel>());

            var rset = stmt.executeQuery();
            while (rset.next()) {
                var board = new BoardModel();
                board.setId(rset.getInt("Id"));
                board.setName(rset.getString("Name"));
                userData.getBoards().add(board);
            }

            return userData;
        }
    }
}
