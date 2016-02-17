 
using System.Collections.Generic;
 
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Utils.DebugContext
{
    /// <summary>
    /// Class that handles inputs and their respective actions
    /// </summary>
   public  class InputHandler: MonoBehaviour
    {
        public static Dictionary<KeyCode, List<UnityAction>> KeyBindActionMaps = new Dictionary<KeyCode, List<UnityAction>>();

        /// <summary>
        /// Registers an action to the internal register
        /// </summary>
        /// <param name="vKeyCode"></param>
        /// <param name="vAction"></param>
        public static void RegisterActions(KeyCode vKeyCode, UnityAction vAction)
        {
            if (!KeyBindActionMaps.ContainsKey(vKeyCode))
            {
                List<UnityAction> vNewActionList = new List<UnityAction>();
                vNewActionList.Add(vAction);
                KeyBindActionMaps.Add(vKeyCode, vNewActionList);
            }
            else
            {
                List<UnityAction> vNewActionList = KeyBindActionMaps[vKeyCode];
                vNewActionList.Add(vAction);    
            }

        }

        void OnGUI()
        {
            var vEvent = Event.current;
             
            if (vEvent.isKey && vEvent.type == EventType.KeyDown)
            {
               if(KeyBindActionMaps.ContainsKey(vEvent.keyCode)) 
                {
                    foreach (var vUnityAction in KeyBindActionMaps[vEvent.keyCode])
                    {
                        vUnityAction.Invoke();
                    }
                }
            }

        }

    }
}
