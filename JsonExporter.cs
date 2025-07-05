using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TaskTrackerWPF
{
    internal class JsonExporter
    {
        public class ExportedSession
        {
            public string SessionName { get; set; }
            public DateTime ExportedAt { get; set; }
            public List<float> DurationsInSeconds { get; set; }
        }


        public static void ExportToJson(string sessionName, IEnumerable<float> durations)
        {
            if (durations is null)
            {
                MessageBox.Show("No data to export.");
                return;
            }

            if (durations.Count() < 2)
            {
                MessageBox.Show("Not enough data to export.");
                return;
            }



            var export = new ExportedSession
            {
                SessionName = sessionName,
                ExportedAt = DateTime.Now,
                DurationsInSeconds = new List<float>(durations)
            };

            string json = JsonSerializer.Serialize(export, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string safeSessionName = Regex.Replace(sessionName, @"[^\w\-]", "_");
            string filename = $"{safeSessionName}_{timestamp}.json";

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = filename,
                DefaultExt = ".json",
                Filter = "JSON files (*.json)|*.json"
            };

            if (dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, json);
                MessageBox.Show("Export successful!", "Done");
            }
        }

    }
}
