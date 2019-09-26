using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lab_1.Singleton
{
    public class LogicaLZW
    {
        const int bufferLength = 10000;

        //METODOS PARA COMPRIMIR
        static public int Comprimir(string RutaOriginal, string[] NombreArchivo, string UbicacionAAlmacenarLZW)
        {
            var diccionarioOriginal = new Dictionary<string, int>();
            var diccionarioTemporal = new Dictionary<string, int>();
            var indice = 1;
            var valorDiccionario = 0;
            //COMPLETADO
            diccionarioOriginal = ObtenerDiccionarioCaracteresEspeciales(RutaOriginal, ref indice, ref diccionarioTemporal);
            valorDiccionario = indice;

            //COMPLETADO
            //var cantidadMaximaDiccionario = ObtenerCantidadMax(diccionarioTemporal, ref indice, RutaOriginal, NombreArchivo, UbicacionAAlmacenarLZW);
            var cantidadMaximaDiccionario = 5;
            EscribirLZW(diccionarioOriginal, valorDiccionario, RutaOriginal, NombreArchivo, UbicacionAAlmacenarLZW, cantidadMaximaDiccionario);

            return 1;
        }

        static public Dictionary<string, int> ObtenerDiccionarioCaracteresEspeciales(string RutaOriginal, ref int indice, ref Dictionary<string, int> diccionarioTemporal)
        {
            var DiccionarioAEscribir = new Dictionary<string, int>();
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
                            if (!DiccionarioAEscribir.ContainsKey(Convert.ToString(Convert.ToChar(byteBuffer[i]))))
                            {
                                DiccionarioAEscribir.Add(Convert.ToString(Convert.ToChar(byteBuffer[i])), indice);
                                diccionarioTemporal.Add(Convert.ToString(Convert.ToChar(byteBuffer[i])), indice);
                                indice++;
                            }
                        }
                    }
                }
            }
            return DiccionarioAEscribir;
        }

        static public int ObtenerCantidadMax( Dictionary<string, int> caracteres, ref int indice, string RutaOriginal, string[] NombreArchivo, string UbicacionAAlmacenarLZW)
        {
            var D = new Dictionary<string, int>();
            D = caracteres;
            using (var stream = new FileStream(RutaOriginal, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var byteBuffer = new byte[bufferLength];
                    var PosibleLlave = string.Empty;
                    var posicionesALaDerecha = 0;

                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        byteBuffer = reader.ReadBytes(bufferLength);
                        for (int i = 0; i < byteBuffer.Length; i++)
                        {
                            PosibleLlave = Convert.ToString(Convert.ToChar(byteBuffer[i]));

                            while (D.ContainsKey(PosibleLlave) && i + posicionesALaDerecha < byteBuffer.Length-1)
                            {
                                posicionesALaDerecha++;
                                PosibleLlave = $"{PosibleLlave}{Convert.ToString(Convert.ToChar(byteBuffer[i + posicionesALaDerecha]))}";
                            }
                            if (i + 1 < byteBuffer.Length)
                            {
                                D.Add(PosibleLlave, indice);
                                indice++;
                                i += posicionesALaDerecha - 1;
                                posicionesALaDerecha = 0;

                            }
                        }
                    }
                    return D.Count;
                }
            }
        }

        static public void EscribirLZW(Dictionary<string, int> DiccionarioOriginal, int indice, string RutaOriginal, string[] NombreArchivo, string UbicacionAAlmacenarLZW, int cantidadMaximaDiccionario)
        {
            using (var stream = new FileStream(RutaOriginal, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    using (var streamWriter = new FileStream($"{UbicacionAAlmacenarLZW}/{NombreArchivo[0]}.lzw", FileMode.OpenOrCreate))
                    {
                        using (var writer = new BinaryWriter(streamWriter))
                        {
                            //var cantidadBytesBinario = Convert.ToString(cantidadMaximaDiccionario,2);
                            //var cantidadBytes = cantidadBytesBinario.Length / 8 + 1;
                            var byteBuffer = new byte[bufferLength];
                            var PosibleLlave = string.Empty;
                            var posicionesALaDerecha = 0;
                            writer.Write(NombreArchivo[1]);
                            foreach (var item in DiccionarioOriginal)
                            {
                                writer.Write($"{item.Key}|{item.Value}");
                            }
                            writer.Write("--");
                            var anterior = string.Empty;
                            var actual = string.Empty;
                            while (reader.BaseStream.Position != reader.BaseStream.Length)
                            {
                                byteBuffer = reader.ReadBytes(bufferLength);

                                


                                for (int i = 0; i < byteBuffer.Length; i++)
                                {
                                    PosibleLlave = Convert.ToString(Convert.ToChar(byteBuffer[i]));

                                    while (DiccionarioOriginal.ContainsKey(PosibleLlave) && i + posicionesALaDerecha <= byteBuffer.Length)
                                    {
                                        posicionesALaDerecha++;
                                        if (i+posicionesALaDerecha < byteBuffer.Length)
                                        {
                                            PosibleLlave = $"{PosibleLlave}{Convert.ToString(Convert.ToChar(byteBuffer[i + posicionesALaDerecha]))}";
                                        }
                                    }
                                    i += posicionesALaDerecha - 1;
                                    posicionesALaDerecha = 0;
                                    if (i + 1 >= byteBuffer.Length)//PARA EL ULTIMO CARACTER
                                    {
                                        //FUNCION MATEMATICA
                                        //var byteAEscribir = Convert.ToByte(DiccionarioOriginal[PosibleLlave]);
                                        var key = DiccionarioOriginal[PosibleLlave.Substring(0, PosibleLlave.Length - 1)];
                                        var claveAEscribir = Convert.ToString(key, 2);
                                        var sizeArray = claveAEscribir.Length / 8 + 1;
                                        if (claveAEscribir.Length < 9)
                                        {
                                            var byteAEscribirL = Convert.ToByte(DiccionarioOriginal[PosibleLlave.Substring(0, PosibleLlave.Length - 1)]);
                                            var Escribir1 = Convert.ToByte(1);
                                            writer.Write(Escribir1);
                                            writer.Write(byteAEscribirL);
                                            indice++;

                                        }
                                        else
                                        {
                                            var ByteArray = new byte[sizeArray];
                                            var SizeByte = Convert.ToByte(sizeArray);
                                            var contadorPosicion = sizeArray - 1;
                                            var LongitudBinario = claveAEscribir.Length;
                                            for (int m = LongitudBinario; m > 0; m -= 8)
                                            {
                                                if (claveAEscribir.Length > 8)
                                                {
                                                    var ByteAEscribirL = claveAEscribir.Substring(m - 8, 8);
                                                    var ByteAEscribir = Convert.ToByte(Convert.ToInt32(ByteAEscribirL, 2));
                                                    ByteArray[contadorPosicion] = ByteAEscribir;
                                                    claveAEscribir = claveAEscribir.Substring(0, m - 8);
                                                }
                                                else
                                                {
                                                    claveAEscribir = claveAEscribir.PadLeft(8, '0');
                                                    var ByteAEscribir = Convert.ToByte(Convert.ToInt32(claveAEscribir, 2));
                                                    ByteArray[contadorPosicion] = ByteAEscribir;
                                                }
                                                contadorPosicion--;
                                            }
                                            writer.Write(SizeByte);
                                            writer.Write(ByteArray);
                                            //for (int j = claveAEscribir.Length -1; j > 0; j-=8)
                                            //{
                                            //    StringArray[contadorString] = claveAEscribir.Substring(j-8, 8);
                                            //    if (StringArray[contadorString].Length < 8)
                                            //    {
                                            //        StringArray[contadorString] = StringArray[contadorString].PadLeft(8,'0');
                                            //    }
                                            //    contadorString--;
                                            //}
                                            //for (int m = 0; m < ByteArray.Length; m++)
                                            //{
                                            //    ByteArray[m] = Convert.ToByte(StringArray[m]);
                                            //}
                                            //writer.Write(sizeArray);
                                            //writer.Write(ByteArray);
                                        }
                                    }
                                    else
                                    {
                                        DiccionarioOriginal.Add(PosibleLlave, indice);
                                        //FUNCION MATEMATICA COMPLETADA
                                        var key = DiccionarioOriginal[PosibleLlave.Substring(0, PosibleLlave.Length - 1)];
                                        var claveAEscribir = Convert.ToString(key, 2);

                                        var sizeArray = claveAEscribir.Length / 8 + 1;
                                        
                                        if (claveAEscribir.Length < 9)
                                        {
                                            var byteAEscribirL = Convert.ToByte(DiccionarioOriginal[PosibleLlave.Substring(0, PosibleLlave.Length - 1)]);
                                            var Escribir1 = Convert.ToByte(1);
                                            writer.Write(Escribir1);
                                            writer.Write(byteAEscribirL);
                                            indice++;
                                            
                                        }
                                        else
                                        {
                                            var ByteArray = new byte[sizeArray];
                                            var SizeByte = Convert.ToByte(sizeArray);
                                            var contadorPosicion = sizeArray-1;
                                            var LongitudBinario = claveAEscribir.Length;
                                            for (int m = LongitudBinario; m > 0; m -=8)
                                            {
                                                if (claveAEscribir.Length > 8)
                                                {
                                                    var ByteAEscribirL = claveAEscribir.Substring(m-8,8);
                                                    var ByteAEscribir = Convert.ToByte(Convert.ToInt32(ByteAEscribirL,2));
                                                    ByteArray[contadorPosicion] = ByteAEscribir;
                                                    claveAEscribir = claveAEscribir.Substring(0, m - 8);
                                                }
                                                else
                                                {
                                                    claveAEscribir = claveAEscribir.PadLeft(8, '0');
                                                    var ByteAEscribir = Convert.ToByte(Convert.ToInt32(claveAEscribir,2));
                                                    ByteArray[contadorPosicion] = ByteAEscribir;
                                                }
                                                contadorPosicion--;
                                            }
                                            writer.Write(SizeByte);
                                            writer.Write(ByteArray);
                                            //for (int j = claveAEscribir.Length -1; j > 0; j-=8)
                                            //{
                                            //    StringArray[contadorString] = claveAEscribir.Substring(j-8, 8);
                                            //    if (StringArray[contadorString].Length < 8)
                                            //    {
                                            //        StringArray[contadorString] = StringArray[contadorString].PadLeft(8,'0');
                                            //    }
                                            //    contadorString--;
                                            //}
                                            //for (int m = 0; m < ByteArray.Length; m++)
                                            //{
                                            //    ByteArray[m] = Convert.ToByte(StringArray[m]);
                                            //}
                                            //writer.Write(sizeArray);
                                            //writer.Write(ByteArray);
                                        }

                                        //ESTO ESTA BUENO
                                        //var byteAEscribir = Convert.ToByte(DiccionarioOriginal[PosibleLlave.Substring(0, PosibleLlave.Length - 1)]);
                                        //writer.Write(byteAEscribir);
                                        //indice++;
                                        //i += posicionesALaDerecha - 1;
                                        //posicionesALaDerecha = 0;

                                    }
                                }
                            }

                            //if (cantidadBytesAUtilizar > 1)
                            //{

                            //}

                            //foreach (var caracter in caracteres)
                            //{
                            //    var cantidadBytes = (caracter.Value / 256) + 1;
                            //    byte[] bytes = new byte[cantidadBytes];
                            //    var contador = 0;
                            //    var binario = Convert.ToString(caracter.Value, 2);

                            //    for (int i = 0; i < binario.Length; i = i + 8)
                            //    {
                            //        var bin = binario.Substring(i, 8);
                            //        if (bin.Length < 8)
                            //        { bin = Convert.ToString(bin).PadLeft(8, '0'); }

                            //        bytes[contador] = Convert.ToByte(Convert.ToInt16(bin));
                            //        contador++;
                            //    }

                            //    writer.Write(cantidadBytes);
                            //    writer.Write(caracter.Key);
                            //    foreach(var bytesAEscribir in bytes)
                            //    {
                            //        writer.Write(bytesAEscribir);
                            //    }
                            //}
                        }
                    }
                }
            }
        }
        
        //METODOS PARA DESCOMPRIMIR
        public static int Descomprimir(string RutaOriginal, string[] nombreArchivo, string UbicacionAAlmacenarLZW)
        {
            var extension = ObtenerExtension(RutaOriginal);

            var CaracteresOriginales = ObtenerDiccionarioOriginal(RutaOriginal);

            CompletarDiccionarioYEscribir(CaracteresOriginales, RutaOriginal, UbicacionAAlmacenarLZW, nombreArchivo, extension);

            return 1;
        }

        public static string ObtenerExtension(string RutaOriginal)
        {
            var ext = string.Empty;
            using (var stream = new FileStream(RutaOriginal, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    ext = reader.ReadString();
                }
            }
            return ext;
        }

        static int ultimaPosicion = 0;
        public static Dictionary<int, string> ObtenerDiccionarioOriginal(string RutaOriginal)
        {
            var DiccionarioOriginal = new Dictionary<int, string>();
            using (var stream = new FileStream(RutaOriginal, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var byteBuffer = new byte[bufferLength];
                    var ExtensionLeida = false;
                    var ByteLeido = string.Empty;

                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        ByteLeido = reader.ReadString();
                        if (ExtensionLeida == false)
                        {
                            ExtensionLeida = true;
                        }
                        else if (ByteLeido != "--")
                        {
                            var separador = ByteLeido.Split('|');
                            DiccionarioOriginal.Add(Convert.ToInt32(separador[0]), separador[1]);
                        }
                        else
                        {
                            ultimaPosicion = Convert.ToInt32(reader.BaseStream.Position);
                            break;
                        }
                    }
                }
            }
            return DiccionarioOriginal;
        }

        public static void CompletarDiccionarioYEscribir(Dictionary<int, string> CaracteresOriginales, string RutaOriginal, string UbicacionAAlmacenarLZW, string[] nombreArchivo, string extension)
        {
            using (var stream = new FileStream(RutaOriginal, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    using (var streamwriter = new FileStream($"{UbicacionAAlmacenarLZW}//{nombreArchivo[0]}.{extension}", FileMode.OpenOrCreate))
                    {
                        using (var writer = new BinaryWriter(streamwriter))
                        {
                            var byteBuffer = new byte[bufferLength];
                            var indice = CaracteresOriginales.Count() + 1;
                            var PosibleLlave = 0;
                            var anterior = string.Empty;
                            var actual = string.Empty;
                            var valorAAgregar = string.Empty;

                            while (reader.BaseStream.Position != reader.BaseStream.Length)
                            {
                                byteBuffer = reader.ReadBytes(bufferLength);
                                var mensaje = string.Empty;
                                for (int i = ultimaPosicion; i < byteBuffer.Length; i++)
                                {
                                    PosibleLlave = Convert.ToInt32(byteBuffer[i]);

                                    if (i != ultimaPosicion)
                                    {
                                        valorAAgregar = $"{anterior}{CaracteresOriginales[PosibleLlave].Substring(0, 1)}";
                                        var bytes = Encoding.Default.GetBytes(CaracteresOriginales[PosibleLlave]);
                                        foreach (var byteC in bytes)
                                            writer.Write(byteC);
                                        CaracteresOriginales.Add(indice, valorAAgregar);
                                        anterior = CaracteresOriginales[PosibleLlave]; ;
                                        indice++;
                                    }
                                    else
                                    {
                                        valorAAgregar = CaracteresOriginales[PosibleLlave];
                                        writer.Write(Convert.ToByte(Convert.ToChar(valorAAgregar)));
                                        anterior = valorAAgregar;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

    }
}