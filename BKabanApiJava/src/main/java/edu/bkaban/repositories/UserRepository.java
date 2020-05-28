package edu.bkaban.repositories;

import edu.bkaban.models.UserModel;
import edu.bkaban.services.DbService;
import org.apache.commons.lang3.ArrayUtils;
import org.springframework.stereotype.Service;

import java.io.UnsupportedEncodingException;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.security.SecureRandom;
import java.sql.SQLException;

@Service
public class UserRepository {
    private final MessageDigest Sha1 = MessageDigest.getInstance("SHA-1");
    private final SecureRandom SecureRandom = new SecureRandom();

    public UserRepository() throws NoSuchAlgorithmException {
    }

    private byte[] getSha1FromString(String str, byte[] salt) {
        Sha1.reset();
        try {
            Sha1.update(ArrayUtils.addAll(str.getBytes("utf-16le"), salt));
        } catch (UnsupportedEncodingException e) {
            e.printStackTrace();
        }
        return Sha1.digest();
    }

    private final String GetUserIdFullMatchSql = "SELECT Id FROM UserTable WHERE PasswordSha1 = ? AND Username = ?";
    private final String GetSaltSql = "SELECT Salt FROM UserTable WHERE Username = ?";
    public Integer getUserIdFullMatch(UserModel user) throws SQLException {
        byte[] salt;
        try (var conn = DbService.getConnection(); var stmt = conn.prepareStatement(GetSaltSql)) {
            stmt.setString(1, user.getUsername());

            var rset = stmt.executeQuery();
            if (!rset.next()){
                return null;
            }
            salt = rset.getBytes(1);
        }
        if (salt == null) {
            return null;
        }

        var pwdhash = getSha1FromString(user.getPassword(), salt);

        try (var conn = DbService.getConnection(); var stmt = conn.prepareStatement(GetUserIdFullMatchSql)) {
            stmt.setBytes(1, pwdhash);
            stmt.setString(2, user.getUsername());

            var rset = stmt.executeQuery();
            if (!rset.next()){
                return null;
            }
            return rset.getInt(1);
        }
    }

    private final String GetUserIdByUsernameSql = "SELECT Id FROM UserTable WHERE Username = ?";
    public Integer getUserIdByUsername(String username) throws SQLException {
        try (var conn = DbService.getConnection(); var stmt = conn.prepareStatement(GetUserIdByUsernameSql)) {
            stmt.setString(1, username);

            var rset = stmt.executeQuery();
            if (!rset.next()){
                return null;
            }
            return rset.getInt(1);
        }
    }

    private final String createUserSql = "INSERT INTO UserTable (Username, PasswordSha1, Salt) OUTPUT INSERTED.Id VALUES (?, ?, ?)";
    public Integer create(UserModel user) throws SQLException {
        var salt = new byte[8];
        SecureRandom.nextBytes(salt);

        try (var conn = DbService.getConnection(); var stmt = conn.prepareStatement(createUserSql)) {
            stmt.setString(1, user.getUsername());
            stmt.setBytes(3, salt);
            stmt.setBytes(2, getSha1FromString(user.getPassword(), salt));

            var rset = stmt.executeQuery();
            if (!rset.next()){
                return null;
            }
            return rset.getInt(1);
        }

    }
}
