/** 
* @file MatrixTools.cs
* @brief Contains the MatrixTools  class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date October 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    /**
    * MatrixTools class 
    * @brief MatrixTools, provides functions and methods to convert, calculate and execute on float[,] data types and NodQuaternion orientations)
    */
    public static class MatrixTools
    {
        /**
        //* MatToQuat
        //* @It converts a 3*3 orientation Matrix to a Quaternion
        //* @param float m[][3] is the original 3*3 matrix
        //* @return NodQuaternionOrientation, the orientation in quaternion
        //* reference: @http://www.cg.info.hiroshima-cu.ac.jp/~miyazaki/knowledge/teche52.html
        */
        public static NodQuaternionOrientation MatToQuat(float[,] m)
        {
            NodQuaternionOrientation q;
            q.w = (m[0, 0] + m[1, 1] + m[2, 2] + 1.0f) / 4.0f;
            q.x = (m[0, 0] - m[1, 1] - m[2, 2] + 1.0f) / 4.0f;
            q.y = (-m[0, 0] + m[1, 1] - m[2, 2] + 1.0f) / 4.0f;
            q.z = (-m[0, 0] - m[1, 1] + m[2, 2] + 1.0f) / 4.0f;
            if (q.w < 0.0f) q.w = 0.0f;
            if (q.x < 0.0f) q.x = 0.0f;
            if (q.y < 0.0f) q.y = 0.0f;
            if (q.z < 0.0f) q.z = 0.0f;
            q.w = (float)Math.Sqrt(q.w);
            q.x = (float)Math.Sqrt(q.x);
            q.y = (float)Math.Sqrt(q.y);
            q.z = (float)Math.Sqrt(q.z);
            if (q.w >= q.x && q.w >= q.y && q.w >= q.z)
            {
                q.w *= +1.0f;
                q.x *= Mathf.Sign(m[2, 1] - m[1, 2]);
                q.y *= Mathf.Sign(m[0, 2] - m[2, 0]);
                q.z *= Mathf.Sign(m[1, 0] - m[0, 1]);
            }
            else if (q.x >= q.w && q.x >= q.y && q.x >= q.z)
            {
                q.w *= Mathf.Sign(m[2, 1] - m[1, 2]);
                q.x *= +1.0f;
                q.y *= Mathf.Sign(m[1, 0] + m[0, 1]);
                q.z *= Mathf.Sign(m[0, 2] + m[2, 0]);
            }
            else if (q.y >= q.w && q.y >= q.x && q.y >= q.z)
            {
                q.w *= Mathf.Sign(m[0, 2] - m[2, 0]);
                q.x *= Mathf.Sign(m[1, 0] + m[0, 1]);
                q.y *= +1.0f;
                q.z *= Mathf.Sign(m[2, 1] + m[1, 2]);
            }
            else if (q.z >= q.w && q.z >= q.x && q.z >= q.y)
            {
                q.w *= Mathf.Sign(m[1, 0] - m[0, 1]);
                q.x *= Mathf.Sign(m[2, 0] + m[0, 2]);
                q.y *= Mathf.Sign(m[2, 1] + m[1, 2]);
                q.z *= +1.0f;
            }

            float r = (float)Math.Sqrt(q.w * q.w + q.x * q.x + q.y * q.y + q.z * q.z);
            q.w /= r;
            q.x /= r;
            q.y /= r;
            q.z /= r;
            return q;
        }
  

        /**
        //	* RotationGlobal()
        //	* @ This Performs transformation From Global Coordinate System To local Nod coordinates
        //	* @params yaw pitch and roll of each nod
        //	* @return 3*3 orientation matrix in nod local coordinate system
        //	*/
        // 
        public static float[,] RotationGlobal(float yaw, float pitch, float roll)
        {
            float[,] a = new float[3, 3];
            a[0, 0] = Mathf.Cos(pitch) * Mathf.Cos(yaw);
            a[1, 0] = (Mathf.Sin(roll) * Mathf.Cos(yaw) * Mathf.Sin(pitch) - Mathf.Sin(yaw) * Mathf.Cos(roll));
            a[2, 0] = (Mathf.Sin(roll) * Mathf.Sin(yaw) + Mathf.Cos(yaw) * Mathf.Sin(pitch) * Mathf.Cos(roll));
            a[0, 1] = Mathf.Cos(pitch) * Mathf.Sin(yaw);
            a[1, 1] = Mathf.Sin(roll) * Mathf.Sin(yaw) * Mathf.Sin(pitch) + Mathf.Cos(yaw) * Mathf.Cos(roll);
            a[2, 1] = (Mathf.Cos(roll) * Mathf.Sin(yaw) * Mathf.Sin(pitch) - Mathf.Cos(yaw) * Mathf.Sin(roll));
            a[0, 2] = -Mathf.Sin(pitch);
            a[1, 2] = Mathf.Cos(pitch) * Mathf.Sin(roll);
            a[2, 2] = (Mathf.Cos(pitch) * Mathf.Cos(roll));
            return a;
        }


        /// <summary>
        /// Eulers to quaternion.
        /// </summary>
        /// <returns>The to quaternion.</returns>
        /// <param name="pitch">Pitch.</param>
        /// <param name="roll">Roll.</param>
        /// <param name="yaw">Yaw.</param>

        public static NodQuaternionOrientation eulerToQuaternion(float pitch, float roll, float yaw)
        {
            float sinHalfYaw = Mathf.Sin(yaw / 2.0f);
            float cosHalfYaw = Mathf.Cos(yaw / 2.0f);
            float sinHalfPitch = Mathf.Sin(pitch / 2.0f);
            float cosHalfPitch = Mathf.Cos(pitch / 2.0f);
            float sinHalfRoll = Mathf.Sin(roll / 2.0f);
            float cosHalfRoll = Mathf.Cos(roll / 2.0f);

            NodQuaternionOrientation result;
            result.x = -cosHalfRoll * sinHalfPitch * sinHalfYaw
                + cosHalfPitch * cosHalfYaw * sinHalfRoll;
            result.y = cosHalfRoll * cosHalfYaw * sinHalfPitch
                + sinHalfRoll * cosHalfPitch * sinHalfYaw;
            result.z = cosHalfRoll * cosHalfPitch * sinHalfYaw
                - sinHalfRoll * cosHalfYaw * sinHalfPitch;
            result.w = cosHalfRoll * cosHalfPitch * cosHalfYaw
                + sinHalfRoll * sinHalfPitch * sinHalfYaw;

            return result;
        }

        /**
        //	* Identity3X3Matrix()  
        //	* @return 3*3  identity matrix
        //	*/
        // 
        public static float[,] Identity3X3Matrix()
        {
            float[,] vIdentMatrix = new float[3, 3];
            vIdentMatrix[0, 0] = 0;
            vIdentMatrix[1, 1] = 0;
            vIdentMatrix[2, 2] = 0;
            return vIdentMatrix;
        }
        /**
        //	* RotationLocal()
        //	* @ This Performs transformation From Nods Local Coordinate System To global coordinates
        //	* @params yaw pitch and roll of each nod
        //	* @return 3*3 orientation matrix in global coordinate system
        //	*/
        // 
        public static float[,] RotationLocal(float yaw, float pitch, float roll)
        {
            float[,] a = new float[3, 3];
            a[0, 0] = Mathf.Cos(pitch) * Mathf.Cos(yaw);
            a[1, 0] = Mathf.Cos(pitch) * Mathf.Sin(yaw);
            a[2, 0] = -Mathf.Sin(pitch);
            a[0, 1] = -Mathf.Cos(roll) * Mathf.Sin(yaw) + Mathf.Cos(yaw) * Mathf.Sin(pitch) * Mathf.Sin(roll);
            a[1, 1] = Mathf.Sin(roll) * Mathf.Sin(yaw) * Mathf.Sin(pitch) + Mathf.Cos(yaw) * Mathf.Cos(roll);
            a[2, 1] = Mathf.Cos(pitch) * Mathf.Sin(roll);
            a[0, 2] = (Mathf.Sin(roll) * Mathf.Sin(yaw) + Mathf.Cos(yaw) * Mathf.Sin(pitch) * Mathf.Cos(roll));
            a[1, 2] = (Mathf.Cos(roll) * Mathf.Sin(yaw) * Mathf.Sin(pitch) - Mathf.Cos(yaw) * Mathf.Sin(roll));
            a[2, 2] = (Mathf.Cos(pitch) * Mathf.Cos(roll));
            return a;
        }

        /**
        //	* multi()
        //	*	@This Function do multiplication between two 3*3 matrices
        //	*	@param matrix a and b
        //	*	@return c = a * b,
        //	*/
        public static float[,] multi(float[,] a, float[,] b)
        {
            float[,] c = new float[3, 3];
            int i, j, k;
            for (i = 0; i < 3; i++)
            {
                for (j = 0; j < 3; j++)
                {
                    c[i, j] = a[i, 0] * b[0, j] + a[i, 1] * b[1, j] + a[i, 2] * b[2, j];

                }
            }
            return c;
        }



        /**
        //* RotationVector()
        //* @It produces a rotation matrix around an arbitrary vector with desired angles
        //* @param vec u, arbitrary unit vector
        //* @param float t, desired angle of rotation
        //* @return float a[][3], The output rotation matrix
        //*/
        public static float[,] RVector(Vector3 u, float t)
        {
            float[,] a = new float[3, 3];
            a[0, 0] = Mathf.Cos(t) + u.x * u.x * (1 - Mathf.Cos(t));
            a[1, 0] = u.x * u.y * (1 - Mathf.Cos(t)) + u.z * Mathf.Sin(t);
            a[2, 0] = u.x * u.z * (1 - Mathf.Cos(t)) - u.y * Mathf.Sin(t);
            a[0, 1] = u.x * u.y * (1 - Mathf.Cos(t)) - u.z * Mathf.Sin(t);
            a[1, 1] = Mathf.Cos(t) + u.y * u.y * (1 - Mathf.Cos(t)); ;
            a[2, 1] = u.z * u.y * (1 - Mathf.Cos(t)) + u.x * Mathf.Sin(t);
            a[0, 2] = u.x * u.z * (1 - Mathf.Cos(t)) + u.y * Mathf.Sin(t);
            a[1, 2] = u.z * u.y * (1 - Mathf.Cos(t)) - u.x * Mathf.Sin(t);
            a[2, 2] = Mathf.Cos(t) + u.z * u.z * (1 - Mathf.Cos(t));
            return a;
        }

        

    }

    /**
   * NodQuaternionOrientation struct 
   * @brief A quaternion structure that is needed to render orientation data 
   */
    public struct NodQuaternionOrientation
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public NodQuaternionOrientation(float _x, float _y, float _z, float _w)
        {
            x = _x;
            y = _y;
            z = _z;
            w = _w;
        }
    };
}
