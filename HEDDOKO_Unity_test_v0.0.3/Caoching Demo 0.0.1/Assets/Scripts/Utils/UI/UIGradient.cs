/** 
* @file UIGradient.cs
* @brief Contains the SquatColoredFeedback class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using UnityEngine;
 using UnityEngine.UI;
using System.Collections.Generic;

namespace Assets.Scripts.Utils.UI
{
    /// <summary>
    /// Applies a 2 color gradient to a UnityEngine.UI object
    /// </summary>
    public class UIGradient : BaseMeshEffect
    {
       
        public Color32 TopColor;
        public Color32 BottomColor;
        [SerializeField]
        private InterpolationMethodType InterpolationMethod = InterpolationMethodType.Sinerp;

        [SerializeField] private float mCosPeriod=0.5f;
        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
            {
                return;
            }


            try
            {
                List<UIVertex> vertexList = new List<UIVertex>();
                vh.GetUIVertexStream(vertexList);
                ModifyVertices(vertexList);
                vh.Clear();
                vh.AddUIVertexTriangleStream(vertexList);
            }
            catch (Exception)
            {
            }


        }

        private void ModifyVertices(List<UIVertex> vertexList)
        {
            int count = vertexList.Count;
            float bottomY = vertexList[0].position.y;

            float topY = vertexList[0].position.y;

            for (int i = 1; i < count; i++)
            {
                float y = vertexList[i].position.y;
                if (y > topY)
                {
                    topY = y;
                }
                else if (y < bottomY)
                {
                    bottomY = y;
                }
            }

            float uiElementHeight = topY - bottomY;

            for (int i = 0; i < count; i++)
            {
                UIVertex uiVertex = vertexList[i];
                float percentage = (uiVertex.position.y - bottomY)/uiElementHeight;
                Color32 vNewColor = Color.white;
                float t = 0;
                switch (InterpolationMethod)
                {
                       
                    case (InterpolationMethodType.Lerp):
                        {
                              t = percentage;
                            vNewColor = Color32.Lerp(BottomColor, TopColor, t);
                            break;
                        }


                    case (InterpolationMethodType.Coserp):
                        {

                              t = percentage;
                            t = 1f - Mathf.Cos(t*Mathf.PI* mCosPeriod);
                            vNewColor = Color32.LerpUnclamped(BottomColor, TopColor, t);
                            break;
                        }

                    case (InterpolationMethodType.Sinerp):
                        {
                              t = percentage;
                            t = Mathf.Sin(t * Mathf.PI * 0.5f);
                            vNewColor = Color32.LerpUnclamped(BottomColor, TopColor, t);
                            break;
                        }

                    case (InterpolationMethodType.Quadtradic):
                        {
                              t = percentage*percentage;
                            vNewColor = Color32.Lerp(BottomColor, TopColor, t);
                            break;
                        }

                    case (InterpolationMethodType.SmoothStep):
                        {
                              t = percentage ;
                            t = t*t*(3f - 2f*t);
                            vNewColor = Color32.Lerp(BottomColor, TopColor, t);
                            break;
                        }

                    case (InterpolationMethodType.SmootherStep):
                        {
                              t = percentage;
                            t = t*t*t*(t*(6f*t - 15f) + 10f);
                            vNewColor = Color32.LerpUnclamped(BottomColor, TopColor, t);
                            break;
                        }
                }
              
                    uiVertex.color = vNewColor; 
                vertexList[i] = uiVertex; 
            }
        }

        public enum InterpolationMethodType
        {
            Lerp,
            Sinerp,
            Coserp,
            Quadtradic,
            SmoothStep,
            SmootherStep

        }
    }
}
