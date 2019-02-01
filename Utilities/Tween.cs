using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;

namespace AudioMarcoPolo.Utilities
{
    public class Tween
    {
        public static implicit operator float(Tween t)
        {
            return t.Value;
        }

        /// <summary>
        /// min value of tween.
        /// </summary>
        private readonly float _min;

        /// <summary>
        /// Indicator for tween direction.
        /// </summary>
        private bool _positveDirection;

        /// <summary>
        /// Tween range.
        /// </summary>
        private readonly float _range;

        /// <summary>
        /// Current positional value.
        /// </summary>
        private float _value;

        public bool Loop;

        public List<Procedure<Tween>> CompletionListeners;

        public struct EvenTimer
        {
            public bool Fired;
            public float Time;
            public Procedure<Tween> Procedure;
        }

        public List<EvenTimer> EventListeners;

        public void AddEventListener(float value, Procedure<Tween> procedure)
        {
            EventListeners.Add(new EvenTimer { Time = value, Procedure = procedure });
        }

        /// <summary>
        /// Creates a tween that for a value that changes over a given time.
        /// </summary>
        /// <param name="time">Time to transition.</param>
        /// <param name="min">Min value.</param>
        /// <param name="max">Max value.</param>
        /// <param name="loop">Loop</param>
        public Tween(TimeSpan time, float min, float max, bool loop = false)
        {
            CompletionListeners = new List<Procedure<Tween>>();
            EventListeners = new List<EvenTimer>();
            Loop = loop;
            _value = 0.0f;
            _positveDirection = true;
            _min = min;
            var max1 = max;
            if (max1 < _min)
            {
                var t = _min;
                _min = max1;
                max1 = t;
                _positveDirection = false;
            }
            _range = max1 - _min;
            TotalTime = time;

            CurrentTime = TimeSpan.Zero;
        }

        /// <summary>
        /// Current value of the tween.
        /// </summary>
        public float Value
        {
            get
            {
                if (_positveDirection)
                {
                    return (_value * _range) + _min;
                }
                return (_range - (_value * _range)) + _min;
            }
        }

        /// <summary>
        /// Returns true if tween is complete.
        /// </summary>
        public bool IsComplete
        {
            get { return Math.Abs(_value - 1.0f) < float.Epsilon; }
        }

        /// <summary>
        /// Current tween time.
        /// </summary>
        public TimeSpan CurrentTime { get; private set; }

        /// <summary>
        /// Total time expected for tween.
        /// </summary>
        public TimeSpan TotalTime { get; private set; }

        public TimeSpan TimeToLive { get { return TotalTime - CurrentTime; } }

        public void Reset(TimeSpan ttl)
        {
            TotalTime = ttl;
            Reset();
        }
        /// <summary>
        /// Resets this tween to it origianal value.
        /// </summary>
        public void Reset()
        {
            _value = 0.0f;
            CurrentTime = TimeSpan.Zero;
            for (var index = 0; index < EventListeners.Count; index++)
            {
                EventListeners[index] = new EvenTimer
                {
                    Time = EventListeners[index].Time,
                    Procedure = EventListeners[index].Procedure,
                    Fired = false
                };
            }
        }

        /// <summary>
        /// Forces the tween to finish.
        /// </summary>
        public void Finish()
        {
            _value = 1.0f;
        }

        /// <summary>
        /// Updates this tween given a timespan.
        /// </summary>
        /// <param name="timeSpan">Time to update.</param>
        public void Update(TimeSpan timeSpan)
        {
            var notify = false;
            if (!IsComplete)
            {
                CurrentTime += timeSpan;
                _value = MathHelper.Clamp((float)(CurrentTime.TotalMilliseconds / TotalTime.TotalMilliseconds), 0.0f,
                                          1.0f);
                for (var index = 0; index < EventListeners.Count; index++)
                {
                    if (!(Value >= EventListeners[index].Time) || EventListeners[index].Fired) continue;
                    EventListeners[index] = new EvenTimer
                    {
                        Time = EventListeners[index].Time,
                        Procedure = EventListeners[index].Procedure,
                        Fired = true
                    };
                    EventListeners[index].Procedure(this);
                }
            }
            else if (Loop)
            {
                notify = true;
                Reset();
                _positveDirection = !_positveDirection;
            }
            else
            {
                notify = true;
            }
            if (!notify) return;
            var remove = new List<Procedure<Tween>>();
            foreach (var listener in CompletionListeners)
            {
                try
                {
                    listener(this);
                }
                catch
                {
                    remove.Add(listener);
                }
            }
            CompletionListeners.RemoveAll(remove.Contains);
            remove.Clear();
        }

    }
}
