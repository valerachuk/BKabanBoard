package edu.bkaban.models.task;

import javax.validation.constraints.NotNull;

public class TaskModelWithPositionAndNewColumn extends TaskModel {
    @NotNull
    private Integer NewColumnId;

    @NotNull
    private Integer Position;

    public Integer getNewColumnId() {
        return NewColumnId;
    }

    public void setNewColumnId(Integer newColumnId) {
        NewColumnId = newColumnId;
    }

    public Integer getPosition() {
        return Position;
    }

    public void setPosition(Integer position) {
        Position = position;
    }

    public TaskModelWithPositionAndNewColumn(Integer id, String name, String description, Integer newColumnId, Integer position) {
        super(id, name, description);
        NewColumnId = newColumnId;
        Position = position;
    }

    public TaskModelWithPositionAndNewColumn() { }
}
