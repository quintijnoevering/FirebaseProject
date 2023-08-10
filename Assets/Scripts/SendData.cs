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

    private UserContainer userWrap;
    private UserContainer createdUser;

    private void Start()
    {
        // Make sure all the password field have ContentType set on password to hide the password
        createPassword.contentType = TMP_InputField.ContentType.Password;
        loginPassword.contentType = TMP_InputField.ContentType.Password;
    }

    #region Firebase Classes
    /// <summary>
    /// Container to get the user JSON data
    /// </summary>
    [System.Serializable]
    public class UserContainer
    {
        public User user;
    }

    /// <summary>
    /// User preset from Firbase with values as:
    /// UID
    /// Email
    /// Display name
    /// 
    /// Update this class if you want other data from the user
    /// </summary>
    [System.Serializable]
    public class User
    {
        public string uid;
        public string email;
        public string displayName;
        // You can add other properties if needed
    }

    /// <summary>
    /// User event preset
    /// </summary>
    public class UserEventComponent
    {
        public string name;
        public string dateEventCompleted;
        public string timeEventCompleted;
    }
    #endregion

    #region Structure functions
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
    public void GetDatabaseData() {
        string path = DatabaseManager.ConstructDatabasePath();
        DatabaseManager.GetJSON(path, gameObject.name, "OnPostSuccesfull", "OnPostFailed");
    }
    #endregion

    #region Firebase functions
    /// <summary>
    /// Send to event to the realtime database in Firebase
    /// Set event values to the given eventName and current datetime
    /// </summary>
    /// <param name="eventName">Set the event name</param>
    public void SendEvent(string eventName)
    {
        string path;
        if (!string.IsNullOrEmpty(eventName))
        {
            DateTime dt = DateTime.Now;
            UserEventComponent userEventComponent = new UserEventComponent() {
                name = eventName,
                dateEventCompleted = dt.ToString("dd-MM-yyyy"),
                timeEventCompleted = dt.ToString("hh:mm:ss")
            };

            path = DatabaseManager.ConstructDatabasePath("Events", userEventComponent.name);
            DatabaseManager.GetAndAdd(path, gameObject.name, "OnPostSuccesfull", "OnPostFailed");

            if (!string.IsNullOrEmpty(userWrap.user.uid))
            {
                path = DatabaseManager.ConstructDatabasePath("All users", userWrap.user.uid, "Events");
                DatabaseManager.PushJSON(path, JsonUtility.ToJson(userEventComponent), gameObject.name, "OnPostSuccesfull", "OnPostFailed");
            }
        }
    }

    /// <summary>
    /// Will increase the number of total clicks of the given button.
    /// Also increases the total clicks of the button from the given player.
    /// The results are visable in the Realtime Database of Firebase.
    /// </summary>
    /// <param name="button">The clicked button</param>
    public void ButtonPressed(GameObject button) {
        string path;
        if (button != null) {
            path = DatabaseManager.ConstructDatabasePath("Events", button.name);
            DatabaseManager.GetAndAdd(path, gameObject.name, "OnPostSuccesfull", "OnPostFailed");
        }

        if (!string.IsNullOrEmpty(userWrap.user.uid)) {
            path = DatabaseManager.ConstructDatabasePath("All users", userWrap.user.uid, "Events", button.name);
            DatabaseManager.GetAndAdd(path, gameObject.name, "OnPostSuccesfull", "OnPostFailed");
        }
    }

    /// <summary>
    /// Add the created user to the Realtime database
    /// </summary>
    public void AddUserToDataSet()
    {
        if (string.IsNullOrEmpty(createdUser.user.uid))
        {
            Debug.Log("User failed to create");
            return;
        }
        string path = DatabaseManager.ConstructDatabasePath( "All users", createdUser.user.uid);
        DatabaseManager.PostJSON(path, JsonUtility.ToJson(createdUser.user), gameObject.name, "OnPostSuccesfull", "OnPostFailed");
    }

    /// <summary>
    /// This function does the following steps in the Jslib
    /// 1. Add user to the Firebase Authentication with the email and password
    /// 2. It updates the user with the given displayName
    /// 3. Send a verification email to the given email
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
    /// Login the user
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
    /// Send password reset link to the given email
    /// </summary>
    public void ResetPasswordWithEmail() {
        if (string.IsNullOrEmpty(resetPasswordUsername.text)) {
            Debug.Log("No username or Password entered");
            return;
        }
        FirebaseAuth.SendPasswordResetEmail(resetPasswordUsername.text, gameObject.name, "OnPostSuccesfull", "OnPostFailed");
    }
    #endregion

    #region Set user classes
    /// <summary>
    /// Set the data of the user in the user class to use in your platform
    /// </summary>
    /// <param name="userData">userData returned by firebase after login</param>
    public void SetLoggedInUser(string userData)
    {
        Debug.LogFormat("Data of the logged in user: {0}", userData);
        userWrap = JsonUtility.FromJson<UserContainer>(userData);
    }

    /// <summary>
    /// Set the created user's data in the user class to use in your platform
    /// </summary>
    /// <param name="userData"></param>
    public void SetCreatedUser(string userData)
    {
        Debug.LogFormat("Data of the created user: {0}", userData);
        createdUser = JsonUtility.FromJson<UserContainer>(userData);

        AddUserToDataSet();
    }
    #endregion

    #region Callbacks
    /// <summary>
    /// Display the returned log
    /// </summary>
    /// <param name="output">Firebase return</param>
    public void OnPostSuccesfull(string output)
    {
        if (!string.IsNullOrEmpty(output)) 
        {
            Debug.LogFormat("Succes: {0}", output);
        }
    }

    /// <summary>
    /// Display the returned error
    /// </summary>
    /// <param name="output">Firebase return</param>
    public void OnPostFailed(string output) 
    {
        if (!string.IsNullOrEmpty(output)) 
        {
            Debug.LogFormat("Error: {0}", output);
        }
    }
    #endregion
}