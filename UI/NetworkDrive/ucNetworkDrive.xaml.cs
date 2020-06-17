using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PrinterCenter.UI
{
    /// <summary>
    /// Interaction logic for ucNetworkDrive.xaml
    /// </summary>
    /// 
   
    public partial class ucNetworkDrive : UserControl
    {
        public ucNetworkDrive()
        {
            InitializeComponent();

            cbAvaiableDrives.ItemsSource = GetAvaliableDriveLetter();
        }
        private ArrayList GetAvaliableDriveLetter()
        {
            ArrayList driveLetters = new ArrayList(26); // Allocate space for alphabet
            for (int i = 65; i < 91; i++) // increment from ASCII values for A-Z
            {
                driveLetters.Add(Convert.ToChar(i)); // Add uppercase letters to possible drive letters
            }

            foreach (string drive in Directory.GetLogicalDrives())
            {
                driveLetters.Remove(drive[0]); // removed used drive letters from possible drive letters
            }

            return driveLetters;
        }
    }
}
