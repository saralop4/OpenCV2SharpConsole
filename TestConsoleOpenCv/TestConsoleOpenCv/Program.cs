using OpenCvSharp;

namespace TestConsoleOpenCv
{
    class Program
    {
        public static void Main(string[] args)
        {
            string pathVideo = GetValidVideoPath();
            if (string.IsNullOrWhiteSpace(pathVideo))
            {
                Console.WriteLine("No ingresó una ruta de video válida.");
                return;
            }

            string pathImage = GetValidPath();
            if (string.IsNullOrWhiteSpace(pathImage))
            {
                Console.WriteLine("No ingresó una ruta de imagen válida.");
                return;
            }

            string filename = GetValidFileName();
            if (string.IsNullOrWhiteSpace(filename))
            {
                Console.WriteLine("No ingresó un nombre de archivo válido.");
                return;
            }

            string format = GetValidFormat();
            if (string.IsNullOrWhiteSpace(format))
            {
                Console.WriteLine("No ingresó un formato válido.");
                return;
            }

            ProcessVideo(pathVideo, pathImage, filename, format);
        }

        private static void ProcessVideo(string pathVideo, string pathImage, string filename, string format)
        {

            //aqui se almacena la entrada de dato que recibimos del path
            using var videoCapture = new VideoCapture(pathVideo);

            //aqui comprobamos si la entrada de datos se esta leyendo de forma incorrecta
            if (!videoCapture.IsOpened())
            {
                Console.WriteLine("Error: No se pudo abrir el video.");
                return;
            }

            int imgIndex = 0;

            //aqui comprobamos si la entrada de datos se esta leyendo de forma correcta
            while (videoCapture.IsOpened())
            {
                using Mat frame = new Mat();
                if (videoCapture.Read(frame) && !frame.Empty())
                {
                    string fullFileName = Path.Combine(pathImage, $"{filename}_{imgIndex}.{format}");
                    Cv2.ImWrite(fullFileName, frame);
                    Console.WriteLine($"Imagen guardada como {filename+"_"+imgIndex}");

                    // Para mostrar el frame usando una ventana de OpenCV
                    Cv2.ImShow("Frame", frame);
                    int key = Cv2.WaitKey(1);
                    if (key == 27)
                        break;

                    imgIndex++;
                }
                else
                {
                    break;
                }
            }

            //liberamos la captura de video que hice de todos los frames y destruimos todas las ventanas
            videoCapture.Release();
            Cv2.DestroyAllWindows();
        }

        private static string GetValidVideoPath()
        {
            Console.WriteLine("Por favor, ingresa la ruta del archivo de video: ");
            return Console.ReadLine();
        }

        private static string GetValidPath()
        {
            Console.Write("Ingresa la ruta donde deseas guardar la imagen: ");
            string path = Console.ReadLine();

            //Path.GetDirectoryName(pathImage) devolvera solo el directorio aunque se escriba al final el nombre del archivo
            return !string.IsNullOrWhiteSpace(path) && Directory.Exists(path) && IsDirectoryWritable(path) ? path : null;
        }

        private static string GetValidFileName()
        {
            Console.Write("Ingresa el nombre del archivo para guardar la imagen (sin extensión): ");
            string filename = Console.ReadLine();

            // aqui Verificamos si el nombre del archivo contiene un punto
            return !string.IsNullOrWhiteSpace(filename) && !filename.Contains(".") ? filename : null;
        }

        private static string GetValidFormat()
        {
            Console.Write("Ingresa el formato de archivo (jpg, png, bmp): ");
            string format = Console.ReadLine()?.ToLower().Trim();
            HashSet<string> allowedFormats = new HashSet<string> { "jpg", "jpeg", "png", "bmp" };
            return !string.IsNullOrWhiteSpace(format) && allowedFormats.Contains(format) ? format : null;
        }

        private static bool IsDirectoryWritable(string dirPath)
        {
            try
            {
                using (FileStream fs = File.Create(Path.Combine(dirPath, Path.GetRandomFileName()), 1, FileOptions.DeleteOnClose))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
