using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using AudioMarcoPolo.UI;
using AudioMarcoPolo.Utilities;

namespace AudioMarcoPolo.Input
{
    public class KeyboardInput
    {
        #region Key Definitions
        #region Key Strings
        private const string D = "D";
        private const string DecimalStr = "Decimal";
        private const string DivideStr = "Divide";
        private const string Zero = "0";
        private const string One = "1";
        private const string Two = "2";
        private const string Three = "3";
        private const string Four = "4";
        private const string Five = "5";
        private const string Six = "6";
        private const string Seven = "7";
        private const string Eight = "8";
        private const string Nine = "9";
        private const string ZeroShift = "}";
        private const string OneShift = "!";
        private const string TwoShift = "@";
        private const string ThreeShift = "#";
        private const string FourShift = "$";
        private const string FiveShift = "%";
        private const string SixShift = "^";
        private const string SevenShift = "&";
        private const string EightShift = "*";
        private const string NineShift = "(";
        private const string NumPad = "NumPad";
        private const string Dot = ".";
        private const string OemCommaStr = "OemComma";
        private const string OemComma = ",";
        private const string OemCommaShift = "<";
        private const string OemPeriodStr = "OemPeriod";
        private const string OemPeriod = ".";
        private const string OemPeriodShift = ">";
        private const string OemQuestionStr = "OemQuestion";
        private const string OemQuestion = "/";
        private const string OemQuestionShift = "?";
        private const string OemSemicolonStr = "OemSemicolon";
        private const string OemSemicolon = ";";
        private const string OemSemicolonShift = ":";
        private const string OemQuotesStr = "OemQuotes";
        private const string OemQuotes = "'";
        private const string OemQuotesShift = "\"";
        private const string OemOpenBracketsStr = "OemOpenBrackets";
        private const string OemOpenBrackets = "[";
        private const string OemOpenBracketsShift = "{";
        private const string OemCloseBracketsStr = "OemCloseBrackets";
        private const string OemCloseBrackets = "]";
        private const string OemCloseBracketsShift = "}";
        private const string OemPipeStr = "OemPipe";
        private const string OemPipe = "\\";
        private const string OemPipeShift = "|";
        private const string OemPlusStr = "OemPlus";
        private const string OemPlus = "=";
        private const string OemPlusShift = "+";
        private const string OemMinusStr = "OemMinus";
        private const string OemMinus = "-";
        private const string OemMinusShift = "_";
        private const string TabStr = "Tab";
        private const string Tab = "     ";
        private const string MultiplyStr = "Multiply";
        private const string Multiply = "*";
        private const string Divide = "/";
        private const string SubtractStr = "Subtract";
        private const string Subtract = "-";
        private const string AddStr = "Add";
        private const string Add = "+";
        private const string Space = " ";
        #endregion

        #region Alpha Keys
        private readonly Keys[] _alphaKeys =
            {
                Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I,
                Keys.J, Keys.K, Keys.L, Keys.M,
                Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V,
                Keys.W, Keys.X, Keys.Y, Keys.Z
            };
        #endregion

        #region All Used Keys
        private readonly Keys[] _keys =
            {
                Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I,
                Keys.J, Keys.K, Keys.L, Keys.M,
                Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V,
                Keys.W, Keys.X, Keys.Y, Keys.Z,
                Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7,
                Keys.D8, Keys.D9,
                Keys.NumPad0, Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4,
                Keys.NumPad5, Keys.NumPad6, Keys.NumPad7, Keys.NumPad8, Keys.NumPad9,
                Keys.OemComma, Keys.OemPeriod, Keys.OemQuestion, Keys.OemSemicolon,
                Keys.OemQuotes, Keys.OemOpenBrackets, Keys.OemCloseBrackets, Keys.OemPipe,
                Keys.OemPlus, Keys.OemMinus,
                Keys.Tab, Keys.Divide, Keys.Multiply, Keys.Subtract, Keys.Add, Keys.Decimal
            };
        #endregion

        #region Numerical Keys
        private readonly Keys[] _numericKeys =
            {
                Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6,
                Keys.D7,
                Keys.D8, Keys.D9
            };
        #endregion
        #endregion


