using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace AudioMarcoPolo.UI
{
    public sealed partial class Toast
    {
        // The guidelines recommend using 100px offset for the content animation.
        const int ContentAnimationOffset = 100;
        private readonly DispatcherTimer _dispatcherTimer;

        public Toast(string title, string message, TimeSpan time)
        {
            InitializeComponent();
            ToastContent.Transitions = new TransitionCollection
            {
                new EntranceThemeTransition
                {
                    FromVerticalOffset = ContentAnimationOffset*-1
                }
            };
            ToastContent.DataContext = Branding.Current;
            Branding.Current.PropertyChanged += (o, e) => { ToastContent.Background = Branding.Current.ForegroundBrush; };
            Title.Text = title;
            Message.Text = message;
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += (o, e) => Hide();
            _dispatcherTimer.Interval = time;
            _dispatcherTimer.Start();

        }

        private void Dismiss(object sender, RoutedEventArgs e)
        {
            Hide();
        }
        private void Hide()
        {
            _dispatcherTimer.Stop();
            var parent = Parent as Popup;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }
    }
}
