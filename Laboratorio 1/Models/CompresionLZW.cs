using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;

namespace Laboratorio_1.Models
{
	public class CompresionLZW
	{
	    public void Compresion(string pathLectura, string pathEscritura)
        { 
            Diccionario_Inicial(pathLectura);
            Cantidad_Bits(pathLectura);
            Escribir_Diccionario(pathEscritura);
            Diccionario_Completo(pathLectura, pathEscritura);
        }
<<<<<<< HEAD
        private Dictionary<string, int> Tabla_Caracteres = new Dictionary<string, int>();
        private Dictionary<string, int> Tabla_Escritura = new Dictionary<string, int>();
        private const int bufferLenght = 500;
        int cantidad_bits { get; set; }

        private void Cantidad_Bits(string pathLectura)
        {
            var buffer = new byte[bufferLenght];
            string UltimoCaracter = "";
            using (var file = new FileStream(pathLectura, FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {

                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        buffer = reader.ReadBytes(bufferLenght);
                        foreach (var item in buffer)
                        {
                            var bytes = Convert.ToString(Convert.ToChar(item));
                            if (!Tabla_Caracteres.ContainsKey((UltimoCaracter + bytes)))
                            {
                                Tabla_Caracteres.Add(UltimoCaracter + bytes, (Tabla_Caracteres.Count + 1));
                                UltimoCaracter = bytes;

                            }

                            else
                            {
                                UltimoCaracter += bytes;
                            }
                        }

                    }
                    var numero = Tabla_Caracteres.Values.Max();
                    var bits = Convert.ToString(numero, 2);
                    cantidad_bits = bits.Length;

                }
            }
            //Escribir aca todo los numero en bytes al archivo de escritura
        }
        private void Diccionario_Inicial(string pathLectura)
        {
            var buffer = new byte[bufferLenght];
            //Se llena el diccionario con los valores iniciales
            using (var file = new FileStream(pathLectura, FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        buffer = reader.ReadBytes(bufferLenght);
                        foreach (var item in buffer)
                        {
                            var bytes = Convert.ToString(Convert.ToChar(item));
                            //13 /r  10 /n
                            if (!Tabla_Caracteres.ContainsKey(bytes) /*&& Convert.ToInt16(Convert.ToChar(item)) != 13*/)
                            {
                                if (Tabla_Caracteres.Count() == 0)
                                {
                                    Tabla_Caracteres.Add(bytes, 1);
                                    Tabla_Escritura.Add(bytes, 1);
                                }
                                else
                                {
                                    Tabla_Caracteres.Add(bytes, (Tabla_Caracteres.Count + 1));
                                    Tabla_Escritura.Add(bytes, (Tabla_Escritura.Count + 1));
                                }
                            }
                        }
                    }
                }

            }

