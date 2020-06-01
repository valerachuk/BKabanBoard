package edu.bkaban.controllers;

import edu.bkaban.repositories.UserDataRepository;
import edu.bkaban.services.AuthService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import javax.servlet.http.HttpSession;
import java.sql.SQLException;

@RestController
@RequestMapping("api/userData")
public class UserDataController {

    @Autowired
    private AuthService _authService;

    @Autowired
    private UserDataRepository _userDataRepository;

    @GetMapping
    private ResponseEntity getUserData(HttpSession session) throws SQLException {
        Integer userId;
        if ((userId = _authService.getUserId(session)) == null) {
            return new ResponseEntity(HttpStatus.UNAUTHORIZED);
        }

        return ResponseEntity.ok(_userDataRepository.getUserData(userId));
    }
}
