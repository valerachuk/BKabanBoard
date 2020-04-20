using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BKabanApi.Models
{
    public class TaskModel
    {
        public int? Id { get; set; }

        [MaxLength(300)]
        public string Name { get; set; }
        
        [MaxLength(1000)]
        public string Description { get; set; }
    }

    public class TaskModelColumnLink : TaskModel
    {
        [Required]
        public int ColumnId { get; set; }
    }
}
