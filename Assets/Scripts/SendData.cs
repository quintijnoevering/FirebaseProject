using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendData : MonoBehaviour
{
    [SerializeField]
    private string playerId = "";

    public void ButtonPressed(GameObject button)
    {
        string path = "";
        if (button != null)
        {
            path = DatabaseManager.ConstructDatabasePath("buttons", button.name);
            DatabaseManager.GetAndAdd(path, gameObject.name, "OnPostSuccesfull", "OnPostFailed");
        }

        if (playerId != "")
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
