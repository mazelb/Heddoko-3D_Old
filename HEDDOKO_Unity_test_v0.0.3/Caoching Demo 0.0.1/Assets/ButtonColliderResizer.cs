/** 
* @file GuiColliderResizer.cs
* @brief Contains the GuiColliderResizer class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved

*/
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
/// <summary>
/// GuiColliderResizerClass: Responsible for resizing the collider attached to the game objects according to the handles 
/// This script will set up the vertices of polygoncolliders on awake
/// positions 
/// </summary>
[RequireComponent(typeof(PolygonCollider2D),typeof(RaycastMask))]
public class ButtonColliderResizer : MonoBehaviour
{ 
    //RectTransforms that will anchor the collider points
    public List<RectTransform> Anchors;
    public RectTransform ButtonRectTransform;
    private PolygonCollider2D mPolygonCollider;

    void Start()
    {
        ButtonRectTransform = GetComponent<RectTransform>();
        ResizeCollider();
        
    }

    /// <summary>
    /// Resize a collider
    /// </summary>
    public void ResizeCollider()
    {
        mPolygonCollider = GetComponent<PolygonCollider2D>();

        Vector2[] vVertices = new Vector2[mPolygonCollider.points.Length];
        for (int i = 0; i < vVertices.Length; i++)
        {
            //Get the anchor point in world space
            Vector2 vAnchoringPoint = Anchors[i].TransformPoint(Anchors[i].rect.center);
            //Get the ButtonRectTransform point in world space
            Vector2 vParentButtonPoint = ButtonRectTransform.TransformPoint(ButtonRectTransform.rect.center); 
            vAnchoringPoint -= vParentButtonPoint;
            //set the anchor point 
            vVertices[i] = vAnchoringPoint;
        }

        mPolygonCollider.points = vVertices;
    }

}
