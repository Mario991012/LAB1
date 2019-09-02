using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace Lab_1.Models
{
    public class Archivos
    {
        [DisplayName("Nombre del Archivo")]
        public string NombreArchivo { get; set; }
        [DisplayName("Razón de Compresión")]
        public double Razon { get; set; }
        [DisplayName("Factor de Compresión")]
        public double Factor { get; set; }
        [DisplayName("Porcentaje de Reducción")]
        public double Porcentaje { get; set; }

        public Archivos()
        {
            NombreArchivo = "";
            Razon = 0;
            Factor = 0;
            Porcentaje = 0;
        }

    }
}