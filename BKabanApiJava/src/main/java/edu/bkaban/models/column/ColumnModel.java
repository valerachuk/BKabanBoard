package edu.bkaban.models.column;

import edu.bkaban.models.task.TaskModel;

import javax.validation.constraints.Size;
import java.util.List;

public class ColumnModel {
    private Integer Id;

    @Size(max = 100)
    private String Name;

    private List<TaskModel> Tasks;

    public List<TaskModel> getTasks() {
        return Tasks;
    }

    public void setTasks(List<TaskModel> tasks) {
        Tasks = tasks;
    }

    public Integer getId() {
        return Id;
    }

    public void setId(Integer id) {
        Id = id;
    }

    public String getName() {
        return Name;
    }

    public void setName(String name) {
        Name = name;
    }

    public ColumnModel(Integer id, String name) {
        Id = id;
        Name = name;
    }

    public ColumnModel() {}
}
