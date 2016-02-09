/**  
* @file AbstractInActivityView.cs 
* @brief Contains the AbstractInActivityView class 
* @author Mohammed Haider(mohamed@heddoko.com) 
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved 
*/
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.ActivitiesContext
{
    /// <summary>
    /// An abstraction of a view when in the activity context in the app
    /// </summary>
    public abstract class AbstractInActivityView: MonoBehaviour
    {
        public virtual void Show()
        {
            
        }

        public virtual void Hide()
        {
            
        }
       public abstract Button Backbutton { get; }

          
    }
}
