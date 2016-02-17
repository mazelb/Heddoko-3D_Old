
/** 
* @file DBSettings.cs
* @brief Contains the DBSettings class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
namespace Assets.Scripts.Utils.DatabaseAccess
{
    /// <summary>
    /// Settings for DBAccess
    /// </summary>
    public static class DBSettings
    {
        public const string DbPassword = "rg;dN+[vt^%5)uJJ";
        public const string DbName = "HeddokoDesktopV0001db.sqlite";
        public const string DbConnectStringNoPw = "URI=file: HeddokoDesktopV0001db.sqlite";
        public const string DbConnectionMapTable = "ConnectionMapping";
        public const string DbSettingsTable = "Settings";
    }
}
