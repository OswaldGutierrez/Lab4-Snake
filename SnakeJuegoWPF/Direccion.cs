using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeJuegoWPF
{
    internal class Direccion
    {

        public readonly static Direccion izquierda = new Direccion(0, -1);
        public readonly static Direccion derecha = new Direccion(0, 1);
        public readonly static Direccion arriba = new Direccion(-1, 0);
        public readonly static Direccion abajo = new Direccion(1, 0);

        public int DesplazamientoFilas { get; }
        public int DesplazamientoColumnas {  get; }

        private Direccion(int desplazamientoFilas, int desplazamientoColumnas)
        {
            DesplazamientoFilas = desplazamientoFilas;
            DesplazamientoColumnas = desplazamientoColumnas;
        }

        public Direccion movOpuesto()
        {
            return new Direccion(-DesplazamientoFilas, -DesplazamientoColumnas);
        }


        /* Sobrescribir Equals y GetHashCode:
         * Este proceso automatizado utiliza las propiedades de tu clase para generar las comparaciones de igualdad y los códigos hash de manera coherente.
         */
        public override bool Equals(object? obj)
        {
            return obj is Direccion direccion &&
                   DesplazamientoFilas == direccion.DesplazamientoFilas &&
                   DesplazamientoColumnas == direccion.DesplazamientoColumnas;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DesplazamientoFilas, DesplazamientoColumnas);
        }

        public static bool operator ==(Direccion? left, Direccion? right)
        {
            return EqualityComparer<Direccion>.Default.Equals(left, right);
        }

        public static bool operator !=(Direccion? left, Direccion? right)
        {
            return !(left == right);
        }
    }
}
