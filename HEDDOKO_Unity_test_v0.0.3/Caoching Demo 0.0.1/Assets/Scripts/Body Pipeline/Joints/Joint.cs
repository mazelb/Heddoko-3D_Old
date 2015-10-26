/** 
* @file Joint.cs
* @brief Contains the Joint class
* @author Mohammed Haider(Mohammed@heddoko.com)
* @date June 2015
*/
using System;
using System.Diagnostics;
using System.IO; 
using UnityEngine;

namespace Assets.Scripts.Body_Pipeline
{
    /**
    * Joint class 
    * @brief An abstract representation of a joint and orientations of various connected modules
    * @note something 
    */

    public class Joint
    {
        private BodyFrame bodyFrame;
        //todo : fusion, mapping and analysis modules.
        public BodyFrame BodyFrame
        {
            get
            {
                lock(this)
                {
                    return bodyFrame;
                }//unlock
            }
            set
            {
                lock (this)
                {
                    bodyFrame = value;
                }
            }
        }
        //represent the different factors of the rotations
        public Vector3 eulerFactor = Vector3.zero;
        public Vector3 quaternionFactor = Vector3.zero;
        public Vector3 vNodIniEuler = Vector3.zero;
        public float[,] NodCurrentOrientation = new float[3, 3];
        public float vLegHeight;

        //Initial rotation
        public Quaternion inverseInitRotation = Quaternion.identity;

 

        /// 
        /// Properties related to data recording. Also used for jig testing.
        /// 

 
 
        private float[] mJointAngles;
 
 
 
 


        /// <summary>
        /// Eulers to quaternion.
        /// </summary>
        /// <returns>The to quaternion.</returns>
        /// <param name="pitch">Pitch.</param>
        /// <param name="roll">Roll.</param>
        /// <param name="yaw">Yaw.</param> 
        /// <summary>
        /// Call this function to update current Joint values.
        /// </summary>
        public virtual void UpdateJoint(bool tracking,  bool fusionFlag, bool mappingFlag)
        {
            
          //  for (int ndx = 0; ndx < mNodSensors.Length; ndx++)
            {
              //  mNodSensors[ndx].UpdateSensor();

                //TODO: for now only one nod sensor per joint ! 
             //   if (ndx == 0)
                {
               //     Vector3 vNodRawEuler = mNodSensors[ndx].curRotationRawEuler;
                    //Debug.Log (vNodRawEuler.z * eulerFactor.y );
                //    vNodRawEuler = new Vector3(vNodRawEuler.x * eulerFactor.x, vNodRawEuler.y * eulerFactor.y, vNodRawEuler.z * eulerFactor.z);
               //     NodQuaternionOrientation vNodRawQuat = eulerToQuaternion(vNodRawEuler.x, vNodRawEuler.y, vNodRawEuler.z);
                  //  Quaternion vNodQuat = new Quaternion(vNodRawQuat.x, vNodRawQuat.y, vNodRawQuat.z, vNodRawQuat.w);
                  //  Quaternion vJointQuat = inverseInitRotation * vNodQuat * Quaternion.Inverse(Quaternion.Euler(quaternionFactor));

                //    if (jointTransform != null)
                    {
                   //     jointTransform.rotation = vJointQuat;
                    }

                }
            }
        }

        /// <summary>
        /// Reset the stretch joint sensors.
        /// </summary>
        public virtual void ResetJoint()
        {
           /* for (int ndx = 0; ndx < mNodSensors.Length; ndx++)
            {
                mNodSensors[ndx].Reset();



                //Debug.Log("Reseting joint nod");
                print("Reseting joint nod");
                Vector3 vNodRawEuler = mNodSensors[ndx].curRotationRawEuler;
                vNodRawEuler = new Vector3(vNodRawEuler.x, vNodRawEuler.y, vNodRawEuler.z);
                vNodIniEuler.Set(vNodRawEuler.x * eulerFactor.x, vNodRawEuler.y * eulerFactor.y, vNodRawEuler.z * eulerFactor.z);
                NodQuaternionOrientation vNodRawQuat = eulerToQuaternion(vNodRawEuler.x, vNodRawEuler.y, vNodRawEuler.z);
                Quaternion vNodQuat = new Quaternion(vNodRawQuat.x, vNodRawQuat.y, vNodRawQuat.z, vNodRawQuat.w);
                inverseInitRotation = Quaternion.Inverse(vNodQuat * Quaternion.Inverse(Quaternion.Euler(quaternionFactor)));

            }*/

        }




