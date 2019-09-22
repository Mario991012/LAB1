using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        public int Comprimir(string RutaOriginal, string[] NombreArchivo, string UbicacionAAlmacenarLZW)
        {
            var caracteres = new Dictionary<string, int>();
            var indice = 1;
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
                            if (!caracteres.ContainsKey(Convert.ToString(Convert.ToChar(byteBuffer[i]))))
                            {
                                caracteres.Add(Convert.ToString(Convert.ToChar(byteBuffer[i])), indice);
                                indice++;
                            }
                        }
                    }
                }
            }

            

            using (var stream = new FileStream(RutaOriginal, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    using (var streamWriter = new FileStream($"{UbicacionAAlmacenarLZW}/{NombreArchivo[0]}.lzw", FileMode.OpenOrCreate))
                    {
                        using (var writer = new BinaryWriter(streamWriter))
                        {
                            writer.Write(NombreArchivo[1]);

                            foreach(var caracter in caracteres)
                            {
                                writer.Write($"{caracter.Value}|{caracter.Key}");
                            }
                            writer.Write("--");
                            var byteBuffer = new byte[bufferLength];
                            var PosibleLlave = string.Empty;
                            var posicionesALaDerecha = 0;

                            while (reader.BaseStream.Position != reader.BaseStream.Length)
                            {
                                byteBuffer = reader.ReadBytes(bufferLength);
                                for (int i = 0; i < byteBuffer.Length; i++)
                                {
                                    PosibleLlave = Convert.ToString(Convert.ToChar(byteBuffer[i]));

                                    while (caracteres.ContainsKey(PosibleLlave) && i + 1 < byteBuffer.Length)
                                    {
                                        posicionesALaDerecha++;
                                        PosibleLlave = $"{PosibleLlave}{Convert.ToString(Convert.ToChar(byteBuffer[i + posicionesALaDerecha]))}";
                                    }

                                    if (i + 1 >= byteBuffer.Length)
                                    {
                                        writer.Write(Convert.ToByte(caracteres[PosibleLlave]));
                                    }else
                                    {
                                        caracteres.Add(PosibleLlave, indice);
                                        indice++;
                                        i += posicionesALaDerecha - 1;
                                        posicionesALaDerecha = 0;
                                        var escrito = PosibleLlave.Substring(0, PosibleLlave.Length - 1);
                                        var write = Convert.ToByte(caracteres[escrito]);
                                        writer.Write(write);
                                    }

                                    

                                }
                            }
                        }
                    }
                }
            }

            return 1;
        }


        public int Descomprimir(string RutaOriginal, string[] nombreArchivo, string UbicacionAAlmacenarLZW)
        {
            var extension = "";
            var ContadorDeLecturas = 0;

            using (var stream = new FileStream(RutaOriginal, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    extension = reader.ReadString();
                }
            }

            using (var stream = new FileStream(RutaOriginal, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    using (var streamwriter = new FileStream($"{UbicacionAAlmacenarLZW}//{nombreArchivo[0]}.{extension}", FileMode.OpenOrCreate))
                    {
                        using (var writer = new BinaryWriter(streamwriter))
                        {
                            var byteBuffer = new byte[bufferLength];
                            var DiccionarioLeido = false;
                            var PosibleLlave = 0;
                            var caracteres = new Dictionary<int, string>();
                            var anterior = string.Empty;
                            var actual = string.Empty;
                            var ultimaPosicion = 0;
                            
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
                                    else if (ByteLeido != "--")
                                    {
                                        var separador = ByteLeido.Split('|');
                                        caracteres.Add(Convert.ToInt32(separador[0]), separador[1]);
                                    }
                                    else if (ByteLeido == "--")
                                    {
                                        ultimaPosicion = Convert.ToInt32(reader.BaseStream.Position + 1);
                                        break;
                                    }
                                }
                            }
                            var indice = caracteres.Count() + 1;

                            while (reader.BaseStream.Position != reader.BaseStream.Length)
                            {
                                byteBuffer = reader.ReadBytes(bufferLength);
                                var mensaje = string.Empty;
                                for (int i = 0; i < byteBuffer.Length; i++)
                                {
                                    PosibleLlave = Convert.ToInt32(byteBuffer[i]);

                                    if (i != 0)
                                    {
                                        mensaje = $"{anterior.Substring(anterior.Length - 1, 1)}{caracteres[PosibleLlave]}";
                                        caracteres.Add(indice, mensaje);
                                        anterior = caracteres[PosibleLlave];
                                        indice++;
                                    }
                                    else
                                    {
                                        mensaje = caracteres[PosibleLlave];
                                        writer.Write(mensaje);
                                        anterior = mensaje;
                                    }


                                }
                                
                            }
                        }
                    }
                }

            }
            
            return 1;
        }
    }
}