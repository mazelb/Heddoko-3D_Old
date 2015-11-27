/** 
* @file GuiColliderResizer.cs
* @brief Contains the GuiColliderResizer class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved

*/
using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// GuiColliderResizerClass: Responsible for resizing the collider attached to the game objects according to the handles 
/// positions
/// </summary>
[RequireComponent(typeof(PolygonCollider2D))]
public class GuiColliderResizer : MonoBehaviour {

    public List<RectTransform> Handles;
    public RectTransform obj;
    private PolygonCollider2D mPolygonCollider;
    public Vector2 res;
    public Camera UiCam;
	// Use this for initialization
	void Awake() {
        Debug.Log(obj.position + " " + obj.localPosition);
        mPolygonCollider = GetComponent<PolygonCollider2D>();
        Vector3[] vVertices = new Vector3[mPolygonCollider.points.Length];//mPolygonCollider.points;
        Vector2[] vVector2  = new Vector2[mPolygonCollider.points.Length];
        for (int i = 0; i < vVertices.Length; i ++)
        {
          //  Vector3[] vvv = new Vector3[4];
        //    Handles[i].GetWorldCorners(vvv);
         //   vVector2[i] = new Vector2(vvv[0].x, vvv[0].y);
          //  vVector2[i] = new Vector2(Handles[i].localPosition.x * Handles[i].localScale.x, Handles[i].localPosition.y * Handles[i].localScale.y);//            vVector2[i]= Handles[i].parent.parent.parent.TransformPoint(Handles[i].localPosition);
         vVertices[i] = Handles[i].localPosition;
          
          //  float vX = Screen.width * Handles[i].anchorMax.x  + Handles[i].anchoredPosition.x;
          //    float vY = Screen.height * Handles[i].anchorMax.y  +  Handles[i].anchoredPosition.y;
          //    RectTransformUtility.ScreenPointToWorldPointInRectangle(Handles[i],)
          //  Vector2 vScreenPoint = new Vector2(vX, vY);
          // RectTransformUtility.ScreenPointToWorldPointInRectangle(Handles[i], vScreenPoint, UiCam, out   vVertices[i]);
          //  vVector2[i] = vVertices[i];

        }
        // mPolygonCollider.points[0] = new Vector2(10, 100);
        mPolygonCollider.points = vVector2;
    }
	
	// Update is called once per frame
	void Update () {

        mPolygonCollider.points[0] = res;
    }
}
