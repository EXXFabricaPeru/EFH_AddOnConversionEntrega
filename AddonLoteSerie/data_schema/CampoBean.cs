using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddonConEntrega.data_schema
{
    public class CampoBean
    {
        public CampoBean()
        {
            validValues = null;
            validDescription = null;
            valorPorDef = "";
            tablaVinculada = "";
            subtipo_campo = SAPbobsCOM.BoFldSubTypes.st_None;
            obligatorio = SAPbobsCOM.BoYesNoEnum.tNO;
            tamano = 0;
        }

        public string nombre_tabla { get; set; }
        public string nombre_campo { get; set; }
        public string descrp_campo { get; set; }
        public SAPbobsCOM.BoFieldTypes tipo_campo { get; set; }
        public SAPbobsCOM.BoFldSubTypes subtipo_campo { get; set; }
        public int tamano { get; set; }
        public SAPbobsCOM.BoYesNoEnum obligatorio { get; set; }
        public string[] validValues { get; set; }
        public string[] validDescription { get; set; }
        public string valorPorDef { get; set; }
        public string tablaVinculada { get; set; }
    }
}