        /////////////////////////////Stretch Sensor Filtering- Angle extraction- will be called in each NodJoint child/////////////////////////
        /**
        //	* SSFiltering()
        //	* @ This Performs filtering the stretch sense data
        //	* @params stretch sense data for this frame (vStrechSenseData) and previous frame(vStrechSenseDataOld)
        //	* @returns filtered stretch sensor data  
        //	*/
        public float SSFiltering(float vStrechSenseData, float vStrechSenseDataOld)
        {

            float vStrechSenseDataNew;
            if (vStrechSenseDataOld != 0)
            {
                vStrechSenseDataNew = vStrechSenseDataOld * 0.55f + vStrechSenseData * 0.45f;
            }
            else
            {
                vStrechSenseDataNew = vStrechSenseData;
            }

            vStrechSenseData = vStrechSenseDataNew;
           
            return vStrechSenseData;
        }

        /**
        //	* SSFiltering()
        //	* @ This Performs filtering the stretch sense data
        //	* @params stretch sense data for this frame (vStrechSenseData) and previous frame(vStrechSenseDataOld)
        //	* @returns filtered stretch sensor data  
        //	*/
        public float SSFilteringTimeConst(float vStrechSenseData, float vStrechSenseDataOld)
        {/*
            float vTimeDifference = Time.time - vTimeJoint;

            vTimeJoint = Time.time;
            float vTimeConst = 1.2f;
            float vStrechSenseDataNew;
            if (vStrechSenseDataOld != 0)
            {
                vStrechSenseDataNew = (vStrechSenseDataOld * vTimeConst + vStrechSenseData * vTimeDifference) / (vTimeDifference + vTimeConst);

            }
            else
            {
                vStrechSenseDataNew = vStrechSenseData;
            }

            vStrechSenseData = vStrechSenseDataNew;*/
            //	print("vStrechSenseDataTC= "+vStrechSenseData+"vTimeDifference=  "+vTimeDifference);
            return vStrechSenseData;
        }

        /////////////////////////////Stretch Sensor Angle extraction- will be called in each NodJoint child////////////////////////////
        /**
        //	* SSAngleMappoly()
        //	* @ This Performs ploynomial mapping of stretch sensor data to angle by getting the maximum and minimum data  
        //	* @params vStrechSenseData:stretch sensor data, TetaMax and Tetamin: maximum and minimum angles of the joint in degrees
        //	* @returns angle of the joint in radian
        //	*/
        float ssMax = 1001f; //1170
        public float ssmin = 14100f;

        public float SSAngleMapPoly(float vStrechSenseData, float TetaMax, float Tetamin)
        {

            float vSSAngleMap;

            if (vStrechSenseData < ssmin && vStrechSenseData > 900)
            {
                ssmin = vStrechSenseData;
            }
            if (vStrechSenseData > ssMax && vStrechSenseData > 900 && vStrechSenseData != 999)
            {
                ssMax = vStrechSenseData;
            }
            if (vStrechSenseData == 0)
            {
                vSSAngleMap = 0f;
            }

            else
            {
                const float PI = (float)Math.PI;
                float TetaMid = (TetaMax + Tetamin) / 2;
                float ssMid = (ssMax + ssmin) / 2f + 0.09f * (ssMax - ssmin);


                //find the polynomial equation coefficients

                float vDet = ssMax * ssMax * ssMid + ssmin * ssmin * ssMax + ssMid * ssMid * ssmin - (ssmin * ssmin * ssMid + ssMax * ssMax * ssmin + ssMid * ssMid * ssMax);
                float vDeta = TetaMax * ssMid + Tetamin * ssMax + TetaMid * ssmin - (Tetamin * ssMid + TetaMax * ssmin + TetaMid * ssMax);
                float vDetb = ssMax * ssMax * TetaMid + ssmin * ssmin * TetaMax + ssMid * ssMid * Tetamin - (ssmin * ssmin * TetaMid + ssMax * ssMax * Tetamin + ssMid * ssMid * TetaMax);
                float vDetc = ssMax * ssMax * ssMid * Tetamin + ssmin * ssmin * ssMax * TetaMid + ssMid * ssMid * ssmin * TetaMax - (ssmin * ssmin * ssMid * TetaMax + ssMax * ssMax * ssmin * TetaMid + ssMid * ssMid * ssMax * Tetamin);

                float vCoefa = vDeta / vDet;
                float vCoefb = vDetb / vDet;
                float vCoefc = vDetc / vDet;

                vSSAngleMap = PI / 180.0f * (vCoefa * vStrechSenseData * vStrechSenseData + vCoefb * vStrechSenseData + vCoefc);

                // print ("vDet=  "+ vDet+" vDeta= " +vDeta +" vDetb= "+vDetb+" vDetc= "+vDetc);
                // print ("vCoefa=  "+ vCoefa+" vCoefb= " +vCoefb +" vCoefc= "+vCoefc);
                }
            return vSSAngleMap;

        }

