/** 
* @file BodyRecordingTester.cs
* @brief Contains the CSVfileReader class
* @author Mohammed Haider (Mohammed@heddoko.com)
* @date October 2015
*/

using Assets.Scripts.Utils; 
using UnityEngine;
namespace Assets.Scripts.TBDeleted
{
    public class BodyRecordingTester : MonoBehaviour
    {
        Body vBody;
        public UnityEngine.UI.Text Text;
        /**
        * Start
        * @param 
        * @brief Starts scanning the recordings from the CSV directory and then read all recording, and starts playing the recording for a specific body. 
        * @return  
        */
        void Start()
        {

            int vRecording = BodyRecordingsMgr.Instance.ScanRecordings(FilePathReferences.sCSVDirectory);
            BodyRecordingsMgr.Instance.ReadAllRecordings();
            string vRecGuid = "DA920FFD-D8D0-436D-979D-48C73031F9F2";
            BodyFramesRecording vBodyFrameRec = BodyRecordingsMgr.Instance.GetRecordingByUUID(vRecGuid);
             vBody = BodiesManager.Instance.GetBodyFromRecordingUUID(vRecGuid);
            vBody.PlayRecording(vRecGuid);
         
        }
        /**
        * ResetSensor
        * @param 
        * @brief Resets the initial frame to the current frame 
        * @return  
        */

        public void ResetSensor()
        {
            vBody.BodyView.ResetJoint(); 
        }

        /**
        * ResetSensor
        * @param 
        * @brief Update called by unity once every frame
        * @return  
        */
        void Update()
        {
            UpdateBodyFrameText();
        }

        /**
      * UpdateBodyFrameText
      * @param 
      * @brief Updates the in game text view with information about the current frame relative to the right upper arm and right forearm
      * @return  
      */
        private void UpdateBodyFrameText()
        {
            BodyFrame b = vBody.BodyView.CurrentFrame;
            string vBodyFrameString = "right upper arm data ";
            vBodyFrameString += b.MapSensorPosToValue[BodyStructureMap.SensorPositions.SP_RightUpperArm];
            vBodyFrameString += "right forearm data ";
            vBodyFrameString += b.MapSensorPosToValue[BodyStructureMap.SensorPositions.SP_RightForeArm];
            Text.text = vBodyFrameString;

        }
   
    }
}
