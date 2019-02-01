using System;

namespace AudioMarcoPolo.Audio.Oscillators
{
    public class SquareOscillator : BaseOscillator
    {
        public override float GetNext(float timedomain)
        {
            return Math.Sign(Math.Sin(2f * Math.PI * timedomain));
        }
    }
}
