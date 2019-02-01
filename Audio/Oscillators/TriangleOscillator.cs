using System;

namespace AudioMarcoPolo.Audio.Oscillators
{
    public class TriangleOscillator : BaseOscillator
    {        
        public override float GetNext(float timedomain)
        {
            return 1f - 4f * (float)Math.Abs
                        (Math.Round(timedomain - 0.25f) - (timedomain - 0.25f));
        }
    }
}
