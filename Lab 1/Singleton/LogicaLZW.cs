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
            var DiccionarioAux = new Dictionary<string, int>();
            var DiccionarioParaLlaves = new Dictionary<string, int>();
            using (var stream = new FileStream(RutaOriginal, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var byteBuffer = new byte[bufferLength];
                    var contadorLlaves = 1;
                    //Para caracteres solos
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        byteBuffer = reader.ReadBytes(bufferLength);
                        for (int i = 0; i < byteBuffer.Length; i++)
                        {
                            if (DiccionarioAux.ContainsKey(Convert.ToString(Convert.ToChar(byteBuffer[i]))) == false)
                            {
                                DiccionarioAux.Add(Convert.ToString(Convert.ToChar(byteBuffer[i])), contadorLlaves);
                                contadorLlaves++;
                            }

                        }
                    }
                }
            }

                    DiccionarioParaLlaves = DiccionarioAux;
                    var LlaveAux = string.Empty;
                    var LlaveConcatenada = string.Empty;
                    var TextoAEscribir = string.Empty;
                    var indicadorPosicion = 0;
            using (var stream = new FileStream(RutaOriginal, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var byteBuffer = new byte[bufferLength];
                    //Para juntar caracteres y codificar mensaje
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        byteBuffer = reader.ReadBytes(bufferLength);
                        for (int i = 0; i < byteBuffer.Length; i++)
                        {
                            if ((DiccionarioParaLlaves.ContainsKey(Convert.ToString(Convert.ToChar(byteBuffer[i]))) == true) &&(i < reader.BaseStream.Length-1))
                            {
                                LlaveAux = Convert.ToString(Convert.ToChar(byteBuffer[i]));
                                indicadorPosicion = i;
                                ConcatenarLlaves(ref DiccionarioParaLlaves, ref LlaveAux, byteBuffer, i);
                                i += j;
                                j = 0;
                            }

                        }
                    }
                }
            }
             
            return DiccionarioParaLlaves;
        }

        static int j = 0;
        public void ConcatenarLlaves(ref Dictionary<string, int> DiccionarioParaLlaves, ref string LlaveAux, byte[] byteBuffer, int i)
        {
            var next = Convert.ToChar(byteBuffer[i + 1]);
            var PosibleLlave = $"{LlaveAux}{next}";
            if (DiccionarioParaLlaves.ContainsKey(PosibleLlave) == false)
            {

                DiccionarioParaLlaves.Add(PosibleLlave, DiccionarioParaLlaves.Count+1);
            }
            else
            {
                ConcatenarLlaves(ref DiccionarioParaLlaves, ref PosibleLlave, byteBuffer, i+1);
                j++;
            }
        }
    }
}