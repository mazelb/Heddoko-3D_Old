﻿/** 
* @file RaycastMask.cs
* @brief Contains the RaycastMask class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved

*/

using UnityEngine; 

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Collider2D))]

/**
* RaycastMask class 
* @brief A script that can be used with Buttons with custom shapes, contingent on a collider2d contouring the shape of the button
* @note something 
*/

public class RaycastMask : MonoBehaviour, ICanvasRaycastFilter
{ 
    private RectTransform mCurrentRectTransform;
    private Collider2D mCollider2D;
    private InformationPanel mInformationPanel;
    public string InformationText;
    #region unity function

    void Awake()
    { 
        mCurrentRectTransform = GetComponent<RectTransform>();
        mCollider2D = GetComponent<Collider2D>();
      //  GameObject vInformationPanelGo = GameObject.FindGameObjectWithTag("InformationPanel");
     //   mInformationPanel = vInformationPanelGo.GetComponent<InformationPanel>();
    }
    /**
      *  IsRaycastLocationValid(Vector2 vScreenPoint, Camera vEventCamera)
      * @param : Vector2 vScreenPoint: location on screen.Camera vEventCamera: the camera capturing the event
      * @brief Checked by unity, this checks if the point of contact is valid 
      */

    /// <summary>
    /// Checked by unity, this checks if the point of contact is valid
    /// </summary>
    /// <param name="vScreenPoint"></param>
    /// <param name="vEventCamera"></param>
    /// <returns></returns>
    public bool IsRaycastLocationValid(Vector2 vScreenPoint, Camera vEventCamera)
    { 
        var vWorlderPoint = Vector3.zero;
        var vIsInside = RectTransformUtility.ScreenPointToWorldPointInRectangle(
            mCurrentRectTransform,
            vScreenPoint,
            vEventCamera,
            out vWorlderPoint
        );
        if (vIsInside)
        {
            vIsInside = mCollider2D.OverlapPoint(vWorlderPoint);
//            mInformationPanel.UpdateInformationPanel(InformationText);
        }
        return vIsInside;

    }
    #endregion
}
