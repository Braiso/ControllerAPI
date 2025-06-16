using System;
using ABB.Robotics.Controllers.Discovery;
using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.RapidDomain;
using System.IO;
using ABB.Robotics.Controllers.Configuration;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics.Eventing.Reader;

namespace ControllerAPI
{
    internal class Data
    {
        static void Main(string[] args)
        {
            // Crea un escáner de red para detectar controladores disponibles (virtuales en este caso)
            NetworkScanner scanner = new NetworkScanner();

            // Obtiene todos los controladores virtuales encontrados en la red
            ControllerInfo[] controllers = scanner.GetControllers(NetworkScannerSearchCriterias.Virtual);

            // Verifica si se encontró al menos un controlador
            if (controllers.Length == 0)
            {
                Console.WriteLine("No se encontraron controladores virtuales.");
                return;
            }

            // Se conecta al primer controlador virtual encontrado, en modo Standalone
            Controller controller = Controller.Connect(controllers[0].SystemId, ConnectionType.Standalone);

            // Inicia sesión en el controlador con el usuario por defecto
            controller.Logon(UserInfo.DefaultUser);

            // Directorio raíz para exportación en la carpeta de la solución
            string solutionDir = AppDomain.CurrentDomain.BaseDirectory;
            string baseDir = Path.Combine(solutionDir, $"ExportacionDatos {controller.SystemName}");
            Directory.CreateDirectory(baseDir);

            // Muestra las tareas RAPID disponibles en el controlador (como T_ROB1, T_ROB2, etc.)
            Console.WriteLine("Tareas RAPID encontradas:");
            foreach (Task task in controller.Rapid.GetTasks())
            {
                Console.WriteLine($"- {task.Name}");

                // Crea un directorio para cada tarea
                string taskDir = Path.Combine(baseDir, task.Name);
                Directory.CreateDirectory(taskDir);

                // Por cada tarea, mostramos sus módulos
                ABB.Robotics.Controllers.RapidDomain.Module[] modules = task.GetModules();
                foreach (ABB.Robotics.Controllers.RapidDomain.Module module in modules)
                {
                    Console.WriteLine($"    Módulo: {module.Name}");

                    // Usa búsqueda de símbolos para obtener datos RAPID de forma segura
                    RapidSymbolSearchProperties sProp = RapidSymbolSearchProperties.CreateDefaultForData();
                    sProp.InUse = false; // Todos los simbolos aunque no estén en uso
                    sProp.Recursive = false; // No buscar recursivamente en procedimientos y funciones

                    RapidSymbol[] symbols = module.SearchRapidSymbol(sProp);

                    // Diccionario para agrupar datos por tipo, para luego escribirlos en archivos separados por tipo
                    var datosPorTipo = new Dictionary<string, List<RapidData>>();


                    // Itera sobre los símbolos encontrados y extrae los valores de datos RAPID para organizarlos por tipo
                    foreach (RapidSymbol symbol in symbols)
                    {
                        try
                        {
                            // Obtiene el objeto RapidData asociado al símbolo
                            RapidData data = module.GetRapidData(symbol);

                            // Obtiene el tipo RAPID declarado (como "num", "robtarget", "tooldata"...)
                            // Si no hay aún una lista para ese RapidData, se crea
                            Type tipoValor = data.Value?.GetType();
                            if (tipoValor == null) continue;

                            // La llave del diccionario sera el nombre del tipo RAPID
                            string tipoNombre = tipoValor.Name;

                            // Si no hay aún una lista para ese tipo, se crea
                            if (!datosPorTipo.ContainsKey(tipoNombre))
                                datosPorTipo[tipoNombre] = new List<RapidData>();

                            // Se añade instsancia de RapidData a la lista correspondiente al tipo
                            datosPorTipo[tipoNombre].Add(data);
                        }
                        catch (Exception ex)
                        {
                            // Manejo de excepciones para evitar que un error detenga la exportación
                            Console.WriteLine($"{symbol.Name},Error: {ex.Message}");
                        }
                    }

                    // Escribe un archivo CSV por tipo de dato
                    foreach (var entry in datosPorTipo)
                    {

                        string tipoArchivo = Path.Combine(taskDir, module.Name + $"_{entry.Key}.csv");
                        using (StreamWriter writer = new StreamWriter(tipoArchivo))
                        {
                            writer.WriteLine(Parse.GenerarEncabezadoDesdeTipo(entry.Key));
                            foreach (var data in entry.Value)
                            {
                                string valor = "<sin valor>";

                                if (entry.Key == "Pos")
                                {
                                     valor = Parse.ParseRapidData(data);
                                }   
                                
                                writer.WriteLine($"{data.Name};{valor}");
                            }
                        }
                    }
                   
                }
            }

            // Cierra la sesión del controlador
            controller.Logoff();

            // Espera entrada del usuario para terminar
            //Console.WriteLine("Presiona una tecla para salir");
            //Console.ReadKey(intercept:true);
        }
    }
}
