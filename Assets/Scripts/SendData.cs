using FirebaseWebGL.Scripts.FirebaseBridge;
using System;
using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

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
    [SerializeField]
    private TMP_InputField loginAvatarID;

    [Header("Password reset input fields")]
    [SerializeField]
    private TMP_InputField resetPasswordUsername;

    [Header("DisplayName reset input fields")]
    [SerializeField]
    private TMP_InputField resetDisplayName;

    [Header("DataName input field")]
    [SerializeField]
    private TMP_InputField dataNameInputField;

    private UserContainer userWrap;
    private UserContainer createdUser;

    public int avatarId;
     
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
    /// Update this class if you want other data from the user
    /// </summary>
    [System.Serializable]
    public class User
    {
        public string uid;
        public string email;
        public string displayName;
        public int avatarId;
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
    /// Returns a JSON format of the database in the GetAllDataOuput
    /// </summary>
    public void GetDatabaseData() {
        string path = DatabaseManager.ConstructDatabasePath();
        DatabaseManager.GetJSON(path, gameObject.name, "GetAllDataOuput", "OnPostFailed");
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
            DatabaseManager.GetAndAdd(path, gameObject.name, "EventOuput", "OnPostFailed");

            if (!string.IsNullOrEmpty(userWrap.user.uid))
            {
                path = DatabaseManager.ConstructDatabasePath("All users", userWrap.user.uid, "Events");
                DatabaseManager.PushJSON(path, JsonUtility.ToJson(userEventComponent), gameObject.name, "EventOuput", "OnPostFailed");
            }
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
        FirebaseAuth.SendPasswordResetEmail(resetPasswordUsername.text, gameObject.name, "ResetPasswordOutput", "OnPostFailed");
    }

    /// <summary>
    /// Send new displayname and update it in the Realtime Database
    /// </summary>
    public void ResetDisplayNameOfLoggedInPlayer() {
        if (string.IsNullOrEmpty(resetDisplayName.text)) {
            Debug.Log("No new username entered");
            return;
        }

        if (!string.IsNullOrEmpty(userWrap.user.uid))
        {
            FirebaseAuth.UpdateDisplayName(resetDisplayName.text, gameObject.name, "ResetDisplayNameOuput", "OnPostFailed");

            string path = DatabaseManager.ConstructDatabasePath("All users", userWrap.user.uid, "displayName");
            DatabaseManager.PostJSON(path, resetDisplayName.text, gameObject.name, "ResetDisplayNameOuput", "OnPostFailed");
        }
        else
        {
            Debug.Log("No player logged in");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventName"></param>
    public void GetEvents(string eventName)
    {
        string path = DatabaseManager.ConstructDatabasePath("All users", userWrap.user.uid, "Events");
        DatabaseManager.GetJSON(path, gameObject.name, "GetListOfEventsOutput", "OnPostFailed");
    }
    #endregion

    #region Set user classes
    /// <summary>
    /// Set the data of the user in the user class to use in your platform
    /// If the loginAvatarID.text is empty or 0, you will get the avatarID that is set earlier from the Realtime Database
    /// If the loginAvatarID.text is not empty or 0, the it will set the avatarid from the textbox loginAvatarID and return all user data
    /// </summary>
    /// <param name="userData">userData returned by firebase after login</param>
    public void SetLoggedInUser(string userData)
    {
        Debug.LogFormat("Data of the logged in user: {0}", userData);
        userWrap = JsonUtility.FromJson<UserContainer>(userData);

        SetNewAvatarID(loginAvatarID.text);
    }

    /// <summary>
    /// Set A new avatar ID for the logged in user
    /// </summary>
    public void SetNewAvatarID(string newAvatarID)
    {
        if (string.IsNullOrEmpty(newAvatarID))
        {
            newAvatarID = loginAvatarID.text;
        }
        // It returns the avatarId of the user at both options
        if (!string.IsNullOrEmpty(newAvatarID))
        {
            string path = DatabaseManager.ConstructDatabasePath("All users", userWrap.user.uid, "avatarId");
            DatabaseManager.PostJSON(path, newAvatarID, gameObject.name, "GetSpecificDataOutput", "OnPostFailed");
        }
        else
        {
            string path = DatabaseManager.ConstructDatabasePath("All users", userWrap.user.uid, "avatarId");
            DatabaseManager.GetJSON(path, gameObject.name, "GetSpecificDataOutput", "OnPostFailed");
        }
    }

    /// <summary>
    /// Get specific values from the realtime database of the loggedIn user by the specific dataName
    /// </summary>
    /// <param name="dataName">The specific name of the Data you want to get. For example avatarId.</param>
    public void GetSpecificData(string dataName)
    {
        if (string.IsNullOrEmpty(dataName))
        {
            if (string.IsNullOrEmpty(dataNameInputField.text))
            {
                Debug.Log("No Dataname set");
                return;
            }
            else
            {
                dataName = dataNameInputField.text;
            }
        }

        if (userWrap == null)
        {
            Debug.Log("No user is loggedIn");
        }
        else
        {
            string path = DatabaseManager.ConstructDatabasePath("All users", userWrap.user.uid, dataName);
            DatabaseManager.GetJSON(path, gameObject.name, "GetSpecificDataOutput", "OnPostFailed");
        }
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
    /// Get the specific data you called
    /// </summary>
    /// <param name="output">Firebase return</param>
    public void GetSpecificDataOutput(string output)
    {
        try
        {
            Debug.Log("Specific data output: " + output);
        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message);
        }
    }

    /// <summary>
    /// Get all data
    /// </summary>
    /// <param name="output">Firebase return</param>
    public void GetAllDataOuput(string output)
    {
        try
        {
            // Set here the where you want the data to be
            Debug.Log("All data output: " + output);
        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message);
        }
    }

    /// <summary>
    /// Get all lsit of events
    /// </summary>
    /// <param name="output">Firebase return</param>
    public void GetListOfEventsOutput(string output)
    {
        try
        {
            Debug.Log("All data output: " + output);
            // Set here the where you want the data to be
            List<UserEventComponent> eventsList = JsonConvert.DeserializeObject<List<UserEventComponent>>(output);

            // Printing the events to the console for verification
            foreach (var ev in eventsList)
            {
                Console.WriteLine($"Name: {ev.name}, Date Completed: {ev.dateEventCompleted}, Time Completed: {ev.timeEventCompleted}");
            }
        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message);
        }
    }

    /// <summary>
    /// Get the login output
    /// </summary>
    /// <param name="output">Firebase return</param>
    public void LoginUserOuput(string output)
    {
        try
        {
            // Set here the where you want the data to be

            Debug.Log("Login output: " + output);
        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message);
        }
    }

    /// <summary>
    /// Get the create user output
    /// </summary>
    /// <param name="output">Firebase return</param>
    public void CreateUserOutput(string output)
    {
        try
        {
            // Set here the where you want the data to be
            Debug.Log("Login output: " + output);
        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message);
        }
    }

    /// <summary>
    /// Get the reset password output
    /// </summary>
    /// <param name="output">Firebase return</param>
    public void ResetPasswordOutput(string output)
    {
        try
        {
            // Set here the where you want the data to be
            Debug.Log("Login output: " + output);
        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message);
        }
    }

    /// <summary>
    /// Get the reset DisplayName output
    /// </summary>
    /// <param name="output">Firebase return</param>
    public void ResetDisplayNameOuput(string output)
    {
        try
        {
            // Set here the where you want the data to be
            Debug.Log("Login output: " + output);
        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message);
        }
    }

    /// <summary>
    /// Get the event output
    /// </summary>
    /// <param name="output">Firebase return</param>
    public void EventOuput(string output)
    {
        try
        {
            // Set here the where you want the data to be
            Debug.Log("Login output: " + output);
        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message);
        }
    }

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