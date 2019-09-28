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
        private static Dictionary<string, byte> Tabla_Caracteres { get; set; }
        private static Dictionary<byte, int> Tabla_Frecuencias { get; set; }
        public static int Tamaño_Datos { get; set; }
        public static decimal Cantidad_Datos;
        private static char separa = new char();

        public void Descompresion(string path_Escritura, string path_Lectura)
        {
            Tabla_Frecuencias = new Dictionary<byte, int>();
            Tabla_Caracteres = new Dictionary<string, byte>();
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
                byte bit = new byte();
                using (var reader = new BinaryReader(File))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        buffer = reader.ReadBytes(bufferLenght);
                        foreach (var item in buffer)
                        {

                            if (separador == 0)
                            {
                                if (Convert.ToChar(item) == '|' || Convert.ToChar(item) == 'ÿ' || Convert.ToChar(item) == 'ß')
                                {
                                    separador = 1;
                                    if (Convert.ToChar(item) == '|') { separa = '|'; }
                                    else if (Convert.ToChar(item) == 'ÿ') { separa = 'ÿ'; }
                                    else { separa = 'ß'; }
                                }
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
                                if (final == 1 && Convert.ToChar(item) == separa)
                                {
                                    final = 2;
                                    separador = 2;
                                }
                                else { final = 0; }

                                if (caracter == "") { caracter = Convert.ToChar(item).ToString(); bit = item; }
                                else if (Convert.ToChar(item) == separa && final == 0)
                                {
                                    Tabla_Frecuencias.Add(bit, Convert.ToInt32(frecuencia));
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
            foreach (KeyValuePair<byte, int> Nodos in Tabla_Frecuencias)
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
            if (Nodo.Hoja()) { Tabla_Caracteres.Add( recorrido, Nodo.Dato); return; }
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
            if (Raiz.Hoja()) Tabla_Caracteres.Add( "1", Raiz.Dato);
            else Codigos_Prefijo(Raiz, "");
        }
        private static void Recorrido(string path_Lectura, string path_Escritura)
        {
            int caracteres_escritos = 0;
            int i = 0;
            string validacion = "";
            int inicio = 0;
            string recorrido = "";
            List<byte> caracteres = new List<byte>();
            using (var file = new FileStream(path_Escritura, FileMode.OpenOrCreate))
            {
                using (var writer = new BinaryWriter(file))
                {

                    using (var File = new FileStream(path_Lectura, FileMode.Open))
                    {

                        var buffer = new byte[bufferLenght];

                        using (var reader = new BinaryReader(File))
                        {

                            while (reader.BaseStream.Position != reader.BaseStream.Length && caracteres_escritos < Cantidad_Datos)
                            {
                                buffer = reader.ReadBytes(bufferLenght);
                                foreach (var item in buffer)
                                {
                                    caracteres_escritos++;
                                    if (inicio == 0 && Convert.ToChar(item) == separa) { inicio = 1; }
                                    else if (inicio == 1 && Convert.ToChar(item) == separa) { inicio = 2; }
                                    else if (inicio == 2)
                                    {
                                        //Inicia descomprecion
                                        var bits = Convert.ToString(item, 2);
                                        var completo = bits.PadLeft(8, '0');
                                        recorrido += completo;
                                        var comparacion = recorrido.ToCharArray();
                                        i = 0;
                                        while (i < recorrido.Length)
                                        {
                                            validacion += comparacion[i];
                                            i++;
                                            if (Tabla_Caracteres.Keys.Contains(validacion))
                                            {
                                                i = 0;
                                                caracteres.Add(Tabla_Caracteres[validacion]);
                                                recorrido = recorrido.Remove(0, validacion.Length);
                                                comparacion = recorrido.ToCharArray();
                                                validacion = "";
                                            }
                                        }
                                        validacion = "";

                                    }
                                    else { inicio = 0; }
                                }

                                writer.Write(caracteres.ToArray());
                                caracteres.Clear();


                            }
                        }
                    }
                }
            }
        }
    }

}
