using System;
using System.Collections.Generic;
using System.Drawing;
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

namespace TaskTrackerWPF
{
    /// <summary>
    /// Interaction logic for RegionSelectorWindow.xaml
    /// </summary>
    public partial class RegionSelectorWindow : Window
    {
        public Rectangle SelectedRegion { get; private set; }
        public Bitmap ReferenceImage { get; private set; }

        public bool IsRegionSelected { get; private set; } = false;
        public event Action<Rectangle, Bitmap> OnRegionSelected;
        public event Action OnRegionSelectionCancelled;

        public RegionSelectorWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        public void ShowSelector()
        {
            // Set the window to be topmost and show it
            this.Topmost = true;
            this.Show();
            this.Activate();
            this.IsRegionSelected = false;
            OnRegionSelectionCancelled?.Invoke();
        }

        private async void LockIn_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            await Task.Delay(200);

            // Get screen position and size
            var screenLeft = (int)this.Left;
            var screenTop = (int)this.Top;
            var screenWidth = (int)this.ActualWidth;
            var screenHeight = (int)this.ActualHeight;

            SelectedRegion = new System.Drawing.Rectangle(screenLeft, screenTop, screenWidth, screenHeight);

            // Capture screen area behind this window
            ReferenceImage = CaptureRegion(SelectedRegion);

            // Hide the selector window (could also use Visibility or Opacity)
            IsRegionSelected = true;
            OnRegionSelected?.Invoke(SelectedRegion, ReferenceImage);
        }

        public static Bitmap CaptureRegion(System.Drawing.Rectangle region)
        {
            Bitmap bmp = new Bitmap(region.Width, region.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(region.Location, System.Drawing.Point.Empty, region.Size);
            }
            return bmp;
        }
    }

}
