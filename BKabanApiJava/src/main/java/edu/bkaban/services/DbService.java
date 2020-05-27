package edu.bkaban.services;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;

public class DbService {
    public static Connection getConnection() throws SQLException {
        var connectionString = "jdbc:sqlserver://DESKTOP-EQ1LBTR;databaseName=BKabanDB;integratedSecurity=true";
        return DriverManager.getConnection(connectionString);
    }
}
