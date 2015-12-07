﻿/** 
* @file Model2D3DSwitch.cs
* @brief Contains the Model2D3DSwitch class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

 
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.RecordingLoading
{
    /// <summary>
    /// Switches between the 3D and 2D model
    /// </summary>
   public class Model2D3DSwitch : MonoBehaviour
    {
        public GameObject Model3D;
        public GameObject Model2D;
        
        /// <summary>
        /// Location placement
        /// </summary>
        public Transform TransformInview3DLocation;
        public Transform TransformInview2DLocation;
        public Transform TransformOutOfViewLocation;

        public Button Button2DSwitch;
        public Button Button3DSwitch;
        public Button RotateCameraLeft;
        public Button RotateCameraRight;

        /// <summary>
        /// Flag to check if only using 2D model
        /// </summary>
        public bool OnlyUsing2D;
        [SerializeField]
        private bool mUsing2DModel = false;

        /// <summary>
        /// On awake hook a listener to the button and set button interaction according to mUsing2DModelFlag
        /// </summary>
        void Awake()
        {
            if (OnlyUsing2D)
            {
                //disable buttons
                Button2DSwitch.gameObject.SetActive(false);
                Button3DSwitch.gameObject.SetActive(false);
                mUsing2DModel = true;
            }
            else
            {
                Button2DSwitch.onClick.AddListener(SwitchTo2DModelView);
                Button3DSwitch.onClick.AddListener(SwitchTo3DModelView);
                Button2DSwitch.interactable = !mUsing2DModel;
                Button3DSwitch.interactable = mUsing2DModel;
            }
        }

        /// <summary>
        /// Switches to a 3d model view
        /// </summary>
        private void SwitchTo3DModelView()
        {
            Bring3DModelIntoView();
            SetButtonInteraction();
        }

        /// <summary>
        /// Switches to a 2D model view
        /// </summary>
        private void SwitchTo2DModelView()
        {
            Bring2DModelIntoView();
            SetButtonInteraction();
        }

        /// <summary>
        /// Sets the button's interaction
        /// </summary>
        private void SetButtonInteraction()
        {
            mUsing2DModel = !mUsing2DModel;
            Button2DSwitch.interactable = !mUsing2DModel;
            Button3DSwitch.interactable = mUsing2DModel;
        }

        /// <summary>
        /// Brings them into view depending on what mode currently in
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true); 

            //set the positions of the model
            if (mUsing2DModel)
            {
                Model2D.transform.position = TransformInview2DLocation.position;
                Model3D.transform.position = TransformOutOfViewLocation.position;
            }
            else
            {
                Model3D.transform.position = TransformInview3DLocation.position;
                Model2D.transform.position = TransformOutOfViewLocation.position;
            } 
        }

        /// <summary>
        /// Brings the 2d model into view and hides the 3d model
        /// </summary>
        private void Bring2DModelIntoView()
        {
            Model2D.transform.position = TransformInview2DLocation.position;
            Model3D.transform.position = TransformOutOfViewLocation.position;
        }

        /// <summary>
        /// Brings the 3d model into view and hides the 2d model
        /// </summary>
        private void Bring3DModelIntoView()
        {
            Model3D.transform.position = TransformInview3DLocation.position;
            Model2D.transform.position = TransformOutOfViewLocation.position;
        }

        /// <summary>
        /// hides the models out of view
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
            Model3D.transform.position = TransformOutOfViewLocation.position;
            Model2D.transform.position = TransformOutOfViewLocation.position;
        }
    }
}
