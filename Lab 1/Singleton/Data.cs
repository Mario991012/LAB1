using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Collections;
using Newtonsoft.Json;

namespace Lab_1.Singleton
{
    public class Data
    {
        private static Data instancia = null;
        public static Data Instancia
        {
            get
            {
                if (instancia == null)
                {
                    instancia = new Data();
                }
                return instancia;
            }
        }


        public Dictionary<char, int> CodigosPrefijo = new Dictionary<char, int>();

        public int Lectura(string path)
        {
            try
            {
                string[] lineas = File.ReadAllLines(path);
                
                foreach (var item in lineas)
                {
                    char[] caracteres = item.ToCharArray();
                    for (int i = 0; i < caracteres.Length; i++)
                    {
                        if (CodigosPrefijo.ContainsKey(caracteres[i]) == false)
                        {
                            CodigosPrefijo.Add(caracteres[i], 1);
                        }
                        else
                        {
                            CodigosPrefijo[caracteres[i]] = CodigosPrefijo[caracteres[i]] + 1;
                        }
                    }
                    
                }
                return 1;
            }
            catch
            {
                return 0;
            }
        }

        
        //public Dictionary<string, string> PalabrasReservadasPredeterminadas = new Dictionary<string, string>();

        //public string nombreTabla { get; set; }
        //public List<string> NombresTabla = new List<string>();

        //public Estructuras_de_Datos.Registro reg = new Estructuras_de_Datos.Registro();

        //public List<Estructuras_de_Datos.NodoB<Estructuras_de_Datos.Registro>> listaNodos = new List<Estructuras_de_Datos.NodoB<Estructuras_de_Datos.Registro>>();

        //public Dictionary<string, Estructuras_de_Datos.ArbolB<Estructuras_de_Datos.Registro>> Arboles = new Dictionary<string, Estructuras_de_Datos.ArbolB<Estructuras_de_Datos.Registro>>();
        

    }
}