        /**
        //	* SSAngleMapLinear()
        //	* @ This Performs Linear mapping of stretch sensor data to angle by getting the maximum and minimum data  
        //	* @params vStrechSenseData:stretch sensor data, TetaMax and Tetamin: maximum and minimum angles of the joint in degrees
        //	* @returns angle of the joint in radian
        //	*/

        public float SSAngleMapLinear(float vStrechSenseData, float TetaMax, float Tetamin)
        {
            float vSSAngleMaplinear;
            if (vStrechSenseData < ssmin && vStrechSenseData > 900)
            {
                ssmin = vStrechSenseData;
            }
            if (vStrechSenseData > ssMax && vStrechSenseData > 1000 && vStrechSenseData != 999)
            {
                ssMax = vStrechSenseData;
            }

            const float PI = (float)Math.PI;
            if (vStrechSenseData == 0)
            {
                vSSAngleMaplinear = 0f;
            }

            else
            {
                vSSAngleMaplinear = PI / 180.0f * ((vStrechSenseData - ssmin) * (TetaMax - Tetamin) / (ssMax - ssmin) + Tetamin);
            }
            return vSSAngleMaplinear;
        }

        //////////////////////////// These are the reference functions for matrix calculation and transformation ///////////////////////////////

        /**
        //	* RotationGlobal()
        //	* @ This Performs transformation From Global Coordinate System To local Nod coordinates
        //	* @params yaw pitch and roll of each nod
        //	* @return 3*3 orientation matrix in nod local coordinate system
        //	*/
        // 
        public float[,] RotationGlobal(float yaw, float pitch, float roll)
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



        /**
        //	* RotationLocal()
        //	* @ This Performs transformation From Nods Local Coordinate System To global coordinates
        //	* @params yaw pitch and roll of each nod
        //	* @return 3*3 orientation matrix in global coordinate system
        //	*/
        // 
        public float[,] RotationLocal(float yaw, float pitch, float roll)
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
    //* RotationVector()
    //* @It produces a rotation matrix around an arbitrary vector with desired angles
    //* @param vec u, arbitrary unit vector
    //* @param float t, desired angle of rotation
    //* @return float a[][3], The output rotation matrix
    //*/
        public float[,] RVector(Vector3 u, float t)
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



        /**
//	* multi()
//	*	@This Function do multiplication between two 3*3 matrices
//	*	@param matrix a and b
//	*	@return c = a * b,
//	*/
        public float[,] multi(float[,] a, float[,] b)
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



        /////////////////////////////////////////////////////////////////////////////////////
        /// 
        /// Jig Test Methods
        /// 
        /// TODO: move to its own CSharp code.
        /// TODO: implement a CSharp wrapper for NodPlugin.dll
        /// 
        //////////////////////////////////////////////////////////////////////////////////////



        /**
         * Initializes everything needed to start recording to file.
         */
        public void StartRecording()
        {
          /*  if (!mRecordData)
            {
                return;
            }

            // Open the encoder COM port, if avialable.
            if (mEncoderCOMPort.Length > 0)
            {
                print("Connecting to encoder on port " + mEncoderCOMPort);
                mEncoderPortStream = new SerialPort(mEncoderCOMPort, 115200);
                //mEncoderPortStream.ReadTimeout = 200;

                if (mEncoderPortStream.IsOpen)
                {
                    mEncoderPortStream.Close();
                }

                try
                {
                    mEncoderPortStream.Open();
                }
                catch (Exception e)
                {
                    print("Couldn't open port " + mEncoderCOMPort + " (exception: " + e.Message + ")");
                }

                // Enabled encoder recording if we successfully connected to it.
                if (mEncoderPortStream.IsOpen)
                {
                    mRecordEncoder = true;
                }
            }

            // Make sure we have a directory to write to.
            string vDataPath = "../../RecordedData";
            if (!Directory.Exists(vDataPath))
            {
                Directory.CreateDirectory(vDataPath);
            }

            // Set the file name to write to and start the file stream.
            int vFileNameIncrement = 1;
            mRecordingFileName = string.Format("{0}/{1}_{2}.csv", vDataPath, mRecordingTitle, vFileNameIncrement);
            while (File.Exists(mRecordingFileName))
            {
                mRecordingFileName = string.Format("{0}/{1}_{2}.csv", vDataPath, mRecordingTitle, (++vFileNameIncrement));
            }

            print("Recording data to file: " + mRecordingFileName);

            mRecordingFileStream = new StreamWriter(mRecordingFileName, true);

            // Reset the stopwatch.
            mStopwatch = Stopwatch.StartNew();

            // Write the heading.
            string vHeading = "";
            vHeading += "Timestamp";

            // Encoder data.
            if (mRecordEncoder)
            {
                vHeading += ",Encoder";
            }

            // Raw IMU data.
            if (mRecordIMURawData)
            {
                vHeading += ",Imu1RawX,Imu1RawY,Imu1RawZ";
                vHeading += ",Imu2RawX,Imu2RawY,Imu2RawZ";
            }

            // Raw fabric sensors data.
            if (mRecordFabricSensorsRawData)
            {
                vHeading += ",FabricSensor1Raw,FabricSensor2Raw,FabricSensor3Raw,FabricSensor4Raw,FabricSensor5Raw";
            }

            // Other data.
            string[] vExtrasHeadings = GetRecordedExtrasHeading();
            foreach (string vExtrasHeading in vExtrasHeadings)
            {
                vHeading += "," + vExtrasHeading;
            }

            mRecordingFileStream.WriteLine(vHeading);*/
        }

