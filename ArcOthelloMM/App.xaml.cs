using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

namespace ArcOthelloMM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static string Extension = ".mm";
        static string KeyName = "ArcOthelloMM";
        static string OpenWith = System.Reflection.Assembly.GetExecutingAssembly().Location;
        static string FileDescription = "ArcOthelloMM Game Save";

        void App_Startup(object sender, StartupEventArgs e)
        {
            if (!IsAssociationSet())
            {
                MessageBoxResult result = TrySetAssociation();

                if (result == MessageBoxResult.Yes)
                {
                    Process proc = new Process();
                    proc.StartInfo.FileName = OpenWith;
                    proc.StartInfo.UseShellExecute = true;
                    proc.StartInfo.Verb = "runas";
                    proc.Start();
                    System.Environment.Exit(1);
                }
            }

            OthelloBoard othelloBoard = new OthelloBoard();

            if(e.Args.Length > 0)
                othelloBoard.LoadGameSave(e.Args[e.Args.Length-1]);

            othelloBoard.Show();
        }

        /// <summary>
        /// Check the registry in read only to verify if the association is correct
        /// </summary>
        /// <returns></returns>
        public static bool IsAssociationSet()
        {
            try
            {
                //check for the keyname
                if ((string)Registry.ClassesRoot.OpenSubKey(Extension, false).GetValue("") != KeyName)
                    return false;
                //check for the path for this keyname
                if ((string)Registry.ClassesRoot.OpenSubKey(KeyName + "\\Shell\\open\\command", false).GetValue("") != "\"" + OpenWith + "\"" + " \"%1\"")
                    return false;
                //check for the icon
                if ((string)Registry.ClassesRoot.OpenSubKey(KeyName + "\\DefaultIcon", false).GetValue("") != "\"" + OpenWith + "\",0")
                    return false;
                //check the user choice
                if (Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\" + Extension, false).GetSubKeyNames().Contains("UserChoice"))
                    return false;
            }
            catch // Cant get a key
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// https://stackoverflow.com/a/2697804/9263555
        /// </summary>
        /// <param name="Extension"></param>
        /// <param name="KeyName"></param>
        /// <param name="OpenWith"></param>
        /// <param name="FileDescription"></param>
        public static MessageBoxResult TrySetAssociation()
        {
            try
            {
                RegistryKey OpenMethod = Registry.ClassesRoot.CreateSubKey(KeyName);
                OpenMethod.SetValue("", FileDescription);
                OpenMethod.CreateSubKey("DefaultIcon").SetValue("", "\"" + OpenWith + "\",0");

                RegistryKey Shell = OpenMethod.CreateSubKey("Shell");
                Shell.CreateSubKey("open").CreateSubKey("command").SetValue("", "\"" + OpenWith + "\"" + " \"%1\"");
                
                RegistryKey BaseKey = Registry.ClassesRoot.CreateSubKey(Extension);
                BaseKey.SetValue("", KeyName);
                
                RegistryKey CurrentUser = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\" + Extension, true);
                CurrentUser.DeleteSubKey("UserChoice", false);

                OpenMethod.Close();
                Shell.Close();
                BaseKey.Close();
                CurrentUser.Close();

                // Tell explorer the file association has been changed
                SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
                return MessageBox.Show(".mm file has been correctly associated with \"" + OpenWith + "\"", "Association success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch(System.UnauthorizedAccessException)
            {
                return MessageBox.Show(KeyName + " couldn't associate .mm file with \"" + OpenWith + "\"\nYes : Try with admin rights\nNo : Ignore this step", "Association failed", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            }
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);
    }
}
