using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.Discovery;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace ControllerAPI
{
    internal sealed class App
    {
        public void Run()
        {

            using (var factory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            }))
            {
                ILogger<RapidExporter> logger = factory.CreateLogger<RapidExporter>();
                logger.LogInformation("Aplicación iniciada");

                // Busqueda de controladores
                var scanner = new NetworkScanner();
                var controllers = scanner.GetControllers(NetworkScannerSearchCriterias.Virtual);

                if (controllers.Length == 0)
                {
                    Console.WriteLine("No se encontraron controladores virtuales.");
                    return;
                }

                Console.WriteLine("Controladores disponibles:");
                Console.WriteLine("-----------------------------------------");
                for (int i = 0; i < controllers.Length; i++)
                {
                    var info = controllers[i];
                    Console.WriteLine($"[{i}] Nombre: {info.SystemName}, IP: {info.IPAddress}, Virtual: {info.IsVirtual}");
                }
                Console.WriteLine("-----------------------------------------");
                Console.Write("Seleccione el índice del controlador: ");

                int index;
                if (!int.TryParse(Console.ReadLine(), out index) || index < 0 || index >= controllers.Length)
                {
                    Console.WriteLine("Selección no válida.");
                    return;
                }

                // Conexion y operaciones
                var selectedController = controllers[index];
                using (var controller = Controller.Connect(selectedController.SystemId, ConnectionType.Standalone))
                {
                    controller.Logon(UserInfo.DefaultUser);
                    Console.WriteLine($"Conectado a: {controller.SystemName}");

                    string solutionDir = AppDomain.CurrentDomain.BaseDirectory;
                    string baseDir = Path.Combine(solutionDir, "ExportacionDatos " + controller.SystemName);
                    Directory.CreateDirectory(baseDir);

                    var exporter = new RapidExporter(logger);
                    exporter.ExportAll(controller, baseDir);
                    exporter.PrintNotImplemented();

                    controller.Logoff();
                }

                logger.LogInformation("Aplicación finalizada");
            }
        }
    }
}
