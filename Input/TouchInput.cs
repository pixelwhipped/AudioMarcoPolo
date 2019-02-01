using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AudioMarcoPolo.Input
{
    public class TouchInput
    {
        public List<Procedure<Vector2>> TapListeners;
        public List<Procedure<Vector2>> MoveListeners;
        public List<Procedure<GestureSample>> GestureListeners;
        public List<Operation<Vector2>> DraggingListeners;
        public List<Operation<Vector2>> DraggedListeners;

        public Vector2 DragFrom = Vector2.Zero;
        public Vector2 Location = Vector2.Zero;


        private readonly List<KeyValuePair<Vector2, TimeSpan>> _taps;
        private readonly List<KeyValuePair<Vector2, TimeSpan>> _touches;
        private readonly List<KeyValuePair<GestureSample, TimeSpan>> _gestures;

        private readonly TimeSpan _tapSensitivity = new TimeSpan(0, 0, 0, 0, 100);
        private readonly TimeSpan _touchSensitivity = new TimeSpan(0, 0, 0, 0, 100);
        private readonly TimeSpan _gestureSensitivity = new TimeSpan(0, 0, 0, 0, 100);

        public List<Vector2> Taps
        {
            get
            {
                return _taps.Select(v => v.Key).ToList();
            }
        }
        public List<Vector2> Touches
        {
            get
            {
                return _touches.Select(v => v.Key).ToList();
            }
        }
        public List<GestureSample> Gestures
        {
            get
            {
                return _gestures.Select(v => v.Key).ToList();
            }
        }

        public bool GesturesEnabled
        {
            set
            {
                if (value)
                {
                    TouchPanel.EnabledGestures = GestureType.Tap |
                                                 GestureType.DragComplete |
                                                 GestureType.Flick |
                                                 GestureType.FreeDrag |
                                                 GestureType.Hold |
                                                 GestureType.HorizontalDrag |
                                                 GestureType.Pinch |
                                                 GestureType.PinchComplete |
                                                 GestureType.DoubleTap |
                                                 GestureType.VerticalDrag;
                }
                else
                {
                    TouchPanel.EnabledGestures = GestureType.None;
                }
            }
            get
            {
                return TouchPanel.EnabledGestures != GestureType.None;
            }
        }

        public bool IsTouched;
        public TouchInput()
        {
            TapListeners = new List<Procedure<Vector2>>();
            MoveListeners = new List<Procedure<Vector2>>();
            GestureListeners = new List<Procedure<GestureSample>>();
            DraggingListeners = new List<Operation<Vector2>>();
            DraggedListeners = new List<Operation<Vector2>>();

            _taps = new List<KeyValuePair<Vector2, TimeSpan>>();
            _touches = new List<KeyValuePair<Vector2, TimeSpan>>();
            _gestures = new List<KeyValuePair<GestureSample, TimeSpan>>();
            GesturesEnabled = true;
        }

        public void Update(GameTime gameTime)
        {

            Vector2 delta;
            var currentTouchState = TouchPanel.GetState();
            _taps.RemoveAll(p => p.Value <= TimeSpan.Zero);
            _touches.RemoveAll(p => p.Value <= TimeSpan.Zero);
            _gestures.RemoveAll(p => p.Value <= TimeSpan.Zero);

            for (var index = 0; index < _taps.Count; index++)
            {
                _taps[index] = new KeyValuePair<Vector2, TimeSpan>(_taps[index].Key,
                    _taps[index].Value - gameTime.ElapsedGameTime);
            }
            for (var index = 0; index < _touches.Count; index++)
            {
                _touches[index] = new KeyValuePair<Vector2, TimeSpan>(_touches[index].Key,
                    _touches[index].Value - gameTime.ElapsedGameTime);
            }
            for (var index = 0; index < _gestures.Count; index++)
            {
                _gestures[index] = new KeyValuePair<GestureSample, TimeSpan>(_gestures[index].Key,
                    _gestures[index].Value - gameTime.ElapsedGameTime);
            }

            while (TouchPanel.IsGestureAvailable && GesturesEnabled)
            {
                var remove = new List<Procedure<GestureSample>>();
                var gesture = TouchPanel.ReadGesture();
                if (_gestures.All(p => p.Key.GestureType != gesture.GestureType))
                {
                    _gestures.Add(new KeyValuePair<GestureSample, TimeSpan>(gesture, new TimeSpan(_gestureSensitivity.Ticks)));
                }

                foreach (var guestureListeners in GestureListeners)
                {
                    try
                    {
                        guestureListeners(gesture);
                    }
                    catch
                    {
                        remove.Add(guestureListeners);
                    }
                }
                GestureListeners.RemoveAll(remove.Contains);
            }

            var first = true;
            var dremove = new List<Operation<Vector2>>();

            //If no touches the notify existing drag listeners
            if (currentTouchState.Count == 0)
            {
                IsTouched = false;
                if (DragFrom != Vector2.Zero && DragFrom != Location)
                {
                    foreach (var listener in DraggedListeners)
                    {
                        try
                        {
                            delta = DragFrom - Location;
                            //Only Notify if changed
                            if (Math.Abs(delta.X) > 32 || Math.Abs(delta.Y) > 32)
                            {
                                listener(DragFrom, Location);
                            }
                        }
                        catch
                        {
                            dremove.Add(listener);
                        }
                    }
                    DraggedListeners.RemoveAll(dremove.Contains);
                    dremove.Clear();
                }
                Location = Vector2.Zero;
                DragFrom = Vector2.Zero;
            }
            else
            {
                IsTouched = true;
            }


            foreach (var touch in currentTouchState)
            {
                var remove = new List<Procedure<Vector2>>();
                TouchLocation prevLoc;
                if (first)
                {
                    delta = touch.Position - Location;
                    if ((touch.State == TouchLocationState.Pressed || touch.State == TouchLocationState.Moved) && DragFrom == Vector2.Zero)
                    {
                        if (Math.Abs(delta.X) > 10 || Math.Abs(delta.Y) > 10 && Location != Vector2.Zero)
                        {
                            DragFrom = touch.Position;
                        }
                    }
                    delta = touch.Position - Location;
                    Location = touch.Position;

                    if (Math.Abs(delta.X) > 10 || Math.Abs(delta.Y) > 10)
                    {

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
                        dremove.Clear();
                    }

                    if (Math.Abs(delta.X) > 8 || Math.Abs(delta.Y) > 8)
                    {

                        foreach (var listener in MoveListeners)
                        {
                            try
                            {
                                listener(Location);
                            }
                            catch
                            {
                                remove.Add(listener);
                            }
                        }
                        MoveListeners.RemoveAll(remove.Contains);
                        remove.Clear();
                    }
                    first = false;
                }

                //Add Touches that dont currently Exist
                if (_touches.All(p => p.Key != new Vector2(touch.Position.X, touch.Position.Y)))
                {
                    _touches.Add(new KeyValuePair<Vector2, TimeSpan>(new Vector2(touch.Position.X, touch.Position.Y), new TimeSpan(_touchSensitivity.Ticks)));
                }


                

                if (touch.State != TouchLocationState.Released) continue;
                if (_taps.All(p => p.Key != new Vector2(touch.Position.X, touch.Position.Y)))
                {
                    _taps.Add(new KeyValuePair<Vector2, TimeSpan>(new Vector2(touch.Position.X, touch.Position.Y), new TimeSpan(_tapSensitivity.Ticks)));
                }


                //Notify Tap Listeners on Release
                if (touch.State != TouchLocationState.Pressed)
                {
                    foreach (var tapListener in TapListeners)
                    {
                        try
                        {
                            if (Math.Abs(Vector2.Distance(DragFrom,Location))<10f)                    
                                tapListener(new Vector2(touch.Position.X, touch.Position.Y));
                        }
                        catch
                        {
                            remove.Add(tapListener);
                        }
                    }
                    TapListeners.RemoveAll(remove.Contains);
                }
                remove.Clear();

                /*if (!touch.TryGetPreviousLocation(out prevLoc))
                    continue;

                if (prevLoc.State == TouchLocationState.Moved)
                {
                    delta = touch.Position - prevLoc.Position;
                    if (Math.Abs(delta.X) >= 6 || Math.Abs(delta.Y) >= 6)
                    {
                        foreach (var moveListener in MoveListeners)
                        {
                            try
                            {
                                moveListener(new Vector2(touch.Position.X, touch.Position.Y));
                            }
                            catch
                            {
                                remove.Add(moveListener);
                            }
                        }
                        MoveListeners.RemoveAll(remove.Contains);
                    }
                }
                remove.Clear();*/

            }
        }

    }
}
