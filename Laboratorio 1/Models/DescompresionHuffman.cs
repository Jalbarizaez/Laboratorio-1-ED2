using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Laboratorio_1.Models
{
    public class DescompresionHuffman
    {
        public const int bufferLenght = 500;
        private static NodoHuff Raiz { get; set; }
        private static Dictionary<char, string> Tabla_Caracteres { get; set; }
        private static Dictionary<char, int> Tabla_Frecuencias { get; set; }
        public static int Tamaño_Datos { get; set; }
        public static decimal Cantidad_Datos;

        public void Descompresion(string path_Escritura, string path_Lectura)
        {
            Tabla_Frecuencias = new Dictionary<char, int>();
            Tabla_Caracteres = new Dictionary<char, string>();
            ArbolHuffman(path_Lectura);
            Obtener_Codigos_Prefijo();
            Recorrido(path_Lectura, path_Escritura);


        }
        static void ArbolHuffman(string path)
        {


            using (var File = new FileStream(path, FileMode.Open))
            {
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

                            if (separador == 0)
                            {
                                if (Convert.ToChar(item) == '|') { separador = 1; }
                                else
                                {
                                    cantidad_datos += Convert.ToChar(item).ToString();
                                }
                            }
                            else if (separador == 2)
                            {
                                break;
                            }
                            else
                            {
                                if (final == 1 && Convert.ToChar(item) == '|')
                                {
                                    final = 2;
                                    separador = 2;
                                }
                                else { final = 0; }

                                if (caracter == "") { caracter = Convert.ToChar(item).ToString(); }
                                else if (Convert.ToChar(item) == '|' && final == 0)
                                {
                                    Tabla_Frecuencias.Add(Convert.ToChar(caracter), Convert.ToInt32(frecuencia));
                                    caracter = "";
                                    frecuencia = "";
                                    final = 1;
                                }
                                else { frecuencia += Convert.ToChar(item).ToString(); }
                            }

                        }
                    }
                }
                Cantidad_Datos = Convert.ToDecimal(cantidad_datos);
            }
            List<NodoHuff> Lista_Frecuencias = new List<NodoHuff>();
            foreach (KeyValuePair<char, int> Nodos in Tabla_Frecuencias)
            {
                Lista_Frecuencias.Add(new NodoHuff(Nodos.Key, (Convert.ToDecimal(Nodos.Value) / Cantidad_Datos)));
            }

            Lista_Frecuencias = Lista_Frecuencias.OrderBy(x => x.Probabilidad).ToList();
            while (Lista_Frecuencias.Count > 1)
            {

                Lista_Frecuencias = Lista_Frecuencias.OrderBy(x => x.Probabilidad).ToList();
                NodoHuff Union = Unir_Nodos(Lista_Frecuencias[1], Lista_Frecuencias[0]);
                Lista_Frecuencias.RemoveRange(0, 2);
                Lista_Frecuencias.Add(Union);
            }
            Raiz = Lista_Frecuencias[0];
        }
        private static void Codigos_Prefijo(NodoHuff Nodo, string recorrido)
        {
            if (Nodo.Hoja()) { Tabla_Caracteres.Add(Nodo.Dato, recorrido); return; }
            else
            {
                if (Nodo.Izquierda != null) Codigos_Prefijo(Nodo.Izquierda, recorrido + "0");
                if (Nodo.Derecha != null) Codigos_Prefijo(Nodo.Derecha, recorrido + "1");
            }
        }
        public static NodoHuff Unir_Nodos(NodoHuff Mayor, NodoHuff Menor)
        {
            NodoHuff Padre = new NodoHuff(Mayor.Probabilidad + Menor.Probabilidad);
            Padre.Izquierda = Mayor;
            Padre.Derecha = Menor;
            return Padre;
        }
        private static void Obtener_Codigos_Prefijo()
        {
            if (Raiz.Hoja()) Tabla_Caracteres.Add(Raiz.Dato, "1");
            else Codigos_Prefijo(Raiz, "");
        }
        private static void Recorrido(string path_Lectura, string path_Escritura)
        {

        }
    }
}