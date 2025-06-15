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
    /// Listens for events on the Controller object
    /// </summary>
    class Listener
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main(string[] args)
        {
            Controller ctrl;
            while ((ctrl = CreateController()) == null)
            {
            }

            ctrl.Logon(UserInfo.DefaultUser);
            ctrl.OperatingModeChanged += new EventHandler<OperatingModeChangeEventArgs>(ctrl_OperatingModeChanged);

            Console.WriteLine("Press any key to terminate");
            Console.ReadKey();

            ctrl.Logoff();
        }

        static Controller CreateController()
        {
            NetworkScanner scanner = new NetworkScanner();
            ControllerInfo[] controllers = scanner.GetControllers(NetworkScannerSearchCriterias.Virtual);
            if (controllers.Length > 0)
            {
                Controller dynamic = Controller.Connect(controllers[0].SystemId, ConnectionType.Standalone);
                return dynamic;
            }
            return null;
        }

        private static void ctrl_OperatingModeChanged(object sender, OperatingModeChangeEventArgs e)
        {
            Console.WriteLine("New Operating mode at: {0} new mode is: {1}", e.Time, e.NewMode);
        }
    }
}
