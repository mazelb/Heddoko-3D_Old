﻿
/** 
* @file ActivitiesContextViewTrain.cs
* @brief Contains the ActivitiesContextViewTrain class
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
    /// Represents the training context view
    /// </summary>
    public class ActivitiesContextViewTrain : MonoBehaviour, IActivitiesContextViewSubcomponent
    {
        /// <summary>
        /// When the model needs to be shown, it will be placed on this anchor
        /// </summary>
        public Transform HeddokoModelEnabledAnchor;

        /// <summary>
        /// When the model needs to be hidden, it will be placed on this anchor
        /// </summary>
        public Transform HeddokoModelHiddenAnchor;

        public Button BackButton;
        public GameObject HeddokoModel; 
        public GameObject BetaHighLimbsGeo;
        public GameObject BetaHighJointsGeo;
        public GameObject BetaHighTorsoGeo;
        public Material RegularTrainingMaterial;
        public Material RegularJointstTrainingMaterial;

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
        /// <summary>
        /// Enables and shows the view
        /// </summary>
        public void Show()
        {
            HeddokoModel.transform.position = HeddokoModelEnabledAnchor.position;
         //   HeddokoModel.transform.rotation = HeddokoModelEnabledAnchor.rotation; 
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
            HeddokoModel.transform.position = HeddokoModelHiddenAnchor.position;
            PlayerStreamManager.Stop();
            PlayerStreamManager.ResetInitialFrame(); 
            gameObject.SetActive(false);
        }
    }
}
