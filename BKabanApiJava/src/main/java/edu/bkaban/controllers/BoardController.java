package edu.bkaban.controllers;

import edu.bkaban.models.board.BoardModel;
import edu.bkaban.models.board.BoardModelWithPosition;
import edu.bkaban.repositories.BoardRepository;
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
@RequestMapping("/api/board")
public class BoardController {

    @Autowired
    private AuthService _authService;

    @Autowired
    private BoardRepository _boardRepository;

    @PostMapping(produces = MediaType.APPLICATION_JSON_VALUE)
    private ResponseEntity createBoard(@Valid @RequestBody BoardModel board, HttpSession session) throws SQLException {
        Integer userId;
        if ((userId = _authService.getUserId(session)) == null) {
            return new ResponseEntity(HttpStatus.UNAUTHORIZED);
        }

        Integer boardId = _boardRepository.createBoard(userId, board);

        if (boardId == null) {
            return new ResponseEntity(HttpStatus.FORBIDDEN);
        }

        return ResponseEntity.ok("{ \"id\": " + boardId + " }");
    }

    @GetMapping("/{id}")
    private ResponseEntity<BoardModel> getBoard(@PathVariable int id, HttpSession session) throws SQLException {
        Integer userId;
        if ((userId = _authService.getUserId(session)) == null) {
            return new ResponseEntity(HttpStatus.UNAUTHORIZED);
        }

        var board = _boardRepository.getFullBoard(userId, id);

        if (board == null) {
            return new ResponseEntity(HttpStatus.FORBIDDEN);
        }

        return ResponseEntity.ok(board);
    }

    @PutMapping
    private ResponseEntity renameBoard(@Valid @RequestBody BoardModel board, HttpSession session) throws SQLException {
        Integer userId;
        if ((userId = _authService.getUserId(session)) == null) {
            return new ResponseEntity(HttpStatus.UNAUTHORIZED);
        }

        if (board.getId() == null) {
            return new ResponseEntity(HttpStatus.BAD_REQUEST);
        }

        boolean isOk = _boardRepository.updateBoardName(userId, board);

        if (isOk) {
            return new ResponseEntity(HttpStatus.OK);
        }

        return new ResponseEntity(HttpStatus.FORBIDDEN);
    }

    @PutMapping("/reorder")
    private ResponseEntity moveBoard(@Valid @RequestBody BoardModelWithPosition board, HttpSession session) throws SQLException {
        Integer userId;
        if ((userId = _authService.getUserId(session)) == null) {
            return new ResponseEntity(HttpStatus.UNAUTHORIZED);
        }

        if (board.getId() == null) {
            return new ResponseEntity(HttpStatus.BAD_REQUEST);
        }

        boolean isOk = _boardRepository.updateBoardPosition(userId, board);

        if (isOk) {
            return new ResponseEntity(HttpStatus.OK);
        }

        return new ResponseEntity(HttpStatus.FORBIDDEN);
    }

    @DeleteMapping("/{id}")
    private ResponseEntity deleteBoard(@PathVariable int id, HttpSession session) throws SQLException {
        Integer userId;
        if ((userId = _authService.getUserId(session)) == null) {
            return new ResponseEntity(HttpStatus.UNAUTHORIZED);
        }

        boolean isOk = _boardRepository.deleteBoard(userId, id);

        if (isOk) {
            return new ResponseEntity(HttpStatus.OK);
        }

        return new ResponseEntity(HttpStatus.FORBIDDEN);
    }


}
