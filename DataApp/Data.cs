using System;
using ABB.Robotics.Controllers.Discovery;
using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.RapidDomain;
using System.IO;
using ABB.Robotics.Controllers.Configuration;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics.Eventing.Reader;
using System.Security.Claims;

namespace ControllerAPI
{
    internal class Data
    {
        static void Main(string[] args)
        {
            // Usada para volcar el valor del datos que se va a esciribir en el CSV 
            string valor;

            // Lista para almacenar los datos que todavia no se han parseado a CSV
            var datosToParse = new List<string>();

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

                            // La llave del diccionario sera un string con el nombre del tipo RAPID
                            string tipoNombre = data.RapidType;

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
                            // Escribe el encabezado del CSV basado en el tipo RAPID
                            writer.WriteLine(Parse.GenerarEncabezadoDesdeTipo(entry.Key));
                            foreach (var data in entry.Value)
                            {

                                if (entry.Key == "pos" && data.Value is Pos pos)
                                {
                                    valor = Parse.ParsePos(pos);
                                }
                                else if (entry.Key == "wobjdata" && data.Value is WobjData wobj)
                                {
                                    valor = Parse.ParseWobjData(wobj);
                                }
                                else if (entry.Key == "accdata" && data.Value is UserDefined accdata)
                                {
                                    valor = Parse.ParseAccData(accdata);
                                }
                                else
                                {
                                    valor = data.Value.ToString();
                                    datosToParse.Add(entry.Key);
                                }
                                
                                writer.WriteLine($"{data.Name};{valor}");
                            }
                        }
                    }
                }
            }

            // Si hay tipos de datos que no se han implementado, los añadimos al final del archivo
            if (datosToParse.Count > 0)
            {
                Console.WriteLine("Tipos de datos no implementados:");
                foreach (string tipoDato in datosToParse)
                {
                    Console.WriteLine($"Implementar {tipoDato}");
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
