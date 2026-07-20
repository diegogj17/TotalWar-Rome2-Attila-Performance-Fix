using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

try
{
    string game = @"C:\Program Files (x86)\Steam\steamapps\common\Total War Rome II\Rome2_original.exe";
    string affinity = @"C:\Program Files (x86)\Steam\steamapps\common\Total War Rome II\SetProcessAffinity.exe";

    Process.Start(affinity);
    Process.Start(game);
}
catch (Exception ex)
{
    MessageBox.Show(ex.ToString());
}