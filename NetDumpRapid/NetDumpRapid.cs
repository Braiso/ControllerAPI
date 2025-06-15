/* 

Copyright (c) 2011, ABB
All rights reserved.

Redistribution and use in source and binary forms, with
or without modification, are permitted provided that 
the following conditions are met:

  * Redistributions of source code must retain the 
    above copyright notice, this list of conditions 
    and the following disclaimer.
  * Redistributions in binary form must reproduce the 
    above copyright notice, this list of conditions 
    and the following disclaimer in the documentation 
    and/or other materials provided with the 
    distribution.
  * Neither the name of ABB nor the names of its 
    contributors may be used to endorse or promote 
    products derived from this software without 
    specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE 
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED 
AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS 
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

*/

using System;
using ABB.Robotics.Controllers.Discovery;
using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.RapidDomain;

namespace ControllerAPI
{
    /// <summary>
    /// Creates a list of all controllers on the network
    /// and "dumps" them and the content of the Rapid domain to the console.
    /// </summary>
    class NetDumpRapid
    {
        static Version _okVer = new Version(5, 6, 0, 0);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main(string[] args)
        {
            NetworkScanner scanner = new NetworkScanner();
            ControllerInfo[] controllers = scanner.GetControllers(NetworkScannerSearchCriterias.Virtual);

            foreach (ControllerInfo c in controllers)
            {
                TestController(c);
            }

            Console.WriteLine("Press any key to terminate");
            Console.ReadKey();
        }

        static void TestController(ControllerInfo ctrl)
        {
            Console.WriteLine("Controller: {0} is Version: {1} SystemName: {2}", ctrl.HostName, ctrl.Version, ctrl.SystemName);
            if (ctrl.Version >= _okVer)
            {
                Controller controller = Controller.Connect(ctrl.SystemId, ConnectionType.Standalone);
                Task[] tasks = controller.Rapid.GetTasks();
                foreach (Task t in tasks)
                {
                    TraceTask(t);
                }
            }
        }

        static void TraceTask(Task t)
        {
            Console.WriteLine("\t Task: {0}", t.Name);
            Module[] modules = t.GetModules();
            foreach (Module m in modules)
            {
                TraceModule(m);
            }
        }

        static void TraceModule(Module m)
        {
            Console.WriteLine("\t\t Module: {0}", m.Name);
            Routine[] routines = m.GetRoutines();
            foreach (Routine r in routines)
            {
                TraceRoutine(r);
            }
        }

        static void TraceRoutine(Routine r)
        {
            Console.WriteLine("\t\t\t Routine: {0}", r.Name);
        }
    }
}
