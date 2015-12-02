
/** 
* @file ActivitiesContextViewTrain.cs
* @brief Contains the ActivitiesContextViewTrain class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.ActivitiesContext.View
{
    /// <summary>
    /// Represents the training context view
    /// </summary>
    public class ActivitiesContextViewTrain : MonoBehaviour, IActivitiesContextViewSubcomponent
    {
        public Button BackButton;
        public GameObject HeddokoModel; 
        public GameObject BetaHighLimbsGeo;
        public GameObject BetaHighJointsGeo;
        public GameObject BetaHighTorsoGeo;
        public Material RegularTrainingMaterial;
        public Material RegularJointstTrainingMaterial;
        /// <summary>
        /// Enables and shows the view
        /// </summary>
        public void Show()
        {
            HeddokoModel.SetActive(true);
            gameObject.SetActive(true);
            BetaHighLimbsGeo.GetComponent<Renderer>().material = RegularTrainingMaterial;
            BetaHighTorsoGeo.GetComponent<Renderer>().material = RegularTrainingMaterial;
            BetaHighJointsGeo.GetComponent<Renderer>().material = RegularJointstTrainingMaterial;
        }
        /// <summary>
        /// Hides the view
        /// </summary>
        public void Hide()
        {
            HeddokoModel.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
