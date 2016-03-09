/**
* @file RenderedBodyPool.cs
* @brief Contains the RenderedBodyPool class
* @author Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
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
        public static List<RenderedBody> sInUsePool  = new List<RenderedBody>(10);
        private static List<RenderedBody> sAvailablePool  = new List<RenderedBody>(10);
        public static Transform ParentGroupTransform;
        //available layers that can be used
        private static Stack<LayerMask> sAvailableLayers = new Stack<LayerMask>(8);
        private static Stack<LayerMask> sInUseLayers = new Stack<LayerMask>(8);
        private static bool sInitiated;
      
        /// <summary>
        /// Request a bodybrain pack resource
        /// </summary>
        /// <param name="vBodyTypes"></param>
        /// <returns></returns>
        public static RenderedBody RequestResource(BodyStructureMap.BodyTypes vBodyTypes)
        {
            if (sInitiated == false)
            {
                sInitiated = true;
                   Init();
            }
            RenderedBody vPooledBody = null;
            if (sAvailableLayers.Count == 0)
            {
                return null;
            }

            if (sAvailablePool.Count != 0)
            {
                vPooledBody = sAvailablePool[0];
                vPooledBody.UpdateBodyType(vBodyTypes);
                vPooledBody.gameObject.SetActive(true);
                sAvailablePool.RemoveAt(0);
                sInUsePool.Add(vPooledBody);
                int vLayer = sAvailableLayers.Pop();
                sInUseLayers.Push(vLayer);
                vPooledBody.CurrentLayerMask =vLayer;
            }
            else
            {
                //instantiate obj from resources
                GameObject vObj = Resources.Load("Prefabs/Models/SegmentedModel") as GameObject;
                GameObject vNew = GameObject.Instantiate(vObj);
                vPooledBody = vNew.GetComponent<RenderedBody>();  
                vPooledBody.gameObject.SetActive(false);
                vPooledBody.Init(vBodyTypes);
                vPooledBody.gameObject.SetActive(true); 
                vNew.transform.SetParent(ParentGroupTransform);
                sInUsePool.Add(vPooledBody);
                int vLayer = sAvailableLayers.Pop();
                sInUseLayers.Push(vLayer);
                vPooledBody.CurrentLayerMask = vLayer;
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
            int vLayer = sInUseLayers.Pop();
            sAvailableLayers.Push(vLayer); 
        }

        /// <summary>
        /// initalize masks
        /// </summary>
        static void Init()
        {
            for (int i = 6; i >= 0; i--)
            {
                sAvailableLayers.Push(LayerMask.NameToLayer("RenderedBody"+i));
            }
            //default model
            sAvailableLayers.Push(LayerMask.NameToLayer("model")); 
        }

    }
}
