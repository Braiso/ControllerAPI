using ABB.Robotics.Controllers.RapidDomain;

namespace ControllerAPI
{
    public static class Parse
    {
        // Devuelve un encabezado CSV basado en las propiedades públicas del tipo .NET correspondiente al tipo RAPID
        public static string GenerarEncabezadoDesdeTipo(string tipo)
        {
            switch (tipo.ToLowerInvariant())
            {
                case "jointtarget":

                    /* Estructura jointtarget
                    < dataobject of jointtarget >
                        < robax of robjoint >
                            < rax_1 of num >
                            < rax_2 of num >
                            < rax_3 of num >
                            < rax_4 of num >
                            < rax_5 of num >
                            < rax_6 of num >
                        < extax of extjoint >
                            < eax_a of num >
                            < eax_b of num >
                            < eax_c of num >
                            < eax_d of num >
                            < eax_e of num >
                            < eax_f of num >
                    */

                    return "Nombre;Robax.Rax_1;Robax.Rax_2;Robax.Rax_3;Robax.Rax_4;Robax.Rax_5;Robax.Rax_6;" +
                           "Extax.Eax_a;Extax.Eax_b;Extax.Eax_c;Extax.Eax_d;Extax.Eax_e;Extax.Eax_f";

                case "robtarget":

                    /* Estructura robtarget
                    < dataobject of robtarget >
                        < trans of pos >
                            < x of num >
                            < y of num >
                            < z of num >
                        < rot of orient >
                            < q1 of num >
                            < q2 of num >
                            < q3 of num >
                            < q4 of num >
                        < robconf of confdata >
                            < cf1 of num >
                            < cf4 of num >
                            < cf6 of num >
                            < cfx of num >
                        < extax of extjoint >
                            < eax_a of num >
                            < eax_b of num >
                            < eax_c of num >
                            < eax_d of num >
                            < eax_e of num >
                            < eax_f of num >
                    */

                    return "Nombre;Trans.X;Trans.Y;Trans.Z;Rot.Q1;Rot.Q2;Rot.Q3;Rot.Q4;" +
                           "Robconf.Cf1;Robconf.Cf4;Robconf.Cf6;Robconf.Cfx;" +
                           "Extax.Eax_a;Extax.Eax_b;Extax.Eax_c;Extax.Eax_d;Extax.Eax_e;Extax.Eax_f";

                case "stoppointdata":

                    /* Estructura stoppointdata
                    < dataobject of stoppointdata >
                        < type of stoppoint >
                        < progsynch of bool >
                        < inpos of inposdata >
                            < position of num >
                            < speed of num >
                            < mintime of num >
                            < maxtime of num >
                            < stoptime of num >
                            < followtime of num >
                            < signal of string >
                            < relation of opnum >
                            < checkvalue of num >
                    */

                    return "Nombre;Type;Progsynch;" +
                           "Inpos.Position;Inpos.Speed;Inpos.Mintime;Inpos.Maxtime;" +
                           "Inpos.Stoptime;Inpos.Followtime;Inpos.Signal;Inpos.Relation;Inpos.Checkvalue";

                case "pos":

                    /*
                    < dataobject of pos >
                    < x of num >
                    < y of num >
                    < z of num >
                    */

                    return "Nombre;X;Y;Z";

                case "wobjdata":

                    /* Estructura wobjdata
                    < dataobject of wobjdata >
                        < robhold of bool >
                        < ufprog of bool >
                        < ufmec of string >
                        < uframe of pose >
                            < trans of pos >
                                <x of num >
                                <y of num>
                                <z of num>
                            < rot of orient >
                                < q1 of num >
                                < q2 of num >
                                < q3 of num >
                                < q4 of num >
                        < oframe of pose >
                            < trans of pos >
                                < x of num >
                                < y of num >
                                < z of num >
                            < rot of orient >
                                < q1 of num >
                                < q2 of num >
                                < q3 of num >
                                < q4 of num >
                    */

                    return "Nombre;Robhold;Ufprog;Ufmec;Uframe.Trans.X;Uframe.Trans.Y;Uframe.Trans.Z;Uframe.Rot.Q1;Uframe.Rot.Q2;Uframe.Rot.Q3;Uframe.Rot.Q4;" +
                        "Oframe.Trans.X;Oframe.Trans.Y;Oframe.Trans.Z;Oframe.Rot.Q1;Oframe.Rot.Q2;Oframe.Rot.Q3;Oframe.Rot.Q4";

                case "accdata":

                    /* Estructura accdata
                    < dataobject of accdata>
                    <acc of num>
                    <ramp of num>
                    < finepramp of num>
                    */

                    return "Nombre;Acc;Ramp;Finepramp";

                case "veldata":

                    /* Estructura veldata
                    < dataobject of veldata >
                        < oride of num >
                        < max of num >
                    */

                    return "Nombre;Oride;Max";

                case "zonedata":

                    /* Estructura zonedata
                    < dataobject of zonedata >
                        < finep of bool >
                        < pzone_tcp of num >
                        < pzone_ori of num >
                        < pzone_eax of num >
                        < zone_ori of num >
                        < zone_leax of num >
                        < zone_reax of num >
                    */

                    return "Nombre;Finep;Pzone_tcp;Pzone_ori;Pzone_eax;Zone_ori;Zone_leax;Zone_reax";

                case "speeddata":

                    /* Estructura speeddata
                    < dataobject of speeddata >
                        < v_tcp of num >
                        < v_ori of num >
                        < v_leax of num >
                        < v_reax of num >
                    */

                    return "Nombre;V_tcp;V_ori;V_leax;V_reax";
                
                case "loaddata":

                    /* Estructura loaddata
                    < dataobject of loaddata >
                        < mass of num >
                        < cog of pos >
                            < x of num >
                            < y of num >
                            < z of num >
                        < aom of orient >
                            < q1 of num >
                            < q2 of num >
                            < q3 of num >
                            < q4 of num >
                        < ix of num >
                        < iy of num >
                        < iz of num >
                    */

                    return "Nombre;Mass;" +
                           "Cog.X;Cog.Y;Cog.Z;" +
                           "Aom.Q1;Aom.Q2;Aom.Q3;Aom.Q4;" +
                           "Ix;Iy;Iz";

                case "tooldata":

                    /* Estructura tooldata
                    < dataobject of tooldata >
                        < robhold of bool >
                        < tframe of pose >
                            < trans of pos >
                                < x of num >
                                < y of num >
                                < z of num >
                            < rot of orient >
                                < q1 of num >
                                < q2 of num >
                                < q3 of num >
                                < q4 of num >
                        < tload of loaddata >
                            < mass of num >
                            < cog of pos >
                                < x of num >
                                < y of num >
                                < z of num >
                            < aom of orient >
                                < q1 of num >
                                < q2 of num >
                                < q3 of num >
                                < q4 of num >
                            < ix of num >
                            < iy of num >
                            < iz of num >
                    */

                    return "Nombre;Robhold;" +
                           "Tframe.Trans.X;Tframe.Trans.Y;Tframe.Trans.Z;" +
                           "Tframe.Rot.Q1;Tframe.Rot.Q2;Tframe.Rot.Q3;Tframe.Rot.Q4;" +
                           "Tload.Mass;" +
                           "Tload.Cog.X;Tload.Cog.Y;Tload.Cog.Z;" +
                           "Tload.Aom.Q1;Tload.Aom.Q2;Tload.Aom.Q3;Tload.Aom.Q4;" +
                           "Tload.Ix;Tload.Iy;Tload.Iz";

                case "confdata":

                    /* Estructura confdata
                    < dataobject of confdata >
                        < cf1 of num >
                        < cf4 of num >
                        < cf6 of num >
                        < cfx of num >
                    */

                    return "Nombre;Cf1;Cf4;Cf6;Cfx";

                case "extjoint":

                    /* Estructura extjoint
                    < dataobject of extjoint >
                        < eax_a of num >
                        < eax_b of num >
                        < eax_c of num >
                        < eax_d of num >
                        < eax_e of num >
                        < eax_f of num >
                    */

                    return "Nombre;Eax_a;Eax_b;Eax_c;Eax_d;Eax_e;Eax_f";

                case "robjoint":

                    /* Estructura robjoint
                    < dataobject of robjoint >
                        < rax_1 of num >
                        < rax_2 of num >
                        < rax_3 of num >
                        < rax_4 of num >
                        < rax_5 of num >
                        < rax_6 of num >
                    */

                    return "Nombre;Rax_1;Rax_2;Rax_3;Rax_4;Rax_5;Rax_6";

                case "orient":

                    /* Estructura orient
                    < dataobject of orient >
                        < q1 of num >
                        < q2 of num >
                        < q3 of num >
                        < q4 of num >
                    */

                    return "Nombre;Q1;Q2;Q3;Q4";

                case "pose":

                    /* Estructura pose
                    < dataobject of pose >
                        < trans of pos >
                            < x of num >
                            < y of num >
                            < z of num >
                        < rot of orient >
                            < q1 of num >
                            < q2 of num >
                            < q3 of num >
                            < q4 of num >
                    */

                    return "Nombre;" +
                           "Trans.X;Trans.Y;Trans.Z;" +
                           "Rot.Q1;Rot.Q2;Rot.Q3;Rot.Q4";

                case "tempmov":
                    /* Estructura tempmov
                    paro of bool
                    tiempo of dnum
                    string fecha
                    string hora
                     */
                    return "Nombre;paro;tiempo;fecha;hora";
                case "volumen":
                    /*
                    RECORD volumen
                        num X_Pos;
                        num X_Neg;
                        num Y_Pos;
                        num Y_Neg;
                    ENDRECORD
                     */
                    return "Nombre;X_Pos;X_Neg;Y_Pos;Y_Neg";
                default:
                    return "Nombre;Valor";
            }
        }
        public static string ParsePos(Pos pos)
        {
            return $"{pos.X};{pos.Y};{pos.Z}";
        }
        public static string ParseWobjData(WobjData wobj)
        {

            return $"{wobj.Robhold};{wobj.Ufprog};{wobj.Ufmec};" +
            $"{wobj.Uframe.Trans.X};{wobj.Uframe.Trans.Y};{wobj.Uframe.Trans.Z};" +
            $"{wobj.Uframe.Rot.Q1};{wobj.Uframe.Rot.Q2};{wobj.Uframe.Rot.Q3};{wobj.Uframe.Rot.Q4};" +
            $"{wobj.Oframe.Trans.X};{wobj.Oframe.Trans.Y};{wobj.Oframe.Trans.Z};" +
            $"{wobj.Oframe.Rot.Q1};{wobj.Oframe.Rot.Q2};{wobj.Oframe.Rot.Q3};{wobj.Oframe.Rot.Q4}";
        }
        public static string ParseAccData(UserDefined accdata)
        {
            double acc = ((Num)accdata.Components[0]).Value;
            double ramp = ((Num)accdata.Components[1]).Value;
            double fineramp = ((Num)accdata.Components[2]).Value;
            return $"{acc};{ramp};{fineramp}";
        }
        public static string ParsePose(Pose pose)
        {
            return $"{pose.Trans.X};{pose.Trans.Y};{pose.Trans.Z};" +
                   $"{pose.Rot.Q1};{pose.Rot.Q2};{pose.Rot.Q3};{pose.Rot.Q4}";
        }
        public static string ParseOrient(Orient orient)
        {
            return $"{orient.Q1};{orient.Q2};{orient.Q3};{orient.Q4}";
        }
        public static string ParseRobJoint(RobJoint rj)
        {
            return $"{rj.Rax_1};{rj.Rax_2};{rj.Rax_3};{rj.Rax_4};{rj.Rax_5};{rj.Rax_6}";
        }
        public static string ParseExtJoint(ExtJoint ej)
        {
            return $"{ej.Eax_a};{ej.Eax_b};{ej.Eax_c};{ej.Eax_d};{ej.Eax_e};{ej.Eax_f}";
        }
        public static string ParseJointTarget(JointTarget jt)
        {
            return $"{jt.RobAx.Rax_1};{jt.RobAx.Rax_2};{jt.RobAx.Rax_3};" +
                   $"{jt.RobAx.Rax_4};{jt.RobAx.Rax_5};{jt.RobAx.Rax_6};" +
                   $"{jt.ExtAx.Eax_a};{jt.ExtAx.Eax_b};{jt.ExtAx.Eax_c};" +
                   $"{jt.ExtAx.Eax_d};{jt.ExtAx.Eax_e};{jt.ExtAx.Eax_f}";
        }
        public static string ParseConfData(ConfData conf)
        {
            return $"{conf.Cf1};{conf.Cf4};{conf.Cf6};{conf.Cfx}";
        }
        public static string ParseSpeedData(UserDefined speed)
        {
            double V_tcp = ((Num)speed.Components[0]).Value;
            double V_ori = ((Num)speed.Components[1]).Value;
            double V_leax = ((Num)speed.Components[2]).Value;
            double V_reax = ((Num)speed.Components[3]).Value;
            return $"{V_tcp};{V_ori};{V_leax};{V_reax}";
        }
        public static string ParseZoneData(UserDefined zone)
        {
            string Finep = (zone.Components[0].ToString());
            double Pzone_tcp = ((Num)zone.Components[1]).Value;
            double Pzone_ori = ((Num)zone.Components[2]).Value;
            double Pzone_eax = ((Num)zone.Components[3]).Value;
            double Zone_ori = ((Num)zone.Components[4]).Value;
            double Zone_leax = ((Num)zone.Components[5]).Value;
            double Zone_reax = ((Num)zone.Components[6]).Value;

            return $"{Finep};{Pzone_tcp};{Pzone_ori};{Pzone_eax};" +
                   $"{Zone_ori};{Zone_leax};{Zone_reax}";
        }
        public static string ParseLoadData(LoadData load)
        {
            return $"{load.Mass};" +
                   $"{load.Cog.X};{load.Cog.Y};{load.Cog.Z};" +
                   $"{load.Aom.Q1};{load.Aom.Q2};{load.Aom.Q3};{load.Aom.Q4};" +
                   $"{load.Ix};{load.Iy};{load.Iz}";
        }
        public static string ParseToolData(ToolData tool)
        {
            return $"{tool.Robhold};" +
                   $"{tool.Tframe.Trans.X};{tool.Tframe.Trans.Y};{tool.Tframe.Trans.Z};" +
                   $"{tool.Tframe.Rot.Q1};{tool.Tframe.Rot.Q2};{tool.Tframe.Rot.Q3};{tool.Tframe.Rot.Q4};" +
                   $"{tool.Tload.Mass};" +
                   $"{tool.Tload.Cog.X};{tool.Tload.Cog.Y};{tool.Tload.Cog.Z};" +
                   $"{tool.Tload.Aom.Q1};{tool.Tload.Aom.Q2};{tool.Tload.Aom.Q3};{tool.Tload.Aom.Q4};" +
                   $"{tool.Tload.Ix};{tool.Tload.Iy};{tool.Tload.Iz}";
        }
        public static string ParseRobTarget(RobTarget rt)
        {
            return $"{rt.Trans.X};{rt.Trans.Y};{rt.Trans.Z};" +
                   $"{rt.Rot.Q1};{rt.Rot.Q2};{rt.Rot.Q3};{rt.Rot.Q4};" +
                   $"{rt.Robconf.Cf1};{rt.Robconf.Cf4};{rt.Robconf.Cf6};{rt.Robconf.Cfx};" +
                   $"{rt.Extax.Eax_a};{rt.Extax.Eax_b};{rt.Extax.Eax_c};" +
                   $"{rt.Extax.Eax_d};{rt.Extax.Eax_e};{rt.Extax.Eax_f}";
        }
        public static string ParseStopPointData(UserDefined sp)
        {

            /* Estructura stoppointdata
            < dataobject of stoppointdata >
                < type of stoppoint >
                < progsynch of bool >
                < inpos of inposdata >
                    < position of num >
                    < speed of num >
                    < mintime of num >
                    < maxtime of num >
                < stoptime of num >
                < followtime of num >
                < signal of string >
                < relation of opnum >
                < checkvalue of num >
            */

            string typeObj = sp.Components[0].ToString();
            string Progsynch = sp.Components[1].ToString();

            var inpos = (UserDefined)sp.Components[2];
            double Inpos_Position = ((Num)inpos.Components[0]).Value;
            double Inpos_Speed = ((Num)inpos.Components[1]).Value;
            double Inpos_Mintime = ((Num)inpos.Components[2]).Value;
            double Inpos_Maxtime = ((Num)inpos.Components[3]).Value;

            double Stoptime = ((Num)sp.Components[3]).Value;
            double Followtime = ((Num)sp.Components[4]).Value;
            string Signal = sp.Components[5].ToString();
            double Relation = ((Num)sp.Components[6]).Value;
            double Checkvalue = ((Num)sp.Components[7]).Value;



            return $"{typeObj};{Progsynch};" +
                   $"{Inpos_Position};{Inpos_Speed};{Inpos_Mintime};{Inpos_Maxtime};" +
                   $"{Stoptime};{Followtime};{Signal};" +
                   $"{Relation};{Checkvalue}";
        }
        public static string ParseVelData(UserDefined vd)
        {
            double oride = ((Num)vd.Components[0]).Value;
            double max = ((Num)vd.Components[1]).Value;
            return $"{oride};{max}";
        }
        public static string ParseTempMov(UserDefined tm)
        {
            string paro = tm.Components[0].ToString();
            double tiempo = ((Dnum)tm.Components[1]).Value;
            string fecha = tm.Components[2].ToString();
            string hora = tm.Components[3].ToString();

            return $"{paro};{tiempo};{fecha};{hora}";
        }

        public static string ParseVolumen(UserDefined v)
        {
            double X_Pos = ((Num)v.Components[0]).Value;
            double X_Neg = ((Num)v.Components[1]).Value;
            double Y_Pos = ((Num)v.Components[2]).Value;
            double Y_Neg = ((Num)v.Components[3]).Value;

            return $"{X_Pos};{X_Neg};{Y_Pos};{Y_Neg}";
        }
    }
}
