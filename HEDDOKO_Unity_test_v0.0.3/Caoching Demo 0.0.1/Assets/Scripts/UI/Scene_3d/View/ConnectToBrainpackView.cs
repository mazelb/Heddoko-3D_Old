/** 
* @file ConnectToBrainpackView.cs
* @brief Contains the ConnectToBrainpackView     class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using Assets.Scripts.Communication.Controller;
using Assets.Scripts.Utils.UnityUtilities;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Scene_3d.View
{
    /**
     * ConnectToBrainpackView class 
     * @brief ConnectToBrainpackView class (This view the in 3d scene that is associated with the "Connect To Brainpack button")
     * @note Note this view requires a PlaceholderTransform of type RectTransform placed in the scene. 
     */

    /// <summary>
    /// ConnectToBrainpackView class (This view the in 3d scene that is associated with the "Connect To Brainpack" button)
    /// </summary>
    public class ConnectToBrainpackView : MonoBehaviour, IInGameMenuItem
    {
        public RectTransform PlaceholderTransform; //once this view is enabled, then the brainpack info panel is shown , the info panel copies 
        //this Placeholder transform's attribute
        /// <summary>
        /// the "Connect to Brainpack" button
        /// </summary>
        public Button CurrentButton
        {
            get
            {
                return gameObject.GetComponent<Button>();
            }
        }
        /**
        * Hide()
        * @brief Hide the associated components from this view
        */

        /// <summary>
        /// Hide the associated components from this view
        /// </summary>
        public void Hide()
        {
            BrainpackConnectionController.Instance.View.Hide();
        }
        /**
        * Hide()
        * @brief Shows the associated components from this view
        */
        /// <summary>
        /// Shows the associated components from this view
        /// </summary>
        public void Show()
        {
            RectTransformUtilities.CopyRectTransformProperties(BrainpackConnectionController.Instance.View.RectTransform, PlaceholderTransform);
            BrainpackConnectionController.Instance.View.Show();

        }
    }
}
