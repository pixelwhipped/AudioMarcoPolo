using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using AudioMarcoPolo.Audio.Oscillators;
using AudioMarcoPolo.Utilities;

namespace AudioMarcoPolo.Audio
{
    [DataContract]
    public class SynthInstrument : IInstrument
    {
        [DataMember]
        public float SampleRate { get; set; }
        [DataMember]
        public float Frequency { get; set; }
        [DataMember]
        public List<BaseOscillator> Oscillators { get; set; }

        public bool IsPlugged { get { return false; } set { } }
        public SynthInstrument()
        {
            Frequency = Note.Frequency["C"];
            SampleRate = 44100;
            Oscillators = new List<BaseOscillator>();
        }


        public List<float> GetData()
        {
            return GetDataPMS();
        }

        //Additive  synthesis
        public List<float> GetDataSynth()
        {
            foreach (var o in Oscillators)
            {
                o.SampleRate = this.SampleRate;
                o.Frequency = this.Frequency;// *(float)Math.Pow(2, (Oscillators.IndexOf(o)));
            }
            List<float> values = new List<float>();
            var DataLen = GetSampleCountForLooping() + 1;
            if (DataLen == 1) DataLen = this.SampleRate;
            for (float t = 0f; t < DataLen; t++)
            {
                float value = 0;
                foreach (var o in Oscillators)
                {
                    value += o.GetOscillation(t);
                }
                if (Oscillators.Count > 0)
                    value /= Oscillators.Count;
                values.Add(value);
            }

            return values;
        }

        //Phase modulation synthesis
        public List<float> GetDataPMS()
        {
            this.SampleRate = 44100;// this.Frequency * 100;
            foreach (var o in Oscillators)
            {
                o.SampleRate = this.SampleRate;
                o.Frequency = this.Frequency;// *(float)Math.Pow(2, (Oscillators.IndexOf(o)));
            }
            List<float> values = new List<float>();
            float fPerOscillation = SampleRate / Frequency;
            var DataLen = fPerOscillation * Oscillators.Count * 2;
            if (DataLen == 0) DataLen = this.SampleRate;



            for (float t = 0f; t < DataLen; t++)
            {


                values.Add(GetNext(t));// * 32767.0f);
            }
            return values;


        }

        public float GetNext(float t)
        {
            float signal = 0;
            foreach (var instrument in Oscillators)
            {
                // Calculate the next sample
                // using a phase modulation , and the instrument wave form algorythm
                signal +=
                    (instrument.GetNext((t * this.Frequency / this.SampleRate + instrument.FMIndex * (float)Math.Sin(t * this.Frequency * instrument.FrequencyRatio * Math.PI / this.SampleRate))) * instrument.invert + (instrument.Offset)) * instrument.Amplitude;
            }
            if (Oscillators.Any()) signal /= Oscillators.Count;
            if (signal > 1.0f)
            {
                signal = 1;
            }
            else if (signal < -1.0f)
            {
                signal = -1.0f;
            }
            return signal;
        }

        //returns the minimum count of samples for looping
        //to avoid glitches
        public float GetSampleCountForLooping()
        {
            var SampleForLoopCount = AudioHelpers.PPCM(Oscillators.Select(o => (int)(SampleRate / (o.Frequency * o.FrequencyRatio))).ToArray());

            return SampleForLoopCount;// *(float)Math.Pow(2, MathUtils.ppcm(Oscillators.Select(o => o.Pitch + 1).ToArray()) / 12);
        }


        static List<Type> KnownTypes = new List<Type>()
        {
            typeof(SynthInstrument),
            typeof(BaseOscillator),
            typeof(SawtoothOscillator),
            typeof(SineOscillator),
            typeof(SquareOscillator),
            typeof(TriangleOscillator)
        };
        public void SaveToFile(Stream stream)
        {
            var ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(SynthInstrument), KnownTypes);
            ser.WriteObject(stream, this);
        }

