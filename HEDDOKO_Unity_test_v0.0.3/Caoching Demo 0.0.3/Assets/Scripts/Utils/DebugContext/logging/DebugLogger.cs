/** 
* @file DebugLogger.cs
* @brief Contains the DebugLogger  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Utils.DebugContext.logging
{
    /// <summary>
    /// 
    /// </summary>
    public class DebugLogger
    {
        public static DebugLogSettings Settings = new DebugLogSettings();
        private static DebugLogger sInstance;
        private static object mInstanceLock = new object();
       // DebugSettingsFileMonitor mFileMonitor = new DebugSettingsFileMonitor();

        private string mLogDirPath;

        private Queue<Log> mMessageQueue = new Queue<Log>();
        private Dictionary<LogType, Func<bool>> sSettingsRegistry = new Dictionary<LogType, Func<bool>>();
        private Dictionary<LogType, OutputLogPath> mLogTypeToLogpathType = new Dictionary<LogType, OutputLogPath>();

        private bool mContinueWorking;
        public static DebugLogger Instance
        {
            get
            {
                lock (mInstanceLock)
                {
                    if (sInstance == null)
                    {

                        sInstance = new DebugLogger();
                        sInstance.Register();
                    }
                }
                return sInstance;
            }
        }
        public void LogMessage(LogType vType, string vMsg)
        {
            try
            {
                bool vCanLog = sSettingsRegistry[vType].Invoke();
                if (vCanLog)
                {
                    Log vLog = new Log();
                    vLog.LogType = vType;
                    vLog.Message = vMsg;
                    Instance.mMessageQueue.Enqueue(vLog);
                }
            }
            catch (Exception e)
            {

                Debug.Log("<color=red>Fatal error:</color> " +e);
            }

        }
        private void Register()
        {
            RegisterSetting();
            RegisterPaths();
        }

        private void RegisterSetting()
        {
            sSettingsRegistry.Add(LogType.ApplicationCommand, () =>
            {
                if (Settings.LogAll || Settings.LogAllApplicationContext)
                    return true;
                return Settings.ApplicationCommandLog;
            });

            sSettingsRegistry.Add(LogType.ApplicationFrame, () =>
            {
                if (Settings.LogAll || Settings.LogAllApplicationContext)
                    return true;
                return Settings.ApplicationFrameData;
            });

            sSettingsRegistry.Add(LogType.ApplicationResponse, () =>
            {
                if (Settings.LogAll || Settings.LogAllApplicationContext)
                    return true;
                return Settings.ApplicationResponseLog;
            });

            sSettingsRegistry.Add(LogType.BrainpackCommand, () =>
            {
                if (Settings.LogAll || Settings.LogAllBrainpackContext)
                    return true;
                return Settings.BrainpackCommandLog;
            });
            sSettingsRegistry.Add(LogType.BrainpackFrame, () =>
            {
                if (Settings.LogAll || Settings.LogAllBrainpackContext)
                    return true;
                return Settings.BrainpackFrameData;
            });
            sSettingsRegistry.Add(LogType.BrainpackResponse, () =>
            {
                if (Settings.LogAll || Settings.LogAllBrainpackContext)
                    return true;
                return Settings.BrainpackResponseLog;
            });
            sSettingsRegistry.Add(LogType.SocketClientSend, () =>
            {
                if (Settings.LogAll )
                    return true;
                return Settings.SocketClientLogging;
            });
            sSettingsRegistry.Add(LogType.SocketClientSettings, () =>
            {
                if (Settings.LogAll)
                    return true;
                return Settings.SocketClientLogging;
            });
            sSettingsRegistry.Add(LogType.SocketClientReceive, () =>
            {
                if (Settings.LogAll)
                    return true;
                return Settings.SocketClientLogging;
            });
            sSettingsRegistry.Add(LogType.SocketClientError, () =>
            {
                if (Settings.LogAll)
                    return true;
                return Settings.SocketClientLogging;
            });
        }

        private void RegisterPaths()
        {
            var vLocation = OutterThreadToUnityThreadIntermediary.Instance.ApplicationPath; 
            string vDirPath = Path.GetDirectoryName(vLocation);
            Instance.mLogDirPath = vDirPath + "\\logs";


            mLogTypeToLogpathType.Add(LogType.ApplicationCommand, OutputLogPath.ApplicationLog);
            mLogTypeToLogpathType.Add(LogType.ApplicationResponse, OutputLogPath.ApplicationLog);
            mLogTypeToLogpathType.Add(LogType.ApplicationFrame, OutputLogPath.ApplicationFrames);
            mLogTypeToLogpathType.Add(LogType.BrainpackCommand, OutputLogPath.BrainpackMsgLog);
            mLogTypeToLogpathType.Add(LogType.BrainpackResponse, OutputLogPath.BrainpackMsgLog);
            mLogTypeToLogpathType.Add(LogType.BrainpackFrame, OutputLogPath.BrainpackFrames);
            mLogTypeToLogpathType.Add(LogType.SocketClientReceive, OutputLogPath.SocketClientLog);
            mLogTypeToLogpathType.Add(LogType.SocketClientSend, OutputLogPath.SocketClientLog);
            mLogTypeToLogpathType.Add(LogType.SocketClientSettings, OutputLogPath.SocketClientLog);
            mLogTypeToLogpathType.Add(LogType.SocketClientError, OutputLogPath.SocketClientLog);



        }

        public void Start()
        {
            mContinueWorking = true;
            Thread vThread = new Thread(Instance.WorkerTask);
            vThread.Start();
           // mFileMonitor.Start();
        }

        public void Stop()
        {
            mContinueWorking = false;
        //    mFileMonitor.Stop();
        }
        private void WorkerTask()
        {
            while (mContinueWorking)
            {
                if (mMessageQueue.Count != 0)
                {
                    Log vLog = mMessageQueue.Dequeue();
                    WriteFile(vLog);
                }
            }
        }

        private void WriteFile(Log vLog)
        {
            try
            {
                string vLogType = mLogTypeToLogpathType[vLog.LogType].ToString();
                string vTodaysDate = DateTime.Now.ToString("yy-MM-dd");
                string vCurrentFilePath = mLogDirPath + "\\" + vLogType + vTodaysDate + ".csv";
                //check if log directory exists first, create it
                if (!Directory.Exists(mLogDirPath))
                {
                    Directory.CreateDirectory(mLogDirPath);
                }


                //Get all the current files stored in this directory
                //
                string[] vFiles = Directory.GetFiles(mLogDirPath);
                //get the files that are of the log type and are less than one and are from todays day
                string[] vFound = vFiles.Where(f =>
                  f.Contains(vLogType.ToString())
                  && f.Contains(vTodaysDate)
                  && (new FileInfo(f).Length / 1000000) < Settings.MaxFileSizeMb
                  ).ToArray();

                if (vFound.Length > 0)
                {
                    vCurrentFilePath = vFound[0];
                }
                else
                {
                    int vCount = vFound.Length;
                    vCurrentFilePath = mLogDirPath + "\\" + vLogType.ToString() + vCount + "_" + vTodaysDate + ".csv";
                    FileStream vFs = File.Create(vCurrentFilePath);
                    vFs.Close();
                }

                //append to the file
                FileStream vFile = new FileStream(vCurrentFilePath, FileMode.Append, FileAccess.Write);
                StreamWriter vStreamWriter = new StreamWriter(vFile);
                vStreamWriter.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " , " + ((int)vLog.LogType) + " , " + vLog.Message);
                vStreamWriter.Close();
            }
            catch (Exception e)
            {
                Debug.Log("<color=red>Failed to write to log type:</color> " + vLog  +" "+e); 
            }
        }
    }

    public enum LogType
    {
        BrainpackCommand = 1,
        BrainpackResponse = 2,
        BrainpackFrame = 3,
        ApplicationCommand = 4,
        ApplicationResponse = 5,
        ApplicationFrame = 6,
        SocketClientSend =7,
        SocketClientReceive = 8,
        SocketClientError = 9,
        SocketClientSettings =10
    }

    public enum OutputLogPath
    {
        ApplicationLog,
        BrainpackMsgLog,
        BrainpackFrames,
        ApplicationFrames,
        SocketClientLog
    }

    public struct Log
    {
        public string Message;
        public LogType LogType;
    }
}
