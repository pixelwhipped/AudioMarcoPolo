using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharpDX.Multimedia;
using SharpDX.XAudio2;

namespace AudioMarcoPolo.Audio
{
    public class SynthVoice
    {

        private readonly AudioFx _audio;
        public readonly AudioChannels Channel;
        private readonly List<SourceVoice> _voices;

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
        //Reload the instrument sample
        private AudioBuffer actualbuffer;
        private uint[] bufferinfo;
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

        public bool IsPlaying { get; set; }

        private int PlayingPitch { get; set; }
        private IInstrument Instrument { get; set; }
        private float ActualFrequencyRatio { get; set; }
        private SourceVoice Voice { get; set; }
        public List<SourceVoice> Voices { get; private set; }
        
        public SynthVoice(AudioFx audio, AudioChannels channel)
        {
            _audio = audio;
            Channel = channel;
            PlayingPitch = -1;
            Voices = new List<SourceVoice>();
            StopDone = new ManualResetEvent(false);
            CancelStop = new ManualResetEvent(false);
        }

        /// <summary>
        /// Load an instrument
        /// </summary>
        /// <param name="instr">Sample instrument, or synthesis instrument</param>
        public void LoadInstrument(IInstrument instr)
        {
            if (Voice != null) Destroy();
            Instrument = instr;
            Voice = new SourceVoice(Device, new WaveFormat(44100,16,1), VoiceFlags.None, 16, false);
            ReloadBuffer(1, true);
            Voices.Add(Voice);
            Voice.SetVolume(1, XAudio2.CommitNow);
            ActualFrequencyRatio = 1;
            PlayingPitch = -1;
        }

        /// <summary>
        /// Play a sound from the sample file
        /// </summary>
        /// <param name="sampleIndex">sample index (starting from zero) in the file</param>
        public void PlaySound(int sampleIndex)
        {
            var asy = Task.Run(() =>
            {

                if ((Voice == null) || (Instrument == null)) return;

                ReloadBuffer(sampleIndex);
                CancelStop.Set();
                //Set the frequency ratio to the voice
                Voice.SetFrequencyRatio(1);


                //Start the play if needed
                if (!IsPlaying)
                {
                    Voice.Start();
                    Voice.SetVolume(1, XAudio2.CommitNow);
                }

                //update state
                IsPlaying = true;
            });
        }

        /// <summary>
        /// Play a sound from the sample file
        /// </summary>
        /// <param name="sampleIndex">sample index (starting from zero) in the file</param>
        public void PlaySound(float frequency, float pan = 1f, float volume = 1f)
        {
            var asy = Task.Run(() =>
            {

                if ((Voice == null) || (Instrument == null)) return;


                //Get the frequenc y of the pitch
                var FrequencyRatio = frequency / 440f;
                //See if we should use another sample
                var reloadNeeded = Instrument.ReloadNeeded(FrequencyRatio, ActualFrequencyRatio);
                if (Instrument.IsPlugged || reloadNeeded || !IsPlaying || stopping)
                {
                    ReloadBuffer(FrequencyRatio, reloadNeeded);
                }
                CancelStop.Set();
                //Set the frequency ratio to the voice
                var realFrequencyRatio = Instrument.GetCorrectFrequencyRatio(FrequencyRatio);
                Voice.SetFrequencyRatio(realFrequencyRatio);
                ActualFrequencyRatio = FrequencyRatio;
                PlayingPitch = -1;

                float[] outputMatrix = new float[8];
                for (int i = 0; i < 8; i++) outputMatrix[i] = 0;
                // pan of -1.0 indicates all left speaker,  
                // 1.0 is all right speaker, 0.0 is split between left and right 


                float left = (0.5f - pan / 2) * volume;
                float right = (0.5f + pan / 2) * volume;


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
                        outputMatrix[0] = outputMatrix[3] = left;
                        outputMatrix[1] = outputMatrix[4] = right;
                        break;
                    case (int)Speakers.FivePointOne:
                    case (int)Speakers.SevenPointOne:
                    case (int)Speakers.FivePointOneSurround:
                        outputMatrix[0] = outputMatrix[4] = left;
                        outputMatrix[1] = outputMatrix[5] = right;
                        break;
                    case (int)Speakers.SevenPointOneSurround:
                        outputMatrix[0] = outputMatrix[4] = outputMatrix[6] = left;
                        outputMatrix[1] = outputMatrix[5] = outputMatrix[7] = right;
                        break;
                }
                Voice.SetOutputMatrix(null, Voice.VoiceDetails.InputChannelCount, Master.VoiceDetails.InputChannelCount, outputMatrix);
                

                //Start the play if needed
                if (!IsPlaying)
                {
                    
                    Voice.Start();
                    Voice.SetVolume(1);
                }

                //update state
                IsPlaying = true;
            });
        }