        public static SynthInstrument ReadFromFile(Stream stream)
        {
            var ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(SynthInstrument), KnownTypes);
            return (SynthInstrument)ser.ReadObject(stream);
        }

        #region IInstrument
        public bool ReloadNeeded(float newFrequencyRatio, float oldFrequencyRatio)
        {
            return false;
        }

        public float GetCorrectFrequencyRatio(float desiredRatio)
        {
            var theFrequency = desiredRatio * Frequency;
            var r = theFrequency / Frequency;
            return r;
        }

        public MemoryStream GetDataStream(float frequencyRatio = 1)
        {
            var sampleData = GetData();
            long sampleCount = sampleData.Count;
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            var RIFF = 0x46464952;
            var WAVE = 0x45564157;
            var formatChunkSize = 16;
            var headerSize = 8;
            var format = 0x20746D66;
            short formatType = 1;
            short tracks = 2;
            short bitsPerSample = 16;
            short frameSize = (short)(tracks * ((bitsPerSample + 7) / 8));
            var bytesPerSecond = (int)(SampleRate * frameSize);
            var waveSize = 4;
            var data = 0x61746164;
            var samples = (int)sampleCount;
            var dataChunkSize = samples * frameSize;
            var fileSize = waveSize + headerSize + formatChunkSize + headerSize + dataChunkSize;
            writer.Write(RIFF);
            writer.Write(fileSize);
            writer.Write(WAVE);
            writer.Write(format);
            writer.Write(formatChunkSize);
            writer.Write(formatType);
            writer.Write(tracks);
            writer.Write((int)SampleRate);
            writer.Write(bytesPerSecond);
            writer.Write(frameSize);
            writer.Write(bitsPerSample);
            writer.Write(data);
            writer.Write(dataChunkSize);

            double sample_l;
            short sl;
            for (var i = 0; i < sampleCount; i++)
            {
                sample_l = sampleData[i] * 30000.0;
                if (sample_l < -32767.0f) { sample_l = -32767.0f; }
                if (sample_l > 32767.0f) { sample_l = 32767.0f; }
                sl = (short)sample_l;
                stream.WriteByte((byte)(sl & 0xff));
                stream.WriteByte((byte)(sl >> 8));
                stream.WriteByte((byte)(sl & 0xff));
                stream.WriteByte((byte)(sl >> 8));
            }
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }


        public MemoryStream GetDataStream(int sampleIndex)
        {
            var sampleData = GetData();
            long sampleCount = sampleData.Count;
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            var RIFF = 0x46464952;
            var WAVE = 0x45564157;
            var formatChunkSize = 16;
            var headerSize = 8;
            var format = 0x20746D66;
            short formatType = 1;
            short tracks = 2;
            short bitsPerSample = 16;
            var frameSize = (short)(tracks * ((bitsPerSample + 7) / 8));
            var bytesPerSecond = (int)(SampleRate * frameSize);
            var waveSize = 4;
            var data = 0x61746164;
            var samples = (int)sampleCount;
            var dataChunkSize = samples * frameSize;
            var fileSize = waveSize + headerSize + formatChunkSize + headerSize + dataChunkSize;
            writer.Write(RIFF);
            writer.Write(fileSize);
            writer.Write(WAVE);
            writer.Write(format);
            writer.Write(formatChunkSize);
            writer.Write(formatType);
            writer.Write(tracks);
            writer.Write((int)SampleRate);
            writer.Write(bytesPerSecond);
            writer.Write(frameSize);
            writer.Write(bitsPerSample);
            writer.Write(data);
            writer.Write(dataChunkSize);

            double sample_l;
            short sl;
            for (var i = 0; i < sampleCount; i++)
            {
                sample_l = sampleData[i] * 30000.0;
                if (sample_l < -32767.0f) { sample_l = -32767.0f; }
                if (sample_l > 32767.0f) { sample_l = 32767.0f; }
                sl = (short)sample_l;
                stream.WriteByte((byte)(sl & 0xff));
                stream.WriteByte((byte)(sl >> 8));
                stream.WriteByte((byte)(sl & 0xff));
                stream.WriteByte((byte)(sl >> 8));
            }
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
        #endregion
    }
}
