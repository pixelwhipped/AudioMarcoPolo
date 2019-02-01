using System;
using System.Runtime.Serialization;

namespace AudioMarcoPolo.Audio.Oscillators
{
    [DataContract]
    public class BaseOscillator
    {
        public string Name { get { return string.Format("{0} [{1}hz]", this.GetType().Name, this.Frequency); } }
        public BaseOscillator()
        {
            Frequency = Note.Frequency["C"];
            SampleRate = 44100f;
            Amplitude = 1f;
            Invert = false;
            Pitch = 0;
        }
        public float invert;
        float amplitude;
        [DataMember]
        public float SampleRate { get; set; }
        [DataMember]
        public float Frequency { get; set; }
        [DataMember]
        public int Pitch { get; set; }
        [DataMember]
        public float Offset { get; set; }
        [DataMember]
        public float Amplitude
        {
            get { return amplitude; }
            set
            {
                amplitude = value;
                if (amplitude < 0) amplitude = 0;
                if (amplitude > 1) amplitude = 1;
            }
        }
        [DataMember]
        public bool Invert
        {
            get { return invert < 0; }
            set { invert = (value) ? -1 : 1; }
        }

        public float FMIndex { get; set; }
        
        [IgnoreDataMember]
        public float FrequencyRatio
        {
            get
            {

                var retval = (float)Math.Pow(2, (double)Pitch / 12d);
                return retval;
            }
        }
        
        public float FMPhase { get; set; }
        public virtual float GetNext(float timedomain)
        {
            return 0;
        }

        private float GetOsccilationTimeDomain(float timedomain)
        {
            float samplesPerOccilation = GetSamplesPerOscillation();
            return (timedomain % samplesPerOccilation) / samplesPerOccilation;
        }

        public float GetSamplesPerOscillation()
        {
            return (SampleRate / (Frequency * FrequencyRatio));
        }

        public virtual float GetOscillation(float timedomain)
        {
            return GetNext(GetOsccilationTimeDomain(timedomain + Offset * GetSamplesPerOscillation())) * invert * amplitude;//;
        }

    }
}
