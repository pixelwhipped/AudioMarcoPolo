using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioMarcoPolo.Input;
using AudioMarcoPolo.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AudioMarcoPolo.UI
{
    public abstract class GComponent : IGComponent
    {
        private bool _visible;
        private readonly BaseGame _game;
        public KeyboardInput KeyboardInput
        {
            get { return _game.KeyboardInput; }
        }

        public UnifiedInput UnifiedInput
        {
            get { return _game.UnifiedInput; }
        }

        public virtual Vector2 Center
        {
            get { return _game.Center; }
        }

        public virtual float Width
        {
            get { return _game.Width; }
        }

        public virtual float Height
        {
            get { return _game.Height; }
        }

        
        public virtual bool IsVisible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        public Rectangle Bounds
        {
            get
        }
        public BaseGame Game
        {
            get { return _game; }
        }
        private readonly SpriteBatch _spriteBatch;

        public SpriteBatch SpriteBatch
        {
            get { return _spriteBatch; }
        }

        protected GComponent(BaseGame game)
        {
            _game = game;
            _spriteBatch = new SpriteBatch(_game.GraphicsDevice);
            _game.KeyboardInput.IsOSKVisable = false;
            _visible = true;
        }

        public abstract IGState Update(GameTime gameTime, BaseGame grid);

        public abstract void Draw(GameTime gameTime, BaseGame grid);
    }
}
