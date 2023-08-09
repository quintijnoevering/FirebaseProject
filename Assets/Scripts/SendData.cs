using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.FirebaseAnalytics;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;
using JetBrains.Annotations;

public class SendData : MonoBehaviour
{
    [Header("Set specific user for the buttons")]
    [SerializeField]
    private string playerId = "";

    [Header("Create user input fields")]
    [SerializeField]
    private TMP_InputField createUsername;
    [SerializeField]
    private TMP_InputField createdislayName;
    [SerializeField]
    private TMP_InputField createPassword;

    [Header("Login user input fields")]
    [SerializeField]
    private TMP_InputField loginUsername;
    [SerializeField]
    private TMP_InputField loginPassword;

    [Header("Password reset input fields")]
    [SerializeField]
    private TMP_InputField resetPasswordUsername;

    private void Start()
    {
        createPassword.contentType = TMP_InputField.ContentType.Password;
        loginPassword.contentType = TMP_InputField.ContentType.Password;
    }

    [System.Serializable]
    public class UserContainer
    {
        public User user;
    }

    [System.Serializable]
    public class User
    {
        public string uid;
        public string email;
        public string displayName;
        // You can add other properties if needed
    }

    public class EventComponent
    {
        public string name;
        public string date;
    }

    public UserContainer userWrap;
    public UserContainer createdUser;

    /// <summary>
    /// Regular expression, which is used to validate an E-Mail address.
    /// </summary>
    public const string MatchEmailPattern =
        @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
        + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
          + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
        + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

    /// <summary>
    /// Checks whether the given Email-Parameter is a valid E-Mail address.
    /// </summary>
    /// <param name="email">Parameter-string that contains an E-Mail address.</param>
    /// <returns>True, wenn Parameter-string is not null and contains a valid E-Mail address;
    /// otherwise false.</returns>
    public static bool IsEmail(string email)
    {
        if (email != null) return Regex.IsMatch(email, MatchEmailPattern);
        else return false;
    }

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
            path = DatabaseManager.ConstructDatabasePath("Events", button.name);
            DatabaseManager.GetAndAdd(path, gameObject.name, "OnPostSuccesfull", "OnPostFailed");
        }

        if (!string.IsNullOrEmpty(userWrap.user.uid))
        {
            path = DatabaseManager.ConstructDatabasePath("All users", userWrap.user.uid, "Events", button.name);
            DatabaseManager.GetAndAdd(path, gameObject.name, "OnPostSuccesfull", "OnPostFailed");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void SendEvent(string eventName)
    {
        string path;
        if (!string.IsNullOrEmpty(eventName))
        {
            DateTime dt = DateTime.Now;
            EventComponent eventComponent = new EventComponent()
            {
                name = eventName,
                date = dt.ToString("yyyy-MM-dd")
            };

            path = DatabaseManager.ConstructDatabasePath("Events");
            DatabaseManager.PostJSON(path, JsonUtility.ToJson(eventComponent), gameObject.name, "OnPostSuccesfull", "OnPostFailed");

            if (!string.IsNullOrEmpty(userWrap.user.uid))
            {
                path = DatabaseManager.ConstructDatabasePath("All users", userWrap.user.uid, "Events");
                DatabaseManager.PostJSON(path, JsonUtility.ToJson(eventComponent), gameObject.name, "OnPostSuccesfull", "OnPostFailed");
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void AddUserToDataSet()
    {
        if (string.IsNullOrEmpty(createdUser.user.uid))
        {
            Debug.Log("No user is logged in");
            return;
        }
        string path = DatabaseManager.ConstructDatabasePath( "All users", createdUser.user.uid);
        DatabaseManager.PostJSON(path, JsonUtility.ToJson(createdUser.user), gameObject.name, "OnPostSuccesfull", "OnPostFailed");
    }

    /// <summary>
    /// 
    /// </summary>
    public void CreateUser()
    {
        if (string.IsNullOrEmpty(createUsername.text) || string.IsNullOrEmpty(createdislayName.text) || string.IsNullOrEmpty(createPassword.text))
        {
            Debug.Log("No username or Password entered");
            return;
        }

        if (!IsEmail(createUsername.text))
        {
            Debug.Log("Enter a valid email adress");
            return;
        }

        FirebaseAuth.CreateUserWithEmailAndPassword(createUsername.text, createPassword.text, createdislayName.text, gameObject.name, "SetCreatedUser", "OnPostFailed");
    }

    /// <summary>
    /// 
    /// </summary>
    public void LoginUser()
    {
        if (string.IsNullOrEmpty(loginUsername.text) || string.IsNullOrEmpty(loginPassword.text))
        {
            Debug.Log("No username or Password entered");
            return;
        }

        if (!IsEmail(loginUsername.text))
        {
            Debug.Log("Enter a  valid email adress");
            return;
        }

        FirebaseAuth.SignInWithEmailAndPassword(loginUsername.text, loginPassword.text,gameObject.name, "SetLoggedInUser", "OnPostFailed");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userData"></param>
    public void SetLoggedInUser(string userData)
    {
        Debug.Log("Userdata: " + userData);
        userWrap = JsonUtility.FromJson<UserContainer>(userData);

        Debug.Log("User ID: " + userWrap.user.uid);
        Debug.Log("Email: " + userWrap.user.email);
        Debug.Log("Display Name: " + userWrap.user.displayName);

        playerId = userWrap.user.uid;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userData"></param>
    public void SetCreatedUser(string userData)
    {
        Debug.Log("Userdata: " + userData);
        createdUser = JsonUtility.FromJson<UserContainer>(userData);

        Debug.Log("User ID: " + createdUser.user.uid);
        Debug.Log("Email: " + createdUser.user.email);
        Debug.Log("Display Name: " + createdUser.user.displayName);

        AddUserToDataSet();
    }

    /// <summary>
    /// 
    /// </summary>
    public void ResetPasswordWithEmail()
    {
        if (string.IsNullOrEmpty(resetPasswordUsername.text))
        {
            Debug.Log("No username or Password entered");
            return;
        }
        FirebaseAuth.SendPasswordResetEmail(resetPasswordUsername.text, gameObject.name, "OnPostSuccesfull", "OnPostFailed");
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
