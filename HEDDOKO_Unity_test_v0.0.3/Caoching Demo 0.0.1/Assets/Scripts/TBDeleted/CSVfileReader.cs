using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utils;
using System.Text;
using UnityEngine;
namespace Assets.Scripts.TBDeleted
{
    public class CSVfileReader : MonoBehaviour
    {
        void Start()
        {

            int t = BodyRecordingsMgr.Instance.ScanRecordings(FilePathReferences.CSVDirectory);
            BodyRecordingsMgr.Instance.ReadAllRecordings();
            string recGuid = "DA920FFD-D8D0-436D-979D-48C73031F9F2";
            BodyFramesRecording bfr = BodyRecordingsMgr.Instance.GetRecordingByUUID(recGuid);
            Body b = BodiesManager.Instance.GetBodyFromRecordingUUID(recGuid);
            //b.InitBody(b.BodyGuid); //from the given body, initialize a new body, containing its segments. 

            b.PlayRecording(recGuid);
         
        }
    }
}
