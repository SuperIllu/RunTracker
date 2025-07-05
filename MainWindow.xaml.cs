using System.Windows;
using Gma.System.MouseKeyHook;
using System.Windows.Threading;

namespace TaskTrackerWPF
{

    public partial class MainWindow : Window
    {
        private IKeyboardMouseEvents _globalHook;
        private SummaryWindow _summaryWindow;
        private RegionSelectorWindow _regionSelectorWindow;

        private TaskTracker _taskTracker;
        private AutoTaskTracker _autoTaskTracker;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Closed += OnClosed;

            _taskTracker = new TaskTracker(Log);
            _regionSelectorWindow = new RegionSelectorWindow();
            _autoTaskTracker = new AutoTaskTracker(_regionSelectorWindow, _taskTracker, Log);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _globalHook = Hook.GlobalEvents();
            _globalHook.KeyDown += OnKeyDown;
            _summaryWindow = new SummaryWindow(_taskTracker);
            _summaryWindow.Show();
            Log("Global key hook started.");
        }

        private void OnClosed(object sender, EventArgs e)
        {
            _globalHook.KeyDown -= OnKeyDown;
            _globalHook.Dispose();
            _summaryWindow.Close();
            _regionSelectorWindow.Close();
            Log("Global key hook stopped.");
        }

        private void OnKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                switch(e.KeyCode)
                {
                    case System.Windows.Forms.Keys.Add:
                        _taskTracker.StartTracking();
                        break;
                    case System.Windows.Forms.Keys.Subtract:
                        _taskTracker.StopTracking();
                        break;
                    case System.Windows.Forms.Keys.Enter:
                        _taskTracker.LogTask();
                        break;
                    case System.Windows.Forms.Keys.Pause:
                        _taskTracker.PauseTracking();
                        break;
                    default:
                        break;
                }
            });
        }

        private void ExportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            string sessionName = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter a name for this session:",
                "Session Export",
                "task_session"
            );

            if (!string.IsNullOrWhiteSpace(sessionName))
            {
                JsonExporter.ExportToJson(sessionName, _taskTracker.GetTaskList());
            }
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void TrackMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _regionSelectorWindow.Show();
        }


        private void Log(string message)
        {
            LogBox.AppendText($"{DateTime.Now:T} - {message}\n");
            LogBox.ScrollToEnd();
        }
    }
}
