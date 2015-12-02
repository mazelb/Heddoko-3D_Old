
/** 
* @file LearnFromRecordingView.cs
* @brief Contains the LearnFromRecordingView class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using Assets.Scripts.UI.MainMenu;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.ActivitiesContext.View
{
    /// <summary>
    /// Represents the learn from recording view
    /// </summary>
    public class LearnFromRecordingView : MonoBehaviour, IActivitiesContextViewSubcomponent
    {
        /// <summary>
        /// When the model needs to be shown, it will be placed on this anchor
        /// </summary>
        public Transform HeddokoModelEnabledAnchor;

        /// <summary>
        /// When the model needs to be hidden, it will be placed on this anchor
        /// </summary>
        public Transform HeddokoModelHiddenAnchor;

        public GameObject HeddokoModel;
        public GameObject BetaHighLimbsGeo;
        public GameObject BetaHighJointsGeo;
        public GameObject BetaHighTorsoGeo;
        public Button CancelButton;
        public Button TrainButton;
        public Material TransparentMaterial;
        public Material TrasparentJointsMaterial;
        private PlayerStreamManager mPlayerStreamManager;

        public PlayerStreamManager PlayerStreamManager
        {
            get
            {
                if (mPlayerStreamManager == null)
                {
                    mPlayerStreamManager = FindObjectOfType<PlayerStreamManager>();
                }
                return mPlayerStreamManager;
            }
        }


        //Todo: meters
        /// <summary>
        /// Enables and displays the view
        /// </summary>
        public void Show()
        {
            HeddokoModel.transform.position = HeddokoModelEnabledAnchor.position;
          //  HeddokoModel.transform.rotation = HeddokoModelEnabledAnchor.rotation;
            gameObject.SetActive(true);
            HeddokoModel.SetActive(true); 
          
          //  BetaHighLimbsGeo.GetComponent<Renderer>().material = TransparentMaterial;
        //    BetaHighTorsoGeo.GetComponent<Renderer>().material = TransparentMaterial;
           // BetaHighJointsGeo.GetComponent<Renderer>().material = TrasparentJointsMaterial;
            PlayerStreamManager.Play();

        }
        /// <summary>
        /// Hides the view
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
            HeddokoModel.transform.position = HeddokoModelHiddenAnchor.position;
           // HeddokoModel.transform.rotation = HeddokoModelHiddenAnchor.rotation;
            HeddokoModel.SetActive(false); 
            PlayerStreamManager.Stop();
            PlayerStreamManager.ResetInitialFrame();
        }
    }
}
