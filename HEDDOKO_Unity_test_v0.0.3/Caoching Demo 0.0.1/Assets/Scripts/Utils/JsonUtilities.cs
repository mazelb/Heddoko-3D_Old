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
    * SuitsCommunicationMgr class 
    * @brief Class that provides json utilities to the interested clients
    */
 
    public class JsonUtilities
    {
        /// <summary>
        /// Converts an object to a json string and saves it locally with the given path parameter. Will throw an exception if an object passed is null
        /// </summary>
        /// <param name="path"></param>
        /// <param name="o"></param>
        public static void ConvertObjectToJson(string path, object obj)
        {
            if (obj == null)
            {
                throw new NullValuePassedException();
            }
            JsonSerializer serializer = new JsonSerializer();
            StreamWriter sw = new StreamWriter(path);
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            serializer.Formatting = Formatting.Indented;
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, obj);
            }
        }
        /// <summary>
        /// From the given file path, return an object of type T from a JSOn file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T JsonFileToObject<T>(string path)
        {
            JsonSerializer deserializer = new JsonSerializer();
            JsonTextReader txtReader = new JsonTextReader(File.OpenText(path));
            deserializer.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
            T deserializedObj = (T)deserializer.Deserialize(txtReader, typeof(T));
            return deserializedObj;
        }

    }
    /// <summary>
    /// Exception should be thrown if a null value type was passed in
    /// </summary>
    public class NullValuePassedException : Exception
    { }
}
