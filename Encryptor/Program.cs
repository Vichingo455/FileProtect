using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Security.Principal;
using System.Reflection;
using System.Diagnostics;

namespace Encryptor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
                WindowsPrincipal pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                bool hasAdministrativeRight = pricipal.IsInRole(WindowsBuiltInRole.Administrator);
                if (!hasAdministrativeRight)
                {
                var dialog = MessageBox.Show("Would you like to start this program as administrator? Starting with admin privileges allows the program to encrypt admin-protected files.","FileProtect™",MessageBoxButtons.YesNoCancel,MessageBoxIcon.Question);
                if (dialog == DialogResult.Yes)
                {
                    string fileName = Assembly.GetExecutingAssembly().Location;
                    ProcessStartInfo processInfo = new ProcessStartInfo();
                    processInfo.Verb = "runas";
                    processInfo.FileName = fileName;

                    try
                    {
                        Process.Start(processInfo);
                        Environment.Exit(0);
                    }
                    catch
                    {
                        Application.Run(new Encryptor());
                    }
                }
                else if (dialog == DialogResult.Cancel)
                {
                    Environment.Exit(0);
                }
                else
                {
                    Application.Run(new Encryptor());
                }
                }
                else
                {
                    Application.Run(new Encryptor());
                }
        }
    }
}
