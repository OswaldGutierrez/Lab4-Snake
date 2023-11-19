using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SnakeJuegoWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly Dictionary<Cuadricula, ImageSource> gridValToImage = new()
        {
            {Cuadricula.Empty, Imagenes.Empty },
            {Cuadricula.Snake, Imagenes.Body },
            {Cuadricula.Food, Imagenes.Food }
        };

        private readonly int filas = 15, columnas = 15;
        private readonly Image[,] gridImages;
        private EstadoDeJuego estadoDeJuego;
        private bool ejecutandoJuego;

        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid();
            estadoDeJuego = new EstadoDeJuego(filas, columnas);
        }

        private async Task ejecutarJuego()
        {
            dibujar();
            Overlay.Visibility = Visibility.Hidden;
            await GameLoop();
        }

        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }

            if (!ejecutandoJuego)
            {
                ejecutandoJuego = true;
                await ejecutarJuego();
                ejecutandoJuego = false;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (estadoDeJuego.GameOver)
            {
                return;
            }

            switch(e.Key)
            {
                case Key.Left:
                    estadoDeJuego.ChangeDirection(Direccion.izquierda);
                    break;
                case Key.Right:
                    estadoDeJuego.ChangeDirection(Direccion.derecha);
                    break;
                case Key.Up:
                    estadoDeJuego.ChangeDirection(Direccion.arriba);
                    break;
                case Key.Down:
                    estadoDeJuego.ChangeDirection(Direccion.abajo);
                    break;
            }
        }

        private async Task GameLoop()
        {
            while (!estadoDeJuego.GameOver)
            {
                await Task.Delay(100);
                estadoDeJuego.Move();
                dibujar();
            }
        }


        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[filas, columnas];
            Gamegrid.Rows = filas;
            Gamegrid.Columns = columnas;

            for(int i = 0; i < filas; i++)
            {
                for(int j = 0; j < columnas; j++)
                {
                    Image image = new Image
                    {
                        Source = Imagenes.Empty
                    };

                    images[i, j] = image;
                    Gamegrid.Children.Add(image);
                }
            }

            return images;

        }

        private void dibujar()
        {
            dibujarCuadricula();
            ScoreText.Text = $"SCORE {estadoDeJuego.Puntaje}";
        }

        private void dibujarCuadricula()
        {
            for (int i = 0; i < filas; i++)
            {
                for(int j = 0;j < columnas; j++)
                {
                    Cuadricula gridVal = estadoDeJuego.Cuadriculita[i, j];
                    gridImages[i, j].Source = gridValToImage[gridVal];
                }
            }
        }
    }
}
