using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Laboratorio_1.Models
{
	public class CompresionLZW
	{
		private static Dictionary<int, string> Tabla_Caracteres { get; set; }
		private const int bufferLenght = 500;

		public void Compresion(string path)
		{
			Tabla_Caracteres = new Dictionary<int, string>();
		}

		private void CrearDiccionario(string path)
		{
			var buffer = new char[bufferLenght];
			
			//Se llena el diccionario con los valores iniciales
			using (var file = new FileStream(path, FileMode.Open))
			{
				using (var reader = new BinaryReader(file))
				{
					while (reader.BaseStream.Position != reader.BaseStream.Length)
					{
						buffer = reader.ReadChars(bufferLenght);
						foreach (var item in buffer)
						{
							//13 /r  10 /n
							if (!Tabla_Caracteres.ContainsValue(Convert.ToString(item)))
							{
								if (Tabla_Caracteres.Count() == 0)
									Tabla_Caracteres.Add(1, Convert.ToString(item));
								else
									Tabla_Caracteres.Add((Tabla_Caracteres.Count + 1), Convert.ToString(item));
							}
						}
					}
				}
			}

			var resultado = new List<int>();

			//Se llena el diccionario por completo, no tocar ahorita ando tratando de que funcione 

		}

	}
}