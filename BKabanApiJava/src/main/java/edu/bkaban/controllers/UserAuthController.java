package edu.bkaban.controllers;

import edu.bkaban.models.UserModel;
import edu.bkaban.repositories.UserRepository;
import edu.bkaban.services.AuthService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import javax.servlet.http.HttpSession;
import javax.validation.Valid;
import java.sql.SQLException;

@RestController
@RequestMapping("/api")
public class UserAuthController {

    @Autowired
    private UserRepository _userRepository;

    @Autowired
    private AuthService _authService;

    @GetMapping(value = "/isLogged", produces = MediaType.APPLICATION_JSON_VALUE)
    private ResponseEntity isLogged(HttpSession session) {
        Integer id = _authService.getUserId(session);
        var res = String.format("{ \"isLogged\": %s }", String.valueOf(id != null));
        return ResponseEntity.ok(String.format("{ \"isLogged\": %s }", String.valueOf(id != null)));
    }

    @DeleteMapping("/logout")
    private void logout(HttpSession session) {
        _authService.logOut(session);
    }

    @PostMapping("/login")
    private ResponseEntity login(@Valid @RequestBody UserModel user, HttpSession session) throws SQLException {
        Integer id;
        if ((id = _userRepository.getUserIdFullMatch(user)) != null) {
            _authService.addUserToSession(session, id);
            return new ResponseEntity(HttpStatus.OK);
        }

        return ResponseEntity.badRequest().body("Invalid username or password");
    }

    @PostMapping("/register")
    private ResponseEntity register(@Valid @RequestBody UserModel user, HttpSession session) throws SQLException {
        if (_userRepository.getUserIdByUsername(user.getUsername()) != null) {
            return ResponseEntity.badRequest().body("This username is already used!");
        }

        Integer userId = _userRepository.create(user);
        if (userId == null ){
            return new ResponseEntity(HttpStatus.INTERNAL_SERVER_ERROR);
        }

        _authService.addUserToSession(session, userId);
        return new ResponseEntity(HttpStatus.OK);
    }
}
