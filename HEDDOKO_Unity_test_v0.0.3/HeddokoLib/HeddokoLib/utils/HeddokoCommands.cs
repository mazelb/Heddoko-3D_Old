 
using System.Text;
using HeddokoLib.networking;

namespace HeddokoLib.utils
{
    /// <summary>
    /// Commands to be used by a command router
    /// </summary>
    public static class HeddokoCommands
    {
        public static string RequestBrainpackList = "0x00";
        public static string SendBrainpackList = "0x01";
        public static string RequestToConnectToBP = "0x02"; 
        public static string BPConnectionFailure = "0x03";
        public static string BPConnectionSucess = "0x04";
        public static string BPDeviceLost = "0x05";
        public static string CloseConnectionToServer = "0x06";
        public static string SendBPData = "0x07";
        public static string SendBTCommand = "0x08";
        public static string RequestBPData = "0x09";
        public static string RequestConnection = "0x10";
        public static string ConnectionAck = "0x11";
        public static string UseRecordingForBody = "0x12";
        public static string UseBrainpackForBody = "0x13";
        public static string StartSceneWithBrainpackConnect = "0x14";
        public static string StopHeddokoUnityClient = "9x99";
        public static string ClientError = "9x00";

        public static string ExtractCommandFromBytes(int vStartIndex, int vLength, byte[] vData)
        {
            string vCommand = "";
            byte[] vExtractedCommand = new byte[vLength];
            int vIteratingIndex = 0;
            for (int i = vStartIndex; i < vLength; i++, vIteratingIndex++)
            {
                vExtractedCommand[vIteratingIndex] = vData[i];
            }
            StringBuilder vSb = new StringBuilder();
            vSb.Append(PacketSetting.Encoding.GetString(vExtractedCommand, 0, vLength));

            vCommand = vSb.ToString();
            return vCommand;
        }

    }
}
