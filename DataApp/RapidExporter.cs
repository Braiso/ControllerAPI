using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.RapidDomain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;

namespace ControllerAPI
{
    /// <summary>
    /// Encapsula la lógica de exportación de datos RAPID desde un controlador ABB
    /// a ficheros CSV.
    /// </summary>
    internal sealed class RapidExporter
    {
        private readonly ILogger _logger;
        private readonly HashSet<string> _notImplemented = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
               
        public RapidExporter(ILogger<RapidExporter> logger = null)
        {
            _logger = logger ?? NullLogger<RapidExporter>.Instance;
        }

        /// <summary>
        /// Exporta todos los datos RAPID del controlador a ficheros CSV.
        /// Recorre todas las tareas y módulos y genera un archivo por tipo de dato.
        /// </summary>
        /// <param name="controller">Controlador ABB conectado.</param>
        /// <param name="baseDir">Directorio base donde se crearán los CSV.</param>
        public void ExportAll(Controller controller, string baseDir)
        {

            // Se recorren los modulos de cada tarea, exportando los datos en el contenidos
            foreach (Task task in controller.Rapid.GetTasks())
            {
                _logger.LogInformation($"Procesando Tarea: {task}");

                // Por cada tarea se establece un directorio donde se guardaran 
                // los CSV correspondientes a cada tarea
                string taskDir = Path.Combine(baseDir, task.Name);
                Directory.CreateDirectory(taskDir);

                // Se exporta cada modulo por separado
                foreach (Module module in task.GetModules())
                {
                    _logger.LogInformation($"Procesando Modulo: {module}");
                    ExportModule(module, taskDir);
                }
            }
        }
        /// <summary>
        /// Muestra por consola la lista de tipos RAPID encontrados
        /// que no tienen implementación de exportación.
        /// </summary>
        public void PrintNotImplemented()
        {
            if (_notImplemented.Count == 0) return;

            Console.WriteLine("\nTipos de datos no implementados:");
            foreach (var t in _notImplemented) Console.WriteLine("- " + t);
        }
        /// <summary>
        /// Exporta todos los datos RAPID de un módulo concreto a ficheros CSV.
        /// La conversión de valores RAPID a CSV se delega a <see cref="TryFormatValue"/>.
        /// Los tipos no implementados se registran para su posterior notificación.
        /// </summary>
        /// <param name="module">Modulo con datos.</param>
        /// <param name="taskDir">Tarea a la que pertenece.</param>
        private void ExportModule(Module module, string taskDir)
        {

            // Propiedades de busqueda
            var sProp = RapidSymbolSearchProperties.CreateDefaultForData();
            // Include symbols in use.
            sProp.InUse = false;
            // Search recursive. Solo en datos declarados en los modulos
            sProp.Recursive = false;
            // Busqueda de simbolos RAPID
            RapidSymbol[] symbols = module.SearchRapidSymbol(sProp);

            // Se crea un diccionario que agrupa todas las variables RAPID (RapidData) por su tipo,
            // usando el nombre del tipo como clave y sin distinguir mayúsculas/minúsculas.
            var datosPorTipo = new Dictionary<string, List<RapidData>>(StringComparer.OrdinalIgnoreCase);

            foreach (RapidSymbol symbol in symbols)
            {
                try
                {
                    RapidData data = module.GetRapidData(symbol);
                    if (data.Value == null) continue;
                    string tipoNombre = data.RapidType;

                    List<RapidData> list;
                    // Intentamos obtener la lista de datos para este tipo RAPID
                    if (!datosPorTipo.TryGetValue(tipoNombre, out list))
                    {
                        // Si no existe, creamos una nueva lista
                        list = new List<RapidData>();

                        // Y la registramos en el diccionario bajo ese tipo
                        datosPorTipo[tipoNombre] = list;
                    }
                    list.Add(data);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{symbol.Name},Error: {ex.Message}");
                }
            }

            // Exportacion 
            foreach (var entry in datosPorTipo)
            {
                // Se crea un CSV por tipo de dato
                string tipoArchivo = Path.Combine(taskDir, module.Name + "_" + entry.Key + ".csv");
                using (var writer = new StreamWriter(tipoArchivo))
                {
                    writer.WriteLine(Parse.GenerarEncabezadoDesdeTipo(entry.Key));

                    foreach (var data in entry.Value)
                    {
                        string valor;
                        
                        // Si es array se tiene que iterar sobre el
                        if (data.IsArray)
                        {

                            var array = data.Value as ArrayData;
                            if (array == null)
                            {
                                // fallback seguro
                                writer.WriteLine($"{data.Name};{data.Value}");
                                continue;
                            }

                            int length = array.Length;

                            for (int i = 0; i < length; i++)
                            {

                                if (!TryFormatValue(entry.Key, array[i], out valor))
                                {
                                    _notImplemented.Add(entry.Key);
                                    valor = array[i].ToString();
                                }

                                writer.WriteLine(data.Name + " {" + i + "}" + ";" + valor);
                            }
                        }
                        else
                        {
                            if (!TryFormatValue(entry.Key, data.Value, out valor))
                            {
                                _notImplemented.Add(entry.Key);
                                valor = data.Value.ToString();
                            }

                            writer.WriteLine(data.Name + ";" + valor);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Convierte un valor RAPID en una representación compatible con CSV,
        /// aplicando el formateo adecuado según el tipo RAPID.
        /// </summary>
        /// <param name="rapidTypeName">
        /// Nombre del tipo RAPID (por ejemplo: <c>num</c>, <c>bool</c>, <c>robtarget</c>, etc.).
        /// Se utiliza para determinar la estrategia de formateo.
        /// </param>
        /// <param name="value">
        /// Valor del dato RAPID obtenido desde el controlador.
        /// Puede ser un tipo simple, un registro o un tipo definido por el usuario.
        /// </param>
        /// <param name="valor">
        /// Cadena resultante con el valor formateado para exportación CSV.
        /// Si el método devuelve <c>false</c>, este valor no es válido.
        /// </param>
        /// <returns>
        /// <c>true</c> si el valor se ha formateado correctamente;  
        /// <c>false</c> si el tipo RAPID no está soportado o no se puede convertir.
        /// </returns>
        private bool TryFormatValue(string rapidTypeName, object value, out string valor)
        {
            valor = null;
            double valorDoble;

            string tipoNET = value.GetType().Name;

            switch (tipoNET)
            {
                case "Num":

                    valorDoble = (Num)value;
                    valor = valorDoble.ToString();
                    return true;

                case "Dnum":

                    valorDoble = (Dnum)value;
                    valor = valorDoble.ToString();
                    return true;
                
                case "Bool":
                case "String":
                    valor = value.ToString();
                    return true;

                case "Pos":
                    valor = Parse.ParsePos((Pos)value);
                    return true;

                case "WobjData":
                    valor = Parse.ParseWobjData((WobjData)value);
                    return true;

                case "JointTarget":
                    valor = Parse.ParseJointTarget((JointTarget)value);
                    return true;

                case "RobTarget":
                    valor = Parse.ParseRobTarget((RobTarget)value);
                    return true;

                case "LoadData":
                    valor = Parse.ParseLoadData((LoadData)value);
                    return true;

                case "ToolData":
                    valor = Parse.ParseToolData((ToolData)value);
                    return true;

                case "ConfData":
                    valor = Parse.ParseConfData((ConfData)value);
                    return true;

                case "ExtJoint":
                    valor = Parse.ParseExtJoint((ExtJoint)value);
                    return true;

                case "RobJoint":
                    valor = Parse.ParseRobJoint((RobJoint)value);
                    return true;

                case "Orient":
                    valor = Parse.ParseOrient((Orient)value);
                    return true;

                case "Pose":
                    valor = Parse.ParsePose((Pose)value);
                    return true;

                case "UserDefined":
                    return TryFormatUserDefined(rapidTypeName, (UserDefined)value, out valor);

                default:
                    return false;
            }
        }
        /// <summary>
        /// Convierte valores RAPID de tipo <see cref="UserDefined"/> a formato CSV.
        /// Este método se encarga de los tipos RAPID definidos por el usuario
        /// (por ejemplo: <c>accdata</c>, <c>stoppointdata</c>, <c>zonedata</c>, etc.),
        /// cuya estructura no está representada por tipos .NET concretos.
        /// </summary>
        /// <param name="rapidTypeName">
        /// Nombre del tipo RAPID definido por el usuario.
        /// Se utiliza para aplicar reglas específicas de formateo cuando sea necesario.
        /// </param>
        /// <param name="ud">
        /// Valor RAPID de tipo <see cref="UserDefined"/> que contiene los campos
        /// y subvalores definidos en el controlador.
        /// </param>
        /// <param name="valor">
        /// Cadena resultante con el valor formateado para exportación CSV.
        /// Si el método devuelve <c>false</c>, este valor no es válido.
        /// </param>
        /// <returns>
        /// <c>true</c> si el valor se ha formateado correctamente;  
        /// <c>false</c> si el tipo RAPID no está soportado o no se puede convertir.
        /// </returns>
        private bool TryFormatUserDefined(string rapidTypeName, UserDefined ud, out string valor)
        {
            valor = null;
            switch (rapidTypeName.ToLowerInvariant())
            {
                case "accdata":
                    valor = Parse.ParseAccData(ud);
                    return true;
                case "stoppointdata":
                    valor = Parse.ParseStopPointData(ud);
                    return true;
                case "veldata":
                    valor = Parse.ParseVelData(ud);
                    return true;
                case "zonedata":
                    valor = Parse.ParseZoneData(ud);
                    return true;
                case "speeddata":
                    valor = Parse.ParseSpeedData(ud);
                    return true;
                case "tempmov":
                    valor = Parse.ParseTempMov(ud);
                    return true;
                case "volumen":
                    valor = Parse.ParseVolumen(ud);
                    return true;
                default:
                    return false;
            }
        }
    }
}