        #region Members
        //  private KeyboardState _currKey;
        //  private KeyboardState _prevKey;
        private readonly List<Procedure<KeyboardInput>> _keyboardListeners;
        public BaseGame Game;
        #endregion

        #region String Input
        private string _currentString = string.Empty;
        public string CurrentLine
        {
            get { return _currentString; }
            set { _currentString = value; }
        }
        public string CurrentChar
        {
            get
            {
                return _currentString == string.Empty ? string.Empty : _currentString.ToCharArray()[_currentString.Length - 1].ToString();
            }
        }

        public List<String> Typed;

        private string _previousString = string.Empty;
        public string PreviousLine
        {
            get { return _previousString; }
        }
        #endregion

        #region OSK

        private class OSKButton
        {
            internal Tween TTL;
            internal Rectangle Rect;
        }


        private readonly List<OSKButton> _oskButtonHits;
        private readonly Texture2D _oskUpper;
        private readonly Texture2D _oskLower;
        private readonly Texture2D _oskFrame;
        private readonly Texture2D _oskShift;
        private readonly Texture2D _oskUnShift;

        private Tween _oskShowHide;
        private Vector2 _oskOffset;
        private bool _oskVisible;
        public Color OSKColor;
        public float OSKAlpha;
        private readonly Dictionary<Keys, Rectangle> _oskRects;
        private Rectangle _oskExitRect = new Rectangle(22, 190, 69, 43);
        private Rectangle _oskCapsRect = new Rectangle(4, 98, 51, 43);
        private readonly List<Keys> _oskCurrentState;
        private readonly List<Keys> _oskPreviousState;
        private bool _isOSKCaps;

        public Rectangle OSKArea;

        public AlignmentX OSKAlignment;
        public bool IsOSKVisable
        {
            set
            {
                if (value == _oskVisible || !_oskShowHide.IsComplete) return;
                if (value)
                {
                    _oskVisible = true;
                    _oskShowHide = new Tween(new TimeSpan(0, 0, 0, 1), 0, 1);
                }
                else
                {
                    _oskVisible = false;
                    _oskShowHide = new Tween(new TimeSpan(0, 0, 0, 1), 1, 0);
                }
            }
            get
            {
                return _oskVisible && _oskShowHide.IsComplete;
            }
        }

        private readonly List<Vector2> _taps;
        #endregion

