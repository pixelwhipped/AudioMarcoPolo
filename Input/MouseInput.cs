using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AudioMarcoPolo.Input
{
    public class MouseInput
    {
        public bool LeftButton { get { return State.LeftButton == ButtonState.Pressed; } }
        public bool MiddleButton { get { return State.MiddleButton == ButtonState.Pressed; } }
        public bool RightButton { get { return State.RightButton == ButtonState.Pressed; } }


        private TimeSpan _leftClick;
        private int _leftClickCount;
        public bool LeftClicked { get { return LeftSingleClick || LeftDoubleClick; } }
        public bool LeftSingleClick { get { return _leftClick > TimeSpan.Zero && _leftClickCount == 1; } }
        public bool LeftDoubleClick { get { return _leftClick > TimeSpan.Zero && _leftClickCount == 2; } }

        private TimeSpan _middleClick;
        private int _middleClickCount;
        public bool MiddleClicked { get { return MiddleSingleClick || MiddleDoubleClick; } }
        public bool MiddleSingleClick { get { return _middleClick > TimeSpan.Zero && _middleClickCount == 1; } }
        public bool MiddleDoubleClick { get { return _middleClick > TimeSpan.Zero && _middleClickCount == 2; } }

        private TimeSpan _rightClick;
        private int _rightClickCount;
        public bool RightClicked { get { return RightSingleClick || RightDoubleClick; } }
        public bool RightSingleClick { get { return _rightClick > TimeSpan.Zero && _rightClickCount == 1; } }
        public bool RightDoubleClick { get { return _rightClick > TimeSpan.Zero && _rightClickCount == 2; } }
        public bool Moved;

        private Vector2 _hotSpot;

        public float X { get { return Location.X; } }
        public float Y { get { return Location.Y; } }

        private TimeSpan _lastChange;

        public bool Hidden
        {
            get
            {
                return Fade <= 0.5f;
            }
            set
            {
                _lastChange = (value) ? _lastChange - new TimeSpan(0, 0, 0, 3) : TimeSpan.Zero;
            }
        }

        public float Fade { get; private set; }


        public List<Procedure<Vector2>> LeftClickListeners;
        public List<Procedure<Vector2>> RightClickListeners;
        public List<Procedure<Vector2>> MiddleClickListeners;
        public List<Operation<Vector2>> DraggingListeners;
        public List<Operation<Vector2>> DraggedListeners;

        public Vector2 DragFrom = Vector2.Zero;

        public List<Procedure<Vector2>> MoveListeners;

        public MouseState State;
        private MouseState _previousState;

        public Vector2 Location
        {
            get
            {

                return new Vector2(State.X - _hotSpot.X, State.Y - _hotSpot.Y);
            }
        }

        public MouseInput()
            : this(Vector2.Zero)
        {

        }
        public MouseInput(Vector2 hotSpot)
        {
            _hotSpot = hotSpot;
            State = Mouse.GetState();
            _previousState = State;
            LeftClickListeners = new List<Procedure<Vector2>>();
            RightClickListeners = new List<Procedure<Vector2>>();
            MiddleClickListeners = new List<Procedure<Vector2>>();
            MoveListeners = new List<Procedure<Vector2>>();
            DraggingListeners = new List<Operation<Vector2>>();
            DraggedListeners = new List<Operation<Vector2>>();
            _leftClick = TimeSpan.Zero;
            _middleClick = TimeSpan.Zero;
            _rightClick = TimeSpan.Zero;
            _lastChange = TimeSpan.Zero;
        }

        public void ComponentChange()
        {
            _leftClick = TimeSpan.Zero;
            _middleClick = TimeSpan.Zero;
            _rightClick = TimeSpan.Zero;
            _lastChange = TimeSpan.Zero;
        }
        public void Update(GameTime gameTime)
        {

            var changed = false;
            _previousState = State;
            var currentState = Mouse.GetState();
            Moved = (_previousState.X != currentState.X || _previousState.X != currentState.X);

            #region LeftButton
            if (currentState.LeftButton != _previousState.LeftButton && _previousState.LeftButton == ButtonState.Released)
            {
                _leftClickCount = _leftClick <= TimeSpan.Zero ? 1 : 2;
                _leftClick = TimeSpan.FromMilliseconds(400);
                changed = true;
            }
            _leftClick -= gameTime.ElapsedGameTime;
            if (_leftClick < TimeSpan.Zero)
                _leftClickCount = 0;
            #endregion
            #region RightButton
            if (currentState.RightButton != _previousState.RightButton && _previousState.RightButton == ButtonState.Released)
            {
                _rightClickCount = _rightClick <= TimeSpan.Zero ? 1 : 2;
                _rightClick = TimeSpan.FromMilliseconds(400);
                changed = true;
            }
            _rightClick -= gameTime.ElapsedGameTime;
            if (_rightClick < TimeSpan.Zero)
                _rightClickCount = 0;
            #endregion
            #region MiddleButton
            if (currentState.MiddleButton != _previousState.MiddleButton && _previousState.MiddleButton == ButtonState.Released)
            {
                _middleClickCount = _middleClick <= TimeSpan.Zero ? 1 : 2;
                _middleClick = TimeSpan.FromMilliseconds(400);
                changed = true;
            }
            _middleClick -= gameTime.ElapsedGameTime;
            if (_middleClick < TimeSpan.Zero)
                _middleClickCount = 0;
            #endregion

            #region X,Y
            if (currentState.X != _previousState.X || currentState.Y != _previousState.Y)
                changed = true;
            #endregion

            if (changed)
            {
                _lastChange = gameTime.TotalGameTime;

            }
            Fade = gameTime.TotalGameTime - _lastChange < new TimeSpan(0, 0, 0, 3) ? MathHelper.Clamp(Fade + 0.05f, 0f, 1f) : MathHelper.Clamp(Fade - 0.05f, 0f, 1f);

            var DragNotified = false;

            var remove = new List<Procedure<Vector2>>();
            var previous = new Vector2(State.X, State.Y);
            var current = new Vector2(currentState.X, currentState.Y);

            var dremove = new List<Operation<Vector2>>();
            if (currentState.LeftButton == ButtonState.Pressed && State.LeftButton == ButtonState.Released)
            {
                DragFrom = current;
            }
            if (currentState.LeftButton == ButtonState.Released && State.LeftButton == ButtonState.Pressed)
            {
                var delta = DragFrom - Location;

                //Only Notify if changed
                if (Math.Abs(delta.X) > 32 || Math.Abs(delta.Y) > 32)
                {
                    DragNotified = true;
                    foreach (var listener in DraggedListeners)
                    {
                        try
                        {
                            listener(DragFrom, Location);
                        }
                        catch
                        {
                            dremove.Add(listener);
                        }
                    }
                }
                DraggedListeners.RemoveAll(dremove.Contains);
                dremove.Clear();
                DragFrom = Vector2.Zero;
            }

            #region Move
            if (previous != current)
            {
                var delta = current - previous;
                if (Math.Abs(delta.X) < 4 || Math.Abs(delta.Y) < 4)
                {
                    foreach (var moveListener in MoveListeners)
                    {
                        try
                        {
                            moveListener(current);
                        }
                        catch
                        {
                            remove.Add(moveListener);
                        }
                    }
                    MoveListeners.RemoveAll(remove.Contains);
                    foreach (var listener in DraggingListeners)
                    {
                        try
                        {
                            listener(DragFrom, Location);
                        }
                        catch
                        {
                            dremove.Add(listener);
                        }
                    }
                    DraggingListeners.RemoveAll(dremove.Contains);

                }
            }
            dremove.Clear();
            remove.Clear();
            #endregion

            #region Left Click

            if (State.LeftButton == ButtonState.Pressed && currentState.LeftButton == ButtonState.Released && !DragNotified)
            {
                foreach (var listener in LeftClickListeners)
                {
                    try
                    {
                        listener(current);
                    }
                    catch
                    {
                        remove.Add(listener);
                    }
                }
                LeftClickListeners.RemoveAll(remove.Contains);
                remove.Clear();
            }
            #endregion

            #region Right Click

            if (State.RightButton == ButtonState.Pressed && currentState.RightButton == ButtonState.Released)
            {
                foreach (var listener in RightClickListeners)
                {
                    try
                    {
                        listener(current);
                    }
                    catch
                    {
                        remove.Add(listener);
                    }
                }
                RightClickListeners.RemoveAll(remove.Contains);
                remove.Clear();
            }
            #endregion

            #region Middle Click

            if (State.MiddleButton == ButtonState.Pressed && currentState.MiddleButton == ButtonState.Released)
            {
                foreach (var listener in MiddleClickListeners)
                {
                    try
                    {
                        listener(current);
                    }
                    catch
                    {
                        remove.Add(listener);
                    }
                }
                MiddleClickListeners.RemoveAll(remove.Contains);
                remove.Clear();
            }
            #endregion

            State = currentState;
        }
    }
}
