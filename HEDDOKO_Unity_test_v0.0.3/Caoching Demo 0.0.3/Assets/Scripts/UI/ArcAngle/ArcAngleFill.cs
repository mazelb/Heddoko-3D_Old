
/** 
* @file ArcAngleFill.cs
* @brief ArcAngleFill class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date January 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using Assets.Scripts.Body_Pipeline.Analysis.Arms;
using Assets.Scripts.Cameras;
using Assets.Scripts.UI.MainMenu;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Calculate the angle between two vectors and sets the Arc gameobject to be oriented in the direction between the two vectors
    /// </summary>
    public class ArcAngleFill : MonoBehaviour
    {

        public Vector3 PositionOffset;

        public Image ImageToFill;
        public GameObject ArcCanvas;
        public Transform ZeroElbowVector;
        public Transform RightUpperArm;
        private float mFill;
        private float mAngle;
        public float MinimalScale;
        public float MaxScale;
        public PlayerStreamManager PlayerStreamManager;
        public Camera WorldSpaceUiCamera;
        public Vector3 NormalToThePlane;
        public Text DisplayAngleText;
        public Transform Torso;
        
        /// <summary>
        /// The angle of the arc
        /// </summary>
        public float Angle
        {
            get
            {
                return mAngle;
            }
        }

        private void Update()
        {
            Body vBody = PlayerStreamManager.CurrentBodyInPlay;
            if (vBody != null)
            {
                UpdateTransform();
            }
        }


        /// <summary>
        /// Updates the orientation of the canvas to be perpendicular to the camera
        /// </summary> 
        public void UpdateTransform()
        {
            Vector3 vUpVector =  NormalToThePlane;

            //Get the projection of the perfect Vector
            Vector3 vPerfectVectProjection = Vector3.ProjectOnPlane(ZeroElbowVector.right, vUpVector);

            //Get the projection of the elbow
            Vector3 vElbowVector = Vector3.ProjectOnPlane(RightUpperArm.right, vUpVector);
            Quaternion Rotation = Quaternion.LookRotation(vUpVector, vPerfectVectProjection);
            transform.rotation = Rotation;

            transform.position = RightUpperArm.position + PositionOffset;
             
            Vector3 vCross = Vector3.Cross(vPerfectVectProjection, vElbowVector);
            float vSign = Mathf.Sign(Vector3.Dot(vUpVector, vCross));
            mAngle = Vector3.Angle(vPerfectVectProjection, vElbowVector);
            mFill = mAngle / 360f;

            //set the image fill from the angles between two vectors
            ImageToFill.fillAmount = mFill;
          
            if (vSign < 0)
            {
                ImageToFill.fillClockwise = true; 
            }

            else
            {
                ImageToFill.fillClockwise = false;
                mAngle *= -1;
            }

            DisplayAngleText.text = (int)mAngle + "°";



        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
        /// <summary>
        /// From the given point parameters, set the zero vector's orientation, the sprite used by the arc image fill and the plane normal 
        /// </summary>
        /// <param name="vPointParameters"></param>
        public void SetParametersFromPoint(CameraMovementPointSetting vPointParameters)
        {
            PositionOffset = vPointParameters.TransformOffset;
            NormalToThePlane = vPointParameters.PlaneNormal;
            Sprite vNewArcSprite = vPointParameters.ArcSprite;
            if (vNewArcSprite != null)
            {
                ImageToFill.sprite = vPointParameters.ArcSprite;
                UpdateTransform();
            }
            else
            {
                Hide();
            }

        }
    }
}
