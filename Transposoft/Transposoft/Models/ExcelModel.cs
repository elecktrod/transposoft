using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transposoft.Models
{
    public class ExcelModel
    {
        public ExcelModel(int id, string name, string cipher, DateTime? dateFrom, DateTime? dateTo)
        {
            this.Id = id;
            this.Name = name;
            this.Cipher = cipher;
            this.DateFrom = dateFrom;
            this.DateTo = dateTo;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Cipher { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
