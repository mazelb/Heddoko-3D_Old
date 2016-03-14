
/** 
* @file LocalDatabaseConnection.cs
* @brief Contains the LocalDatabaseConnection class, implementing IDatabaseConnection interface
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
using System;
using System.IO;
using Assets.Scripts.Frames_Pipeline.BodyFrameConversion;
using Assets.Scripts.UI.Settings;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.DatabaseAccess;
using UnityEngine;
using JetBrains.Annotations;
using Mono.Data.Sqlite;

namespace Assets.Scripts.Communication.DatabaseConnectionPipe.DatabaseConnection
{
    /// <summary>
    /// A connection to a local database. This connection will persist until the 
    /// </summary>
    public class LocalDatabaseConnection : IDatabaseConnection
    {
        private SqliteConnection mDbConnection;

        public bool Equals([NotNull] IDatabaseConnection other)
        {
            if (other == null) throw new ArgumentNullException("other");
            throw new NotImplementedException();
        }

        public Guid UUID { get; private set; }

        public string DbConnectionUUID { get; private set; }

        public bool Connect(Action vCallback = null)
        {
            bool vReturn = false;
            if (mDbConnection == null)
            {
                //check if the file exists first. 
                string vFilePath = ApplicationSettings.LocalDbPath;
                if (File.Exists(vFilePath))
                {
                    mDbConnection = new SqliteConnection("URI=file:" + vFilePath);
                    mDbConnection.Open();
                    vReturn = true;
                    if (vCallback != null)
                    {
                        vCallback.Invoke();
                    }
                }
            }

            return vReturn;
        }

        /// <summary>
        /// Disconnect from the database
        /// </summary>
        /// <param name="vCallback"></param>
        public void Disconnect(Action vCallback = null)
        {
            try
            {
                mDbConnection.Close();
            }
            catch (SqliteException vEx)
            {
                Debug.Log(vEx.ToString());
            }

        }

        /// <summary>
        /// Query the database with a command
        /// </summary>
        /// <param name="vCommand"></param>
        /// <param name="vCallback"></param>
        /// <returns></returns>
        public bool Query(string vCommand, Action vCallback = null)
        {
            SqliteCommand vCmd = mDbConnection.CreateCommand();
            vCmd.CommandText = vCommand;
            vCmd.ExecuteNonQuery(); 
            if (vCallback != null)
            {
                vCallback.Invoke();
            }
            return false;
        }


        public bool CreateRecording(BodyFramesRecording vRecording)
        {
            bool vResult = false;
            
            using (var vCmd = mDbConnection.CreateCommand())
            {
                using (var vTransaction = mDbConnection.BeginTransaction())
                {
                    string vDatePattern = @"M/d/yyyy hh:mm:ss tt";
                    string vTimeNowUsEn = DateTime.UtcNow.ToString(vDatePattern);
                    string vRecordingInsertion = "INSERT INTO " + DBSettings.RecordingsTable +
                                         " (id,submitted_by,complex_equipment_id,folder_id,title,created_at,updated_at) " +
                                         "VALUES ('" + vRecording.BodyRecordingGuid + "', '" + vRecording.BodyGuid + "' , '" + vRecording.SuitGuid + "'," +
                                         "'DEFAULT','" + vRecording.Title + "' , '" + vTimeNowUsEn + "','" + vTimeNowUsEn + "')";
                    string vMoveMetaData = Guid.NewGuid().ToString();
                    string vMovementDatInsert = "INSERT INTO " + DBSettings.RecordingMeta +
                                                " (id ,movement_id,start_frame,end_frame) ";
                    string vMovementDatValue = "VALUES('" + vMoveMetaData + "', '" + vRecording.BodyRecordingGuid + "'," +
                                            0 + "," + (vRecording.RecordingRawFrames.Count - 1) + ")";
                    try
                    {
                        vCmd.CommandText = vRecordingInsertion;
                        vCmd.ExecuteNonQuery();
                        vCmd.CommandText = vMovementDatInsert + vMovementDatValue;
                        vCmd.ExecuteNonQuery();
                     
                        string vFrameInsertion = "INSERT INTO " + DBSettings.FramesTable +
                                                 " (id,movement_id,format_revision,timestamp) ";
                        string vFrameValue = "VALUES (";

                        string vRawDataInsertion = "INSERT INTO " + DBSettings.RawDataTable +
                                                 " (id ,movement_id,data) ";
                        string vRawDataValues = "VALUES (";

                        string vInstantDataInsert = "INSERT INTO " + DBSettings.InstantaneousData +
                                                 " (id ,type,data,movement_id) ";
                        string vInstantDatVal = "VALUES (";


                        //initialize variable to avoid GC punishment
                        BodyFrame vConvertedFrame = null;
                        BodyRawFrame vRawFrame = null;
                        string vInstantDataJson = "";
                        string vJoinedRawFrameData = "";
                        for (int i = 0; i < vRecording.RecordingRawFrames.Count; i++)
                        {
                            vRawFrame = vRecording.RecordingRawFrames[i];
                            vConvertedFrame = RawFrameConverter.ConvertRawFrame(vRawFrame);

                            //+++++++++++++++++++++Frames table insertion +++++++++++++++++++++++++++++++++++++++++++++++
                            vFrameValue += i + ",'" + vRecording.BodyRecordingGuid + "','" + vRecording.FormatRevision + "',"
                                      + vConvertedFrame.Timestamp + ")";
                            vCmd.CommandText = vFrameInsertion + vFrameValue;
                            vCmd.ExecuteNonQuery();
                           
                            //+++++++++++++++++++++raw_data table insertion +++++++++++++++++++++++++++++++++++++++++++++
                            vJoinedRawFrameData = string.Join(",", vRawFrame.RawFrameData);
                            vRawDataValues += i + ",'" + vRecording.BodyRecordingGuid + "','" + vJoinedRawFrameData + "')";
                            vCmd.CommandText = vRawDataInsertion + vRawDataValues;
                            vCmd.ExecuteNonQuery();
                         
                            //+++++++++++++++++++++instantaneous_data table insertion +++++++++++++++++++++
                            try
                            {
                                vInstantDataJson = JsonUtilities.SerializeObjToJson(vConvertedFrame);
                                vInstantDatVal += i + "," + "'default'" + ",'" + vInstantDataJson + "','"
                                          + vRecording.BodyRecordingGuid + "')";
                               // mConnection.Query(vInstantDataInsert + vInstantDatVal);
                                vCmd.CommandText = vInstantDataInsert + vInstantDatVal;
                                vCmd.ExecuteNonQuery();
                            }
                            catch (Exception e)
                            {

                                Debug.Log(" json serialization error " + e);
                            }
                             
                            //+++++++++++++++++++ reset value parameters
                            vFrameValue = vRawDataValues = vInstantDatVal = "VALUES (";
                        }
                        vResult = true;
                    }
                    catch (SqliteException vEx)
                    {
                        Debug.Log("Problem " + vEx);
                    }
                
                    vTransaction.Commit();
                }
            }


            /*
             
              bool vResult = false;
             
            
              return vResult;*/

            return vResult;
        }

    }
}
