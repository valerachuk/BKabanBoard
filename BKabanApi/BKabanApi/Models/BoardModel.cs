using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BKabanApi.Models
{
    public class BoardModel
    {
        public int? Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        public IEnumerable<ColumnModel> Columns { get; set; }
    }
}
