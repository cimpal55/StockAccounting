using System;
using System.Collections.Generic;
using System.Text;

namespace StockAccounting.Checklist.Models.Data
{
    public class ToolkitHistoryModel
    {
        public int Id { get; set; }

        public int ToolkitId { get; set; }

        public int EmployeeId { get; set; }

        public DateTime Created { get; set; }
    }
}
