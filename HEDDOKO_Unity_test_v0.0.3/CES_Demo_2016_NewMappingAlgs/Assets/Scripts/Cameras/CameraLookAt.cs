using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Cameras
{
    /// <summary>
    /// Simple camera look at script
    /// </summary>
    public class CameraLookAt : MonoBehaviour
    {

        public Transform Target;
        public Vector3 Offset;

        void Update()
        {
            if (Target)
            {
                transform.LookAt(Target.position + Offset);
            }
             
        }
    }
}