        public KeyboardInput(BaseGame game)
            : this(game, Color.White, .75f, false)
        {

        }
        public KeyboardInput(BaseGame game, Color oskColor, float oskAlpha, bool visible)
        {
            Game = game;
            Typed = new List<string>();
            _oskShowHide = new Tween(new TimeSpan(0, 0, 0, 0, 500), 1, 0);
            _oskShowHide.Finish();
            _oskButtonHits = new List<OSKButton>();
            OSKAlpha = oskAlpha;
            OSKColor = oskColor;
            _oskVisible = visible;
            _oskLower = Game.Content.Load<Texture2D>(@"OSK\OSKLC.png");
            _oskUpper = Game.Content.Load<Texture2D>(@"OSK\OSKUC.png");
            _oskFrame = Game.Content.Load<Texture2D>(@"OSK\OSKF.png");
            _oskUnShift = Game.Content.Load<Texture2D>(@"OSK\OSKUSFT.png");
            _oskShift = Game.Content.Load<Texture2D>(@"OSK\OSKSFT.png");
            OSKAlignment = AlignmentX.Center;
            _oskRects = new Dictionary<Keys, Rectangle>
            {
                {Keys.Escape, new Rectangle(3, 5, 53, 45)},
                {Keys.NumPad1, new Rectangle(58,5,33,45)},
                {Keys.NumPad2, new Rectangle(94,5,33,45)},
                {Keys.NumPad3, new Rectangle(130,5,33,45)},
                {Keys.NumPad4, new Rectangle(166,5,33,45)},
                {Keys.NumPad5, new Rectangle(200,5,33,45)},
                {Keys.NumPad6, new Rectangle(238,5,33,45)},
                {Keys.NumPad7, new Rectangle(274,5,33,45)},
                {Keys.NumPad8, new Rectangle(310,5,33,45)},
                {Keys.NumPad9, new Rectangle(346,5,33,45)},
                {Keys.NumPad0, new Rectangle(382,5,33,45)},
               // {Keys.D1, new Rectangle(58,5,33,45)},
               // {Keys.D2, new Rectangle(94,5,33,45)},
               // {Keys.D3, new Rectangle(130,5,33,45)},
               // {Keys.D4, new Rectangle(166,5,33,45)},
               // {Keys.D5, new Rectangle(200,5,33,45)},
               // {Keys.D6, new Rectangle(238,5,33,45)},
               // {Keys.D7, new Rectangle(274,5,33,45)},
               // {Keys.D8, new Rectangle(310,5,33,45)},
               // {Keys.D9, new Rectangle(346,5,33,45)},
               // {Keys.D0, new Rectangle(382,5,33,45)},
                {Keys.Back, new Rectangle(417,5,52,45)},
                {Keys.Tab, new Rectangle(4,52,51,43)},
                {Keys.Q, new Rectangle(58,52,33,43)},
                {Keys.W, new Rectangle(94,52,33,43)},
                {Keys.E, new Rectangle(130,52,33,43)},
                {Keys.R, new Rectangle(166,52,33,43)},
                {Keys.T, new Rectangle(200,52,33,43)},
                {Keys.Y, new Rectangle(238,52,33,43)},
                {Keys.U, new Rectangle(274,52,33,43)},
                {Keys.I, new Rectangle(310,52,33,43)},
                {Keys.O, new Rectangle(346,52,33,43)},
                {Keys.P, new Rectangle(382,52,33,43)},
                {Keys.Enter, new Rectangle(418,52,51,89)},
                {Keys.CapsLock, new Rectangle(4,98,51,43)},
                {Keys.A, new Rectangle(58,98,33,43)},
                {Keys.S, new Rectangle(94,98,33,43)},
                {Keys.D, new Rectangle(130,98,33,43)},
                {Keys.F, new Rectangle(166,98,33,43)},
                {Keys.G, new Rectangle(200,98,33,43)},
                {Keys.H, new Rectangle(238,98,33,43)},
                {Keys.J, new Rectangle(274,98,33,43)},
                {Keys.K, new Rectangle(310,98,33,43)},
                {Keys.L, new Rectangle(346,98,33,43)},
                
                {Keys.LeftShift, new Rectangle(3,143,53,44)},
                {Keys.Z, new Rectangle(58,144,33,43)},
                {Keys.X, new Rectangle(94,144,33,43)},
                {Keys.C, new Rectangle(130,144,33,43)},
                {Keys.V, new Rectangle(166,144,33,43)},
                {Keys.B, new Rectangle(200,144,33,43)},
                {Keys.N, new Rectangle(238,144,33,43)},
                {Keys.M, new Rectangle(274,144,33,43)},
                
                {Keys.OemQuotes, new Rectangle(382,98,33,43)}, //"               
                {Keys.OemComma, new Rectangle(310,144,33,43)}, //;
                {Keys.OemPeriod, new Rectangle(346,144,33,43)}, //:

                //{Keys.OemSemicolon, new Rectangle(346,52,33,43)}, SHIFT oemSemicolon
                {Keys.Up, new Rectangle(382,144,51,43)},
                {Keys.RightShift, new Rectangle(418,143,51,44)},
                {Keys.Space, new Rectangle(94,190,249,43)},
                {Keys.Left, new Rectangle(346,190,33,43)},
                {Keys.Down, new Rectangle(382,190,33,43)},
                {Keys.Right, new Rectangle(418,190,33,43)}
                };

            _keyboardListeners = new List<Procedure<KeyboardInput>>();
            _taps = new List<Vector2>();
            _oskCurrentState = new List<Keys>();
            _oskPreviousState = new List<Keys>();
            OSKArea = new Rectangle((int)(Game.Center.X - (_oskLower.Width / 2f)), (int)Game.Height, _oskLower.Width,
                _oskLower.Height);
        }
        public void Draw(SpriteBatch batch)
        {
            batch.Begin();
            batch.Draw(_oskFrame, _oskOffset, null, Color.White * OSKAlpha);

            if ((Pressed(Keys.LeftShift) || Pressed(Keys.RightShift) && !_isOSKCaps))
            {
                batch.Draw(_oskUpper, _oskOffset, null, Color.White * OSKAlpha);
            }
            else if (_isOSKCaps)
            {
                batch.Draw(_oskUpper, _oskOffset, null, Color.White * OSKAlpha);
                var r = new Rectangle((int)(_oskOffset.X + _oskCapsRect.X), (int)(_oskOffset.Y + _oskCapsRect.Y), _oskCapsRect.Width,
                    _oskCapsRect.Height);
                batch.Draw(BaseGame.Pixel, r, null, (Branding.ForegroundColor * .5f));
            }
            else
            {
                batch.Draw(_oskLower, _oskOffset, null, Color.White * OSKAlpha);
            }
            if (Pressed(Keys.LeftShift) || Pressed(Keys.RightShift))
            {
                batch.Draw(_oskUnShift, _oskOffset, null, Color.White * OSKAlpha);
            }
            else
            {
                batch.Draw(_oskShift, _oskOffset, null, Color.White * OSKAlpha);
            }
            batch.End();
            batch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            foreach (var b in _oskButtonHits)
            {
                var r = new Rectangle((int)(_oskOffset.X + b.Rect.X), (int)(_oskOffset.Y + b.Rect.Y), b.Rect.Width,
                    b.Rect.Height);
                batch.Draw(BaseGame.Pixel, r, null, (Branding.ForegroundColor * b.TTL) * .5f);
                // batch.Draw(BaseGame.Pixel, new Rectangle(r.X,r.Y,r.Width,1), null, Color.Blue);
                // batch.Draw(BaseGame.Pixel, new Rectangle(r.X, r.Y+r.Height, r.Width, 1), null, Color.Blue);
                // batch.Draw(BaseGame.Pixel, new Rectangle(r.X, r.Y, 1, r.Height), null, Color.Blue);
                // batch.Draw(BaseGame.Pixel, new Rectangle(r.X+r.Width, r.Y, 1, r.Height), null, Color.Blue);
            }
            batch.End();
        }
        public void Update(GameTime gameTime)
        {
            #region update ui area
            _oskShowHide.Update(gameTime.ElapsedGameTime);
            switch (OSKAlignment)
            {
                case AlignmentX.Left:
                    _oskOffset = new Vector2(0, Game.Height - (_oskLower.Height * (float)Math.Sin(_oskShowHide * (MathHelper.PiOver2))));
                    break;
                case AlignmentX.Center:
                    _oskOffset = new Vector2(Game.Center.X - (_oskLower.Width / 2f), Game.Height - (_oskLower.Height * (float)Math.Sin(_oskShowHide * (MathHelper.PiOver2))));
                    break;
                case AlignmentX.Right:
                    _oskOffset = new Vector2(Game.Width - _oskLower.Width, Game.Height - (_oskLower.Height * (float)Math.Sin(_oskShowHide * (MathHelper.PiOver2))));
                    break;
            }
            OSKArea = new Rectangle((int)_oskOffset.X, (int)_oskOffset.Y, _oskLower.Width, _oskLower.Height);

            foreach (var b in _oskButtonHits)
                b.TTL.Update(gameTime.ElapsedGameTime);
            _oskButtonHits.RemoveAll(b => b.TTL.IsComplete);
            #endregion

            #region reset states
            _oskPreviousState.Clear();
            _oskPreviousState.AddRange(_oskCurrentState);
            _oskCurrentState.Clear();

            foreach (var k in Keyboard.GetState().GetPressedKeys().Where(k => !_oskCurrentState.Contains(k)))
                _oskCurrentState.Add(k);

            _taps.Clear();
            if (Game.UnifiedInput.Action)
                _taps.Add(Game.UnifiedInput.Location);
            foreach (var t in Game.TouchInput.Touches.Where(t => !_taps.Contains(t)))
            {
                _taps.Add(t);
            }
            if (_oskVisible)
            {
                //bool shift;
                //var shR = new Rectangle(_oskRects[Keys.LeftShift].X + (int)_oskOffset.X, _oskRects[Keys.LeftShift].Y + (int)_oskOffset.Y,
                //        _oskRects[Keys.LeftShift].Width, _oskRects[Keys.LeftShift].Width);
                //var shL = new Rectangle(_oskRects[Keys.LeftShift].X + (int)_oskOffset.X, _oskRects[Keys.LeftShift].Y + (int)_oskOffset.Y,
                //        _oskRects[Keys.LeftShift].Width, _oskRects[Keys.LeftShift].Width);
                //if (_taps.Any(k => shR.Contains(k) || shL.Contains(k)))
                // {
                //    shift = true;
                // }

                foreach (var t in _taps)
                {

                    var p = new Point((int)t.X, (int)t.Y);
                    foreach (var r in _oskRects)
                    {
                        var or = new Rectangle(r.Value.X + (int)_oskOffset.X, r.Value.Y + (int)_oskOffset.Y,
                            r.Value.Width,
                            r.Value.Height);
                        if (or.Contains(p))
                        {
                            _oskCurrentState.Add(r.Key);


                            _oskButtonHits.Add(new OSKButton
                            {
                                TTL = new Tween(new TimeSpan(0, 0, 0, 1), 1, 0),
                                Rect = new Rectangle(r.Value.X, r.Value.Y, r.Value.Width, r.Value.Height)
                            });
                        }
                        if (
                            new Rectangle(_oskExitRect.X + (int)_oskOffset.X, _oskExitRect.Y + (int)_oskOffset.Y,
                                _oskExitRect.Width, _oskExitRect.Width).Contains(p))
                        {
                            IsOSKVisable = false;
                        }
                    }

                }
            }
            #endregion

            if (TypedKey(Keys.CapsLock))
            {
                _isOSKCaps = !_isOSKCaps;
            }
            Typed.Clear();
            foreach (var t in _keys)
            {
                if (!TypedKey(t)) continue;
                var l = t.ToString();
                if (l.Length == 1)
                {
                    //if (Released(Keys.LeftShift) && Released(Keys.RightShift))
                    //    l = l.ToLower();
                    if ((Pressed(Keys.LeftShift) || Pressed(Keys.RightShift) && !_isOSKCaps))
                    {
                        l = l.ToUpper();
                    }
                    else if (_isOSKCaps)
                    {
                        l = l.ToUpper();
                    }
                    else
                    {
                        l = l.ToLower();
                    }
                }
                else
                {
                    #region Name Numbers

                    if (l.StartsWith(D) & !l.Equals(DecimalStr) &
                        !l.Equals(Divide))
                    {
                        l = l.Substring(1);
                        if (Pressed(Keys.LeftShift) || Pressed(Keys.RightShift))
                        {
                            switch (l)
                            {
                                case Zero:
                                    {
                                        l = ZeroShift;
                                        break;
                                    }
                                case One:
                                    {
                                        l = OneShift;
                                        break;
                                    }
                                case Two:
                                    {
                                        l = TwoShift;
                                        break;
                                    }
                                case Three:
                                    {
                                        l = ThreeShift;
                                        break;
                                    }
                                case Four:
                                    {
                                        l = FourShift;
                                        break;
                                    }
                                case Five:
                                    {
                                        l = FiveShift;
                                        break;
                                    }
                                case Six:
                                    {
                                        l = SixShift;
                                        break;
                                    }
                                case Seven:
                                    {
                                        l = SevenShift;
                                        break;
                                    }
                                case Eight:
                                    {
                                        l = EightShift;
                                        break;
                                    }
                                case Nine:
                                    {
                                        l = NineShift;
                                        break;
                                    }
                            }
                        }
                    }
                    #endregion

                    else if (l.StartsWith(NumPad))
                    {
                        l = l.Substring(6);
                    }
                    else
                    {
                        switch (l)
                        {
                            case OemCommaStr:
                                {
                                    l = (Pressed(Keys.LeftShift) || Pressed(Keys.RightShift))
                                            ? OemCommaShift
                                            : OemComma;
                                    break;
                                }
                            case OemPeriodStr:
                                {
                                    l = (Pressed(Keys.LeftShift) || Pressed(Keys.RightShift))
                                            ? OemPeriodShift
                                            : OemPeriod;
                                    break;
                                }
                            case OemQuestionStr:
                                {
                                    l = (Pressed(Keys.LeftShift) || Pressed(Keys.RightShift))
                                            ? OemQuestionShift
                                            : OemQuestion;
                                    break;
                                }
                            case OemSemicolonStr:
                                {
                                    l = (Pressed(Keys.LeftShift) || Pressed(Keys.RightShift))
                                            ? OemSemicolonShift
                                            : OemSemicolon;
                                    break;
                                }
                            case OemQuotesStr:
                                {
                                    l = (Pressed(Keys.LeftShift) || Pressed(Keys.RightShift))
                                            ? OemQuotesShift
                                            : OemQuotes;
                                    break;
                                }
                            case OemOpenBracketsStr:
                                {
                                    l = (Pressed(Keys.LeftShift) || Pressed(Keys.RightShift))
                                            ? OemOpenBracketsShift
                                            : OemOpenBrackets;
                                    break;
                                }
                            case OemCloseBracketsStr:
                                {
                                    l = (Pressed(Keys.LeftShift) || Pressed(Keys.RightShift))
                                            ? OemCloseBracketsShift
                                            : OemCloseBrackets;
                                    break;
                                }
                            case OemPipeStr:
                                {
                                    l = (Pressed(Keys.LeftShift) || Pressed(Keys.RightShift))
                                            ? OemPipeShift
                                            : OemPipe;
                                    break;
                                }
                            case OemPlusStr:
                                {
                                    l = (Pressed(Keys.LeftShift) || Pressed(Keys.RightShift))
                                            ? OemPlusShift
                                            : OemPlus;
                                    break;
                                }
                            case OemMinusStr:
                                {
                                    l = (Pressed(Keys.LeftShift) || Pressed(Keys.RightShift))
                                            ? OemMinusShift
                                            : OemMinus;
                                    break;
                                }
                            case TabStr:
                                {
                                    l = Tab;
                                    break;
                                }
                            case MultiplyStr:
                                {
                                    l = Multiply;
                                    break;
                                }
                            case DivideStr:
                                {
                                    l = Divide;
                                    break;
                                }
                            case SubtractStr:
                                {
                                    l = Subtract;
                                    break;
                                }
                            case AddStr:
                                {
                                    l = Add;
                                    break;
                                }
                            case DecimalStr:
                                {
                                    l = Dot;
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                    }
                }
                Typed.Add(l);
                _currentString += l;
            }

            // Check input for spacebar
            if (TypedKey(Keys.Space) && _currentString != string.Empty &&
                _currentString[_currentString.Length - 1].ToString() != Space)
            {
                _currentString += Space;
            }


            // Check input for backspace
            if (TypedKey(Keys.Back) && _currentString != string.Empty)
            {
                _currentString = _currentString.Remove(_currentString.Length - 1, 1);
            }

            // Check input for enter
            if (TypedKey(Keys.Enter))
            {
                _previousString = _currentString;
                _currentString = string.Empty;
            }
            if (Any()) Game.UnifiedInput.Hidden = true;
            if (_oskCurrentState == _oskPreviousState) return;

            var remove = new List<Procedure<KeyboardInput>>();
            foreach (var keyboardListener in _keyboardListeners)
                try
                {
                    keyboardListener(this);
                }
                catch
                {
                    remove.Add(keyboardListener);
                }
            _keyboardListeners.RemoveAll(remove.Contains);
        }

        #region Listeners
        public void AddKeyboardListener(Procedure<KeyboardInput> listener)
        {
            _keyboardListeners.Add(listener);
        }
        #endregion

        #region Functions
        public bool TypedKey(Keys k)
        {
            return !_oskCurrentState.Contains(k) && _oskPreviousState.Contains(k);
        }

        public bool Pressed(Keys k)
        {
            return _oskCurrentState.Contains(k);
        }

        public bool Released(Keys k)
        {
            return !Pressed(k);
        }

        public bool Any()
        {
            return _oskCurrentState.Any(k => !_oskPreviousState.Contains(k));
        }

        public bool AnyAlpha()
        {
            return _oskCurrentState.Any(k => _alphaKeys.Contains(k) && !_oskPreviousState.Contains(k));
        }

        public bool AnyNumeric()
        {
            return _oskCurrentState.Any(k => _numericKeys.Contains(k) && !_oskPreviousState.Contains(k));
        }

        public bool AnyAlphaNumeric()
        {
            return AnyNumeric() || AnyAlpha();
        }
        #endregion
    }
}
