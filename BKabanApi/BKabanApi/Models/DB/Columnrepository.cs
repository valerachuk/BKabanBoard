using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BKabanApi.Models.DB
{
    public interface IColumnRepository
    {
        int? createColumn(int userId, ColumnModel column);
        bool updateColumn(int userId, ColumnModel column);
        bool deleteColumn(int userId, ColumnModel column);
    }
}
