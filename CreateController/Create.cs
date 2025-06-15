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

using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.Discovery;
using System;

namespace ControllerAPI
{
    /// <summary>
    /// Creates a controller instance from various sources.
    /// </summary>
    class Create
    {
        static string sysID = "";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main(string[] args)
        {
            DynamicCreation();
            StaticCreation();

            Console.WriteLine("Press any key to terminate");
            Console.ReadKey();
        }

        static void DynamicCreation()
        {
            NetworkScanner scanner = new NetworkScanner();
            ControllerInfo[] controllers = scanner.GetControllers(NetworkScannerSearchCriterias.Virtual);
            if (controllers.Length > 0)
            {
                sysID = controllers[0].SystemId.ToString();
                Controller dynamic = Controller.Connect(controllers[0].SystemId, ConnectionType.Standalone);
                TraceController(dynamic);
            }
        }

        static void StaticCreation()
        {
            try
            {
                Controller @static = Controller.Connect(new Guid(sysID), ConnectionType.Standalone);
                TraceController(@static);
            }
            catch (Exception ex)
            {
                // The Guid needs to be altered to you local Guid. Debug and break on line 68 get the Guid from 
                // the system id property.
                Console.WriteLine("The Guid needs to be altered to you local Guid. Debug and break on line 68(controller[0].SystemId) get the Guid from the system id property.");
            }


        }

        static void TraceController(Controller ctrl)
        {
            Console.WriteLine("Controller-System: {0} time is: {1}", ctrl.SystemName, ctrl.DateTime);
        }
    }
}
