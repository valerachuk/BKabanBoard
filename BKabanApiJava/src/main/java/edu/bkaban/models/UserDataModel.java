package edu.bkaban.models;

import edu.bkaban.models.board.BoardModel;

import java.util.List;

public class UserDataModel {
    private List<BoardModel> Boards;

    public List<BoardModel> getBoards() {
        return Boards;
    }

    public void setBoards(List<BoardModel> boards) {
        Boards = boards;
    }
}
