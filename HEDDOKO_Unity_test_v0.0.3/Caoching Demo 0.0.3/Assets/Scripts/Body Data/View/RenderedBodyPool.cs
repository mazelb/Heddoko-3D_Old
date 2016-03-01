/**
* @file RenderedBodyPool.cs
* @brief Contains the RenderedBodyPool class
* @author Mohammed Haider(mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Body_Data.View
{
    /// <summary>
    /// Contains a pool of inactive and active rendered bodies
    /// </summary>
   public static class RenderedBodyPool
    {
        private static List<RenderedBody> sInUsePool  = new List<RenderedBody>(10);
        private static List<RenderedBody> sAvailablePool  = new List<RenderedBody>(10);

        /// <summary>
        /// Request a bodybrain pack resource
        /// </summary>
        /// <param name="vBodyTypes"></param>
        /// <returns></returns>
        public static RenderedBody RequestResource(BodyStructureMap.BodyTypes vBodyTypes)
        {
            RenderedBody vPooledBody = null;
            if (sAvailablePool.Count != 0)
            {
                vPooledBody = sAvailablePool[0];
                vPooledBody.UpdateBodyType(vBodyTypes);
                vPooledBody.gameObject.SetActive(true);
                sAvailablePool.RemoveAt(0);
                sInUsePool.Add(vPooledBody);
            }
            else
            {
                //instantiate obj from resources
                vPooledBody = Resources.Load<RenderedBody>("Prefabs/Models/SegmentedModel");
                vPooledBody.gameObject.SetActive(false);
                vPooledBody.Init(vBodyTypes);
                vPooledBody.gameObject.SetActive(true); 
                sInUsePool.Add(vPooledBody);
            }
            return vPooledBody;
        }

        /// <summary>
        /// Releases a rendered body from the InUse pool, cleans up resources and makes it available for use
        /// </summary>
        /// <param name="vRenderedBody"></param>
        public static void ReleaseResource(RenderedBody vRenderedBody)
        {
            vRenderedBody.Cleanup();
            sInUsePool.Remove(vRenderedBody);
            sAvailablePool.Add(vRenderedBody);
        }


    }
}
