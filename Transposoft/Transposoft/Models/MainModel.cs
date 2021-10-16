using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transposoft.Models
{
    public class MainModel : ExcelModel
    {
        public MainModel(ExcelModel model, int isExt, int? extID) : base(model.Id, model.Name, model.Cipher, model.DateFrom, model.DateTo)
        {
            this.IsExt = isExt;
            this.ExtID = extID;
        }

        public int IsExt { get; set; }
        public int? ExtID { get; set; }
    }
}
