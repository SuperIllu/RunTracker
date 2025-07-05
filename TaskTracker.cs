using System.Diagnostics;

namespace TaskTrackerWPF
{
    public class TaskTracker
    {
        private float _minDistance;
        public bool IsTracking { get; private set; }  = false;
        private Action<string> _logAction;
        public event Action OnLoggingStarted;
        public event Action OnLoggingStopped;
        public event Action<float> OnTaskLogged;

        private Stopwatch _stopwatch = new Stopwatch();
        private List<float> _taskDurations;

        public TaskTracker(Action<string> logAction, float minDistance = 10)
        {
            // Set the logging action to be used for task tracking
            _logAction = logAction;
            _minDistance = minDistance;
            Debug.Assert(_logAction != null);
        }


        public void StartTracking()
        {
            if (IsTracking)
            {
                _logAction("Already tracking");
                return;
            }
            IsTracking = true;
            _logAction("Tracking active");
            _stopwatch.Restart();
            _taskDurations = new List<float>();
            OnLoggingStarted?.Invoke();
        }


        public void StopTracking()
        {
            if (!IsTracking)
            {
                _logAction("Not currently tracking");
                return;
            }

            _logAction("Tracking stopped");
            _stopwatch.Stop();
            IsTracking = false;

            if (_taskDurations.Count > 0)
            {
                float totalDuration = _taskDurations.Sum();
                _logAction($"#Tasks: {_taskDurations.Count} in {totalDuration}s");
                float longestTask = _taskDurations.Max();
                float shortestTask = _taskDurations.Min();
                _logAction($"Average: {totalDuration / _taskDurations.Count}s [{shortestTask}s; {longestTask}s]");
            }
            else
            {
                _logAction("No tasks were logged during this session.");
            }
            OnLoggingStopped?.Invoke();
        }

        public void LogTask()
        {
            if (!IsTracking)
            {
                return;
            }
            
            float elapsedTime = _stopwatch.ElapsedMilliseconds / 1000f; // Convert milliseconds to seconds
            if (elapsedTime < _minDistance)
            {
                return;
            }

            elapsedTime = (float)Math.Round(elapsedTime, 2); // Round to 2 decimal places
            _taskDurations.Add(elapsedTime);
            _logAction($"Task {_taskDurations.Count} took: {elapsedTime}s");

            _stopwatch.Restart(); // Reset the stopwatch for the next task
            OnTaskLogged?.Invoke(elapsedTime);
        }

        public float GetCurrentTaskDuration()
        {
            if (!IsTracking)
            {
                return 0f;
            }
            return _stopwatch.ElapsedMilliseconds / 1000f; // Convert milliseconds to seconds
        }

        public float GetLastTaskDuration()
        {
            if (_taskDurations.Count == 0)
            {
                return 0f;
            }
            return _taskDurations.Last();
        }

        public float GetAverageTaskDuration()
        {
            if (_taskDurations.Count == 0)
            {
                return 0f;
            }
            return _taskDurations.Average();
        }

        internal IEnumerable<float> GetTaskList()
        {
            return _taskDurations.AsEnumerable();
        }

        internal void PauseTracking()
        {
            throw new NotImplementedException();
        }
    }
}
