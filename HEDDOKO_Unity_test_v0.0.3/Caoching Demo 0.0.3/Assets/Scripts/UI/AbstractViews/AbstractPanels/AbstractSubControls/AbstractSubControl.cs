/** 
* @file AbstractSubControl.cs
* @brief Contains the AbstractSubControl class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date Ma 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/


using System;
using Assets.Scripts.UI.AbstractViews.Enums;
using UnityEngine;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls
{
    /// <summary>
    /// Abstract subcontrol: An abstraction of a panels sub controls
    /// </summary>
 public abstract   class AbstractSubControl : MonoBehaviour, IEquatable<AbstractControlPanel>
 {
     private SubControlType mSubControlType;
     

     public SubControlType ControlType
     {
         get { return mSubControlType; }
     }

     public bool Equals(AbstractControlPanel other)
     {
         if (other != null)
         {
             return gameObject.GetInstanceID() == other.gameObject.GetInstanceID();
         }
         return false;
     }
 }
}
