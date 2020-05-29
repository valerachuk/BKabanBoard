package edu.bkaban.models.column;

import javax.validation.constraints.NotNull;

public class ColumnModelBoardLink extends ColumnModel {
    @NotNull
    private Integer BoardId;

    public Integer getBoardId() {
        return BoardId;
    }

    public void setBoardId(Integer boardId) {
        BoardId = boardId;
    }

    public ColumnModelBoardLink(Integer id, String name, Integer boardId) {
        super(id, name);
        BoardId = boardId;
    }

    public ColumnModelBoardLink() { }
}
