using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

try
{
    string gameDir = AppContext.BaseDirectory;
    string game = Path.Combine(gameDir, "Attila_original.exe");
    string affinity = Path.Combine(gameDir, "SetProcessAffinityAttila.exe");

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
