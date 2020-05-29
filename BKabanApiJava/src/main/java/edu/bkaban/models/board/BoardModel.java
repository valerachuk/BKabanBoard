package edu.bkaban.models.board;

import edu.bkaban.models.column.ColumnModel;
import org.hibernate.validator.constraints.Length;

import java.util.List;

public class BoardModel {

    private Integer Id;

    @Length(max = 100)
    private String Name;

    private List<ColumnModel> Columns;

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

    public List<ColumnModel> getColumns() {
        return Columns;
    }

    public void setColumns(List<ColumnModel> columns) {
        Columns = columns;
    }

    public BoardModel(Integer id, String name) {
        Id = id;
        Name = name;
    }

    public BoardModel() {}
}
