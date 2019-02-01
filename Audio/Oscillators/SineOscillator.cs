using System;

namespace AudioMarcoPolo.Audio.Oscillators
{
    public class SineOscillator:BaseOscillator
    {
        public override float GetNext(float timedomain)
        {
            return 2f*(float)Math.Sin(Math.PI * timedomain);            
        }
    }
}