        /**
         * Writes the collected data to file.
         */
        public void RecordDataToFile()
        {
            // Performance check.
           /* if (mRecordingFileStream == null)
            {
                return;
            }

            // Wait for Nods to connect before collecting data.
            if (!mNodSensors[0].IsConnected() || !mNodSensors[1].IsConnected())
            {
                print("Waiting for IMUs to connect before recording...");
                print("First IMU connected: " + mNodSensors[0].IsConnected());
                print("Second IMU connected: " + mNodSensors[1].IsConnected());
                return;
            }

            // Respect the indicated frame rate, so that we're not overloaded with data.
            if (mRecordingFrameInterval > 0 && Time.frameCount % mRecordingFrameInterval != 0)
            {
                return;
            }

            // Update the encoder readings, if available.
            ReadEncoder();

            // Start with a timestamp.
            string vDataLine = "";
            vDataLine += "" + mStopwatch.ElapsedMilliseconds;

            // Append the encoder value.
            if (mRecordEncoder)
            {
                vDataLine += "," + mEncoderData;
            }

            // Append raw IMU data.
            if (mRecordIMURawData)
            {
                vDataLine += "," + RetrieveRawIMUData(mNodSensors[0]);
                vDataLine += "," + RetrieveRawIMUData(mNodSensors[1]);
            }

            // Append raw fabric sensor data.
            if (mRecordFabricSensorsRawData)
            {
                vDataLine += "," +
                    NodContainer.svaModuleData[1] + "," +
                    NodContainer.svaModuleData[2] + "," +
                    NodContainer.svaModuleData[3] + "," +
                    NodContainer.svaModuleData[4] + "," +
                    NodContainer.svaModuleData[5];
            }

            // Other data.
            float[] vExtras = GetRecordedExtras();
            foreach (float vExtra in vExtras)
            {
                vDataLine += "," + vExtra;
            }

         */  // mRecordingFileStream.WriteLine(vDataLine);
        }

        /**
         * Retrieve data from the encoder.
         *
         * See: Heddoko-src/Testing/stretchSense_encoder/dual_csv.py
         */
        public void ReadEncoder()
        {
            // Performance check.
          //  if (mEncoderCOMPort.Length == 0 || mEncoderPortStream == null || !mEncoderPortStream.IsOpen)
            {
          //      return;
            }

            // Retrieve data.
            string vRawData = "";
            try
            {
         //       vRawData = mEncoderPortStream.ReadLine();
            }
            catch (Exception e)
            {
          //      print("Could not read from encoder: " + e.Message);
            }

            // Update encoder values.
            if (vRawData.Length > 0)
            {
          //      mEncoderData = Convert.ToDouble(vRawData);
            }

         //   if (mDebugRecording)
            {
         //       print("Encoder raw data: " + vRawData);
            }
        }

        /**
         * Retrieves raw data from an IMU.
         */
        public string RetrieveRawIMUData(NodSensor vIMU)
        {
            Vector3 vRawEuler = vIMU.curRotationRawEuler;

            return vRawEuler.x + "," + vRawEuler.y + "," + vRawEuler.z;
        }



        /**
         * Sample method showing how to add a heading for custom data to be recorded.
         * 
         * Define the method in the child class as "public override string[] GetRecordedExtrasHeading() { ... }"
         */
        public virtual string[] GetRecordedExtrasHeading()
        {
            string[] vExtraHeadings = { "Custom1", "Example2" };

            return vExtraHeadings;
        }

        /**
         * Sample method showing how to add values for custom data to be recorded.
         * 
         * Define the method in the child class as "public override float[] GetRecordedExtras() { ... }"
         */
        public virtual float[] GetRecordedExtras()
        {
            float[] vExtras = { 982.0f, 107.7f };

            return vExtras;
        }



 
         
    }
}
