/** 
* @file IActivitiesContextViewSubcomponent.cs
* @brief Contains the IActivitiesContextViewSubcomponent interface
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using JetBrains.Annotations;

namespace Assets.Scripts.UI.ActivitiesContext.View
{
    /// <summary>
    /// Interface for subcomponents of all activitiesContextView
    /// </summary>
    interface IActivitiesContextViewSubcomponent
    {
        void Show();
        void Hide();
    }
}
