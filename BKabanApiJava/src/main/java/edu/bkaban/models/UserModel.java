package edu.bkaban.models;

import javax.validation.constraints.NotNull;
import javax.validation.constraints.Size;

public class UserModel {

    @NotNull(message = "Username required")
    @Size(min = 2, max = 20, message = "Username length have to be between 2 and 20")
    private String Username;

    @NotNull(message = "Password required")
    @Size(min = 6, max = 100, message = "Password length have to be between 6 and 100")
    private String Password;

    public String getUsername() {
        return Username;
    }

    public void setUsername(String username) {
        Username = username;
    }

    public String getPassword() {
        return Password;
    }

    public void setPassword(String password) {
        Password = password;
    }

    public UserModel(String username, String password) {
        Username = username;
        Password = password;
    }

    public UserModel() {}
}
