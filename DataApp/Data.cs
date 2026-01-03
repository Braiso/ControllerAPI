using System;
using ABB.Robotics.Controllers.Discovery;
using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.RapidDomain;
using System.IO;
using System.Collections.Generic;
using System.Linq;


namespace ControllerAPI
{
    internal class Data
    {
        static void Main2(string[] args)
        {
            
            string valor; // Usada para volcar el valor del datos que se va a esciribir en el CSV 
            double valorDouble; // Se usa para castear num y denum
            HashSet<string> datosToParse = new HashSet<string>(); // Lista para almacenar los datos que todavia no se han parseado a CSV
            NetworkScanner scanner = new NetworkScanner();  // Crea un escáner de red para detectar controladores disponibles (virtuales en este caso)
            ControllerInfo[] controllers = scanner.GetControllers(NetworkScannerSearchCriterias.Virtual); // Obtiene todos los controladores virtuales encontrados en la red

            // Verifica si se encontró al menos un controlador
            if (controllers.Length == 0)
            {
                Console.WriteLine("No se encontraron controladores virtuales.");
                return;
            }

            // Mostrar lista de controladores
            Console.WriteLine("Controladores disponibles:");
            Console.WriteLine("-----------------------------------------");

            for (int i = 0; i < controllers.Count(); i++)
            {
                ControllerInfo info = controllers[i];

                Console.WriteLine(
                    $"[{i}] Nombre: {info.SystemName}, " +
                    $"IP: {info.IPAddress}, " +
                    $"Virtual: {info.IsVirtual}"
                );
            }

            Console.WriteLine("-----------------------------------------");
            Console.Write("Seleccione el índice del controlador: ");


            // Leer selección del usuario
            if (!int.TryParse(Console.ReadLine(), out int index) ||
                index < 0 || index >= controllers.Count())
            {
                Console.WriteLine("Selección no válida.");
                return;
            }

            ControllerInfo selectedController = controllers[index];

            // Conectarse al controlador seleccionado
            Controller controller = Controller.Connect(
                selectedController.SystemId,
                ConnectionType.Standalone
            );

            Console.WriteLine($"Conectado a: {controller.SystemName}");

            // Inicia sesión en el controlador con el usuario por defecto
            controller.Logon(UserInfo.DefaultUser);

            // Directorio raíz para exportación en la carpeta de la solución
            string solutionDir = AppDomain.CurrentDomain.BaseDirectory;
            string baseDir = Path.Combine(solutionDir, $"ExportacionDatos {controller.SystemName}");
            Directory.CreateDirectory(baseDir);

            // Muestra las tareas RAPID disponibles en el controlador (como T_ROB1, T_ROB2, etc.)
            Console.WriteLine("\nTareas RAPID encontradas:");
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

                            // Filtrar datos tipo referencia: sin valor (triggdata, socketdev, etc.)
                            if (data.Value != null)
                            {
                                // La llave del diccionario sera un string con el nombre del tipo RAPID
                                string tipoNombre = data.RapidType;

                                // Si no hay aún una lista para ese tipo, se crea
                                if (!datosPorTipo.ContainsKey(tipoNombre))
                                    datosPorTipo[tipoNombre] = new List<RapidData>();

                                // Se añade instsancia de RapidData a la lista correspondiente al tipo
                                datosPorTipo[tipoNombre].Add(data);
                            }
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


                                // Los tipos de datos RAPID pueden ser diferentes de los .NET
                                string TipoNET = data.Value.GetType().Name;
                                // Console.WriteLine($"{entry.Key} -> {TipoNET}");

                                 switch (TipoNET)
                                {
                                    case "Num":
                                        // Se  necesita castear double para que cambie los "." por "," en numeros decimales
                                        valorDouble =(Num)data.Value;
                                        valor = valorDouble.ToString();
                                        break;
                                    case "Dnum":
                                        // Se  necesita castear double para que cambie los "." por "," en numeros decimales
                                        valorDouble = (Dnum)data.Value;
                                        valor = valorDouble.ToString();
                                        break;
                                    case "Bool":
                                        valor = data.Value.ToString();
                                        break;
                                    case "String":
                                        valor = data.Value.ToString();
                                        break;
                                    case "Pos":
                                        valor = Parse.ParsePos((Pos)data.Value);
                                        break;
                                    case "WobjData":
                                        valor = Parse.ParseWobjData((WobjData)data.Value);
                                        break;
                                    case "JointTarget":
                                        valor = Parse.ParseJointTarget((JointTarget)data.Value);
                                        break;
                                    case "RobTarget":
                                        valor = Parse.ParseRobTarget((RobTarget)data.Value);
                                        break;
                                    case "LoadData":
                                        valor = Parse.ParseLoadData((LoadData)data.Value);
                                        break;
                                    case "ToolData":
                                        valor = Parse.ParseToolData((ToolData)data.Value);
                                        break;
                                    case "ConfData":
                                        valor = Parse.ParseConfData((ConfData)data.Value);
                                        break;
                                    case "ExtJoint":
                                        valor = Parse.ParseExtJoint((ExtJoint)data.Value);
                                        break;
                                    case "RobJoint":
                                        valor = Parse.ParseRobJoint((RobJoint)data.Value);
                                        break;
                                    case "Orient":
                                        valor = Parse.ParseOrient((Orient)data.Value);
                                        break;
                                    case "Pose":
                                        valor = Parse.ParsePose((Pose)data.Value);
                                        break;
                                    case "UserDefined":
                                        // UserDefined
                                        switch (entry.Key)
                                        {
                                            case "accdata":
                                                valor = Parse.ParseAccData((UserDefined)data.Value);
                                                break;
                                            case "stoppointdata":
                                                valor = Parse.ParseStopPointData((UserDefined)data.Value);
                                                break;
                                            case "veldata":
                                                valor = Parse.ParseVelData((UserDefined)data.Value);
                                                break;
                                            case "zonedata":
                                                valor = Parse.ParseZoneData((UserDefined)data.Value);
                                                break;
                                            case "speeddata":
                                                valor = Parse.ParseSpeedData((UserDefined)data.Value);
                                                break;
                                            default:
                                                // Tipos RAPID sin implementar
                                                valor = data.Value.ToString();
                                                datosToParse.Add(entry.Key);
                                                break;
                                        }
                                        break;
                                    default:
                                        // Tipos .NET sin implementar (ArrayData)
                                        valor = data.Value.ToString();
                                        datosToParse.Add(entry.Key);
                                        break;

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
                Console.WriteLine("\nTipos de datos no implementados:");
                foreach (string tipoDato in datosToParse)
                {
                    Console.WriteLine($"- {tipoDato}");
                }
            }

            // Cierra la sesión del controlador
            controller.Logoff();

        }
    }
}
