using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
using MonoGame.Framework;
using AudioMarcoPolo.Interfaces;
using AudioMarcoPolo.UI;

namespace AudioMarcoPolo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : IParent
    {
        readonly BaseGame _game;

        // Used to determine the correct height to ensure our custom UI fills the screen.
        private Rect _windowBounds;

        // Desired width for the settings UI. UI guidelines specify this should be 346 or 646 depending on your needs.
        private const double SettingsWidth = 346;

        // This is the container that will hold our custom content.
        private Popup _settingsPopup;
        private readonly Popup _toastPopup = new Popup();
        private readonly Popup _pausePopup = new Popup();
        private Pause _pausePane;

        private readonly Queue<DelayedToast> _toasts;

        public GamePage(string launchArguments)
        {
            _toasts = new Queue<DelayedToast>();
            InitializeComponent();

            // Create the game.
            _game = XamlGame<BaseGame>.Create(launchArguments, Window.Current.CoreWindow, this);
            _game.ParentInterface = this;
            _windowBounds = Window.Current.Bounds;
            // Added to listen for events when the window size is updated.
            Window.Current.SizeChanged += (s, e) =>
            {
                _windowBounds = Window.Current.Bounds;
                _pausePopup.Width = _windowBounds.Width;
                _pausePopup.Height = _windowBounds.Height;
            };
            SettingsPane.GetForCurrentView().CommandsRequested += (s, e) =>
            {
                UICommandInvokedHandler handler = command =>
                {
                    // Create a Popup window which will contain our flyout.
                    if (_settingsPopup == null)
                    {
                        _settingsPopup = new Popup();

                        _settingsPopup.Closed += (s2, e2) =>
                        {
                            Window.Current.Activated -= OnWindowActivated;
                            if (_toasts.Count == 0) return;
                            var t = _toasts.Dequeue();
                            ShowToast(t.Toast, t.Title, t.Time);
                        };
                        Window.Current.Activated += OnWindowActivated;

                        _settingsPopup.IsLightDismissEnabled = true;
                        _settingsPopup.Width = SettingsWidth;
                        _settingsPopup.Height = _windowBounds.Height;

                        // Add the proper animation for the panel.
                        _settingsPopup.ChildTransitions = new TransitionCollection
                                                                    {
                                                                        new PaneThemeTransition
                                                                        {
                                                                            Edge =
                                                                                (SettingsPane.Edge ==
                                                                                SettingsEdgeLocation.Right)
                                                                                    ? EdgeTransitionLocation.Right
                                                                                    : EdgeTransitionLocation.Left
                                                                        }
                                                                    };

                        // Create a SettingsFlyout the same dimenssions as the Popup.
                        var mypane = _game.Settings;
                        mypane.Width = SettingsWidth;
                        mypane.Height = _windowBounds.Height;
                        _settingsPopup.Child = mypane;
                    }
                    _settingsPopup.SetValue(Canvas.LeftProperty, SettingsPane.Edge == SettingsEdgeLocation.Right ? (_windowBounds.Width - SettingsWidth) : 0);
                    _settingsPopup.SetValue(Canvas.TopProperty, 0);
                    _settingsPopup.IsOpen = true;
                };
                e.Request.ApplicationCommands.Add(new SettingsCommand("SettingsID", "Game Settings", handler));
            };


        }

        public void ShowToast(string toast, string title = "Achievement", TimeSpan? time = null)
        {
            if (_toastPopup.IsOpen || _settingsPopup != null && _settingsPopup.IsOpen)
            {
                _toasts.Enqueue(new DelayedToast
                {
                    Title = title,
                    Toast = toast,
                    Time = time ?? TimeSpan.FromSeconds(5)
                });
                return;
            }

            //  settingsPopup.IsLightDismissEnabled = true;
            _toastPopup.Width = _windowBounds.Width;
            _toastPopup.Height = 80;

            // Add the proper animation for the panel.
            _toastPopup.ChildTransitions = new TransitionCollection
                {
                    new PaneThemeTransition
                    {
                        Edge = EdgeTransitionLocation.Top
                    }
                };

            // Create a SettingsFlyout the same dimenssions as the Popup.
            var mypane = new Toast(title, toast, time ?? TimeSpan.FromSeconds(5))
            {
                Width = _windowBounds.Width,
                Height = 80
            };
            // Place the SettingsFlyout inside our Popup window.
            _toastPopup.Child = mypane;

            // Let's define the location of our Popup.
            _toastPopup.SetValue(Canvas.LeftProperty, 0);
            _toastPopup.SetValue(Canvas.TopProperty, 0);

            _toastPopup.Closed += (s, e) =>
            {
                if (_toasts.Count == 0) return;
                var t = _toasts.Dequeue();
                ShowToast(t.Toast, t.Title, t.Time);

            };
            _toastPopup.IsOpen = true;
        }


        public void ShowPause(bool pause)
        {
            if (pause)
            {
                _pausePopup.Width = _windowBounds.Width;
                _pausePopup.Height = _windowBounds.Height;
                if (_pausePopup.IsOpen)
                {
                    _pausePane.Width = _windowBounds.Width;
                    _pausePane.Height = _windowBounds.Height;
                    return;
                }

                _pausePane = new Pause
                {
                    Width = _windowBounds.Width,
                    Height = _windowBounds.Height,
                };
                _pausePopup.Child = _pausePane;
                _pausePopup.SetValue(Canvas.LeftProperty, 0);
                _pausePopup.SetValue(Canvas.TopProperty, 0);
                _pausePopup.IsOpen = true;
                return;
            }
            _pausePopup.IsOpen = false;
        }

        /// <summary>
        /// We use the window's activated event to force closing the Popup since a user maybe interacted with
        /// something that didn't normally trigger an obvious dismiss.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="e">Event data describing the conditions that led to the event.</param>
        private void OnWindowActivated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                _settingsPopup.IsOpen = false;
            }
        }
    }
}