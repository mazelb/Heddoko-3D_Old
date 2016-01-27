/** 
* @file SecretSliderMenu.cs
* @brief Contains the AnalysisCSecretSliderMenuontentPanel class
* @author Mohammed Haider(Mohammed@heddoko.com)
* @date January 2016
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.NFLDemo
{
    /// <summary>
    /// secret slider menu that brings in to view after a person has entered the secret
    /// </summary>
    public class SecretSliderMenu : MonoBehaviour
    {
        public Animator Animator;
        public Slider Slider;
        public Text Text;
        public NFLDemoController NflDemoController;

        private string[] vKonamicode =
        {
            "UpArrow", "UpArrow", "DownArrow", "DownArrow", "LeftArrow", "RightArrow",
            "LeftArrow", "RightArrow", "B", "A", "Return"
        };

        private int currentPos = 0;
        [SerializeField]
        private bool inKonami = false;
        [SerializeField]
        private bool vDispatched = false;

        private void OnGUI()
        {
            Event e = Event.current;
            if (e.isKey && Input.anyKeyDown && !inKonami && e.keyCode.ToString() != "None")
            {
                konamiFunction(e.keyCode);
            }
        }

        private void Update()
        {
            if (!vDispatched)
            {
                if (inKonami)
                {
                    Animator.SetBool("Show", true);
                    vDispatched = false;
                }
            }
        }

        private void konamiFunction(KeyCode incomingKey)
        {

            var incomingKeyString = incomingKey.ToString();
            if (incomingKeyString == vKonamicode[currentPos])
            {
                currentPos++;

                if ((currentPos + 1) > vKonamicode.Length)
                {
                    inKonami = true;
                    currentPos = 0;
                }
            }
            else
            {
                currentPos = 0;
            }

        }

        public void UpdateCtrlVal()
        {
            NflDemoController.Timer = Slider.value;
            Text.text = Slider.value.ToString("0.00");
        }
}
}
