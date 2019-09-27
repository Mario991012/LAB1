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
        public Dictionary<byte, string> CodigosPrefijoArchivoActual = new Dictionary<byte, string>();
        public Dictionary<string, Archivos> DatosDeArchivos = new Dictionary<string, Archivos>();
        public Dictionary<string, byte> CodigoPD = new Dictionary<string, byte>();
        const int bufferLength = 1000;

        public int CompresiónHuffman(string path, string[] nombreArchivo, string pathHuffman)
        {
            var ListaNodos = new List<Estructuras.Nodo>();
            var Frecuencias = new Dictionary<byte, int>();

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
                    }
                    ListaNodos.Sort((x, y) => x.CompareTo(y));

                }
            }

            while (ListaNodos.Count >= 2)
            {
                Estructuras.Nodo padre = new Estructuras.Nodo();
                ListaNodos[0].recorridoIzq = true;
                ListaNodos[1].recorridoDer = true;
                padre.izq = ListaNodos[0];
                padre.der = ListaNodos[1];
                padre.Frecuencia = padre.izq.Frecuencia + padre.der.Frecuencia;
                ListaNodos[0].padre = padre;
                ListaNodos[1].padre = padre;
                var cont = 0;
                var agregado = false;
                foreach (var item in ListaNodos)
                {
                    cont++;
                    if (padre.Frecuencia <= item.Frecuencia)
                    {
                        ListaNodos.Insert(cont - 1, padre);
                        agregado = true;
                        break;
                    }
                }

                if (agregado == false)
                {
                    ListaNodos.Add(padre);
                }

                ListaNodos.Remove(ListaNodos[0]);
                ListaNodos.Remove(ListaNodos[0]);
            }

            Estructuras.ArbolS tree = new Estructuras.ArbolS();
            tree.raiz = ListaNodos[0];
            ListaNodos.Clear();
            ObteniendoCodigoPrefijo(tree.raiz);
            CodigosPrefijo.Add(nombreArchivo[0], CodigosPrefijoArchivoActual);

            using (var stream = new FileStream(path, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    using (var writeStream = new FileStream($"{pathHuffman}/{nombreArchivo[0]}.huff", FileMode.OpenOrCreate))
                    {
                        using (var writer = new BinaryWriter(writeStream))
                        {
                            var DiccionarioActual = CodigosPrefijo[nombreArchivo[0]];
                            writer.Write(nombreArchivo[1]);

                            foreach (var codigo in DiccionarioActual)
                            {
                                writer.Write($"{codigo.Key}|{codigo.Value}|");
                            }
                            writer.Write("--");

                            var byteBuffer = new byte[bufferLength];
                            var PosibleLlave = string.Empty;
                            while (reader.BaseStream.Position != reader.BaseStream.Length)
                            {
                                byteBuffer = reader.ReadBytes(bufferLength);
                                
                                for (int i = 0; i < byteBuffer.Length; i++)
                                {
                                    PosibleLlave = $"{PosibleLlave}{DiccionarioActual[byteBuffer[i]]}";
                                    
                                    while(PosibleLlave.Length > 8)
                                    {
                                        var decimaln = Convert.ToInt32(PosibleLlave.Substring(0, 8) , 2);
                                        var Caracter = Convert.ToByte(decimaln);
                                        writer.Write(Caracter);
                                        PosibleLlave = PosibleLlave.Substring(8);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            CodigosPrefijoArchivoActual.Clear();
            return 1;
        }

        static string codigo;
        public void ObteniendoCodigoPrefijo(Estructuras.Nodo nodo)
        {
            if (nodo != null)
            {
                ObteniendoCodigoPrefijo(nodo.izq);
                if (nodo.Valor != 'N')
                {
                    ConcatenandoCodigoPrefijo(nodo);
                    var val = (byte)nodo.Valor;
                    CodigosPrefijoArchivoActual.Add(val, codigo);
                    codigo = "";
                }
                else if (nodo.Valor == 'N' && nodo.izq == null && nodo.der == null)
                {
                    ConcatenandoCodigoPrefijo(nodo);
                    CodigosPrefijoArchivoActual.Add(nodo.Valor, codigo);
                    codigo = "";
                }
                ObteniendoCodigoPrefijo(nodo.der);
            }
        }

        public void ConcatenandoCodigoPrefijo(Estructuras.Nodo nodo)
        {
            if (nodo.recorridoIzq == true)
            {
                codigo = $"0{codigo}";
                ConcatenandoCodigoPrefijo(nodo.padre);
            }
            else if (nodo.recorridoDer == true)
            {
                codigo = $"1{codigo}";
                ConcatenandoCodigoPrefijo(nodo.padre);
            }
        }

        public int Descompresion(string path, string[] nombreArchivo, string pathHuffman)
        {
            var extension = "";
            var ContadorDeLecturas = 0;

            using (var stream = new FileStream(path, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    extension = reader.ReadString();
                }
            }

            using (var stream = new FileStream(path, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    using (var streamwriter = new FileStream($"{pathHuffman}//{nombreArchivo[0]}.{extension}", FileMode.OpenOrCreate))
                    {
                        using (var archivo = new BinaryWriter(streamwriter))
                        {
                            var byteBuffer = new byte[bufferLength];
                            var DiccionarioLeido = false;
                            var LlavePosible = string.Empty;
                            while (reader.BaseStream.Position != reader.BaseStream.Length)
                            {
                                if (DiccionarioLeido == false)
                                {
                                    var ByteLeido = reader.ReadString();
                                    if (ContadorDeLecturas == 0)
                                    {
                                        extension = ByteLeido;
                                        ContadorDeLecturas++;
                                    }
                                    else if (ByteLeido != "--" && DiccionarioLeido == false)
                                    {
                                        var separador = ByteLeido.Split('|');
                                        CodigoPD.Add(separador[1], Convert.ToByte(separador[0]));
                                    }
                                    else if (ByteLeido == "--")
                                    {
                                        DiccionarioLeido = true;
                                    }
                                }
                                else
                                {
                                    byteBuffer = reader.ReadBytes(bufferLength);

                                    for (int i = 0; i < byteBuffer.Length; i++)
                                    {
                                        LlavePosible = $"{LlavePosible}{Convert.ToString(Convert.ToInt32(byteBuffer[i]),2).PadLeft(8,'0')}";
                                        var contadorBits = 1;
                                        while(contadorBits <= LlavePosible.Length)
                                        {
                                            if(CodigoPD.ContainsKey(LlavePosible.Substring(0,contadorBits)))
                                            {
                                                archivo.Write(CodigoPD[LlavePosible.Substring(0, contadorBits)]);
                                                LlavePosible = LlavePosible.Substring(contadorBits);
                                                contadorBits = 0;
                                            }
                                            contadorBits++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    CodigoPD.Clear();
                }
            }
            return 1;
        }
    }
}