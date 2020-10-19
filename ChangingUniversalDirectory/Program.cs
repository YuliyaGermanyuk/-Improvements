using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using DocsVision.Platform.ObjectModel;
using DocsVision.Platform.ObjectManager;
using DocsVision.Platform.ObjectManager.SearchModel;
using DocsVision.Platform.ObjectManager.Metadata;
using DocsVision.TakeOffice.Cards.Constants;

namespace DeleteOblImprov
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 1
            string connect = "ConnectAddress=http://dv5/DocsVision/StorageServer/StorageServerService.asmx"; //BaseName=DV_SKB_WorkBase";  // Рабочая база
            //string connect = "ConnectAddress=http://dv-test/DocsVision/StorageServer/StorageServerService.asmx;";       // Тестовая база
            SessionManager manager = SessionManager.CreateInstance(connect);
            //manager.Connect(connect);
            UserSession session = manager.CreateSession();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DeleteOblImprov(session));

            manager.CloseSession(session.Id);
        }
    }
}