            //Escribir aqui el diccionario al archivo
        }
=======
		private void Diccionario_Inicial(string pathLectura)
		{
			var buffer = new char[bufferLenght];	
			//Se llena el diccionario con los valores iniciales
			using (var file = new FileStream(pathLectura, FileMode.Open))
			{
				using (var reader = new BinaryReader(file))
				{
					while (reader.BaseStream.Position != reader.BaseStream.Length)
					{
						buffer = reader.ReadChars(bufferLenght);
						foreach (var item in buffer)
						{
							if (!Tabla_Caracteres.ContainsKey(Convert.ToString(item)) && Convert.ToInt16(item) != 13)
							{
								if (Tabla_Caracteres.Count() == 0)
									Tabla_Caracteres.Add(Convert.ToString(item), 1);
								else
									Tabla_Caracteres.Add(Convert.ToString(item), (Tabla_Caracteres.Count + 1));
							}
						}
					}
				}
			}
		}
>>>>>>> 5fd54198459c9d165fe0536ef373f8c7f1d38920
        private void Escribir_Diccionario(string pathEscritura)
        {
            char separador = '|';
            var escritura = new byte[bufferLenght];
<<<<<<< HEAD

            if (Tabla_Caracteres.Keys.Contains("|"))
            {
                separador = 'ÿ';
                if (Tabla_Caracteres.Keys.Contains("ÿ"))
                {
                    separador = 'ß';
                }
            }

=======
			if (Tabla_Caracteres.Keys.Contains("|"))
			{
				separador = 'ÿ';
				if (Tabla_Caracteres.Keys.Contains("ÿ"))
				{
					separador = 'ß';
				}
			}
>>>>>>> 5fd54198459c9d165fe0536ef373f8c7f1d38920
            using (var file = new FileStream(pathEscritura, FileMode.OpenOrCreate))
            {
                using (var writer = new BinaryWriter(file))
                {


                    //Escribe el caracter junto con su Frecuencia
                    foreach (KeyValuePair<string, int> Valores in Tabla_Caracteres)
                    {
                        escritura = Encoding.UTF8.GetBytes(Valores.Key.ToString().ToArray());//+ Valores.Value.ToString() + "|");
                        writer.Write(escritura);
                        escritura = Encoding.UTF8.GetBytes(Valores.Value.ToString().ToArray());//+ Valores.Value.ToString() + "|");
                        writer.Write(escritura);
                        writer.Write(Convert.ToByte(separador));
                    }
                    writer.Write(Convert.ToByte(separador));
                }

            }

        }
        private void Diccionario_Completo(string pathLectura, string pathEscritura)
<<<<<<< HEAD
        {
            var buffer = new byte[bufferLenght];
            var resultado = new List<int>();
            string UltimoCaracter = "";
            using (var file = new FileStream(pathLectura, FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        buffer = reader.ReadBytes(bufferLenght);
                        foreach (var item in buffer)
                        {

                            var bytes = Convert.ToString(Convert.ToChar(item));

                            if (!Tabla_Escritura.ContainsKey((UltimoCaracter + bytes)))
                            {
                                Tabla_Escritura.Add(UltimoCaracter + bytes, (Tabla_Escritura.Count + 1));
                                resultado.Add(Tabla_Escritura[UltimoCaracter]);
                                UltimoCaracter = bytes;
                                // agrega el numero                                         
                            }

                            else
                            {
                                UltimoCaracter += bytes;
                            }
                        }
                        resultado.Add(Tabla_Escritura[UltimoCaracter]);
                    }
                }
            }
            //Escribir aca todo los numero en bytes al archivo de escritura
        }
    }
=======
		{
			var buffer = new char[bufferLenght];
			var resultado = new List<int>();
			string UltimoCaracter = "";
			using (var file = new FileStream(pathLectura, FileMode.Open))
			{
				using (var reader = new BinaryReader(file))
				{
					while (reader.BaseStream.Position != reader.BaseStream.Length)
					{
						buffer = reader.ReadChars(bufferLenght);
						foreach (var item in buffer)
						{
							if (!Tabla_Caracteres.ContainsKey((UltimoCaracter + Convert.ToString(Convert.ToChar(item)))))
							{
								Tabla_Caracteres.Add((UltimoCaracter + Convert.ToString(Convert.ToChar(item))), (Tabla_Caracteres.Count + 1));
								resultado.Add(Tabla_Caracteres[UltimoCaracter]);
								UltimoCaracter = (UltimoCaracter + Convert.ToString(Convert.ToChar(item))).Substring(((UltimoCaracter + Convert.ToString(Convert.ToChar(item))).Length - 1), 1);
							}
							else
							{
								UltimoCaracter = (UltimoCaracter + Convert.ToString(Convert.ToChar(item)));
							}
						}
						resultado.Add(Tabla_Caracteres[UltimoCaracter]);
						int mayor = resultado.Max();
					}
				}
			}
		}
	}
>>>>>>> 5fd54198459c9d165fe0536ef373f8c7f1d38920
}