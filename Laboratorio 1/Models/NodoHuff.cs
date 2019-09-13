using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Laboratorio_1.Models
{
    public class NodoHuff
    {
        public byte Dato { get; set; }
        public decimal Probabilidad { get; set; }
        public NodoHuff Izquierda { get; set; }
        public NodoHuff Derecha { get; set; }
        public NodoHuff Padre { get; set; }

        public NodoHuff (byte dato, decimal probabilidad)
        {
            Dato = dato;
            Probabilidad = probabilidad;
            Izquierda = null;
            Derecha = null;
            Padre = null;
        }
        public NodoHuff(decimal probabilidad)
        {
            Probabilidad = probabilidad;
            Izquierda = null;
            Derecha = null;
            Padre = null;
        }
        public bool Hoja()
        {
            if (Derecha == null && Izquierda == null) return true;
            else return false;
        }
    }
}