﻿/** 
* @file BodiesManager.cs
* @brief Contains the BodiesManager class
* @author Mazen Elbawab (mazen@heddoko.com)
* @date June 2015
*/
using UnityEngine; 
using System.Collections.Generic;

/**
* BodiesManager class 
* @brief manager class for Bodies (interface later)
*/
public sealed class BodiesManager : MonoBehaviour
{
    #region Singleton definition
    private static readonly BodiesManager instance = new BodiesManager();

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static BodiesManager()
    {
    }

    private BodiesManager()
    {
    }

    public static BodiesManager Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    //Bodies available
    public List<Body> Bodies = new List<Body>();

    /**
    * BodyExist()
    * @param vBodyUUID: The body UUID
    * @return bool: True if the body exists
    * @brief searches if the body exists in the manager
    */
    public bool BodyExist(string vBodyUUID)
    {
        return Bodies.Exists(x => x.BodyGuid == vBodyUUID);
    }

    /**
    * CreateNewBody()
    * @brief Creates a new body and adds it to the list
    */
    public void CreateNewBody(string vBodyUUID = "")
    {
        Body vBody = new Body();
        vBody.InitBody(vBodyUUID);
        Bodies.Add(vBody);
    }

    /**
    * AddNewBody()
    * @param vBodyUUID: The body UUID
    * @brief Adds a body to the list
    */
    public void AddNewBody(string vBodyUUID)
    {
        //If the body doesn't exist create a new one otherwise do nothing
        if (!BodyExist(vBodyUUID))
        {
            CreateNewBody(vBodyUUID);
        }
    }

    /**
    * GetBodyFromUUID()
    * @param vBodyUUID: The Body UUID
    * @return Body: The body found
    * @brief looks for a body by its UUID
    */
    public Body GetBodyFromUUID(string vBodyUUID)
    {
        if (BodyExist(vBodyUUID))
        {
            return Bodies.Find(x => x.BodyGuid == vBodyUUID);
        }

        return null;
    }

    /**
    * GetBodyFromRecordingUUID()
    * @param vRecUUID: The body recording UUID
    * @return Body: The body found
    * @brief looks for a body based on its recording UUID
    */
    public Body GetBodyFromRecordingUUID(string vRecUUID)
    {
        //Find the recording first
        BodyFramesRecording vTempRecording = BodyRecordingsMgr.Instance.GetRecordingByUUID(vRecUUID);
        if(vTempRecording != null)
        {
            //Find the related body
           // return GetBodyFromUUID(vTempRecording.BodyRecordingGuid);
            return GetBodyFromUUID(vTempRecording.BodyGuid);
        }
          
        return null;
    }
}
