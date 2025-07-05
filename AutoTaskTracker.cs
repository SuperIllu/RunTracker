using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace TaskTrackerWPF
{
    internal class AutoTaskTracker
    {
        public RegionSelectorWindow SelectorWindow { get; }

        private Action<string> _log;
        private TaskTracker _taskTracker;
        private DispatcherTimer _timer;
        private Bitmap _screenExample;
        private Rectangle _coordinates;

        bool _sameRegionFound = false;

        public AutoTaskTracker(RegionSelectorWindow selectorWindow, TaskTracker taskTracker, Action<string> log)
        {
            SelectorWindow = selectorWindow ?? throw new ArgumentNullException(nameof(selectorWindow));
            SelectorWindow.OnRegionSelected += OnRegionSelected;
            SelectorWindow.OnRegionSelectionCancelled += OnRegionSelectionCancelled;

            _log = log;
            _taskTracker = taskTracker ?? throw new ArgumentNullException(nameof(taskTracker));

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0.2)
            };
            _timer.Tick += (s, e) => CheckRegion(s, e);
        }

        private void CheckRegion(object sender, EventArgs e)
        {
            Console.WriteLine("Checking region...");
            if (!SelectorWindow.IsRegionSelected) return;

            var currentPixels = RegionSelectorWindow.CaptureRegion(_coordinates);
            if (currentPixels.Width != _screenExample.Width || currentPixels.Height != _screenExample.Height)
            {
                _log("WARN: Region size changed, ignoring!!!");
                return;
            }

            bool pixelsAreSame = PixelsAreSame(currentPixels);
            // no change -> don't do anything
            if (pixelsAreSame == _sameRegionFound) return;


            if (!pixelsAreSame)
            {
                // switched away from the screen with the example
                _sameRegionFound = false;
                if (!_taskTracker.IsTracking)
                {
                    _taskTracker.StartTracking();
                }
            }
            else
            {
                // switched back to the screen with the example
                _sameRegionFound = true;
                TriggerTaskCompleted();
            }

        }

        private void TriggerTaskCompleted()
        {
            _taskTracker.LogTask();
        }

        private bool PixelsAreSame(Bitmap currentPixels)
        {
            for (int x = 0; x < _screenExample.Width; x+=2)
            {
                for (int y = 0; y < _screenExample.Height; y+=2)
                {
                    Color expectedColour = _screenExample.GetPixel(x, y);
                    Color seenColour = currentPixels.GetPixel(x, y);
                    if (expectedColour != seenColour)
                    {
                        // region is different
                        return false;
                    }
                }
            }

            return true;
        }

        private void OnRegionSelectionCancelled()
        {
            _timer?.Stop();
        }

        private void OnRegionSelected(Rectangle coordinates, Bitmap rectangle)
        {
            _timer.Start();
            _screenExample = rectangle;
            _coordinates = coordinates;
            _sameRegionFound = true;
            //_taskTracker.StartTracking();
        }
    }
}
