 

namespace Assets.Scripts.Utils.UnityUtilities
{
    /// <summary>
    /// This structure essentially informs the unity main thread that another thread has updated an object. As such, the 
    /// main unity thread will check in its update and check if the trigger has been flagged
    /// </summary>
    public class OutterThreadToUnityTrigger
    {
        public bool Triggered;
        public bool InterestedVariable;
        public object Args;
        public void Reset()
        {
            Triggered = false;
            InterestedVariable = false;
        }

        public void Trigger(bool vPlaceholdername)
        {
            Triggered = true;
            InterestedVariable = vPlaceholdername;
        }
    }
}
