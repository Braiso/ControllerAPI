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
    public static class Parse
    {
        // Devuelve un encabezado CSV basado en las propiedades públicas del tipo .NET correspondiente al tipo RAPID
        public static string GenerarEncabezadoDesdeTipo(string tipo)
        {
            // Normaliza el tipo a minúsculas para evitar problemas de coincidencia
            tipo = tipo.ToLowerInvariant();

            if (tipo == "pos")
            {
                return ("Nombre;X;Y;Z");
            }
            else if (tipo == "wobjdata")
            {
                return "Nombre;Robhold;Ufprog;Ufmec;Uframe.Trans.X;Uframe.Trans.Y;Uframe.Trans.Z;Uframe.Rot.Q1;Uframe.Rot.Q2;Uframe.Rot.Q3;Uframe.Rot.Q4";
            }
            else if (tipo == "accdata")
            {
                return "Nombre;Acc;Ramp;Finepramp";
            }
            else
            {
                return ("Nombre;Valor");
            }
        }
        public static string ParsePos(Pos pos)
        {
            return $"{pos.X};{pos.Y};{pos.Z}";
        }

        public static string ParseWobjData(WobjData wobj)
        {
            return $"{wobj.Robhold};{wobj.Ufprog};{wobj.Ufmec};" +
            $"{wobj.Uframe.Trans.X};{wobj.Uframe.Trans.Y} ; {wobj.Uframe.Trans.Z};" +
            $"{wobj.Uframe.Rot.Q1} ;{wobj.Uframe.Rot.Q2} ; {wobj.Uframe.Rot.Q3} ; {wobj.Uframe.Rot.Q4}";
        }

        public static string ParseAccData(UserDefined accdata)
        {

            string acc = accdata.Components[0].ToString();
            string ramp = accdata.Components[1].ToString();
            string finepramp = accdata.Components[2].ToString();
            return $"{acc};{ramp};{finepramp}";
        }
    }
}
