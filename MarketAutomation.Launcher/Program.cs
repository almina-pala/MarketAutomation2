using System.Diagnostics;
using System.Net.Sockets;

const int apiPort = 7116;

string launcherDirectory = AppContext.BaseDirectory;

string publishApiPath = Path.Combine(
    launcherDirectory,
    "Api",
    "MarketAutomation2.API.exe");

string publishDesktopPath = Path.Combine(
    launcherDirectory,
    "Desktop",
    "MarketAutomation2.Desktop.exe");

#if DEBUG
string solutionDirectory = Path.GetFullPath(
    Path.Combine(launcherDirectory, "../../../../"));

string debugApiPath = Path.Combine(
    solutionDirectory,
    "MarketAutomation2.API",
    "bin",
    "Debug",
    "net8.0",
    "MarketAutomation2.API.exe");

string debugDesktopPath = Path.Combine(
    solutionDirectory,
    "MarketAutomation2.Desktop",
    "bin",
    "Debug",
    "net8.0-windows",
    "MarketAutomation2.Desktop.exe");
#endif

string? apiPath = null;
string? desktopPath = null;

if (File.Exists(publishApiPath) && File.Exists(publishDesktopPath))
{
    apiPath = publishApiPath;
    desktopPath = publishDesktopPath;
}
#if DEBUG
else if (File.Exists(debugApiPath) && File.Exists(debugDesktopPath))
{
    apiPath = debugApiPath;
    desktopPath = debugDesktopPath;
}
#endif

if (apiPath == null)
{
    MessageBox(
        "API uygulaması bulunamadı.\n\n" +
        "Beklenen konum:\n" + publishApiPath +
        "\n\nLütfen publish scriptini çalıştırın:\n" +
        "  .\\scripts\\publish.ps1",
        "Market Automation - API Hatası",
        true);

    return;
}

if (desktopPath == null)
{
    MessageBox(
        "Desktop uygulaması bulunamadı.\n\n" +
        "Beklenen konum:\n" + publishDesktopPath +
        "\n\nLütfen publish scriptini çalıştırın:\n" +
        "  .\\scripts\\publish.ps1",
        "Market Automation - Desktop Hatası",
        true);

    return;
}

Process? apiProcess = null;

try
{
    apiProcess = Process.Start(new ProcessStartInfo
    {
        FileName = apiPath,
        WorkingDirectory = Path.GetDirectoryName(apiPath)!,
        UseShellExecute = false,
        CreateNoWindow = true,
        WindowStyle = ProcessWindowStyle.Hidden,
        Environment =
        {
            ["ASPNETCORE_ENVIRONMENT"] = "Production"
        }
    });

    if (apiProcess == null)
    {
        MessageBox("API başlatılamadı.", "Market Automation", true);
        return;
    }

    bool apiReady = await WaitForPortAsync("127.0.0.1", apiPort, 15000);

    if (!apiReady)
    {
        MessageBox(
            "Market Automation API başlatılamadı.\n\n" +
            "HTTPS bağlantısı hazırlanamadı.\n\n" +
            "Beklenen adres:\nhttps://localhost:7116",
            "Market Automation - API Hatası",
            true);

        return;
    }

    var desktopProcess = Process.Start(new ProcessStartInfo
    {
        FileName = desktopPath,
        WorkingDirectory = Path.GetDirectoryName(desktopPath)!,
        UseShellExecute = true
    });

    if (desktopProcess != null)
    {
        await desktopProcess.WaitForExitAsync();
    }
}
catch (Exception ex)
{
    MessageBox(
        "Market Automation başlatılırken hata oluştu.\n\n" + ex.Message,
        "Market Automation",
        true);
}
finally
{
    if (apiProcess != null)
    {
        try
        {
            if (!apiProcess.HasExited)
            {
                apiProcess.Kill(true);
            }
        }
        catch
        {
        }
    }
}

static async Task<bool> WaitForPortAsync(
    string host,
    int port,
    int timeoutMilliseconds)
{
    var startTime = DateTime.UtcNow;

    while ((DateTime.UtcNow - startTime).TotalMilliseconds < timeoutMilliseconds)
    {
        try
        {
            using var client = new TcpClient();
            await client.ConnectAsync(host, port);
            return true;
        }
        catch
        {
            await Task.Delay(300);
        }
    }

    return false;
}

static void MessageBox(string message, string title, bool error)
{
    System.Windows.Forms.MessageBox.Show(
        message,
        title,
        System.Windows.Forms.MessageBoxButtons.OK,
        error
            ? System.Windows.Forms.MessageBoxIcon.Error
            : System.Windows.Forms.MessageBoxIcon.Information);
}
