package edu.bkaban.models.task;

import javax.validation.constraints.NotNull;

public class TaskModelColumnLink extends TaskModel {
    @NotNull
    private Integer ColumnId;

    public Integer getColumnId() {
        return ColumnId;
    }

    public void setColumnId(Integer columnId) {
        ColumnId = columnId;
    }

    public TaskModelColumnLink(Integer id, String name, String description, Integer columnId) {
        super(id, name, description);
        ColumnId = columnId;
    }

    public TaskModelColumnLink() { }
}
