using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

try
{
    string gameDir = AppContext.BaseDirectory;
    string game = Path.Combine(gameDir, "Rome2_original.exe");
    string affinity = Path.Combine(gameDir, "SetProcessAffinityRome2.exe");

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