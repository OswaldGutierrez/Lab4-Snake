using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace SnakeJuegoWPF
{
    internal class HistorialDePuntajes
    {
        private const string RutaArchivo = "historial.json";
        private const int MaximoPuntajes = 5;

        public List<Puntaje> Puntajes { get; private set; }

        public HistorialDePuntajes()
        {
            CargarHistorial();
        }

        private void CargarHistorial()
        {
            if (File.Exists(RutaArchivo))
            {
                string contenido = File.ReadAllText(RutaArchivo);
                Puntajes = JsonConvert.DeserializeObject<List<Puntaje>>(contenido);
            }
            else
            {
                Puntajes = new List<Puntaje>();
            }
        }

        public void AgregarPuntaje(Puntaje nuevoPuntaje)
        {
            Puntajes.Add(nuevoPuntaje);
            Puntajes = Puntajes.OrderByDescending(p => p.Puntuacion).Take(MaximoPuntajes).ToList();
            GuardarHistorial();
        }

        private void GuardarHistorial()
        {
            string contenido = JsonConvert.SerializeObject(Puntajes);
            File.WriteAllText(RutaArchivo, contenido);
        }
    }
}
