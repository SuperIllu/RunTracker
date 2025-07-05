using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace TaskTrackerWPF
{
    /// <summary>
    /// Interaction logic for SummaryWindow.xaml
    /// </summary>
    public partial class SummaryWindow : Window
    {

        private TaskTracker _taskTracker;
        private DispatcherTimer _timer;

        public SummaryWindow(TaskTracker tracker)
        {
            InitializeComponent();
            _taskTracker = tracker ?? throw new ArgumentNullException(nameof(tracker));
            tracker.OnLoggingStarted += OnLoggingStarted;
            tracker.OnLoggingStopped += OnLoggingStopped;
            tracker.OnTaskLogged += OnTaskLogged;

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0.2)
            };
            _timer.Tick += (s, e) => UpdateCurrentTime();
            _timer.Start();

            StateDot.Fill = Brushes.OrangeRed;
        }

        private void UpdateCurrentTime()
        {
            ElapsedText.Text = FormatSecondsWithMS(_taskTracker.GetCurrentTaskDuration());
        }

        private void OnTaskLogged(float taskTime)
        {
            ElapsedText.Text = taskTime.ToString("F2") + "s";
            string last = FormatSeconds(_taskTracker.GetLastTaskDuration());
            string average = FormatSeconds(_taskTracker.GetAverageTaskDuration());

            DurationText.Text = $"Last: {last}, Avrg: {average}";
        }

        private void OnLoggingStopped()
        {
            StateText.Text = "Paused";
            StateDot.Fill = Brushes.OrangeRed;
        }

        private void OnLoggingStarted()
        {
            StateText.Text = "Tracking";
            ElapsedText.Text = "00:00";
            StateDot.Fill = Brushes.LimeGreen;
        }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove(); // Enables window dragging
        }

        public static string FormatSecondsWithMS(float s)
        {
            TimeSpan time = TimeSpan.FromSeconds(s);
            return $"{(int)time.TotalMinutes:00}:{time.Seconds:00}:{time.Milliseconds:000}";
        }


        public static string FormatSeconds(float s)
        {
            TimeSpan time = TimeSpan.FromSeconds(s);
            return $"{(int)time.TotalMinutes:00}:{time.Seconds:00}";
        }
    }
}
