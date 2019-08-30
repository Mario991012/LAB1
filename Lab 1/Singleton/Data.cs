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

        public Dictionary<string, Dictionary<char, string>> CodigosPrefijo = new Dictionary<string, Dictionary<char, string>>(); 
        public Estructuras.ArbolS Arbol = new Estructuras.ArbolS();
        public Dictionary<char, int> Frecuencias = new Dictionary<char, int>();
        public List<Estructuras.Nodo> ListaNodos = new List<Estructuras.Nodo>();
        public Dictionary<char, string> AuxCodigosPrefijo = new Dictionary<char, string>();

        public int Lectura(string path, string nombreArchivo)
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
                CrearArbol(ListaNodos, nombreArchivo, lineas);
                return 1;
            }
            catch
            {
                return 0;
            }
        }

        public void CrearArbol(List<Estructuras.Nodo> ListNodos, string nombreArchivo, string[] lineas)
        {
            while (ListNodos.Count >= 2)
            {
                Estructuras.Nodo padre = new Estructuras.Nodo();
                ListaNodos[0].recorridoIzq = true;
                ListaNodos[1].recorridoDer = true;
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
            //Crear codigos prefijo y agregarlos a un diccionario de codigos prefijo de un archivo
            CodigoPrefijo(tree.raiz, nombreArchivo);
            CodigosPrefijo.Add(nombreArchivo, AuxCodigosPrefijo);
            codigo = "";
            //Para crear archivo que contenga el codigo binario
            EscrituraHuffman(nombreArchivo, AuxCodigosPrefijo, lineas);
        }

        static string codigo = "";
        public void CodigoPrefijo(Estructuras.Nodo nodo, string nombreArchivo)
        {
            if (nodo != null)
            {
                CodigoPrefijo(nodo.izq, nombreArchivo);
                if(nodo.Valor != ' ')
                {
                    Codigo(nodo);
                    AuxCodigosPrefijo.Add(nodo.Valor, codigo);
                    codigo = "";
                }
                CodigoPrefijo(nodo.der, nombreArchivo);
            }
        }

        public void Codigo(Estructuras.Nodo nodo)
        {
            if(nodo.recorridoIzq == true)
            {
                codigo = "0" + codigo;
                Codigo(nodo.padre);
            }else if(nodo.recorridoDer == true)
            {
                codigo = "1" + codigo;
                Codigo(nodo.padre);
            }
        }

        public void EscrituraHuffman(string FileName, Dictionary<char, string> dictionary, string[] lineas)
        {
            var Text = "";
            foreach (var item in lineas)
            {
                char[] caracteres = item.ToCharArray();

                for (int i = 0; i < caracteres.Length; i++)
                {
                    foreach (var item2 in dictionary.Keys)
                    {
                        if (item2 == caracteres[i])
                        {
                            Text += dictionary[item2];
                            break;
                        }

                    }
                }  
            }
            ConvertirADecimalYEscrituraTXT(Text, dictionary);
            System.IO.File.WriteAllText(@"C:\Users\Marcos Andrés CM\Desktop\Cuarto ciclo 2019\EDII\PRUEBAS PARA LAB\FileName.txt", Text);
            //string DireccionArchivo = "C:\\Users\\Marcos Andrés CM\\Desktop\\Cuarto ciclo 2019\\EDII\\PRUEBAS PARA LAB";

            //using (FileStream flujoArchivo = new FileStream(DireccionArchivo, FileAccess.Write, FileShare.None)) ;
            /*StreamWriter archivo = new StreamWriter("C:\\Users\\Marcos Andrés CM\\Desktop\\Cuarto ciclo 2019\\EDII\\PRUEBAS PARA LAB");
            foreach (var item in dictionary.Values)
            {
                archivo.WriteLine(item);
            }
            archivo.Close();*/
        }
        public void ConvertirADecimalYEscrituraTXT(string Text, Dictionary<char, string> dictionary)
        {
            var respuesta = "";
            var enlistado = 0;
            var ascii = string.Empty;
            List<string> decimales = new List<string>(); //Lista que almacenara los 8 bits
            for (int i = 0; i < Text.Length; i += 8)
            {
                respuesta = Text.Substring(i, 8);
                enlistado = Convert.ToInt32(respuesta, 2);
                decimales.Add(enlistado.ToString());
            }
            //NO BORRAR ESTO. ESTO ES UNA DE LAS DOS FORMAS PARA OBTENER EN ASCII
            
            /*for (int i = 0; i < decimales.Count; i++)
            {
                char asciiCode = Convert.ToChar(decimales[i]);
                ascii += asciiCode;
            }*/

            List <string> keys = new List<string>();
            List<string> values = new List<string>();

            foreach (var item in dictionary.Keys)
            {
                keys.Add(item.ToString());
            }
            foreach (var item2 in dictionary.Values)
            {
                values.Add(item2);
            }
            var answer = "";
            for (int i = 0; i < keys.Count; i++)
            {
                answer = keys[i] + "|" + values[i] + ",";
                System.IO.File.AppendAllText(@"C:\Users\Marcos Andrés CM\Desktop\Cuarto ciclo 2019\EDII\PRUEBAS PARA LAB\FileName.txt", answer);

            }
            for (int i = 0; i < decimales.Count; i++)
            {
                ascii += (char)Convert.ToByte(decimales[i].Substring(0, decimales[i].Length));

            }
            
            using (StreamWriter tr2 = File.AppendText(@"C:\Users\Marcos Andrés CM\Desktop\Cuarto ciclo 2019\EDII\PRUEBAS PARA LAB\FileName.txt"))
            {
                tr2.WriteLine(ascii);
            }
            //var lectura = "";
            //StreamReader tr = new StreamReader(@"C:\Users\Marcos Andrés CM\Desktop\Cuarto ciclo 2019\EDII\PRUEBAS PARA LAB\FileName.txt");
            //while ((lectura = tr.ReadLine()) == null)
            //{
            //    //System.IO.File.WriteAllText(@"C:\Users\Marcos Andrés CM\Desktop\Cuarto ciclo 2019\EDII\PRUEBAS PARA LAB\FileName.txt", ascii);
            //}
            //System.IO.File.AppendAllText(@"C:\Users\Marcos Andrés CM\Desktop\Cuarto ciclo 2019\EDII\PRUEBAS PARA LAB\FileName.txt", ascii);

        }
    }
}
