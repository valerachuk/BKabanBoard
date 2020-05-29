package edu.bkaban.services;

import org.springframework.stereotype.Service;

import javax.servlet.http.HttpSession;

@Service
public class AuthService {
    private final boolean IsDebug = true;
    private final String KeyName = "userId";
    private final Integer DebugUserId = 1;

    public Integer getUserId(HttpSession session) {
        return IsDebug ? DebugUserId : (Integer) session.getAttribute(KeyName);
    }

    public void addUserToSession(HttpSession session, int id) {
        session.setAttribute(KeyName, id);
    }

    public void logOut(HttpSession session) {
        session.removeAttribute(KeyName);
    }
}
