using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BKabanApi.Models
{
    public class ColumnModel
    {
        public int? Id { get; set; }
        
        [MaxLength(100)]
        public string Name { get; set; }

        public IEnumerable<TaskModel> Tasks { get; set; }
    }

    public class ColumnModelBoardLink : ColumnModel
    {
        [Required]
        public int? BoardId { get; set; }
    }

    public class ColumnModelWithPosition : ColumnModel
    {
        [Required]
        public int? Position { get; set; }
    }
}
