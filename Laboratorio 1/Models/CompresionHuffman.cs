using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Laboratorio_1.Models
{
    public class CompresionHuffman
    {
        private const int bufferLenght = 500;
        private static NodoHuff Raiz { get; set; }
        private static Dictionary<char, int> Tabla_Frecuencias { get; set; }
        private static Dictionary<char,string> Tabla_Caracteres { get; set; }
		private static Dictionary<char, int> Tabla_Frecuencias2 { get; set; }
        private static decimal Cantidad_Datos;

        public void Compresion (string path_Lectura,string path_Escritura)
        {
            Tabla_Frecuencias = new Dictionary<char, int>();
            Tabla_Caracteres = new Dictionary<char, string>();
            ArbolHuffman(path_Lectura);
            Obtener_Codigos_Prefijo();
            Recorrido(path_Lectura, path_Escritura);
            Escribir_Valor_y_Frecuencia(path_Escritura);   
        }

        private static NodoHuff Unir_Nodos(NodoHuff Mayor, NodoHuff Menor)
        {
            NodoHuff Padre = new NodoHuff(Mayor.Probabilidad+Menor.Probabilidad);
            Padre.Izquierda = Mayor;
            Padre.Derecha = Menor;
            return Padre;
        }
        private static void ArbolHuffman(string path)
        {
            using (var File = new FileStream(path, FileMode.Open))
            {
                var buffer = new byte[bufferLenght];
                using (var reader = new BinaryReader(File))
                {
                    Cantidad_Datos = reader.BaseStream.Length;
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        buffer = reader.ReadBytes(bufferLenght);
                        foreach (var item in buffer)
                        {
							if (Tabla_Frecuencias.Keys.Contains(Convert.ToChar(item)))
							{
								Tabla_Frecuencias[Convert.ToChar(item)]++;
							}
							else Tabla_Frecuencias.Add(Convert.ToChar(Convert.ToChar(item)), 1);

                        }
                    }
                }
            }
            List<NodoHuff> Lista_Frecuencias = new List<NodoHuff>();
			//Tomar este codigo para hacer el arbol, porque lo primero que me da es la lista de frecuencias
            foreach(KeyValuePair<char,int> Nodos in Tabla_Frecuencias)
            {
				//(Dato,Probabilidad)
                Lista_Frecuencias.Add(new NodoHuff(Nodos.Key, (Convert.ToDecimal(Nodos.Value) / Cantidad_Datos)));
            }
			//Uniendo Nodos
            while (Lista_Frecuencias.Count >1)
            {
                Lista_Frecuencias = Lista_Frecuencias.OrderBy(x => x.Probabilidad).ToList();
                NodoHuff Union = Unir_Nodos(Lista_Frecuencias[1], Lista_Frecuencias[0]);
                Lista_Frecuencias.RemoveRange(0, 2);
                Lista_Frecuencias.Add(Union);
            }
            Raiz = Lista_Frecuencias[0];
           //Aqui el arbol ya esta terminado
        }
        private static void Codigos_Prefijo(NodoHuff Nodo, string recorrido)
        {
            if (Nodo.Hoja()) Tabla_Caracteres.Add(Nodo.Dato, recorrido);
            else
            {
                if (Nodo.Izquierda != null) Codigos_Prefijo(Nodo.Izquierda, recorrido + "0");
                if (Nodo.Derecha != null) Codigos_Prefijo(Nodo.Derecha, recorrido + "1");
            }
        }
		//LLena la tabla de caracteres con su ascii y su prefijo
        private static void Obtener_Codigos_Prefijo()
        {
            if (Raiz.Hoja()) Tabla_Caracteres.Add(Raiz.Dato, "1");
            else Codigos_Prefijo(Raiz, "");
        }
        private static void Recorrido(string path_Lectura, string path_Escritura)
        {
            string recorrido = "";
            using (var file = new FileStream(path_Escritura, FileMode.OpenOrCreate))
            {
                using (var File = new FileStream(path_Lectura, FileMode.Open))
                {

                    var buffer = new byte[bufferLenght];
                    var Bytes = new List<byte>();
                    using (var reader = new BinaryReader(File))
                    {
                        using (var writer = new BinaryWriter(file))
                        {
                            while (reader.BaseStream.Position != reader.BaseStream.Length)
                            {
                                buffer = reader.ReadBytes(bufferLenght);
								//Lee el archivo letra por letra
                                foreach (var item in buffer)
                                {
                                    recorrido += Tabla_Caracteres[Convert.ToChar(item)];
                                    if (recorrido.Length >= 8)
                                    {
                                        while (recorrido.Length > 8)
                                        {
                                            Bytes.Add(Convert.ToByte(recorrido.Substring(0, 8), 2));
											//Junta el codigo prefico en grupos de 8 en 8
                                            recorrido = recorrido.Remove(0, 8);
                                        }
                                    }
                                }
								//Escribe la lista de Bytes y se imprimen como ascci
                                writer.Write(Bytes.ToArray());
                                Bytes.Clear();
                            }
                            for (int i = recorrido.Length; i < 8; i++)
                            {
                                recorrido += "0";
                            }
                            writer.Write(Convert.ToByte(recorrido, 2));

                        }
                    }
                }
            }
        }

        private static void Escribir_Valor_y_Frecuencia(string path)
        {
            using (var file = new FileStream(path, FileMode.OpenOrCreate))
            {
                using (var writer = new StreamWriter(file))
                {
                    writer.Write(Cantidad_Datos.ToString() + "|");
					
					//Escribe el caracter junto con su Frecuencia
                    foreach (KeyValuePair<char, int> Valores in Tabla_Frecuencias)
                    {
                        writer.Write(Valores.Key.ToString() + Valores.Value.ToString() + "|");
                    }
                    writer.Write("|");
                }
            }
        }
    }
}