using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lab_1.Singleton
{
    public class LogicaLZW
    {
        private static LogicaLZW instancia = null;
        public static LogicaLZW Instancia
        {
            get
            {
                if (instancia == null)
                {
                    instancia = new LogicaLZW();
                }
                return instancia;
            }
        }

    }
}