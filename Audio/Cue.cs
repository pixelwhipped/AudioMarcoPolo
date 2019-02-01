using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SharpDX.Multimedia;
using SharpDX.XAudio2;

namespace AudioMarcoPolo.Audio
{
    public class Cue
    {
        internal string Sound;
        private readonly AudioFx _audio;
        public readonly AudioChannels Channel;
        private SourceVoice s;
        public float volume;

        private XAudio2 Device
        {
            get
            {
                switch (Channel)
                {
                    case AudioChannels.Music:
                        return _audio.Music;
                    case AudioChannels.Effect:
                        return _audio.Effects;
                    default:
                        return _audio.Synth;
                }                
            }
        }

        private MasteringVoice Master
        {
            get
            {
                switch (Channel)
                {
                    case AudioChannels.Music:
                        return _audio._musicVoice;
                    case AudioChannels.Effect:
                        return _audio._effectsVoice;
                    default:
                        return _audio._synthVoice;
                }
            }
        }

        private readonly byte[] _byteStream;
        private readonly List<SourceVoice> _voices;
        public bool Loop;
        private SoundStream Stream
        {
            get
            {
                var m = new MemoryStream(_byteStream);
                return new SoundStream(m);
            }
        }

        public Cue(AudioFx audio, AudioChannels channel, string path)
        {
            Channel = channel;
            Sound = path;
            _audio = audio;
            var s = Windows.ApplicationModel.Package.Current.InstalledLocation.OpenStreamForReadAsync(Path.Combine(_audio.Game.Content.RootDirectory, path));
            s.Wait();
            var stream = s.Result;
            _byteStream = ReadFully(stream);
            _voices = new List<SourceVoice>();
            volume = 1f;
        }

        public Cue(AudioFx audio, AudioChannels channel, Stream stream)
        {
            Channel = channel;
            Sound = "Stream";
            _audio = audio;
           //var s = Windows.ApplicationModel.Package.Current.InstalledLocation.OpenStreamForReadAsync(Path.Combine(_audio.Game.Content.RootDirectory, path));
           // s.Wait();
           // var stream = s.Result;
            _byteStream = ReadFully(stream);
            _voices = new List<SourceVoice>();
            volume = 1f;
        }


        public static byte[] ReadFully(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        internal SourceVoice Start(float pan = 0)
        {
            if (_voices.Any(v => v.State.BuffersQueued <= 0))
            {
                s = _voices.First(v => v.State.BuffersQueued <= 0);
            }
            else
            {
                s = new SourceVoice(Device, Stream.Format, true);
                
                _voices.Add(s);
            }
            var b = new AudioBuffer
            {
                Stream = Stream.ToDataStream(),
                AudioBytes = (int)Stream.Length,
                LoopCount = Loop ? AudioBuffer.LoopInfinite : 0,
                Flags = BufferFlags.EndOfStream
            };
            s.SubmitSourceBuffer(b, Stream.DecodedPacketsInfo);
            float[] outputMatrix = new float[8]; 
            for (int i=0; i<8; i++) outputMatrix[i] = 0;
            // pan of -1.0 indicates all left speaker,  
            // 1.0 is all right speaker, 0.0 is split between left and right 
            
            float left = 0.5f - pan / 2;
            float right = 0.5f + pan / 2;


            switch (Master.ChannelMask) 
            { 
            case (int)Speakers.Mono: 
                outputMatrix[0] = 1.0f; 
                break;
            case (int)Speakers.Stereo: 
            case (int)Speakers.TwoPointOne: 
            case (int)Speakers.Surround: 
                outputMatrix[0] = left; 
                outputMatrix[1] = right; 
                break; 
            case (int)Speakers.Quad: 
                outputMatrix[0] = outputMatrix[2] = left; 
                outputMatrix[1] = outputMatrix[3] = right; 
                break; 
            case (int)Speakers.FourPointOne: 
                outputMatrix[ 0 ] = outputMatrix[ 3 ] = left; 
                outputMatrix[ 1 ] = outputMatrix[ 4 ] = right; 
                break; 
            case (int)Speakers.FivePointOne: 
            case (int)Speakers.SevenPointOne: 
            case (int)Speakers.FivePointOneSurround: 
                outputMatrix[ 0 ] = outputMatrix[ 4 ] = left; 
                outputMatrix[ 1 ] = outputMatrix[ 5 ] = right; 
                break; 
            case (int)Speakers.SevenPointOneSurround : 
                outputMatrix[ 0 ] = outputMatrix[ 4 ] = outputMatrix[ 6 ] = left; 
                outputMatrix[ 1 ] = outputMatrix[ 5 ] = outputMatrix[ 7 ] = right; 
                break; 
            }
            s.SetOutputMatrix(null, s.VoiceDetails.InputChannelCount,Master.VoiceDetails.InputChannelCount, outputMatrix);
            s.Start();
            return s;
        }

        private float vset;
        private bool setv;
        public void SetVolume(float v)
        {
            vset = MathHelper.Clamp(v, 0f, 1f);
            if (s != null && setv == false)
            {
                setv = true;
                Task.Run(() =>
                {
                    while (Math.Abs(vset - s.Volume) > 0.0000001f)
                    {
                        if (vset < s.Volume)
                        {
                            s.SetVolume(s.Volume-0.000025f);
                        }
                        else
                        {
                            s.SetVolume(s.Volume + 0.000025f);
                        }
                    }
                    setv = false;
                });
            }
            else
            {
                vset = v;
            }
        }
    }
}
