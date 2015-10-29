/** 
* @file ExampleOfRecordingPlay.cs
* @brief Contains the ExampleOfRecordingPlay class and functionalities
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date October 2015
*/
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Utils;
/**
* BodyView class 
* @brief Contains an example on how to play a Body's recording
*/
public class ExampleOfRecordingPlay : MonoBehaviour
{

    private Body mBody;

    public Button PlayButton;
    private bool mPlayButtonPushed;
    public Button ResetButton;
    /**
    * Start()
    * @brief Automatically called by unity on start. This start function will prep the body to be able to play a recording 
    */ 
    private void Start()
    {
        BodyRecordingsMgr.Instance.ScanRecordings(FilePathReferences.sCsvDirectory+);
    }

    /**
    * Start 
    * @brief Will play the recording for the prepped body
    */
    public void Play()
    {
        if(!mPlayButtonPushed)
        {
            if(mBody != null)
            {
                mPlayButtonPushed = true;
                PlayButton.gameObject.SetActive(false);
            }
        }        
    }
    /**
    * ResetInitialFrame 
    * @brief will set the Initial Frame
    */
    public void ResetInitialFrame()
    {
        mBody.View.ResetJoint();
    }
    /**
    * Pause 
    * @brief Pauses the recording's play back
    */
    public void Pause()
    {
        //todo
    }


}
