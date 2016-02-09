/**
* @file ImuEntry.cs
* @brief Contains the ImuEntry class
* @author Mohammed Haider(mohammed@heddoko.com)
* @date January 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System; 

namespace Assets.Scripts.Frames_Pipeline.BodyFrameConversion
{
    /// <summary>
    /// An imu entry received by the sensors
    /// </summary>
    public class ImuEntry
    {
        private double mYaw;
        private double mPitch;
        private double mRoll;

        /// <summary>
        /// For a Raw entry, convert it into a yaw pitch and row
        /// </summary>
        /// <param name="vRawEntry"></param>
        public ImuEntry(string vRawEntry)
        {
            //char separator[] = ';';
            string[] vSeperatedEntries = vRawEntry.Split(';');
            if (vSeperatedEntries.Length < 3)
            {
                //this is an error
                Console.WriteLine("error failed to parse data");
                this.mYaw = 0.0;
                this.mPitch = 0.0;
                this.mRoll = 0.0;
                return;
            }
            this.mRoll = ConvertRawDataToFloat(vSeperatedEntries[0]);
            this.mPitch = ConvertRawDataToFloat(vSeperatedEntries[1]);
            this.mYaw = ConvertRawDataToFloat(vSeperatedEntries[2]);
        }

        public ImuEntry(double y, double p, double r)
        {
            this.mYaw = y;
            this.mPitch = p;
            this.mRoll = r;
        }

        public void UpdateImuEntry(string rawEntry)
        {
            string[] separatedEntry = rawEntry.Split(';');
            if (separatedEntry.Length < 3)
            {
                //this is an error
                Console.WriteLine("error failed to parse data");
                this.mYaw = 0.0;
                this.mPitch = 0.0;
                this.mRoll = 0.0;
                return;
            }
            this.mRoll = ConvertRawDataToFloat(separatedEntry[0]);
            this.mPitch = ConvertRawDataToFloat(separatedEntry[1]);
            this.mYaw = ConvertRawDataToFloat(separatedEntry[2]);
        }

        public double MYaw
        {
            get
            {
                return mYaw;
            }

            set
            {
                mYaw = value;
            }
        }

        public double MPitch
        {
            get
            {
                return mPitch;
            }

            set
            {
                mPitch = value;
            }
        }

        public double MRoll
        {
            get
            {
                return mRoll;
            }

            set
            {
                mRoll = value;
            }
        }

        override public string ToString()
        { 
            return mPitch.ToString("F6") + ";" + this.mRoll.ToString("F6") + ";" + this.mYaw.ToString("F6");
        }


        static public double ConvertRawDataToFloat(string val)
        {
            //try swaping the bytes
            //string swapped = "0000";
            if (val.Length >= 4)
            {
                string vByte = val[0].ToString() + val[1] ;
                string vByte2 = val[2].ToString() + val[3] ;
                Byte byte_1 = Byte.Parse(vByte, System.Globalization.NumberStyles.HexNumber);
                Byte byte_2 = Byte.Parse(vByte2, System.Globalization.NumberStyles.HexNumber);
                 
                int data = ((int)byte_1) | (((int)byte_2) << 8);
                float fVal = (float)(data << 16);
                fVal = fVal / (1 << 29);

                return (double)fVal;
 
            }
            return 0.0;
        }

    }
}
