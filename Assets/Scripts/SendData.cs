using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendData : MonoBehaviour
{
    public void ButtonPressed()
    {
        string path = DatabaseManager.ConstructDatabasePath("iets");

        DatabaseManager.GetAndAdd(path, gameObject.name, "OnPostSuccesfull", "OnPostFailed");
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
