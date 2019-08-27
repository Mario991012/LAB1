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

        public Estructuras.ArbolS Arbol = new Estructuras.ArbolS();
        public List<Estructuras.ArbolS> ListaArboles = new List<Estructuras.ArbolS>();
        public Dictionary<char, int> Frecuencias = new Dictionary<char, int>();
        public List<Estructuras.Nodo> ListaNodos = new List<Estructuras.Nodo>();

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
                        if (Frecuencias.ContainsKey(caracteres[i]) == false)
                        {
                            Frecuencias.Add(caracteres[i], 1);
                        }
                        else
                        {
                            Frecuencias[caracteres[i]] = Frecuencias[caracteres[i]] + 1;
                        }
                    }
                }

                foreach(var nodos in Frecuencias)
                {
                    Estructuras.Nodo nodo = new Estructuras.Nodo();
                    nodo.Frecuencia = nodos.Value;
                    nodo.Valor = nodos.Key;
                    ListaNodos.Add(nodo);
                    ListaNodos.Sort((x, y) => x.CompareTo(y));
                }
                CrearArbol(ListaNodos);
                return 1;
            }
            catch
            {
                return 0;
            }
        }

        public void CrearArbol(List<Estructuras.Nodo> ListNodos)
        {
            while (ListNodos.Count >= 2)
            {
                Estructuras.Nodo padre = new Estructuras.Nodo();
                padre.izq = ListNodos[0];
                padre.der = ListNodos[1];
                padre.Frecuencia = padre.izq.Frecuencia + padre.der.Frecuencia;
                ListNodos[0].padre = padre;
                ListNodos[1].padre = padre;
                ListNodos.Add(padre);
                ListNodos.Remove(ListNodos[0]);
                ListNodos.Remove(ListNodos[0]);
                ListaNodos.Sort((x, y) => x.CompareTo(y));

            }
            Estructuras.ArbolS tree = new Estructuras.ArbolS();
            tree.raiz = ListNodos[0];
            ListNodos.Clear();
            ListaArboles.Add(tree);
        }
        
        //public Dictionary<string, string> PalabrasReservadasPredeterminadas = new Dictionary<string, string>();

        //public string nombreTabla { get; set; }
        //public List<string> NombresTabla = new List<string>();

        //public Estructuras_de_Datos.Registro reg = new Estructuras_de_Datos.Registro();

        //public List<Estructuras_de_Datos.NodoB<Estructuras_de_Datos.Registro>> listaNodos = new List<Estructuras_de_Datos.NodoB<Estructuras_de_Datos.Registro>>();

        //public Dictionary<string, Estructuras_de_Datos.ArbolB<Estructuras_de_Datos.Registro>> Arboles = new Dictionary<string, Estructuras_de_Datos.ArbolB<Estructuras_de_Datos.Registro>>();
        

    }
}
