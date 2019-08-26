using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estructuras
{
    public class Nodo
    {
        public int Frecuencia { get; set; }
        public char Valor { get; set; }
        public Nodo izq { get; set; }
        public Nodo der { get; set; }

        public Nodo()
        {
            Frecuencia = 0;
            izq = null;
            der = null;
            Valor = ' ';
        }

        public int CompareTo(object obj)
        {
            var comparado = (Nodo)obj;
            return Frecuencia.CompareTo(comparado.Frecuencia);
        }
    }
}
