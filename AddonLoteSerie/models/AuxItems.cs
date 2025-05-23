using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddonConEntrega.models
{
    class AuxItems
    {
        public string ItemCode { get; set; }
        public double QtyReq { get; set; }
        public double QtyReal { get; set; }
        public bool Change { get; set; }

        public AuxItems(string ItemCode, double QtyReq, double QtyReal)
        {
            this.ItemCode = ItemCode;
            this.QtyReq = QtyReq;
            this.QtyReal = QtyReal;
            this.Change = QtyReal > 0 ? (QtyReq != QtyReal) : false;


        }
    }
}
