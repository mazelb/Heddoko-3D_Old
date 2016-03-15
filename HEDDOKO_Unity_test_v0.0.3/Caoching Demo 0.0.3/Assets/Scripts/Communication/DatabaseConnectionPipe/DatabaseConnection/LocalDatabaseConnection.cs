
/** 
* @file LocalDatabaseConnection.cs
* @brief Contains the LocalDatabaseConnection class, implementing IDatabaseConnection interface
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Update the database with a passed in recording
        /// </summary>
        /// <param name="vRecording"></param>
        /// <returns></returns>
        public bool CreateRecording(BodyFramesRecording vRecording)
        {
            bool vResult = false;

            using (var vCmd = mDbConnection.CreateCommand())
            {
                using (var vTransaction = mDbConnection.BeginTransaction())
                {

                    string vDatePattern = @"M/d/yyyy hh:mm:ss tt";
                    string vTimeNowUsEn = DateTime.UtcNow.ToString(vDatePattern);




                    try
                    {
                        vCmd.CommandText = "INSERT INTO movements (id, complex_equipment_id , profile_id, submitted_by , folder_id,title ,created_at ,updated_at) "
                                       + " VALUES (@param1, @param2,@param3, @param4, @param5, @param6,@param7, @param8)";
                        vCmd.Parameters.Add(new SqliteParameter("@param1", vRecording.BodyRecordingGuid));
                        vCmd.Parameters.Add(new SqliteParameter("@param2", vRecording.SuitGuid));
                        vCmd.Parameters.Add(new SqliteParameter("@param3", vRecording.BodyGuid));
                        vCmd.Parameters.Add(new SqliteParameter("@param4", vRecording.BodyGuid));
                        vCmd.Parameters.Add(new SqliteParameter("@param5", "Default"));
                        vCmd.Parameters.Add(new SqliteParameter("@param6", vRecording.Title));
                        vCmd.Parameters.Add(new SqliteParameter("@param7", vTimeNowUsEn));
                        vCmd.Parameters.Add(new SqliteParameter("@param8", vTimeNowUsEn));
                        vCmd.ExecuteNonQuery();

                        vCmd.CommandText = "INSERT INTO movement_meta (id ,movement_id,start_frame,end_frame) "
                                       + " VALUES (@param1, @param2,@param3, @param4)";
                        vCmd.Parameters.Add(new SqliteParameter("@param1", Guid.NewGuid().ToString()));
                        vCmd.Parameters.Add(new SqliteParameter("@param2", vRecording.BodyRecordingGuid));
                        vCmd.Parameters.Add(new SqliteParameter("@param3", "0"));
                        vCmd.Parameters.Add(new SqliteParameter("@param4", (vRecording.RecordingRawFrames.Count - 1) + ""));
                        vCmd.ExecuteNonQuery();

                        //initialize variable to avoid GC punishes
                        BodyFrame vConvertedFrame = null;
                        BodyRawFrame vRawFrame = null;
                        string vInstantDataJson = "";
                        string vJoinedRawFrameData = "";
                        for (int i = 0; i < vRecording.RecordingRawFrames.Count; i++)
                        {
                            try
                            {
                                vRawFrame = vRecording.RecordingRawFrames[i];
                                vConvertedFrame = RawFrameConverter.ConvertRawFrame(vRawFrame);
                                //+++++++++++++++++++++Frames table insertion +++++++++++++++++++++++++++++++++++++++++++++++

                                vCmd.CommandText = "INSERT INTO frames (id,movement_id,format_revision,timestamp) "
                                        + " VALUES (@param1, @param2,@param3, @param4)";

                                vCmd.Parameters.Add(new SqliteParameter("@param1", i + ""));
                                vCmd.Parameters.Add(new SqliteParameter("@param2", vRecording.BodyRecordingGuid));
                                vCmd.Parameters.Add(new SqliteParameter("@param3", vRecording.FormatRevision));
                                vCmd.Parameters.Add(new SqliteParameter("@param4", vConvertedFrame.Timestamp + ""));
                                vCmd.ExecuteNonQuery();

                                //+++++++++++++++++++++raw_data table insertion +++++++++++++++++++++++++++++++++++++++++++++

                                vCmd.CommandText = "INSERT INTO raw_data (id ,movement_id,data) "
                                       + " VALUES (@param1, @param2, @param3)";

                                vCmd.Parameters.Add(new SqliteParameter("@param1", i + ""));
                                vCmd.Parameters.Add(new SqliteParameter("@param2", vRecording.BodyRecordingGuid));
                                vCmd.Parameters.Add(new SqliteParameter("@param3", string.Join(",", vRawFrame.RawFrameData)));
                                vCmd.ExecuteNonQuery();
                                //+++++++++++++++++++++instantaneous_data table insertion +++++++++++++++++++++
                                vInstantDataJson = JsonUtilities.SerializeObjToJson(vConvertedFrame);
                                vCmd.CommandText = "INSERT INTO instantaneous_data (id ,type,data,movement_id) "
                                      + " VALUES (@param1, @param2, @param3,@param4)";

                                vCmd.Parameters.Add(new SqliteParameter("@param1", i + ""));
                                vCmd.Parameters.Add(new SqliteParameter("@param2", "default"));
                                vCmd.Parameters.Add(new SqliteParameter("@param3", vInstantDataJson));
                                vCmd.Parameters.Add(new SqliteParameter("@param4", vRecording.BodyRecordingGuid));

                                vCmd.ExecuteNonQuery();
                            }
                            catch (Exception e)
                            {

                                Debug.Log(" error " + e);
                            }

                        }
                        vResult = true;
                    }
                    catch (SqliteException vEx)
                    {
                        Debug.Log("SqliteException " + vEx);
                    }

                    vTransaction.Commit();
                }
            }

            return vResult;
        }

        /// <summary>
        /// get a recording based on the id passed in
        /// </summary>
        /// <param name="vRecordingId">the recording id</param>
        /// <returns></returns>
        public BodyFramesRecording GetRawRecording(string vRecordingId)
        {
            BodyFramesRecording vBodyFramesRecording = new BodyFramesRecording();
            List<BodyRawFrame> vRawFrames = new List<BodyRawFrame>(500);

            SqliteDataReader vDataReader = null;
            string vBodyGuid = "";
            string vSuitGuid = "";
            string vFormatVersion = "0";

            using (var vCmd = mDbConnection.CreateCommand())
            {
                using (var vTransaction = mDbConnection.BeginTransaction())
                {
                    try
                    {
                        vCmd.CommandText = "SELECT * FROM " + DBSettings.RecordingsTable +
                                      " WHERE id = '" + vRecordingId + "'";
                        vDataReader = vCmd.ExecuteReader();
                        while (vDataReader.Read())
                        {
                            vSuitGuid = vDataReader["complex_equipment_id"].ToString();
                            vBodyGuid = vDataReader["profile_id"].ToString();
                        }
                        vDataReader.Close();
                        //Get the format revision from the first frame hit.
                        vCmd.CommandText = "SELECT format_revision FROM " + DBSettings.FramesTable + " " +
                                           "WHERE movement_id ='" + vRecordingId + "' LIMIT 1";

                        vDataReader = vCmd.ExecuteReader();
                        while (vDataReader.Read())
                        {
                            vFormatVersion = vDataReader["format_revision"].ToString();
                        }

                        vDataReader.Close();
                        //Get raw data from the raw data table. Ids are the positions
                        //of the raw data frames
                        vCmd.CommandText = "SELECT data FROM " + DBSettings.RawDataTable +
                                           " WHERE raw_data.movement_id = '" + vRecordingId +
                                           "' order by raw_data.id";
                        vDataReader = vCmd.ExecuteReader();
                        string vTemp = "";

                        while (vDataReader.Read())
                        {
                            vTemp = vDataReader["data"].ToString();
                            BodyRawFrame vRawFrame = new BodyRawFrame();
                            vRawFrame.BodyRecordingGuid = vRecordingId;
                            vRawFrame.BodyGuid = vBodyGuid;
                            vRawFrame.SuitGuid = vSuitGuid;
                            
                            vRawFrame.RawFrameData = vTemp.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            vRawFrames.Add(vRawFrame);
                        }
                        vTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        Debug.Log("<color=red><b>ERROR!</b></color> " + e);
                        vTransaction.Rollback();
                        //todo: callback error
                    }

                }
                vBodyFramesRecording.BodyRecordingGuid = vRecordingId;
                vBodyFramesRecording.BodyGuid = vBodyGuid;
                vBodyFramesRecording.SuitGuid = vSuitGuid;
                vBodyFramesRecording.FromDatFile = false;
                vBodyFramesRecording.RecordingRawFrames = vRawFrames;
                vBodyFramesRecording.FormatRevision = vFormatVersion;
            }
            return vBodyFramesRecording;
        }

    }
}
