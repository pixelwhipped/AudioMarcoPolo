using System.IO;

namespace AudioMarcoPolo.Audio
{
    public interface IInstrument
    {
        //This is to get the datastream to gives to the AudioBuffer for the SourceVoice
        MemoryStream GetDataStream(float frequencyRatio = 1);
        MemoryStream GetDataStream(int SampleIndex);
        //This method will be used by sample based instruments, to provide the frequency ratio to apply
        //according to the sample's tone's frequency 
        float GetCorrectFrequencyRatio(float desiredRatio);

        //Indicate if the buffer should be reloaded (when a pich changes, to select the best one
        bool ReloadNeeded(float newFrequencyRatio, float oldFrequencyRatio);
        bool IsPlugged { get; set; }
    }
}
