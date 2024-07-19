using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.AccessControl;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoIntroAsync
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
 
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;

            var sw = new Stopwatch();
            sw.Start();


            var directorioActual = AppDomain.CurrentDomain.BaseDirectory;
            var destinoBaseSecuencial = Path.Combine(directorioActual,  @"Imagenes\resultado-secuencial");
            var destinoBaseParelelo = Path.Combine(directorioActual, @"Imagenes\resultado-paralelo");
            PrepararEjecucion(destinoBaseParelelo, destinoBaseSecuencial);


            Console.WriteLine("inicio");
            List<Imagen> imagenes = ObtenerImagenes();

            sw.Reset();
            sw.Start();

            var tareasEnumerable = imagenes.Select(async imagen =>
            {
                await ProcesarImagen(destinoBaseParelelo, imagen);
            });

            await Task.WhenAll(tareasEnumerable);

            Console.WriteLine("Paralelo - duracion en segundos: {0}",
                sw.ElapsedMilliseconds / 1000.00);

            sw.Stop();
            //var tareas = new List<Task>()
            //{
            //    RealizarProcesamientoLargoA(),
            //    RealizarProcesamientoLargoB(),
            //    RealizarProcesamientoLargoC()
            //};

            //await Task.WhenAll(tareas);


            //await RealizarProcesamientoLargoA();
            //await RealizarProcesamientoLargoB();
            //await RealizarProcesamientoLargoC();


            sw.Stop();

            //var duracion = $"El programa se ejecuto en {sw.ElapsedMilliseconds / 1000.0} segundos";
            //Console.WriteLine(duracion);

            pictureBox1.Visible = false;

              
        }

        private static List<Imagen> ObtenerImagenes()
        {
            var imagenes = new List<Imagen>();

            for (int i = 0; i < 7; i++)
            {
                imagenes.Add(

                    new Imagen()
                    {
                        Nombre = $"Cacicazgos {i}.png",
                        URL = "https://i.pinimg.com/originals/f5/04/24/f504246d7f9ffe6aa412a15d7bc33fb2.jpg"
                    });
                imagenes.Add(
                      new Imagen()
                      {
                          Nombre = $"Desangles {i}.png",
                          URL = "https://i.pinimg.com/564x/ac/19/40/ac1940b48394f38642ca9ddf7d467f88.jpg"
                      });

                imagenes.Add(
                    new Imagen()
                    {
                        Nombre = $"Alcazar {i}.jpg",
                        URL = "https://tiermaker.com/images/chart/chart/memes-de-juan-1250021/whatsapp-image-2021-09-15-at-134016jpeg.png"
                    });
            }
            return imagenes;

        }
        private void BorrarArchivos(string directorio)
        {
            var archivos = Directory.EnumerateFiles(directorio);
            foreach (var archivo in archivos)
            {
                File.Delete(archivo);
            }
        }

        private void PrepararEjecucion(string destinoParalelo,
            string destinoBaseSecuencial)
        {
            if (!Directory.Exists(destinoParalelo))
            {
                Directory.CreateDirectory(destinoParalelo);
            }
            if (!Directory.Exists(destinoBaseSecuencial))
            {
                Directory.CreateDirectory(destinoBaseSecuencial);
            }

            BorrarArchivos(destinoBaseSecuencial);
            BorrarArchivos(destinoParalelo);
        }


        private async Task ProcesarImagen(string directorio, Imagen imagen)
        {
            using (var httpClient = new HttpClient())
            {
                var respuesta = await httpClient.GetAsync(imagen.URL);

                if (respuesta.IsSuccessStatusCode)
                {
                    var contenido = await respuesta.Content.ReadAsByteArrayAsync();

                    using (var ms = new MemoryStream(contenido))
                    {
                        using (var bitmap = new Bitmap(ms))
                        {
                            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            var destino = Path.Combine(directorio, imagen.Nombre);
                            bitmap.Save(destino);
                        }
                    }
                }
            }
        }


        private async Task<String> ProcesamientoLargo()
        {
            await Task.Delay(5000);
            return "Juan";
        }

        private async Task RealizarProcesamientoLargoA()
        {
            await Task.Delay(1000);
            Console.WriteLine("Proceso A finalizado");

        }

        private async Task RealizarProcesamientoLargoB()
        {
            await Task.Delay(1000);
            Console.WriteLine("Proceso B finalizado");
        }

        private async Task RealizarProcesamientoLargoC()
        {
            await Task.Delay(1000);
            Console.WriteLine("Proceso C finalizado");
        }
    }
}
