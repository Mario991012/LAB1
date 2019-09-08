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

        public Dictionary<string, Dictionary<byte, string>> CodigosPrefijo = new Dictionary<string, Dictionary<byte, string>>();
        public Estructuras.ArbolS Arbol = new Estructuras.ArbolS();
        public Dictionary<byte, int> Frecuencias = new Dictionary<byte, int>();
        public List<Estructuras.Nodo> ListaNodos = new List<Estructuras.Nodo>();
        public Dictionary<byte, string> AuxCodigosPrefijo = new Dictionary<byte, string>();
        public Dictionary<string, string> DescompCodigosPrefijo = new Dictionary<string, string>();
        public List<string> NombreArchivos = new List<string>();
        public Dictionary<string, Archivos> DatosDeArchivos = new Dictionary<string, Archivos>();
        public Dictionary<string, byte> CodigoPD = new Dictionary<string, byte>();
        const int bufferLength = 1000;

        public int Leer(string path, string[] nombreArchivo, string pathHuffman)
        {
            using (var stream = new FileStream(path, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var byteBuffer = new byte[bufferLength];
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        byteBuffer = reader.ReadBytes(bufferLength);
                        for (int i = 0; i < byteBuffer.Length; i++)
                        {
                            if (Frecuencias.ContainsKey(byteBuffer[i]) == false)
                            {
                                Frecuencias.Add(byteBuffer[i], 1);
                            }
                            else
                            {
                                Frecuencias[byteBuffer[i]] = Frecuencias[byteBuffer[i]] + 1;
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
                    
                }
            }
            CrearArbol(ListaNodos, nombreArchivo, pathHuffman, path);

            return 1;
        }
        //public int Lectura(string path, string[] nombreArchivo, string pathHuffman)
        //{
        //    var lineas = File.ReadAllLines(path);

        //    for (int i = 0; i < lineas.Length; i++)
        //    {
        //        lineas[i] = $"{lineas[i]}\r";
        //    }

            
        //    foreach (var item in lineas)
        //    {
        //        var array = item.ToCharArray();
        //        var caracteres = new List<byte>();

        //        foreach (var item2 in array)
        //        {
        //            var x = Convert.ToByte(item2);
        //            caracteres.Add(x);
        //        }
        //        for (int i = 0; i < caracteres.Count; i++)
        //        {
        //            if (Frecuencias.ContainsKey(caracteres[i]) == false)
        //            {
        //                Frecuencias.Add(caracteres[i], 1);
        //            }
        //            else
        //            {
        //                Frecuencias[caracteres[i]] = Frecuencias[caracteres[i]] + 1;
        //            }
        //        }
        //    }

        //    foreach (var nodos in Frecuencias)
        //    {
        //        Estructuras.Nodo nodo = new Estructuras.Nodo();
        //        nodo.Frecuencia = nodos.Value;
        //        nodo.Valor = nodos.Key;
        //        ListaNodos.Add(nodo);
        //        ListaNodos.Sort((x, y) => x.CompareTo(y));
        //    }
        //    CrearArbol(ListaNodos, nombreArchivo, lineas, pathHuffman);
        //    return 1;
        //}

        public void CrearArbol(List<Estructuras.Nodo> ListNodos, string[] nombreArchivo, string pathHuffman, string path)
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
            ObteniendoCodigoPrefijo(tree.raiz);
            NombreArchivos.Add(nombreArchivo[0]);
            CodigosPrefijo.Add(nombreArchivo[0], AuxCodigosPrefijo);
            codigo = "";
            EscrituraHuffman(nombreArchivo, AuxCodigosPrefijo, pathHuffman, path);


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
                    var val = (byte)nodo.Valor;
                    AuxCodigosPrefijo.Add(val, codigo);
                    codigo = "";
                }
                else if (nodo.Valor == 'N' && nodo.izq == null && nodo.der == null)
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

        public void EscrituraHuffman(string[] FileName, Dictionary<byte, string> DiccionarioCP, string pathHuffman, string path)
        {
            using (var stream = new FileStream(path, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    using (var writeStream = new FileStream($"{pathHuffman}/{FileName[0]}.huff", FileMode.OpenOrCreate))
                    {
                        using (var writer = new BinaryWriter(writeStream))
                        {
                            var byteBuffer = new byte[bufferLength];
                            //while (reader.BaseStream.Position != reader.BaseStream.Length)
                            //{
                                byteBuffer = reader.ReadBytes(bufferLength);

                                var Text = new List<char>();
                                var ListaAscii = new List<string>();
                                var caracteres = new List<byte>();
                                foreach (var caracter in byteBuffer)
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
                                var DicAux = CodigosPrefijo[FileName[0]];
                                writer.Write(FileName[1]);
                                writer.Write("\r");
                                foreach (var codigo in DicAux)
                                {
                                    writer.Write($"{codigo.Key}|{codigo.Value}|");
                                }
                                writer.Write("\r");
                                writer.Write("--");
                                writer.Write("\r");

                                foreach (var item in ListaAscii)
                                {
                                    var ascii = Convert.ToChar(Convert.ToByte(item));
                                    writer.Write($"{ascii}");
                                }
                                writer.Write("\r");

                            //}

                        }
                    }
                }
            }
            AuxCodigosPrefijo.Clear();
        }

        public int Descompresion(string path, string nombreArchivo, string pathHuffman)
        {
            List<string> BinaryList = new List<string>();
            //try
            //{
            var lineas = File.ReadAllLines(path);
            var indexseparador = 0;
            var extension = lineas[0];
            for (int i = 0; i < lineas.Length; i++)
            {
                if (lineas[i] == "-")
                {
                    break;
                }
                else
                {
                    indexseparador++;
                }
            }

            for (int k = 1; k < indexseparador; k++)
            {
                var separador = lineas[k].Split('|');
                for (int i = 0; i < separador.Length - 1; i += 2)
                {
                    var separadordecimal = Convert.ToInt32(separador[i]);
                    CodigoPD.Add(separador[i + 1], (byte)separadordecimal);
                }
            }

            for (int i = indexseparador + 1; i < lineas.Length; i++)
            {
                if (lineas[i].Length > 0)
                {
                    var asciiArray = lineas[i].ToCharArray();

                    for (int j = 0; j < asciiArray.Length; j++)
                    {
                        var BinaryCode = Convert.ToString(asciiArray[j], 2).PadLeft(8, '0');
                        BinaryList.Add(BinaryCode);
                    }
                }
            }
            ObtenerCaracter(BinaryList, CodigoPD, pathHuffman, nombreArchivo, extension);

            return 1;
            //}
            //    catch
            //    {
            //        return 0;
            //    }
        }

        static int CountOfBytes = 0;
        static string BinaryC = "";
        static int PosicionLinea = 0;
        public void ObtenerCaracter(List<string> BinaryList, Dictionary<string, byte> CodigoPD, string pathHuffman, string nombreArchivo, string extension)
        {
            using (StreamWriter archivo = new StreamWriter($"{pathHuffman}//{nombreArchivo}.{extension}"))
            {

                var CharOfBytes = BinaryList[PosicionLinea].ToCharArray();
                reinicio:
                do
                {
                    BinaryC = $"{BinaryC}{CharOfBytes[CountOfBytes]}";
                    CountOfBytes++;

                    RecorrerBinaryListRecursivamente(BinaryList, CodigoPD, ref BinaryC, nombreArchivo, extension);
                    if (TextoDescomprimido.Length == 10)
                    {
                        archivo.Write(TextoDescomprimido);
                        TextoDescomprimido = string.Empty;
                        archivo.Flush();
                    }
                } while (CountOfBytes < CharOfBytes.Length);

                PosicionLinea++;
                CountOfBytes = 0;
                if (BinaryList.Count > PosicionLinea)
                {
                    CharOfBytes = BinaryList[PosicionLinea].ToCharArray();
                    goto reinicio;
                }
            }



        }

        static string TextoDescomprimido = "";
        public void RecorrerBinaryListRecursivamente(List<string> BinaryList, Dictionary<string, byte> CodigoPD, ref string BinaryC, string nombreArchivo, string extension)
        {
            if (CodigoPD.ContainsKey(BinaryC))
            {
                TextoDescomprimido = $"{TextoDescomprimido}{CodigoPD[BinaryC]}";
                BinaryC = string.Empty;
            }
        }

    }
}