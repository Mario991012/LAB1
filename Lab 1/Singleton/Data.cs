using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Lab_1.Models;
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
        public Dictionary<string, string> DescompCodigosPrefijo = new Dictionary<string, string>();
        public List<string> NombreArchivos = new List<string>();
        public Dictionary<string, Archivos> DatosDeArchivos = new Dictionary<string, Archivos>();

        //Para descompresion
        public Dictionary<string, char> CodigoPD = new Dictionary<string, char>(); 

        public int Lectura(string path, string[] nombreArchivo, string pathHuffman)
        {
            //try
            //{
                var lineas = File.ReadAllLines(path);

                foreach (var item in lineas)
                {
                    var caracteres = item.ToCharArray();
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

                foreach (var nodos in Frecuencias)
                {
                    Estructuras.Nodo nodo = new Estructuras.Nodo();
                    nodo.Frecuencia = nodos.Value;
                    nodo.Valor = nodos.Key;
                    ListaNodos.Add(nodo);
                    ListaNodos.Sort((x, y) => x.CompareTo(y));
                }
                CrearArbol(ListaNodos, nombreArchivo, lineas, pathHuffman);
                return 1;
            //}
            //catch
            //{
            //    return 0;
            //}
        }

        public void CrearArbol(List<Estructuras.Nodo> ListNodos, string[] nombreArchivo, string[] lineas, string pathHuffman)
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
                var cont = 0;
                var agregado = false;
                foreach (var item in ListNodos)
                {
                    cont++;
                    if (padre.Frecuencia <= item.Frecuencia)
                    {
                        ListNodos.Insert(cont - 1, padre);
                        agregado = true;
                        break;
                    }
                }

                if (agregado == false)
                {
                    ListNodos.Add(padre);
                }

                ListNodos.Remove(ListNodos[0]);
                ListNodos.Remove(ListNodos[0]);

            }

            Estructuras.ArbolS tree = new Estructuras.ArbolS();
            tree.raiz = ListNodos[0];
            ListNodos.Clear();
            //Crear codigos prefijo y agregarlos a un diccionario de codigos prefijo de un archivo
            ObteniendoCodigoPrefijo(tree.raiz);
            NombreArchivos.Add(nombreArchivo[0]);
            CodigosPrefijo.Add(nombreArchivo[0], AuxCodigosPrefijo);
            codigo = "";
            //Para crear archivo que contenga el codigo binario
            EscrituraHuffman(nombreArchivo, AuxCodigosPrefijo, lineas, pathHuffman);


        }

        static string codigo = "";
        public void ObteniendoCodigoPrefijo(Estructuras.Nodo nodo)
        {
            if (nodo != null)
            {
                ObteniendoCodigoPrefijo(nodo.izq);
                if (nodo.Valor != 'N')
                {
                    ConcatenandoCodigoPrefijo(nodo);
                    AuxCodigosPrefijo.Add(nodo.Valor, codigo);
                    codigo = "";
                }else if (nodo.Valor == 'N' && nodo.izq == null && nodo.der == null)
                {
                    ConcatenandoCodigoPrefijo(nodo);
                    AuxCodigosPrefijo.Add(nodo.Valor, codigo);
                    codigo = "";
                }
                ObteniendoCodigoPrefijo(nodo.der);
            }
        }

        public void ConcatenandoCodigoPrefijo(Estructuras.Nodo nodo)
        {
            if (nodo.recorridoIzq == true)
            {
                codigo = "0" + codigo;
                ConcatenandoCodigoPrefijo(nodo.padre);
            }
            else if (nodo.recorridoDer == true)
            {
                codigo = "1" + codigo;
                ConcatenandoCodigoPrefijo(nodo.padre);
            }

        }

        public static string ValidateUtf8(char txt)
        {
            StringBuilder sbOutput = new StringBuilder();
            char ch;
                ch = txt;
                if ((ch >= 0x0020 && ch <= 0xD7FF) ||(ch >= 0xE000 && ch <= 0xFFFD) ||ch == 0x0009 ||ch == 0x000A ||ch == 0x000D || ch == 0xD9F0)
                {
                    sbOutput.Append(ch);
                }
            return sbOutput.ToString();
        }

        public void EscrituraHuffman(string[] FileName, Dictionary<char, string> DiccionarioCP, string[] lineas, string pathHuffman)
        {
            var Text = new List<char>();
            var ListaAscii = new List<string>();

            foreach (var item in lineas)
            {
                var caracteres = item.ToCharArray();

                foreach (var caracter in caracteres)
                {
                    var codigoPrefijo = DiccionarioCP[caracter].ToCharArray();
                    foreach (var binario in codigoPrefijo)
                    {
                        Text.Add(binario);

                        if (Text.Count >= 8)
                        {
                            var enlistado = "";
                            var ascii = 0;
                            foreach (var bin in Text)
                            {
                                enlistado = $"{enlistado}{bin}";
                            }
                            ascii = Convert.ToInt32(enlistado, 2);
                            ListaAscii.Add(ascii.ToString());
                            Text.Clear();
                        }
                    }
                }
            }

            var TextoComprimido = "";
            foreach (var item in ListaAscii)
            {
                var ascii = Convert.ToChar(Convert.ToByte(item));
                var utf8Text = ValidateUtf8(ascii);
                TextoComprimido = $"{TextoComprimido}{utf8Text}";
            }

            var answer = "";

            var DicAux = CodigosPrefijo[FileName[0]];

            foreach (var codigo in DicAux)
            {
                answer = $"{answer}{codigo.Key}|{codigo.Value}|";
            }


            using (StreamWriter archivo = new StreamWriter($"{pathHuffman}//{FileName[0]}.huff"))
            {
                archivo.WriteLine(FileName[1]);
                archivo.WriteLine(answer);
                archivo.WriteLine("-");
                archivo.WriteLine(TextoComprimido);
            }
            AuxCodigosPrefijo.Clear();
        }

        #region Descompresion
        public int Descompresion(string path, string nombreArchivo, string pathHuffman)
        {
            List<string> BinaryList = new List<string>();
            try
            {
                var lineas = File.ReadAllLines(path);
                var indexseparador = 0;
                for (int i = 0; i < lineas.Length; i++)
                {
                    if(lineas[i] == "-")
                    {
                        break;
                    }else
                    {
                        indexseparador++;
                    }
                }

                for (int k = 0; k < indexseparador; k++)
                {
                    var separador = lineas[k].Split('|');
                    var sep = separador.Length;
                    for (int i = 0; i < separador.Length - 1; i += 2)
                    {
                        CodigoPD.Add(separador[i + 1], Convert.ToChar(separador[i]));
                    }
                }

                for (int i = indexseparador + 1; i < lineas.Length; i++)
                {
                    if (lineas[i].Length >0)
                    {
                        var asciiArray = lineas[i].ToCharArray();
                        for (int j = 0; j < asciiArray.Length; j++)
                        {
                            var CharToInt = asciiArray[j].ToString();
                            var BinaryCode = Convert.ToString(asciiArray[j],2);
                            BinaryList.Add(BinaryCode);
                        }
                    }     
                }
                ObtenerCaracter(BinaryList, CodigoPD);

                return 1;
            }
            catch
            {
               return 0;
            }      
        }

        static int CountOfBytes = 0;
        static string FinalText = "";
        static string BinaryC = "";
        static bool Chequeo = false;
        static int PosicionLinea = 0;
        public void ObtenerCaracter(List<string> BinaryList, Dictionary<string, char> CodigoPD)
        {
            var CharOfBytes = BinaryList[PosicionLinea].ToCharArray(); //vector que cada elemento sera un 0 o 1 del binario
                
            do
            {
                if(CountOfBytes == CharOfBytes.Length)
                {
                    CountOfBytes = 0;
                    PosicionLinea++;
                    CharOfBytes = BinaryList[PosicionLinea].ToCharArray();
                }
                BinaryC = $"{BinaryC}{CharOfBytes[CountOfBytes]}";
                CountOfBytes++;
                RecorrerBinaryListRecursivamente(BinaryList, CodigoPD, ref FinalText, ref Chequeo, BinaryC);
            } while ((Chequeo == false) && CountOfBytes == CharOfBytes.Length);

            if ((Chequeo == false) && CountOfBytes >= CharOfBytes.Length)//Se deben de concatenar 2 binarios
            {
                CountOfBytes = 0;
                PosicionLinea++;
                ObtenerCaracter(BinaryList, CodigoPD);
            }
            else if(Chequeo == true)
            {
                BinaryC = "";
                if(CountOfBytes == CharOfBytes.Length)
                {
                    PosicionLinea++;
                    CountOfBytes = 0;
                }
                Chequeo = false;
                ObtenerCaracter(BinaryList, CodigoPD);
            }

        }
        public void RecorrerBinaryListRecursivamente(List<string> BinaryList, Dictionary<string, char> CodigoPD, ref string FinalText, ref bool chequeo, string BinaryC)
        {
                if (CodigoPD.ContainsKey(BinaryC))
                {
                    FinalText = $"{FinalText}{CodigoPD[BinaryC]}";
                    chequeo = true;
                    //BinaryList.Remove();
                }
        }
        #endregion
    }
}
