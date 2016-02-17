
/** 
* @file DebugContextChecker.cs
* @brief Contains the DebugContextChecker class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/


using UnityEngine;

namespace Assets.Demos
{
    /// <summary>
    /// The purpose of this script is to check if the platform is in a debug/development context. If so, then the children are
    /// enabled, else left disabled
    /// </summary>
   public class DebugContextChecker: MonoBehaviour
    {
        [SerializeField] private GameObject mChildren;
        void Awake()
        {
            bool vIsDebug = false;
#if UNITY_EDITOR 
            vIsDebug = true;
#endif
#if DEVELOPMENT_BUILD
           vIsDebug = true;
#endif
            mChildren.SetActive(vIsDebug);

        }
    }
}
