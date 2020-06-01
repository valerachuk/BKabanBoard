package edu.bkaban.controllers;

import edu.bkaban.models.IdDTO;
import edu.bkaban.models.task.TaskModel;
import edu.bkaban.models.task.TaskModelColumnLink;
import edu.bkaban.models.task.TaskModelWithPositionAndNewColumn;
import edu.bkaban.repositories.TaskRepository;
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
@RequestMapping("/api/task")
public class TaskController {

    @Autowired
    private TaskRepository _taskRepository;

    @Autowired
    private AuthService _authService;

    @PostMapping
    private ResponseEntity createTask(@Valid @RequestBody TaskModelColumnLink task, HttpSession session) throws SQLException {
        Integer userId;
        if ((userId = _authService.getUserId(session)) == null) {
            return new ResponseEntity(HttpStatus.UNAUTHORIZED);
        }

        Integer taskId = _taskRepository.createTask(userId, task);
        if (taskId == null) {
            return new ResponseEntity(HttpStatus.FORBIDDEN);
        }

        return ResponseEntity.ok(new IdDTO(taskId));
    }

    @PutMapping
    private ResponseEntity updateTask(@Valid @RequestBody TaskModel task, HttpSession session) throws SQLException {
        Integer userId;
        if ((userId = _authService.getUserId(session)) == null) {
            return new ResponseEntity(HttpStatus.UNAUTHORIZED);
        }

        if (task.getId() == null || task.getName() == null && task.getDescription() == null) {
            return new ResponseEntity(HttpStatus.BAD_REQUEST);
        }

        boolean isOk = _taskRepository.updateTask(userId, task);
        if (isOk) {
            return new ResponseEntity(HttpStatus.OK);
        }

        return new ResponseEntity(HttpStatus.FORBIDDEN);
    }

    @PutMapping("/reorder")
    private ResponseEntity moveTask(@Valid @RequestBody TaskModelWithPositionAndNewColumn task, HttpSession session) throws  SQLException {
        Integer userId;
        if ((userId = _authService.getUserId(session)) == null) {
            return new ResponseEntity(HttpStatus.UNAUTHORIZED);
        }

        if (task.getId() == null) {
            return new ResponseEntity(HttpStatus.BAD_REQUEST);
        }

        boolean isOk = _taskRepository.updateTaskPositionAndColumn(userId, task);
        if (isOk) {
            return new ResponseEntity(HttpStatus.OK);
        }

        return new ResponseEntity(HttpStatus.FORBIDDEN);
    }

    @DeleteMapping("/{id}")
    private ResponseEntity deleteTask(@PathVariable int id, HttpSession session) throws SQLException {
        Integer userId;
        if ((userId = _authService.getUserId(session)) == null) {
            return new ResponseEntity(HttpStatus.UNAUTHORIZED);
        }

        boolean isOk = _taskRepository.deleteTask(userId, id);
        if (isOk) {
            return new ResponseEntity(HttpStatus.OK);
        }

        return new ResponseEntity(HttpStatus.FORBIDDEN);
    }
}
