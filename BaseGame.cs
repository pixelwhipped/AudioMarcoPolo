using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using AudioMarcoPolo.ParticleSystem;
using AudioMarcoPolo.ParticleSystem.ParticleModifiers;
using AudioMarcoPolo.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using AudioMarcoPolo.Audio;
using AudioMarcoPolo.Content;
using AudioMarcoPolo.Input;
using AudioMarcoPolo.Interfaces;
using AudioMarcoPolo.UI;

namespace AudioMarcoPolo
{
    public class BaseGame : Game, IGState
    {
        public static Random Random = new Random();        

        private bool _paused;
        public bool IsPaused
        {
            get
            {
                var p = ApplicationView.Value != ApplicationViewState.FullScreenLandscape || _paused;
                if (ParentInterface != null) ParentInterface.ShowPause(p);
                return p;
            }
            set { _paused = value; }
        }

        public GraphicsDeviceManager Graphics;

        public SpriteBatch SpriteBatch { get; private set; }

        public static Texture2D Pixel;
        public float Width { get { return Graphics.GraphicsDevice.Viewport.Width; } }
        public float Height { get { return Graphics.GraphicsDevice.Viewport.Height; } }
        public Vector2 Center { get { return new Vector2(Width / 2f, Height / 2f); } }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(
                    0, 0, (int)Width, (int)Height);
            }
        }
        public AudioFx Audio { get; set; }

        public IParent ParentInterface;
        private Settings _settings;


        public Settings Settings
        {
            get { return _settings ?? (_settings = new Settings(this)); }
        }

        public BaseGame Game { get { return this; } }

        public GamePersistance<GameData> GameData { get; private set; }
        public TouchInput TouchInput { get; private set; }
        public KeyboardInput KeyboardInput { get; private set; }
        public MouseInput MouseInput { get; private set; }
        public UnifiedInput UnifiedInput { get; private set; }

        public bool IsVisible { get; set; }

        private float _transition;

        public float Transition
        {
            get { return _transition; }
            set { _transition = MathHelper.Clamp(value, 0f, 1f); }
        }

        private bool _fade1Xin;
        private bool _fade2Xin;
        public float FadeX1 { get; protected set; }
        public float FadeX2 { get; protected set; }

        private IGState _currentComponent;
        private IGState _previousComponent;

        private RenderTarget2D _currentTarget;
        private RenderTarget2D _previousTarget;
        private RenderTarget2D _restoreCurrentTarget;

        public ParticleEngine ParticleSystem;
        public Color ParticleColor;

        public bool HasPrevious { get { return false; } }
        public IGState NextComponent { get; set; }

        private List<Vector2> _taps;

        public bool EnableGoBack;
        public Cue BackGroundTune;
       
        public BaseGame()
        {
            IsVisible = true;
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Branding.BackgroundColor = Color.Black;            
        }
        public BaseGame(bool isVisible)
        {
            IsVisible = isVisible;
        }

        public static byte[] LoadStream(string path)
        {
            var s = TitleContainer.OpenStream(path);
            using (var ms = new MemoryStream())
            {
                s.CopyTo(ms);
                return ms.ToArray();

            }
        }

        protected override void Initialize()
        {
            Audio = new AudioFx(this);
            _currentTarget = new RenderTarget2D(Graphics.GraphicsDevice, Graphics.PreferredBackBufferWidth,
                                    Graphics.PreferredBackBufferHeight, false,
                                    SurfaceFormat.Color,
                                   DepthFormat.Depth24,
                                   0,
                                   RenderTargetUsage.PreserveContents);
            _previousTarget = new RenderTarget2D(Graphics.GraphicsDevice, Graphics.PreferredBackBufferWidth,
                                                Graphics.PreferredBackBufferHeight, false,
                                   SurfaceFormat.Color,
                                   DepthFormat.Depth24,
                                   0,
                                   RenderTargetUsage.PreserveContents);
            _currentComponent = this;
            FadeX1 = 1f;
            FadeX2 = 1f;
            _fade1Xin = false;
            _fade2Xin = false;

            base.Initialize();
        }
        protected override void LoadContent()
        {
            _taps = new List<Vector2>();
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            GameData = new GamePersistance<GameData>(this);
            Pixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Pixel.SetData(new[] { Color.White });
            Textures.LoadContent(Content);
            Fonts.LoadContent(Content);

            ParticleSystem = new ParticleEngine();
            ParticleColor = Color.Black;
            MouseInput = new MouseInput();
            TouchInput = new TouchInput();
            UnifiedInput = new UnifiedInput(this);
            KeyboardInput = new KeyboardInput(this);

            
            UnifiedInput.MoveListeners.Add((v) =>
            {
                ParticleSystem.Add(new Emitter(v, new TimeSpan(0, 0, 0, 1),
                    e => ParticleFactory.GenerateParticle(e, Textures.ParticleStar,
                        ParticleColor), 5, 20));
            });
            UnifiedInput.TapListeners.Add(OnTap);            
            UnifiedInput.DraggedListeners.Add(OnDragged);

            //Branding.BackgroundColor = Color.Red;

            IsMouseVisible = false;
            BackGroundTune = Game.Audio.Play(Cues.Background, AudioChannels.Music, true);
            
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected async override void Update(GameTime gameTime)
        {
            if (IsPaused) return;
            if(NextComponent==null)
                NextComponent = new MainMenu(this, null, Color.White);
            if (_currentComponent.Transition >= 1)
            {
                TouchInput.Update(gameTime);
                MouseInput.Update(gameTime);
                UnifiedInput.Update(gameTime);
                KeyboardInput.Update(gameTime);
            }
            
            if (_fade1Xin)
            {
                FadeX1 += 0.025f;
                if (FadeX1 >= 1f) _fade1Xin = false;
            }
            else
            {
                FadeX1 -= 0.025f;
                if (FadeX1 <= .5f) _fade1Xin = true;
            }
            if (_fade2Xin)
            {
                FadeX2 += 0.05f;
                if (FadeX2 >= 1f) _fade2Xin = false;
            }
            else
            {
                FadeX2 -= 0.05f;
                if (FadeX2 <= 0.5f) _fade2Xin = true;
            }

            if ((_currentComponent ?? this).Transition >= 1f)
                NextComponent = (_currentComponent ?? this).Update(gameTime, this);

            if (_currentComponent != null)
            {
                _currentComponent.Transition += 0.05f;
                if (EnableGoBack)
                {
                    if (_currentComponent.HasPrevious &&
                        _taps.Any(t => new Rectangle((int) Width - 64, 0, 64, 64).Contains(t)))
                    {
                        //Audio.Play(Cues.Fail);
                        _currentComponent.Back();
                    }


                    if (_currentComponent.HasPrevious && KeyboardInput.TypedKey(Keys.Escape))
                    {
                        //Audio.Play(Cues.Fail);
                        _currentComponent.Back();
                    }
                }
            }
            if (_previousComponent != null)
                _previousComponent.Transition -= 0.05f;

            if (NextComponent != null && NextComponent != _currentComponent)
            {
                GameData.Save();
                _previousComponent = _currentComponent;
                _currentComponent = NextComponent;
            }

            _taps.Clear();
            base.Update(gameTime);
        }

        public void BeginRenderTargetDraw(RenderTarget2D target)
        {
            Graphics.GraphicsDevice.SetRenderTarget(target);
        }

        public void EndRenderTargetDraw()
        {
            Graphics.GraphicsDevice.SetRenderTarget(_restoreCurrentTarget);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (IsPaused) return;
            ParticleSystem.Update(gameTime);
            if (_currentComponent != null)
            {
                _restoreCurrentTarget = _currentTarget;
                Graphics.GraphicsDevice.SetRenderTarget(_currentTarget);
                Graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 1.0f, 0);
                _currentComponent.Draw(gameTime, this);
            }
            if (_previousComponent != null)
            {
                _restoreCurrentTarget = _previousTarget;
                Graphics.GraphicsDevice.SetRenderTarget(_previousTarget);
                Graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 1.0f, 0);
                _previousComponent.Draw(gameTime, this);
            }
            _restoreCurrentTarget = null;
            Graphics.GraphicsDevice.SetRenderTarget(null);

            GraphicsDevice.Clear(Branding.BackgroundColor);

            SpriteBatch.Begin();
            if (_currentComponent != null)
                SpriteBatch.Draw(_currentTarget, Vector2.Zero, Color.White * _currentComponent.Transition);
            if (_previousComponent != null)
                SpriteBatch.Draw(_previousTarget, Vector2.Zero, Color.White * _previousComponent.Transition);

            if (EnableGoBack)
            {
                if (_currentComponent != null && _currentComponent.HasPrevious)
                    SpriteBatch.Draw(Textures.Back, new Rectangle((int) Width - 64, 0, 64, 64), null, Color.White);
            }
           // SpriteBatch.Draw(Textures.Cursor, new Rectangle((int)MouseInput.X, (int)MouseInput.Y, Textures.Cursor.Width, Textures.Cursor.Height), null,
           //                      Color.White * MouseInput.Fade);
            ParticleSystem.Draw(SpriteBatch);
            SpriteBatch.End();            
            KeyboardInput.Draw(SpriteBatch);
            base.Draw(gameTime);
        }


        public IGState Update(GameTime gameTime, BaseGame game)
        {
            return this;
        }

        public void Draw(GameTime gameTime, BaseGame game)
        {

        }

        public void OnTap(Vector2 a)
        {
            if (_currentComponent!=this) _currentComponent.OnTap(a);
        }

        public void OnDragged(Vector2 a, Vector2 b)
        {
            if (_currentComponent!=this) _currentComponent.OnDragged(a, b);
        }

        public void Back()
        {

        }

        public async Task<IUICommand> ShowMessageAsync(string title, string message, Action onOk = null, Action onCancel = null)
        {

            var md = new MessageDialog(message, title);

            md.Commands.Add(new UICommand("Ok", ui => { if (onOk != null) onOk(); }));
            if (onCancel != null)
                md.Commands.Add(new UICommand("Cancel", ui => onCancel()));
            var c = await md.ShowAsync();
            return c;
        }

        public void ShowToast(string toast, string title = "Achievement", TimeSpan? time = null)
        {
            if (ParentInterface != null)
                ParentInterface.ShowToast(toast, title, time ?? TimeSpan.FromSeconds(5));
        }

    }
}
