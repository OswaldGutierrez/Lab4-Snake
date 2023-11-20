using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SnakeJuegoWPF
{

    // Clase responsable para cargar las imágenes que representarán el juego.
    public static class Imagenes
    {
        public readonly static ImageSource Empty = cargarImagen("Empty.png");
        public readonly static ImageSource Body = cargarImagen("Body.png");
        public readonly static ImageSource Head = cargarImagen("Head.png");
        public readonly static ImageSource Food = cargarImagen("Food.png");
        public readonly static ImageSource DeadBody = cargarImagen("DeadBody.png");
        public readonly static ImageSource DeadHead = cargarImagen("DeadHead.png");


        // Método para cargar las imágenes
        private static ImageSource cargarImagen(string nombreArchivo)
        {
            return new BitmapImage(new Uri($"Assets/{nombreArchivo}", UriKind.Relative));
        }

    }
}
