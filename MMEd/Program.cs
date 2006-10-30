using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MMEd
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //run the tests here if NUnit is being difficult:
            //new MMEd.Tests.Serialisation().TestBinarySerialisationIsInvertible();
            //new MMEd.Tests.Serialisation().TestXmlSerialisationIsInvertible();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}