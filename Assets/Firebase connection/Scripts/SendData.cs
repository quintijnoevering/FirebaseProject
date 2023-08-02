using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendData : MonoBehaviour
{
    [SerializeField]
    private string playerId = "";

    /// <summary>
    /// Returns a JSON format of the database in the OnPostSuccesfull
    /// </summary>
    public void GetDatabaseData()
    {
        string path = DatabaseManager.ConstructDatabasePath();
        DatabaseManager.GetJSON(path, gameObject.name, "OnPostSuccesfull", "OnPostFailed");
    }

    /// <summary>
    /// Will increase the number of total clicks of the given button.
    /// Also increases the total clicks of the button from the given player.
    /// The results are visable in the Realtime Database of Firebase.
    /// </summary>
    /// <param name="button">The clicked button</param>
    public void ButtonPressed(GameObject button)
    {
        string path;
        if (button != null)
        {
            path = DatabaseManager.ConstructDatabasePath("buttons", button.name);
            DatabaseManager.GetAndAdd(path, gameObject.name, "OnPostSuccesfull", "OnPostFailed");
        }

        if (!string.IsNullOrEmpty(playerId))
        {
            path = DatabaseManager.ConstructDatabasePath(playerId, "clickedButtons", button.name);
            DatabaseManager.GetAndAdd(path, gameObject.name, "OnPostSuccesfull", "OnPostFailed");
        }
    }

    public void OnPostSuccesfull(string output)
    {
        Debug.Log(output);
    }

    public void OnPostFailed(string output)
    {
        Debug.Log(output);
    }
}
