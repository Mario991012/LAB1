using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

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


        public Dictionary<string, int> CaracteresDelArchivo = new Dictionary<string, int>();

        const int bufferLength = 1000;

        public int LecturaArchivo(string RutaOriginal, string[] NombreArchivo, string UbicacionAAlmacenarLZW)
        {
            CaracteresDelArchivo = CreacionDiccionario(RutaOriginal, NombreArchivo, UbicacionAAlmacenarLZW);//Este se tiene que escribir
            return 1;
        }

        public Dictionary<string, int> CreacionDiccionario(string RutaOriginal, string[] NombreArchivo, string UbicacionAAlmacenarLZW)
        {
            var limiteCaracteresUnicos = 0;
            var DiccionarioLZW = new Dictionary<string, int>();
            var contadorLlaves = 1;
            using (var stream = new FileStream(RutaOriginal, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var byteBuffer = new byte[bufferLength];
                    

                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        byteBuffer = reader.ReadBytes(bufferLength);
                        for (int i = 0; i < byteBuffer.Length; i++)
                        {
                            if (DiccionarioLZW.ContainsKey(Convert.ToString(Convert.ToChar(byteBuffer[i]))) == false)
                            {
                                DiccionarioLZW.Add(Convert.ToString(Convert.ToChar(byteBuffer[i])), contadorLlaves);
                                limiteCaracteresUnicos++;
                                contadorLlaves++;
                            }

                        }
                    }
                }
            }

            var LlaveAux = string.Empty;

            using (var stream = new FileStream(RutaOriginal, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    using (var streamWriter = new FileStream($"{UbicacionAAlmacenarLZW}/{NombreArchivo[0]}.LZW", FileMode.OpenOrCreate))
                    {
                        using (var writer = new BinaryWriter(streamWriter))
                        {
                            var byteBuffer = new byte[bufferLength];
                            //Para juntar caracteres y codificar mensaje
                            writer.Write(NombreArchivo[1]);
                            foreach (var item in DiccionarioLZW)
                            {
                                writer.Write($"{item.Key}|{item.Value}");
                            }
                            writer.Write("--");
                            var j = 0;
                            while (reader.BaseStream.Position != reader.BaseStream.Length)
                            {
                                var PosibleLlave = string.Empty;
                                byteBuffer = reader.ReadBytes(bufferLength);
                                for (int i = 0; i < byteBuffer.Length; i++)
                                {
                                    LlaveAux = Convert.ToString(Convert.ToChar(byteBuffer[i]));
                                    PosibleLlave = LlaveAux;
                                reinicio:
                                    if (DiccionarioLZW.ContainsKey(PosibleLlave) && (i < reader.BaseStream.Length - 1))
                                    {
                                        var next = Convert.ToChar(byteBuffer[i + 1]);
                                        PosibleLlave = $"{PosibleLlave}{next}";
                                        j++;
                                        i += j - 1;
                                        goto reinicio;
                                    }
                                    else
                                    {
                                        DiccionarioLZW.Add(PosibleLlave, contadorLlaves);
                                        contadorLlaves++;
                                        
                                        i += j - 1;
                                        j = 0;
                                        var ByteEscrito = Convert.ToByte(DiccionarioLZW[LlaveAux]);
                                        writer.Write(DiccionarioLZW[LlaveAux]);
                                    }

                                }
                            }
                }
            }
                }
            }
             
            return DiccionarioLZW;
        }

        static int j = 0;
        public void ConcatenarLlaves(ref Dictionary<string, int> DiccionarioParaLlaves, ref string LlaveAux, byte[] byteBuffer, int i, long ultimaposicion, string UbicacionAAlmacenarLZW)
        {
            using (var streamWriter = new FileStream(UbicacionAAlmacenarLZW, FileMode.OpenOrCreate))
            {
                using (var writer = new BinaryWriter(streamWriter))
                {
                    bucle:
                    var next = Convert.ToChar(byteBuffer[i + 1]);
                    var PosibleLlave = $"{LlaveAux}{next}";
                    if (DiccionarioParaLlaves.ContainsKey(PosibleLlave) && (i < ultimaposicion - 1))
                    {
                        ConcatenarLlaves(ref DiccionarioParaLlaves, ref PosibleLlave, byteBuffer, i + 1, ultimaposicion, UbicacionAAlmacenarLZW);

                        j++;

                    }
                    else
                    {
                        DiccionarioParaLlaves.Add(PosibleLlave, DiccionarioParaLlaves.Count + 1);
                        goto bucle;
                    }
                }
            }
        }
    }
}