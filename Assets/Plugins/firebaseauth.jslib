mergeInto(LibraryManager.library, {
    CreateUserWithEmailAndPassword: function(email, password, displayName, objectName, callback, fallback) {
        var parsedEmail = UTF8ToString(email);
        var parsedPassword = UTF8ToString(password);
        var parsedDisplayName = UTF8ToString(displayName);
        var parsedObjectName = UTF8ToString(objectName);
        var parsedCallback = UTF8ToString(callback);
        var parsedFallback = UTF8ToString(fallback);

        try {
            window.firebaseCreateUserWithEmailAndPassword(window.auth, parsedEmail, parsedPassword)
                .then(function(userCredential) {
                    // The user has been successfully created
                    var user = userCredential.user;
                    window.unityInstance.SendMessage(parsedObjectName, parsedCallback, "");

                    window.updateProfile(window.auth.currentUser, {
                        displayName: parsedDisplayName
                    }).then(function() {

                        window.sendEmailVerification(user).then(function() {
                            window.unityInstance.SendMessage(parsedObjectName, parsedCallback, JSON.stringify(userCredential));
                        }).catch(function(error) {
                            window.unityInstance.SendMessage(parsedObjectName, parsedFallback, error.message);
                        });

                    }, function(error) {
                            window.unityInstance.SendMessage(parsedObjectName, parsedFallback, error.message);
                    });        
                })
                .catch(function(error) {
                    // There was an error in the account creation process
                    var errorCode = error.code;
                    var errorMessage = error.message;
                    window.unityInstance.SendMessage(parsedObjectName, parsedFallback, errorMessage);
                });
        } catch (error) {
            window.unityInstance.SendMessage(parsedObjectName, parsedFallback, error.message);
        }
    },

    SendPasswordResetEmail: function(email, objectName, callback, fallback) {
        var parsedEmail = UTF8ToString(email);
        var parsedObjectName = UTF8ToString(objectName);
        var parsedCallback = UTF8ToString(callback);
        var parsedFallback = UTF8ToString(fallback);

        try {
            window.firebaseSendPasswordResetEmail(window.auth, parsedEmail)
                .then(function() {
                    // Mail had been send
                    window.unityInstance.SendMessage(parsedObjectName, parsedCallback, "Success: email send to " + parsedEmail);
                })
                .catch(function(error) {
                    // There was an error in the sign-in process
                    var errorMessage = JSON.stringify(error, Object.getOwnPropertyNames(error));
                    window.unityInstance.SendMessage(parsedObjectName, parsedFallback, errorMessage);
                });
        } catch (error) {
            var errorMessage = JSON.stringify(error, Object.getOwnPropertyNames(error));
            window.unityInstance.SendMessage(parsedObjectName, parsedFallback, errorMessage);
        }
    },

    SignInWithEmailAndPassword: function(email, password, objectName, callback, fallback) {
        var parsedEmail = UTF8ToString(email);
        var parsedPassword = UTF8ToString(password);
        var parsedObjectName = UTF8ToString(objectName);
        var parsedCallback = UTF8ToString(callback);
        var parsedFallback = UTF8ToString(fallback);

        try {
            window.firebaseSignInWithEmailAndPassword(window.auth, parsedEmail, parsedPassword)
                .then(function(userCredential) {
                    // The user has been successfully signed in
                    console.log(userCredential);
                    window.unityInstance.SendMessage(parsedObjectName, parsedCallback, JSON.stringify(userCredential));
                })
                .catch(function(error) {
                    // There was an error in the sign-in process
                    var errorMessage = JSON.stringify(error, Object.getOwnPropertyNames(error));
                    window.unityInstance.SendMessage(parsedObjectName, parsedFallback, errorMessage);
                });
        } catch (error) {
            var errorMessage = JSON.stringify(error, Object.getOwnPropertyNames(error));
            window.unityInstance.SendMessage(parsedObjectName, parsedFallback, errorMessage);
        }
    },

    OnAuthStateChanged: function (objectName, onUserSignedIn, onUserSignedOut) {
        var parsedObjectName = UTF8ToString(objectName);
        var parsedOnUserSignedIn = UTF8ToString(onUserSignedIn);
        var parsedOnUserSignedOut = UTF8ToString(onUserSignedOut);

        window.auth.onAuthStateChanged(function(user) {
            if (user) {
                window.unityInstance.SendMessage(parsedObjectName, parsedOnUserSignedIn, JSON.stringify(user));
            } else {
                window.unityInstance.SendMessage(parsedObjectName, parsedOnUserSignedOut, "User signed out");
            }
        });

    }
});
