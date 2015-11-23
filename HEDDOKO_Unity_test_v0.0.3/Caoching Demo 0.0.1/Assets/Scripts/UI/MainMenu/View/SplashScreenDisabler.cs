/** 
* @file SplashScreenDisabler.cs
* @brief Contains the SplashScreenDisabler class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/


using System.Reflection;
using UnityEngine;

namespace Assets.Scripts.UI.MainScene.View
{
    /**
    * SplashScreenDisabler class 
    * @brief SplashScreenDisabler class Disables the splash screen after a set time, then enables the main
    * menu
    * @note something 

    */
    public class SplashScreenDisabler : MonoBehaviour
    {
        public GameObject MainMenuObj;
        public float DisableTime = 1.5f;

        void Update()
        {
            DisableTime -= Time.deltaTime;
            if (DisableTime < 0)
            {
                MainMenuObj.SetActive(true);
                gameObject.SetActive(false);
            }
        }
    }
}
