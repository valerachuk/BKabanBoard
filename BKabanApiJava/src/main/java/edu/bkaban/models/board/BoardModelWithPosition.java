package edu.bkaban.models.board;

import javax.validation.constraints.NotNull;

public class BoardModelWithPosition extends BoardModel {

    @NotNull
    private Integer Position;

    public Integer getPosition() {
        return Position;
    }

    public void setPosition(Integer position) {
        Position = position;
    }

    public BoardModelWithPosition(Integer id, String name, Integer position) {
        super(id, name);
        Position = position;
    }

    public BoardModelWithPosition() { }
}
