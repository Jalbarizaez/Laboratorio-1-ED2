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
    }
}