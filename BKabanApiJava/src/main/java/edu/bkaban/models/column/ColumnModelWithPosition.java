package edu.bkaban.models.column;

import javax.validation.constraints.NotNull;

public class ColumnModelWithPosition extends ColumnModel {
    @NotNull
    private Integer Position;

    public Integer getPosition() {
        return Position;
    }

    public void setPosition(Integer position) {
        Position = position;
    }

    public ColumnModelWithPosition(Integer id, String name, Integer position) {
        super(id, name);
        Position = position;
    }

    public ColumnModelWithPosition() { }
}