        /// <summary>
        /// Start to play a note, It will be stopped until you call "StopNote"
        /// </summary>
        /// <param name="notePitch"></param>
        public void PlayNote(int notePitch)
        {
            var asy = Task.Run(() =>
            {

                if ((Voice == null) || (Instrument == null)) return;

                //If we play another note
                if ((notePitch != PlayingPitch) || !IsPlaying || stopping)
                {
                    //Get the frequenc y of the pitch
                    var FrequencyRatio = XAudio2.SemitonesToFrequencyRatio(notePitch) / 2f;
                    //See if we should use another sample
                    var reloadNeeded = Instrument.ReloadNeeded(FrequencyRatio, ActualFrequencyRatio);
                    if (Instrument.IsPlugged || reloadNeeded || !IsPlaying || stopping)
                    {
                        ReloadBuffer(FrequencyRatio, reloadNeeded);
                    }
                    CancelStop.Set();
                    //Set the frequency ratio to the voice
                    var realFrequencyRatio = Instrument.GetCorrectFrequencyRatio(FrequencyRatio);
                    Voice.SetFrequencyRatio(realFrequencyRatio);
                    ActualFrequencyRatio = FrequencyRatio;
                    PlayingPitch = notePitch;
                }

                //Start the play if needed
                if (!IsPlaying)
                {
                    Voice.Start();
                    Voice.SetVolume(1, XAudio2.CommitNow);
                }

                //update state
                IsPlaying = true;
            });
        }
        /// <summary>
        /// Play a note with a duration
        /// </summary>
        /// <param name="notePitch">Semitone number</param>
        /// <param name="duration">Duration in milliseconds</param>
        /// <returns></returns>
        public async Task PlayNote(int notePitch, int duration)
        {
            await Task.Run(() =>
            {
                PlayNote(notePitch);
                new ManualResetEvent(false).WaitOne(duration);
                StopNote();
            });
        }

        /// <summary>
        /// Silence
        /// </summary>
        /// <param name="duration">Duration in milliseconds</param>
        /// <returns></returns>
        public async Task PlaySilence(int duration)
        {
            await Task.Run(() =>
            {
                StopNote();
                new ManualResetEvent(false).WaitOne(duration);
                StopNote();
            });
        }
        private void ReloadBuffer(float frequencyRatio, bool reloadNeeded)
        {
            if ((actualbuffer == null) || reloadNeeded)
            {
                SoundStream stream = new SoundStream(Instrument.GetDataStream(frequencyRatio));
                actualbuffer = new AudioBuffer()
                {
                    Stream = stream,
                    AudioBytes = (int)stream.Length,
                    Flags = BufferFlags.EndOfStream,
                    PlayBegin = 0,
                    PlayLength = 0,
                    LoopCount = (Instrument.IsPlugged) ? 0 : 255
                };
                bufferinfo = stream.DecodedPacketsInfo;
            }
            if (IsPlaying) Voice.Stop(XAudio2.CommitNow);
            Voice.FlushSourceBuffers();
            Voice.SubmitSourceBuffer(actualbuffer, bufferinfo);
            if (IsPlaying) Voice.Start(XAudio2.CommitNow);
        }

        private void ReloadBuffer(int sampleIndex)
        {
            SoundStream stream = new SoundStream(Instrument.GetDataStream(sampleIndex));
            actualbuffer = new AudioBuffer()
            {
                Stream = stream,
                AudioBytes = (int)stream.Length,
                Flags = BufferFlags.EndOfStream,
                PlayBegin = 0,
                PlayLength = 0,
                LoopCount = 255
            };
            bufferinfo = stream.DecodedPacketsInfo;
            if (IsPlaying) Voice.Stop(XAudio2.CommitNow);
            Voice.FlushSourceBuffers();
            Voice.SubmitSourceBuffer(actualbuffer, bufferinfo);
            if (IsPlaying) Voice.Start(XAudio2.CommitNow);
        }

        ManualResetEvent CancelStop;
        ManualResetEvent StopDone;
        bool stopping;
        /// <summary>
        /// Stop the note with a fadeout
        /// We can gives him a callback to execute when the note is stoped
        /// </summary>
        /// <param name="callback">Callback to exececute when the fadeout is done</param>
        public void StopNote(Action callback = null)
        {
            if (stopping) return;
            if ((Voice == null) || (Instrument == null) || (!IsPlaying)) return;
            stopping = true;
            CancelStop.Reset();
            var asy = Task.Run(() =>
            {
                /*
                Voice.Stop(PlayFlags.Tails, XAudio2.CommitNow);
                IsPlaying = false;
                PlayingPitch = -1;
                if (callback != null) callback();
                return;
                */
                //fadeout loop
                for (var i = Voice.Volume; (i >= 0.15) && (!CancelStop.WaitOne(20)); i -= 0.05f)
                {
                    Voice.SetVolume(i);
                }
                //If the stop has not been canceled
                if (!CancelStop.WaitOne(1))
                {
                    Voice.Stop(PlayFlags.Tails, XAudio2.CommitNow);
                    IsPlaying = false;
                    PlayingPitch = -1;
                    if (callback != null) callback();
                }
                else
                {
                    Voice.SetVolume(1);
                }
                StopDone.Set();
                stopping = false;
            });
        }
        /// <summary>
        /// Stop the playback and flush the voice buffer
        /// </summary>
        public void Destroy()
        {
            //Stop and flush the voice
            CancelStop.Set();

            Voice.Stop();
            Voice.FlushSourceBuffers();
            Voices.Remove(Voice);
            ActualFrequencyRatio = 0;
            //Voice.Dispose();
            //Voice = null;
        }
    }
}
