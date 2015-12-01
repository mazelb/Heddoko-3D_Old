/** 
* @file MainMenuController.cs
* @brief Contains the MainMenuController class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved 
*/
 
using Assets.Scripts.UI.MainMenu.View; 
using UnityEngine; 

namespace Assets.Scripts.UI.MainMenu.Controller
{
    /// <summary>
    /// Represents the controller for the main menu. This class allows for the changing of context views
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {

        //The main menu view
        public MainMenuView MainMenuView;

        public SplashScreen SplashScreen;
        private MainMenuState mCurrentState = MainMenuState.SplashScreen;


        void Awake()
        {
            bool vSplashScreenEnabled = SplashScreen != null && SplashScreen.gameObject.activeSelf;
            //if conditions fail, load directly into mainmenu view
            if (!vSplashScreenEnabled)
            {
                ChangeState(MainMenuState.MainMenu);
            }
            MainMenuView.BrainpackButton.onClick.AddListener(() => ChangeState(MainMenuState.BrainpackView));
            MainMenuView.ActivitiesButton.onClick.AddListener(() => ChangeState(MainMenuState.ActivityContext));
            MainMenuView.BrainpackConnectionView.BackButton.onClick.AddListener(() => ChangeState(MainMenuState.MainMenu));
        }
        /// <summary>
        /// changes the state of the main menu
        /// </summary>
        /// <param name="vNewState"></param>
        private void ChangeState(MainMenuState vNewState)
        {
            switch (mCurrentState)
            {

                case (MainMenuState.SplashScreen):
                    {
                        if (vNewState == MainMenuState.MainMenu)
                        {
                            MainMenuView.ShowMainMenuContextView();
                            mCurrentState = MainMenuState.MainMenu;
                            break;
                        }
                        break;
                    }

                case (MainMenuState.ActivityContext):
                    {
                        if (vNewState == MainMenuState.MainMenu)
                        {
                            MainMenuView.HideActivityContextView();
                            MainMenuView.ShowMainMenuContextView();
                            mCurrentState = MainMenuState.MainMenu;
                            break;
                        }
                        break;
                    }

                case (MainMenuState.BrainpackView):
                    {

                        if (vNewState == MainMenuState.MainMenu)
                        {
                            MainMenuView.HideBrainpackContextView();
                            MainMenuView.ShowMainMenuContextView();
                            mCurrentState = MainMenuState.MainMenu;
                            break;
                        }
                        break;
                    }

                case (MainMenuState.MainMenu):
                    {
                        if (vNewState == MainMenuState.ActivityContext)
                        {
                            MainMenuView.HideMainMenuContextView();
                            MainMenuView.ShowActivitiesContextView();
                            mCurrentState = MainMenuState.ActivityContext;
                            break;
                        }

                        if (vNewState == MainMenuState.BrainpackView)
                        {
                            MainMenuView.HideMainMenuContextView();
                            MainMenuView.ShowBrainpackContextView();
                            mCurrentState = MainMenuState.BrainpackView;
                            break;
                        }

                        break;
                    }

            }
        }

        /// <summary>
        /// The Splash screen has finished loading. 
        /// </summary>
        public void SplashScreenTransitionFinished()
        {
            ChangeState(MainMenuState.MainMenu);
        }
        /// <summary>
        /// Determine which state the main menu is in
        /// </summary>
        public enum MainMenuState
        {
            SplashScreen,
            MainMenu,
            BrainpackView,
            ActivityContext
        }
 
    }
}
