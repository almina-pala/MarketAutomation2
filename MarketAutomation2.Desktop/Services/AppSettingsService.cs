using System.IO;
using System.Text.Json;
using MarketAutomation2.Desktop.Models;

namespace MarketAutomation2.Desktop.Services
{
    public class AppSettingsService
    {
        private readonly string _filePath;

        public AppSettingsService()
        {
            string folderPath = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "MarketAutomation2");

            Directory.CreateDirectory(folderPath);

            _filePath = Path.Combine(
                folderPath,
                "settings.json");
        }

        public AppSettings Load()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    return new AppSettings();
                }

                string json = File.ReadAllText(_filePath);

                var settings =
                    JsonSerializer.Deserialize<AppSettings>(json);

                return settings ?? new AppSettings();
            }
            catch
            {
                return new AppSettings();
            }
        }

        public void Save(AppSettings settings)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json =
                JsonSerializer.Serialize(settings, options);

            File.WriteAllText(_filePath, json);
        }

        public void Reset()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }
    }
}