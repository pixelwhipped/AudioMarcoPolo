using Microsoft.Xna.Framework;
using System.ComponentModel;
using Windows.UI.Xaml.Media;

namespace AudioMarcoPolo.UI
{

    // Create a class that implements INotifyPropertyChanged.
    public class Branding : INotifyPropertyChanged
    {
        public static Branding Current
        {
            get { return _instance ?? (_instance = new Branding()); }
        }
        private static Branding _instance;
        private static Color _backgroundColor = Color.DarkGreen;

        public static Color BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                _backgroundColor = value;
                // Call NotifyPropertyChanged when the source property 
                // is updated.
                Current.ForegroundBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(ForegroundColor.A, ForegroundColor.R, ForegroundColor.G, ForegroundColor.B));
                Current.BackgroundBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(BackgroundColor.A, BackgroundColor.R, BackgroundColor.G, BackgroundColor.B));
                Current.BoarderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(BoarderColor.A, BoarderColor.R, BoarderColor.G, BoarderColor.B));

            }
        }
        public static Color ForegroundColor { get { return Color.Lerp(_backgroundColor, Color.White, .2f); } }
        public static Color BoarderColor { get { return Color.Lerp(ForegroundColor, Color.Black, .2f); } }

        private SolidColorBrush _foregroundBrush;
        private SolidColorBrush _backgroundBrush;
        private SolidColorBrush _boarderBrush;

        // Declare the PropertyChanged event.
        public event PropertyChangedEventHandler PropertyChanged;

        public Branding()
        {
            _foregroundBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(ForegroundColor.A, ForegroundColor.R, ForegroundColor.G, ForegroundColor.B));
            _backgroundBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(BackgroundColor.A, BackgroundColor.R, BackgroundColor.G, BackgroundColor.B));
            _boarderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(BoarderColor.A, BoarderColor.R, BoarderColor.G, BoarderColor.B));
        }

        // Create the property that will be the source of the binding.
        public SolidColorBrush ForegroundBrush
        {
            get { return _foregroundBrush; }
            set
            {
                _foregroundBrush = value;
                // Call NotifyPropertyChanged when the source property 
                // is updated.
                NotifyPropertyChanged("ForgroundBrush");
            }
        }
        public SolidColorBrush BackgroundBrush
        {
            get { return _backgroundBrush; }
            set
            {
                _backgroundBrush = value;
                // Call NotifyPropertyChanged when the source property 
                // is updated.
                NotifyPropertyChanged("BackgroundBrush");
            }
        }
        public SolidColorBrush BoarderBrush
        {
            get { return _boarderBrush; }
            set
            {
                _boarderBrush = value;
                // Call NotifyPropertyChanged when the source property 
                // is updated.
                NotifyPropertyChanged("BoarderBrush");
            }
        }

        // NotifyPropertyChanged will raise the PropertyChanged event, 
        // passing the source property that is being updated.
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }
    }

}
