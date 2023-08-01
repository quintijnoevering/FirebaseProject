using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendData : MonoBehaviour
{
    public void ButtonPressed(GameObject pressedButton)
    {
        string path = DatabaseManager.ConstructDatabasePath();

        DatabaseManager.PostJSON(path, 1.ToString(), pressedButton.name, "OnPostSuccesful", "OnPostFailed");

    }
}
