﻿/** 
* @file BrainpackComPortText.cs
* @brief Contains the BrainpackComPortText class
* @author Mohammed Haider(Mohammed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using System.IO;
using System.Text.RegularExpressions;
using Assets.Scripts.Communication.Controller;
using UnityEngine;

namespace Assets.Scripts.Utils.DebugContext
{

    /// <summary>
    /// Sets the comPort of the BrainpackConnectionController, used for debbuging.
    /// </summary>
    public class BrainpackComPortText : MonoBehaviour
    {
        public UnityEngine.UI.Text Text;
        public UnityEngine.UI.InputField ComInputField;
        public UnityEngine.UI.Button PairButton;
        //Matches strings that begin with COM and proceeded by at least 1 number
        string mStringRgx = @"^(?i)com(?-i)\d+";

        /// <summary>
        /// Sets the brainpack Controller's comport 
        /// </summary>
        public void SetBPControllerPort(string s)
        {
            Regex myRegex = new Regex(mStringRgx, RegexOptions.Multiline);
            bool vMatch = false;
            foreach (Match myMatch in myRegex.Matches(s))
            {
                if (myMatch.Success)
                {
                    vMatch = true;
                }
            }
            if (vMatch)
            {
                PairButton.interactable = true;
                BrainpackConnectionController.Instance.BrainpackComPort = s.ToUpper();
            }
            else
            {
                PairButton.interactable = false;
            }

        }

    
   
    }
}
