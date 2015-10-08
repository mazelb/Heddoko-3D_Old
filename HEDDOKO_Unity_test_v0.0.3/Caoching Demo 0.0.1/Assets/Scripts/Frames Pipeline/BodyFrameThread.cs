/**
* @file BodyFrameThread.cs
* @brief Contains the bodyframethread class
* @author Mazen Elbawab (mazen@heddoko.com)
* @date June 2015
*/

using UnityEngine;
using System.Collections;

/**
* BodyFrameThread class 
* @brief child class for communication threads
*/
public class BodyFrameThread : ThreadedJob
{
    //TODO: Handling different sources: Recording or Suit Comm
    //TODO: Handling Frame by Frame transmition
    
    /**
    * ThreadFunction()
    * @brief The thread loop, overwrite this in the base class
    */
    protected override void ThreadFunction()
    {
        //TODO:
        //Do your threaded task. DON'T use the Unity API here
        while (true) ;
    }
    
    /**
    * OnFinished()
    * @brief Callback when the thread is done executing
    */
    protected override void OnFinished()
    {
        //This is executed by the Unity main thread when the job is finished
        //TODO: 
    }
}
