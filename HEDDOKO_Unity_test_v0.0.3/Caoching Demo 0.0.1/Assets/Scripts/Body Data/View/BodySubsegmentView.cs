/** 
* @file BodySubsegmentView.cs
* @brief The view for a body's subsegment. Application of transforms happen in here. 
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date October 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using System.Collections.Generic;
using Assets.Scripts.UI._2DSkeleton;
using UnityEngine;
namespace Assets.Scripts.Body_Data.view
{
    /**
    * BodySubsgmentView  class 
    * @brief BodySubsegmentView class (represents one subsegment view)
    */
    public class BodySubsegmentView : MonoBehaviour
    {
        //the transforms required to transform
        public BodySubSegment AssociatedSubSegment;
        public List<Transform> SubSegmentTransforms = new List<Transform>();

        //Initial subsegment position 
        private Vector3 mInitialPosition = Vector3.zero;

        //Sprite Transform2D
        private ISpriteMover mSpriteMover;

        //  public Transform SpriteTransform;
        //This flag turned on, mean that the assigned Transform is a 3D model, else use a 2D sprite
        private bool mUsing3DModel = true;
        /**
        * View
        * @param 
        * @brief View associated with this body
        * @note: a new gameobject is created and this Body is added into it as a component
        * @return returns the view associated with this body
        */
        public void AssignTransforms()
        {
            //find the object in the scene with the tag
            GameObject[] vGameObjects = GameObject.FindGameObjectsWithTag(gameObject.name);

            //Initialize the view of each subsegments related object
            foreach (GameObject vObj in vGameObjects)
            {
                Transform vTransform = vObj.transform;
                SubSegmentTransforms.Add(vTransform);

                //Todo: add proper setting for the initial position 
                mInitialPosition = vTransform.localPosition;
            }
            try
            {
                GameObject v2DGameObject = GameObject.FindGameObjectWithTag(gameObject.name + "2D");
                if (v2DGameObject != null)
                {
                    // SpriteTransform = v2DGameObject.transform;
                    mSpriteMover = v2DGameObject.GetComponent<ISpriteMover>();
                }
            }
            catch (UnityException exception)
            {
                //exception is thrown if the tag ins't found in the tag manager.
            }
            //Find the 2D representation of the object in the scene 


        }

        public void ApplyTransformations(Quaternion vNewOrientation)
        {
            foreach (Transform vObjTransform in SubSegmentTransforms)
            {
                //vObjTransform.rotation = vNewOrientation;
                vObjTransform.localRotation = vNewOrientation;
            }

            //Apply 2D transformation
            /* if (SpriteTransform != null)
             {
                 //convert to Euler
                 Vector3 vEuler = vNewOrientation.eulerAngles;
                 vEuler.z = vEuler.vLeftLegHipMulti;
                 vEuler.vLeftLegHipMulti = 0;
                 vEuler.y = 0;
                 SpriteTransform.rotation = Quaternion.Euler(vEuler);
             }*/
            if (mSpriteMover != null)
            {
                mSpriteMover.ApplyTransformations();

            }
        }


        public void ApplyTranslations(float vNewDisplacement)
        {
            foreach (Transform vObjTransform in SubSegmentTransforms)
            {
                Vector3 v3 = vObjTransform.localPosition;
                v3.y = vNewDisplacement + 0.15f;
                vObjTransform.localPosition = v3;
            }

            //Apply 2D translations
            /* if (SpriteTransform != null)
                { 
                    Vector3 v3 = SpriteTransform.localPosition;
                    v3.y = vNewDisplacement;
                    SpriteTransform.localPosition = v3;
                } */
            if (mSpriteMover != null)
            {
                mSpriteMover.ApplyTranslations(vNewDisplacement);
            }
        }

        public void ResetOrientations()
        {
            foreach (Transform vObjTransform in SubSegmentTransforms)
            {
                vObjTransform.localRotation = Quaternion.identity;
                //vObjTransform.rotation = Quaternion.identity;
            }

            //Apply to 2D model
            /*            if (SpriteTransform != null)
                        {
                            SpriteTransform.rotation = Quaternion.identity;
                        }*/
            if (mSpriteMover != null)
            {
                mSpriteMover.ResetOrientations();
            }
            if (Camera.main != null)
            {
                Camera.main.Render();
            }
        }

        /**
        * UpdateOrientation(Quaternion vNewOrientation)
        * @param Quaternion vNewOrientation: the new orientation of the subsegment
        * @brief Updates the current orientation with the passed in parameter
        */
        internal void UpdateOrientation(Quaternion vNewOrientation)
        {
            try
            {
                ApplyTransformations(vNewOrientation);
            }
            catch (Exception)
            {

            }
        }

        /**
        * UpdateOrientation(Quaternion vNewOrientation)
        * @param Quaternion vNewOrientation: the new orientation of the subsegment
        * @brief Updates the current orientation with the passed in parameter
        */
        internal void UpdatePosition(float vNewDisplacemetn)
        {
            try
            {
                ApplyTranslations(vNewDisplacemetn);
            }
            catch (Exception)
            {

            }
        }

        /**
        * ResetOrientation()
        * @brief Resets the current orientation to its initial value
        */
        internal void ResetOrientation()
        {
            ResetOrientations();
        }

        /**
        * Awake()
        * @brief On Awake: set the segment's initial orientation .
        */
        internal void Awake()
        {
            AssignTransforms();
        }
    }
}