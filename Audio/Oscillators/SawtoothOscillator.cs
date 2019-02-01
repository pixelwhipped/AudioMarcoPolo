using System;

namespace AudioMarcoPolo.Audio.Oscillators
{
    public class SawtoothOscillator : BaseOscillator
    {
        public override float GetNext(float timedomain)
        {
            // 2 * ( t/a - floor( t/a + 1/2 ) )
            return 2f * (timedomain - (float)Math.Floor(timedomain + 0.5f));
        }
    }
}
