/** 
* @file MouseGOMover.cs
* @brief  Mouse gameobject mover: applies movement to a gameobject according to mouse position
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date October 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using UnityEngine;

namespace Assets.Scripts.Utils.UnityUtilities
{
    public class MouseGOMover : MonoBehaviour
    {
        public Texture2D MouseOver;
        public Texture2D OnMouseDown;

        public LayerMask CollisionLayer;
        Ray mRay;
        RaycastHit mHit;
        private bool mIsDraggingObject;
        private Vector3 mDirection;
        public Vector2 mHotSpot = Vector2.zero;
        void Update()
        {
            mRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mRay, 150f, CollisionLayer))
            {
                if (Input.GetMouseButton(0))
                {
                    mIsDraggingObject = true; 
                }
                Cursor.SetCursor(MouseOver, mHotSpot, CursorMode.Auto);

            }
            else
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto); 
            }
        }
     /*   void OnMouseEnter()
        {
            Cursor.SetCursor(MouseOver, mHotSpot, CursorMode.Auto);
        }
        void OnMouseExit()
        {
            
        }*/


    }
}