using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABB.Robotics.Controllers.Discovery;
using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.RapidDomain;

namespace ControllerAPI
{
    class Parse
    {
        // Devuelve un encabezado CSV basado en las propiedades públicas del tipo .NET correspondiente al tipo RAPID
        public static string GenerarEncabezadoDesdeTipo(string tipo)
        {
            if (tipo == "Pos")
            {
                return ("Nombre;X;Y;Z");
            }
            else
            {
                return ("Nombre;Valor");
            }
        }
        public static string ParseRapidData(RapidData data)
        {
            // Si es un tipo Pos, devuelve el formato específico
            if (data.RapidType == "pos")
            {
                Pos pos = (Pos)data.Value;
                return ParsePos(pos);
            }
            else
            {
                // Para otros tipos, devuelve el nombre y el valor como cadena
                return data.Value.ToString();
            }
        }
        static string ParsePos(Pos pos)
        {
            return $"{pos.X};{pos.Y};{pos.Z}";
        }
    }
}
