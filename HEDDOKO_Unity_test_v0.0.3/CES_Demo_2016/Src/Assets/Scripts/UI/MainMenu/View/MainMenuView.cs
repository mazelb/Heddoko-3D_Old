/** 
* @file MainMenuView.cs
* @brief Contains the MainMenuView class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using Assets.Scripts.UI.ActivitiesContext.Controller;
using Assets.Scripts.UI.ActivitiesContext.View;
using Assets.Scripts.UI.RecordingLoading.View;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.MainMenu.View
{
    /// <summary>
    /// MainMenuView class:  The view of the main menu, directly after the splash Screen
    /// </summary>
    public class MainMenuView : MonoBehaviour
    {

        public Button BrainpackButton;
        public Button ActivitiesButton;
        public Button RecordingsSelectionButton;

        //The Activities context view
        public ActivitiesContextController ActivitiesContext;

        //The Brainpack/Bluetooth connection view
        public MainMenuBrainpackView BrainpackConnectionView;

        public RecordingSelectionView RecordingSelectionView;
        /// <summary>
        /// Shows the Main menu view
        /// </summary>
        public void ShowMainMenuContextView()
        {
            BrainpackButton.interactable = true;
            ActivitiesButton.interactable = true;
            gameObject.SetActive(true);

        }

        /// <summary>
        /// hide the Main menu view
        /// </summary>
        public void HideMainMenuContextView()
        {
            BrainpackButton.interactable = false;
            ActivitiesButton.interactable = false;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Show the Brainpack context view
        /// </summary>
        public void ShowBrainpackContextView()
        {
            BrainpackConnectionView.Show();
        }

        /// <summary>
        /// Hides the Brainpack context view
        /// </summary>
        public void HideBrainpackContextView()
        {
            BrainpackConnectionView.Hide();
        }

        /// <summary>
        /// Shows the activities context view
        /// </summary>
        public void ShowActivitiesContextView()
        {
            ActivitiesContext.SwitchtoMainActivityView();
        }

        /// <summary>
        /// hides the activities context view
        /// </summary>
        public void HideActivityContextView()
        {
            ActivitiesContext.SetToIdleState();
        }

        /// <summary>
        /// Shows the recording selections view
        /// </summary>
        public void ShowRecordingsSelection()
        {
            RecordingSelectionView.Show();
        }

        /// <summary>
        /// Hides the recordings selections view
        /// </summary>
        public void HideRecordingsSelection()
        {
            RecordingSelectionView.Hide();
        }

    }
}
