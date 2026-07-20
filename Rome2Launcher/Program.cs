using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

try
{
    string game = @"C:\Program Files (x86)\Steam\steamapps\common\Total War Rome II\Rome2_original.exe";
    string affinity = @"C:\Program Files (x86)\Steam\steamapps\common\Total War Rome II\SetProcessAffinity.exe";
    string gameDir = Path.GetDirectoryName(game)!;

    Process.Start(affinity);

    var startInfo = new ProcessStartInfo(game)
    {
        WorkingDirectory = gameDir,
    };
    foreach (var arg in args)
    {
        startInfo.ArgumentList.Add(arg);
    }
    Process.Start(startInfo);
}
catch (Exception ex)
{
    MessageBox.Show(ex.ToString());
}