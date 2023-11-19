using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeJuegoWPF
{
    internal class Posicion
    {
        public int Filas { get; }
        public int Columnas { get; }

        public Posicion(int filas, int columnas)
        {
            Filas = filas;
            Columnas = columnas;
        }

        public Posicion Translate(Direccion dir)
        {
            return new Posicion(Filas + dir.DesplazamientoFilas, Columnas + dir.DesplazamientoColumnas);
        }


        /* Sobrescribir Equals y GetHashCode:
         * Este proceso automatizado utiliza las propiedades de tu clase para generar las comparaciones de igualdad y los códigos hash de manera coherente.
         */
        public override bool Equals(object? obj)
        {
            return obj is Posicion posicion &&
                   Filas == posicion.Filas &&
                   Columnas == posicion.Columnas;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Filas, Columnas);
        }

        public static bool operator ==(Posicion? left, Posicion? right)
        {
            return EqualityComparer<Posicion>.Default.Equals(left, right);
        }

        public static bool operator !=(Posicion? left, Posicion? right)
        {
            return !(left == right);
        }
    }
}
