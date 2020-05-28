package edu.bkaban.models;

import javax.validation.constraints.NotNull;
import javax.validation.constraints.Size;

public class TaskModel {
    private Integer Id;

    @Size(max = 300)
    private String Name;

    @Size(max = 1000)
    private String Description;

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

    public String getDescription() {
        return Description;
    }

    public void setDescription(String description) {
        Description = description;
    }

    public TaskModel(Integer id, String name, String description) {
        Id = id;
        Name = name;
        Description = description;
    }

    public TaskModel() {}
}

