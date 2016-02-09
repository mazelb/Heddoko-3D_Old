
/** 
* @file DecryptionVersion0.cs
* @brief Contains the DecryptionVersion0 class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date January 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.Collections;
using System.IO;
using Assets.Scripts.UI.Loading;
using UnityEngine;

namespace Assets.Scripts.Frames_Pipeline.BodyFrameEncryption.Decryption
{
    /// <summary>
    /// As of 1-27-2016, the encryption method obfuscates data by adding 0x80
    /// </summary>
    internal class DecryptionVersion0 : IFrameDecryptor
    {
        public bool StopDecryption { get; set; }

        public string Decrypt(string vFilepath)
        {
            string vStringOut = Guid.NewGuid() + "\r\n" + Guid.NewGuid() + "\r\n" + Guid.NewGuid() + "\r\n";
            try
            {
                /*   using (FileStream fs = new FileStream(vFilepath, FileMode.Open, FileAccess.Read))
                   {

                       Int32 vReadbyte = 0x00;
                       while ((vReadbyte = (Int32)fs.ReadByte()) != -1)
                       {
                           Int32 vTemp = vReadbyte + 0x80;
                           vStringOut += Convert.ToChar((byte)vTemp); 
                       }
                   }*/
              
                byte[] vByteArr = File.ReadAllBytes(vFilepath); 

                for (int i = 0; i < vByteArr.Length; i++)
                {
                    Int32 vReadbyte = vByteArr[i]; 
                    Int32 vTemp = vReadbyte + 0x80;
                    vStringOut += (char)((byte)vTemp);
                    if (StopDecryption)
                    {
                        return ""; 
                    }
                   // LoadingBoard.UpdateAnimation();
                }
            }

            catch
            {
                //todo: place a error logger here
            }

            return vStringOut;

        }

        public IEnumerator U3DDecryptFile(string vFilePath, Action<string> vGetter)
        {
            Debug.Log(" in decryption  start");
            string vOutPut = Guid.NewGuid() + "\r\n" + Guid.NewGuid() + "\r\n" + Guid.NewGuid() + "\r\n";

            using (FileStream fs = new FileStream(vFilePath, FileMode.Open, FileAccess.Read))
            {
                Int32 vReadbyte = 0x00;
                while ((vReadbyte = (Int32)fs.ReadByte()) != -1)
                {
                    Int32 vTemp = vReadbyte - 0x80;
                    vOutPut += Convert.ToChar((byte)vTemp);
                    yield return null;
                }
            }
            Debug.Log("completed decryption");
            vGetter(vOutPut);
        }

        
    }
}
