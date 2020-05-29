package edu.bkaban.controllers;

import edu.bkaban.models.column.ColumnModel;
import edu.bkaban.models.column.ColumnModelBoardLink;
import edu.bkaban.models.column.ColumnModelWithPosition;
import edu.bkaban.repositories.ColumnRepository;
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
@RequestMapping("/api/column")
public class ColumnController {

    @Autowired
    private AuthService _authService;

    @Autowired
    private ColumnRepository _columnRepository;

    @PostMapping(produces = MediaType.APPLICATION_JSON_VALUE)
    private ResponseEntity createColumn(@Valid @RequestBody ColumnModelBoardLink column, HttpSession session) throws SQLException {
        Integer userId;
        if ((userId = _authService.getUserId(session)) == null) {
            return new ResponseEntity(HttpStatus.UNAUTHORIZED);
        }

        Integer columnId = _columnRepository.createColumn(userId, column);
        if (columnId == null) {
            return new ResponseEntity(HttpStatus.FORBIDDEN);
        }

        return ResponseEntity.ok().body("{ \"id\": " + columnId + " }");
    }

    @PutMapping
    private ResponseEntity updateColumn(@Valid @RequestBody ColumnModel column, HttpSession session) throws SQLException {
        Integer userId;
        if ((userId = _authService.getUserId(session)) == null) {
            return new ResponseEntity(HttpStatus.UNAUTHORIZED);
        }

        if (column.getId() == null) {
            return new ResponseEntity(HttpStatus.BAD_REQUEST);
        }

        boolean result = _columnRepository.updateColumn(userId, column);

        if (result) {
            return new ResponseEntity(HttpStatus.OK);
        }

        return new ResponseEntity(HttpStatus.FORBIDDEN);
    }

    @PutMapping("reorder")
    private ResponseEntity moveColumn(@Valid @RequestBody ColumnModelWithPosition column, HttpSession session) throws SQLException {
        Integer userId;
        if ((userId = _authService.getUserId(session)) == null) {
            return new ResponseEntity(HttpStatus.UNAUTHORIZED);
        }

        if (column.getId() == null) {
            return new ResponseEntity(HttpStatus.BAD_REQUEST);
        }

        boolean result = _columnRepository.updateColumnPosition(userId, column);

        if (result) {
            return new ResponseEntity(HttpStatus.OK);
        }

        return new ResponseEntity(HttpStatus.FORBIDDEN);
    }

    @DeleteMapping("/{id}")
    private ResponseEntity deleteColumn(@PathVariable int id, HttpSession session) throws SQLException {
        Integer userId;
        if ((userId = _authService.getUserId(session)) == null) {
            return new ResponseEntity(HttpStatus.UNAUTHORIZED);
        }

        boolean result = _columnRepository.deleteColumn(userId, id);

        if (result) {
            return new ResponseEntity(HttpStatus.OK);
        }

        return new ResponseEntity(HttpStatus.FORBIDDEN);
    }

}
