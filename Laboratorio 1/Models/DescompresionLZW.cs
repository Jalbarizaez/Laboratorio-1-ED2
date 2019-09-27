using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Laboratorio_1.Models
{
    public class DescompresionLZW
    {
        private Dictionary<string, int> Tabla_Caracteres = new Dictionary<string, int>();
        private const int bufferLenght = 750;
        int cantidad_bits { get; set; }
        private static char separa = new char();

        public void Descompresion(string pathEscritura, string pathLectura)
        {
            DiccionarioLzw(pathLectura);
            Escritura(pathLectura, pathEscritura);
        }

        public void DiccionarioLzw(string path)
        {
            using (var File = new FileStream(path, FileMode.Open))
            {
                byte bit = new byte();
                int separador = 0;
                var buffer = new byte[bufferLenght];
                string cantidad_datos = "";
                string frecuencia = "";
                string caracter = "";
                int final = 0;
                using (var reader = new BinaryReader(File))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        buffer = reader.ReadBytes(bufferLenght);
                        foreach (var item in buffer)
                        {
                            var charr = Convert.ToChar(item);
                            if (separador == 0)
                            {
                                if (charr == '|' || charr == 'ÿ' || charr == 'ß')
                                {
                                    separador = 1;
                                    if (charr == '|') { separa = '|'; }
                                    else if (charr == 'ÿ') { separa = 'ÿ'; }
                                    else { separa = 'ß'; }
                                }
                                else
                                {
                                    cantidad_datos += charr.ToString();
                                }
                            }
                            else if (separador == 2)
                            {
                                break;
                            }
                            else
                            {
                                if (final == 1 && charr == separa)
                                {
                                    final = 2;
                                    separador = 2;
                                }
                                else { final = 0; }

                                if (caracter == "") { caracter = charr.ToString(); bit = item; }
                                else if (charr == separa && final == 0)
                                {
                                    Tabla_Caracteres.Add(Convert.ToChar(bit).ToString(), Convert.ToInt32(frecuencia));
                                    caracter = "";
                                    frecuencia = "";
                                    final = 1;
                                }
                                else { frecuencia += charr.ToString(); }
                            }
                        }
                    }
                }
                cantidad_bits = Convert.ToInt32(cantidad_datos);
            }
            var x = Tabla_Caracteres;
        }
        private void Escritura(string path_Lectura, string path_Escritura)
        {
            var buffer = new byte[bufferLenght];
            string previo = "";
            string actual = "";
            string nuevo = "";
            List<int> recorre = new List<int>();
            int inicio = 0;
            string recorrido = "";
            List<byte> caracteres = new List<byte>();
            using (var file = new FileStream(path_Escritura, FileMode.OpenOrCreate))
            {
                using (var writer = new BinaryWriter(file))
                {

                    using (var File = new FileStream(path_Lectura, FileMode.Open))
                    {



                        using (var reader = new BinaryReader(File))
                        {

                            while (reader.BaseStream.Position != reader.BaseStream.Length)
                            {
                                buffer = reader.ReadBytes(bufferLenght);
                                foreach (var item in buffer)
                                {

                                    if (inicio == 0 && Convert.ToChar(item) == separa) { inicio = 1; }
                                    else if (inicio == 1 && Convert.ToChar(item) == separa) { inicio = 2; }
                                    else if (inicio == 2)
                                    {

                                        //Inicia descomprecion
                                        var bits = Convert.ToString(item, 2);
                                        var completo = bits.PadLeft(8, '0');
                                        recorrido += completo;
                                        if (recorrido.Length >= cantidad_bits)
                                        {
                                            while (recorrido.Length > cantidad_bits)
                                            {
                                                var numero = recorrido.Substring(0, cantidad_bits);
                                                var convertir = Convert.ToInt32(numero, 2);
                                                //recorre.Add(convertir);
                                                //i++;
                                                //Junta el codigo prefico en grupos de 8 en 8
                                                recorrido = recorrido.Remove(0, cantidad_bits);
                                                if (Tabla_Caracteres.ContainsKey((convertir)))
                                                {
                                                    previo = actual;
                                                    actual = Tabla_Caracteres[convertir];
                                                    nuevo = previo + actual.Substring(0, 1);
                                                    if (!Tabla_Caracteres.ContainsValue((nuevo)))

                                                    {
                                                        Tabla_Caracteres.Add(Tabla_Caracteres.Count + 1, nuevo);
                                                    }
                                                    writer.Write(Encoding.UTF8.GetBytes(actual.ToArray()));
                                                }
                                                else
                                                {
                                                    previo = actual;
                                                    nuevo = previo + actual.Substring(0, 1);

                                                    if (!Tabla_Caracteres.ContainsValue((nuevo)))

                                                    {
                                                        Tabla_Caracteres.Add(Tabla_Caracteres.Count + 1, nuevo);
                                                    }
                                                    if (convertir != 0)
                                                    {
                                                        actual = Tabla_Caracteres[convertir];
                                                    }

                                                    writer.Write(Encoding.UTF8.GetBytes(actual.ToArray()));

                                                }





                                            }
                                        }

                                    }
                                    else { inicio = 0; }
                                }

                                //writer.Write(caracteres.ToArray());
                                // caracteres.Clear();


                            }
                        }
                    }
                }
            }
        }
    }
}