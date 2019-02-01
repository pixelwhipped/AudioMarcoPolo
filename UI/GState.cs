using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AudioMarcoPolo.Input;
using AudioMarcoPolo.Interfaces;

namespace AudioMarcoPolo.UI
{
    public abstract class GState: IGState, IDisposable
    {



        public BaseGame Game
        {
            get { return _game; }
        }
        private readonly SpriteBatch _spriteBatch;

        public SpriteBatch SpriteBatch
        {
            get { return _spriteBatch; }
        }

        public Vector2 Center
        {
            get { return Game.Center; }
        }

        public float Width
        {
            get { return Game.Width; }
        }

        public float Height
        {
            get { return Game.Height; }
        }

        public bool IsVisible
        {
            get { return Math.Abs(Transition - 0f) > float.Epsilon;}
            set { Transition = (value)?1f:0f;}
        }

        public Settings Settings
        {
            get { return Game.Settings; }
        }

        public GamePersistance<GameData> GameData
        {
            get { return Game.GameData; }
        }        

        private float _transition;

        public float Transition
        {
            get { return _transition; }
            set { _transition = MathHelper.Clamp(value, 0f, 1f); }
        }

        public bool HasPrevious
        {
            get { return PreviousComponent != null; }
        }

        public IGState NextComponent { get; set; }

        public IGState PreviousComponent;
        

        protected GState(BaseGame game, IGState previous)
        {
            
            _game = game;
            _spriteBatch = new SpriteBatch(_game.GraphicsDevice);
            _game.KeyboardInput.IsOSKVisable = false;                        
            if (previous != null)
                previous.NextComponent = null;
            PreviousComponent = previous;
            NextComponent = null;
        }

        
        private readonly BaseGame _game;
        public KeyboardInput KeyboardInput
        {
            get { return _game.KeyboardInput; }
        }

        public UnifiedInput UnifiedInput
        {
            get { return _game.UnifiedInput; }
        }

    

        public abstract IGState Update(GameTime gameTime, BaseGame grid);

        public abstract void Draw(GameTime gameTime, BaseGame grid);
        public virtual void OnTap(Vector2 a)
        {
            
        }

        public virtual void OnDragged(Vector2 a, Vector2 b)
        {
            
        }

        public virtual void Back()
        {
            PreviousComponent.NextComponent = null;
            NextComponent = PreviousComponent;
        }


        public void Dispose()
        {
            SpriteBatch.Dispose();
        }
    }
}
