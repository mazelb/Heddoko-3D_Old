using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI; 
namespace Assets.Scripts.UI.MainScene
{
    /// <summary>
    /// Class represents a recording button controller
    /// </summary>
    public class AvailableRecordingButtonController: MonoBehaviour
    {
        private Button mCurrentButton;
        //on Awake set the current button to the attached button
        void Awake()
        {
            mCurrentButton = GetComponent<Button>();
        }



    }
}
