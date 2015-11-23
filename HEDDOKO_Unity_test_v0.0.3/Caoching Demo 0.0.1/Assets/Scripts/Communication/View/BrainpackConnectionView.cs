/** 
* @file BrainpackConnectionView.cs
* @brief Contains the BrainpackConnectionView class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved

*/

using Assets.Scripts.Communication.Controller;
using Assets.Scripts.UI; 
using Assets.Scripts.Utils.UnityUtilities;
using Assets.Scripts.Utils.UnityUtilities.Repos;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Communication.View
{
    /// <summary>
    /// Represents the brainpack connection view in scene
    /// </summary>
    /**
     * BrainpackConnectionView class 
     * @brief Represents the brainpack connection view in scene 
     */
    public class BrainpackConnectionView: MonoBehaviour
    {
        
      
        public Sprite HalomanConnected;
        public Sprite HalomanConnecting;
        public Sprite HalomanFailure;
        public Image Haloman;
        public Image HaloForHaloman;
        public Button PairButton;
        public Image UnpairButton;
        private WarningBoxView mWarningBoxView;
        public FadeInFadeOutEffect FadeInFadeOutEffect;

        /// <summary>
        /// Returns the WarningBoxView of this current view
        /// </summary>
        /**
        * WarningBox
        * @brief Returns the WarningBoxView of this current view  
        */
        private WarningBoxView WarningBox
        {
            get
            {
                if (mWarningBoxView == null)
                {
                    //find it the warning box in the scene
                    mWarningBoxView = GameObject.FindObjectOfType<WarningBoxView>();
                    if (mWarningBoxView == null)
                    {
                        GameObject vWarningBoxPanel = Instantiate(PrefabRepo.WarningIconPanelPrefab);
                        vWarningBoxPanel.SetActive(true);
                        //get the current transforms child index
                        int vChildIndex = transform.GetSiblingIndex();
                        //get the parent transform 
                        Transform vParent = transform.parent;
                        vWarningBoxPanel.transform.SetParent(vParent, false);
                        vWarningBoxPanel.transform.SetSiblingIndex(vChildIndex + 1);
                        mWarningBoxView = vWarningBoxPanel.GetComponent<WarningBoxView>();
                        vWarningBoxPanel.SetActive(false);
                    }
                }
                return mWarningBoxView;
            }
        }


        /// <summary>
        /// Returns the RectTransform associated with this view
        /// </summary>
        /**
        * RectTransform
        * @brief Returns the RectTransform associated with this view
        */
        public RectTransform RectTransform
        {
            get
            {
                return GetComponent<RectTransform>();  
            }
        }


        /// <summary>
        /// On Start, hook listener's into Controller events
        /// </summary>
        /**
        * Start()
        * @brief On Start, hook listener's into Controller events
        */
        void Start()
        {
            BrainpackConnectionController.ConnectingStateEvent += OnConnection;
            BrainpackConnectionController.ConnectedStateEvent += OnConnect;
            BrainpackConnectionController.DisconnectedStateEvent += OnDisconnect;
            BrainpackConnectionController.FailedToConnectStateEvent += FailedConnection;
            PairButton.onClick.AddListener(PairButtonEngaged);
            DontDestroyOnLoad(gameObject);
        }
        /// <summary>
        ///  Display the failed connection views
        /// </summary>
        /**
        * FailedConnection()
        * @brief Display the failed connection views
        */
        private void FailedConnection()
        {
            FadeInFadeOutEffect.enabled = true;
            HaloForHaloman.enabled = true;
            HaloForHaloman.gameObject.SetActive(true);
            HaloForHaloman.sprite = HalomanFailure;
            FadeInFadeOutEffect.FadeEffectTime = 0.5f;
            BrainpackConnectionController.Instance.ResetTries();
            PairButton.gameObject.SetActive(true);
            WarningBox.Show();
            //ModalPanel.Instance().Choice(TryConnectionAgain, );
        }
        /// <summary>
        ///  Display the failed Disconnected views
        /// </summary>
        /**
        * FailedConnection()
        * @brief Display the failed Disconnected views
        */
        private void OnDisconnect()
        {
            HaloForHaloman.gameObject.SetActive(false);
            WarningBox.Show();
        }
        /// <summary>
        /// Display the connecting views
        /// </summary>
        /**
        * FailedConnection()
        * @brief Display the connecting views
        */
        private void OnConnection()
        {
            FadeInFadeOutEffect.enabled = true;
            HaloForHaloman.enabled = true;
            HaloForHaloman.gameObject.SetActive(true); 
            HaloForHaloman.sprite = HalomanConnecting;
            FadeInFadeOutEffect.FadeEffectTime = 2.5f;
        }
        /// <summary>
        /// on connect view
        /// </summary>
        /**
        * OnConnect()
        * @brief Display the succesfull connection views
        */
        void OnConnect()
        {
            FadeInFadeOutEffect.enabled = true;
            HaloForHaloman.enabled = true;
            HaloForHaloman.gameObject.SetActive(true);
            HaloForHaloman.sprite = HalomanConnected;
            FadeInFadeOutEffect.FadeEffectTime = 1.5f; 
        }
        /// <summary>
        /// Enables and shows the brainpack connection view
        /// </summary>
        /**
        * Show()
        * @brief Enables and shows the brainpack connection view
        */
        public void Show()
        {
            gameObject.SetActive(true);
        }
        /// <summary>
        ///  Disables and hides the brainpack connection view
        /// </summary>
        /**
        * Show()
        * @brief  Disables and hides the brainpack connection view
        */
        public void Hide()
        {
            FadeInFadeOutEffect.enabled = false;
            HaloForHaloman.enabled = false;
            gameObject.SetActive(false);
        }
        /// <summary>
        ///  The pairing button has been engaged
        /// </summary>
        /**
        * PairButtonEngaged()
        * @brief   The pairing button has been engaged
        */
        private void PairButtonEngaged()
        {
            BrainpackConnectionController.Instance.ConnectToBrainpack();
            PairButton.gameObject.SetActive(false);
            WarningBox.Hide();
        }

        /// <summary>
        /// Sets the warning box text to the passed in param
        /// </summary>
        /// <param name="vMsg">The message to change to </param>
        /**
        *  SetWarningBoxMessage(string vMsg)
        * @brief Sets the warning box text to the passed in param
        * @param string vMsg: The message to change to 
        */
        internal void SetWarningBoxMessage(string vMsg)
        {
            WarningBox.WarningText.text= vMsg;
            WarningBox.WarningText.text.Replace("\\n", "\n");
        }


 
    }
}
