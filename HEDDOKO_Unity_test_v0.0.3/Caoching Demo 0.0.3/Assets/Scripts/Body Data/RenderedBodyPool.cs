/** 
* @file RenderedBodyPool.cs
* @brief Contains the RenderedBodyPool  class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date October 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System.Collections.Generic;
using Assets.Scripts.UI.Analysis;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Body_Data
{
    /// <summary>
    /// Provides and releases a RenderedBody from a pool of objects
    /// </summary>
    public  static class RenderedBodyPool
    {
        private const int MaxSize = 10;
        private static List<RenderedBodyComponent> sAvailable = new List<RenderedBodyComponent>(MaxSize);
        private static List<RenderedBodyComponent> sInUse = new List<RenderedBodyComponent>(MaxSize);
        private static GameObject ParentGameObject;
        public static RenderedBodyComponent RequestRenderedBody(BodyStructureMap.BodyTypes vBodyType, Transform vParent)
        {
            return null;
            /*if (sAvailable.Count != 0)
            {
                RenderedBodyComponent vPooledObject = sAvailable[0];
                sInUse.Add(vPooledObject);
                sAvailable.RemoveAt(0);
                vPooledObject.SetBodyType(vBodyType);
                vPooledObject.gameObject.SetActive(true); 
                return vPooledObject;
            }
            else
            {
                RenderedBodyComponent vPooledGo =
                    Resources.Load("Prefabs/Models/SegmentedModel") as RenderedBodyComponent;//new GameObject();
                vPooledGo.gameObject.SetActive(false);
                vPooledGo.SetBodyType(vBodyType);
                //ddoko-3D\HEDDOKO_Unity_test_v0.0.3\Caoching Demo 0.0.3\Assets\Resources\Prefabs\Models

                vPooledGo.transform.parent = vPooledGo.transform;
                vPooledGo.transform.parent = GridViewCellParent.transform;

                vPooledGo.name = "AnalysisSubViewComponent";
                vPooledGo.AddComponent<GraphicRaycaster>();
                Image v = vPooledGo.AddComponent<Image>();
                Color vcolor = v.color;
                vcolor.a = 0;
                v.color = vcolor;
                GridViewCell vSubviewComp =
                    vPooledGo.AddComponent<GridViewCell>();
                vSubviewComp.ControlCanvas = vPooledGo.GetComponent<Canvas>();
                if (vSubviewComp.ControlCanvas == null)
                {
                    vSubviewComp.ControlCanvas = vPooledGo.AddComponent<Canvas>();
                }
                vSubviewComp.CameraControl = SubViewCameraPool.GetCamControl;
                SetButtonFunctionality(vSubviewComp);
                sInUseSubViews.Add(vSubviewComp);


                return vSubviewComp;
            }*/
        }

        public static void ReleaseRenderedBody(RenderedBodyComponent vRenderedBody)
        {
            
        }

    }
}
