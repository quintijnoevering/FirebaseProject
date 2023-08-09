using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DatabaseManager : MonoBehaviour
{
    /// <summary>
    /// Constructs a database path by joining the given node names.
    /// </summary>
    /// <param name="nodes">An array of node names to form the path.</param>
    /// <returns>The database path as a string.</returns>
    public static string ConstructDatabasePath(params string[] nodes)
    {
        return string.Join("/", nodes);
    }

    /// <summary>
    /// Gets JSON from a specified path
    /// Will return a snapshot of the JSON in the callback output
    /// </summary>
    /// <param name="path"> Database path </param>
    /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
    /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
    /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
    [DllImport("__Internal")]
    public static extern void GetJSON(string path, string objectName, string callback, string fallback);

    /// <summary>
    /// Posts JSON to a specified path
    /// </summary>
    /// <param name="path"> Database path </param>
    /// <param name="value"> JSON string to post to the specified path </param>
    /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
    /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
    /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
    [DllImport("__Internal")]
    public static extern void PostJSON(string path, string value, string objectName, string callback, string fallback);

    /// <summary>
    /// Pushes JSON to a specified path with a Firebase generated unique key
    /// </summary>
    /// <param name="path"> Database path </param>
    /// <param name="value"> JSON string to push to the specified path </param>
    /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
    /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
    /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
    //[DllImport("__Internal")]
    //public static extern void PushJSON(string path, string value, string objectName, string callback, string fallback);

    /// <summary>
    /// Updates JSON in a specified path
    /// </summary>
    /// <param name="path"> Database path </param>
    /// <param name="value"> JSON string to update in the specified path </param>
    /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
    /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
    /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
    [DllImport("__Internal")]
    public static extern void UpdateJSON(string path, string value, string objectName, string callback, string fallback);

    /// <summary>
    /// Deletes JSON in a specified path
    /// </summary>
    /// <param name="path"> Database path </param>
    /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
    /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
    /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
    [DllImport("__Internal")]
    public static extern void DeleteJSON(string path, string objectName, string callback, string fallback);

    /// <summary>
    /// Deletes JSON in a specified path
    /// </summary>
    /// <param name="path"> Database path </param>
    /// <param name="objectName"> Name of the gameobject to call the callback/fallback of </param>
    /// <param name="callback"> Name of the method to call when the operation was successful. Method must have signature: void Method(string output) </param>
    /// <param name="fallback"> Name of the method to call when the operation was unsuccessful. Method must have signature: void Method(string output). Will return a serialized FirebaseError object </param>
    [DllImport("__Internal")]
    public static extern void GetAndAdd(string path, string objectName, string callback, string fallback);
}
