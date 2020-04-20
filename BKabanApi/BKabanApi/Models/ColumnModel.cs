using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BKabanApi.Models
{
    public class ColumnModel
    {
        public int? Id { get; set; }
        
        [MaxLength(100)]
        public string Name { get; set; }

        public IEnumerable<TaskModel> Tasks { get; set; }
    }
}
