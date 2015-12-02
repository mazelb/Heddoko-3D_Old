
/** 
* @file LearnFromRecordingView.cs
* @brief Contains the LearnFromRecordingView class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.ActivitiesContext.View
{
    /// <summary>
    /// Represents the learn from recording view
    /// </summary>
    public class LearnFromRecordingView : MonoBehaviour, IActivitiesContextViewSubcomponent 
    {
        public GameObject HeddokoModel;
        public GameObject BetaHighLimbsGeo;
        public GameObject BetaHighJointsGeo;
        public GameObject BetaHighTorsoGeo;
        public Button CancelButton;
        public Button TrainButton;
        public Material TransparentMaterial;
        public Material TrasparentJointsMaterial;
        
        //Todo: meters
        /// <summary>
        /// Enables and displays the view
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
            HeddokoModel.SetActive(true);
            BetaHighLimbsGeo.GetComponent<Renderer>().material = TransparentMaterial;
            BetaHighTorsoGeo.GetComponent<Renderer>().material = TransparentMaterial;
            BetaHighJointsGeo.GetComponent<Renderer>().material = TrasparentJointsMaterial;
        }
        /// <summary>
        /// Hides the view
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
            HeddokoModel.SetActive(false);
        }
    }
}
