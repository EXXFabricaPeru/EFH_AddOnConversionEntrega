using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddOnUpdPrice.Functionality
{
    public class SelectedRows
    {
        public string Marcar { get; set; } = "N";
        public string CDIT { get; set; } = string.Empty;
        public string DSIT { get; set; } = string.Empty;
        public string MNPU { get; set; } = string.Empty;
        public double LMPU { get; set; } = 0;
        public string MNPM { get; set; } = string.Empty;
        public double LMPM { get; set; } = 0;
        public string MNRU { get; set; } = string.Empty;
        public double PRPU { get; set; } = 0;
        public string MNRM { get; set; } = string.Empty;
        public double PRPM { get; set; } = 0;

    }
}
