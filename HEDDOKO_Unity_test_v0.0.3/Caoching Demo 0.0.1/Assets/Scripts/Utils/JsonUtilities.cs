using Newtonsoft.Json;
using System.IO;
using System;

/**
* @file JsonUtilities.cs
* @brief Contains the JsonUtilities class
* @author Mohammed Haider(mohammed@heddoko.com)
* @date June 2015
*/
namespace Assets.Scripts.Utils
{

    /**
    * JsonUtilities class 
    * @brief Class that provides json utilities to the interested clients
    */

    public class JsonUtilities
    {
        /**
        * ConvertObjectToJson(string vPath, object vObj)
        * @brief  Converts an object to Json Format and saves it to disk 
        * @param  string vPath: path to save to, object vObj: the object that needs to be seralized to a Json format
        * @note a NullValuePassedException will be thrown if vObj is null
        */
        public static void ConvertObjectToJson(string vPath, object vObj)
        {
            if (vObj == null)
            {
                throw new NullValuePassedException();
            }
            JsonSerializer vSerializer = new JsonSerializer();
            StreamWriter vStreamWriter = new StreamWriter(vPath);
            vSerializer.NullValueHandling = NullValueHandling.Ignore;
            vSerializer.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            vSerializer.Formatting = Formatting.Indented;
            using (JsonWriter vWriter = new JsonTextWriter(vStreamWriter))
            {
                vSerializer.Serialize(vWriter, vObj);
            }
        } 
        /**
        * JsonFileToObject<T>(string vPath)
        * @brief  From the given file path, return an object of type T from a JSOn file 
        * @param  string vPath: path to Json file from 
        * @return The deserialize json file to a type T
        */
        public static T JsonFileToObject<T>(string vPath)
        {
            JsonSerializer vDeserializer = new JsonSerializer();
            JsonTextReader vTxtReader = new JsonTextReader(File.OpenText(vPath));
            vDeserializer.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
            T vDeserializedObj = (T)vDeserializer.Deserialize(vTxtReader, typeof(T));
            return vDeserializedObj;
        }

    }
    /// <summary>
    /// Exception should be thrown if a null value type was passed in
    /// </summary>
    /// 
    /**
    * NullValuePassedException class 
    * @brief exception that should be thrown when an innaproriate null value was passed to an method
    */
    public class NullValuePassedException : Exception
    {
    }
}
