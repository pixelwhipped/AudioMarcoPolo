using System;

namespace AudioMarcoPolo.Interfaces
{
    public interface IParent
    {
        void ShowToast(string toast, string title = "Achievement", TimeSpan? time = null);
        void ShowPause(bool p);
    }
}
