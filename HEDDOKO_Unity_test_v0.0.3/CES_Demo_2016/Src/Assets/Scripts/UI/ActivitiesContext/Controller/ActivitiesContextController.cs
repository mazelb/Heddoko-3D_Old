/**  
* @file ActivitiesContextController.cs 
* @brief Contains the ActivitiesContextController class 
* @author Mohammed Haider(mohamed@heddoko.com) 
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved 
*/

using System.Xml.Serialization;
using Assets.Scripts.UI.ActivitiesContext.View;
using Assets.Scripts.UI.MainMenu;
using Assets.Scripts.UI.MainMenu.Controller;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.ActivitiesContext.Controller
{
    /** 
    * ActivitiesContextController class  
    * @brief ActivitiesContextController: the controller for the activities context view
    * @note Note something of interest 
    */
    public class ActivitiesContextController : MonoBehaviour
    {
        //current view state 
        public ActivitiesContextViewState CurrentState = ActivitiesContextViewState.Idle;
        public ActivitiesContextView ActivitesContextView;
        public MainMenuController MainMenuController;
        public PlayerStreamManager PlayerStreamManager;

        /// <summary>
        /// On Awake set up the buttons
        /// </summary>
        void Awake()
        {
            ActivitesContextView.MainView.BackButton.onClick.AddListener(ReturnToMainMenu);
            ActivitesContextView.MainView.ActivityTrainingButton.onClick.AddListener(SwitchToLearningViewState);
            ActivitesContextView.LearningView.SquatButton.onClick.AddListener(SwitchToLearnByRecordingState);
            ActivitesContextView.LearningView.Backbutton.onClick.AddListener(SwitchtoMainActivityView);
            ActivitesContextView.LearnFromRecordingView.CancelButton.onClick.AddListener(SwitchToLearningViewState);
            ActivitesContextView.LearnFromRecordingView.TrainButton.onClick.AddListener(SwitchtoTrainingViewState);
            ActivitesContextView.TrainingView.BackButton.onClick.AddListener(SwitchToLearnByRecordingState);
            //LearningView
        }
        /// <summary>
        /// changes states to Learn
        /// </summary>
        private void SwitchToLearnByRecordingState()
        {
            ChangeState(ActivitiesContextViewState.LearnByRecording);
        }

        /// <summary>
        /// Changes the state of the view
        /// </summary>
        /// <param name="vNewState"></param>
        private void ChangeState(ActivitiesContextViewState vNewState)
        {
            switch (CurrentState)
            {
                case (ActivitiesContextViewState.Idle):
                    {
                        if (vNewState == ActivitiesContextViewState.Main)
                        {
                            CurrentState = ActivitiesContextViewState.Main;
                            ActivitesContextView.Show();
                            ActivitesContextView.SwitchToMainView(); 
                        }
                    }
                    break;
                case (ActivitiesContextViewState.Main):
                    {
                        if (vNewState == ActivitiesContextViewState.Idle)
                        {
                            CurrentState = ActivitiesContextViewState.Idle;
                            ActivitesContextView.SwitchToIdleView();
                            ActivitesContextView.Hide();
                            MainMenuController.SwitchToMainMenu();
                            break;
                        }
                        if (vNewState == ActivitiesContextViewState.Learn)
                        {
                            CurrentState = ActivitiesContextViewState.Learn; 
                            ActivitesContextView.HideMainView();
                            ActivitesContextView.SwitchToLearningView();
                            break;
                        }
                    }
                    break;
                case (ActivitiesContextViewState.Learn):
                {
                    if (vNewState == ActivitiesContextViewState.LearnByRecording)
                    {
                        CurrentState = ActivitiesContextViewState.LearnByRecording;
                        ActivitesContextView.HideLearningView();
                        ActivitesContextView.SwitchToLearnByRecordingView();
                        break;
                    }
                        if (vNewState == ActivitiesContextViewState.Main)
                        {
                            CurrentState = ActivitiesContextViewState.Main;
                            ActivitesContextView.HideLearningView();
                            ActivitesContextView.Show();
                            ActivitesContextView.SwitchToMainView();
                        }
                        break;
                }

                case (ActivitiesContextViewState.LearnByRecording):
                    {
                        if (vNewState == ActivitiesContextViewState.Learn)
                        {
                            CurrentState = ActivitiesContextViewState.Learn;
                            ActivitesContextView.HideLearnByRecordingView();
                            ActivitesContextView.SwitchToLearningView();
                            break;
                        }
                        if (vNewState == ActivitiesContextViewState.Train)
                        {
                            CurrentState = ActivitiesContextViewState.Train;
                            ActivitesContextView.HideLearnByRecordingView();
                            ActivitesContextView.SwitchToTrainingView();
                            PlayerStreamManager.SetBodytoStreamFromBrainpack();
                            break;
                        }

                        break;
                    }

                case (ActivitiesContextViewState.Train):
                {
                    if (vNewState == ActivitiesContextViewState.LearnByRecording)
                    {
                        CurrentState = ActivitiesContextViewState.LearnByRecording;
                        ActivitesContextView.HideTrainingView();
                        ActivitesContextView.SwitchToLearnByRecordingView();
                        PlayerStreamManager.SetBodyToStreamFromRecording();
                    }
                    break;
                }
            }
        }
        /// <summary>
        /// Switch to learning view state
        /// </summary>
        public void SwitchToLearningViewState()
        {
            ChangeState(ActivitiesContextViewState.Learn);
        }

        /// <summary>
        /// Switches to the training view state
        /// </summary>
        public void SwitchtoTrainingViewState()
        {
            ChangeState(ActivitiesContextViewState.Train);
        }
        /// <summary>
        /// Change the current state to main
        /// </summary>
        public void SwitchtoMainActivityView()
        {
            ChangeState(ActivitiesContextViewState.Main);
        }
        /// <summary>
        /// Return to the main menu
        /// </summary>
        public void ReturnToMainMenu()
        {
            ChangeState(ActivitiesContextViewState.Idle); 
        }
        /// <summary>
        /// Change the current state idle
        /// </summary>
        public void SetToIdleState()
        {
            if (CurrentState != ActivitiesContextViewState.Idle)
            {
                ChangeState(ActivitiesContextViewState.Idle);
            }
        }
        public enum ActivitiesContextViewState
        {
            Idle,
            Main,
            Learn,
            LearnByRecording,
            Train
        }
    }
}
