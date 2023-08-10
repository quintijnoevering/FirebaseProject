mergeInto(LibraryManager.library, {
    GetJSON: function (path, objectName, callback, fallback) {
        var parsedPath = UTF8ToString(path);
        var parsedObjectName = UTF8ToString(objectName);
        var parsedCallback = UTF8ToString(callback);
        var parsedFallback = UTF8ToString(fallback);

        try {     
            const ref = window.firebaseRef(window.database, parsedPath);
            window.firebaseOnValue(ref, function(snapshot){
                window.unityInstance.SendMessage(parsedObjectName, parsedCallback, JSON.stringify(snapshot.val()));
            });
        } catch (error) {
            window.unityInstance.SendMessage(parsedObjectName, parsedFallback, error.message);
        }
    },
    GetAndAdd: function(path, objectName, callback, fallback) {
        var parsedPath = UTF8ToString(path);
        var parsedObjectName = UTF8ToString(objectName);
        var parsedCallback = UTF8ToString(callback);
        var parsedFallback = UTF8ToString(fallback);

        const ref = window.firebaseRef(window.database, parsedPath);

        window.firebaseTransaction(ref, function(currentData) {
            if (currentData !== null) { // Checks if data exists at this location
                if (!isNaN(currentData)) {
                    // Return the incremented value. Firebase will use this as the new value.
                    return currentData + 1;
                } else {
                    // If not a number, set to 1
                    return 1;
                }
            } else {
                // If the data doesn't exist, set it to 1
                return 1;
            }
        }).then(function() {
            // The transaction is complete
        }).catch(function(error) {
            // The transaction failed
            window.unityInstance.SendMessage(parsedObjectName, parsedFallback, error.message);
        });
    },
    PostJSON: function (path, value, objectName, callback, fallback) {
        var parsedPath = UTF8ToString(path);
        var parsedValue = JSON.parse(UTF8ToString(value));
        var parsedObjectName = UTF8ToString(objectName);
        var parsedCallback = UTF8ToString(callback);
        var parsedFallback = UTF8ToString(fallback);

        try {
            const ref = window.firebaseRef(window.database, parsedPath);
            window.firebaseSet(ref, parsedValue).then(function() {
            }).catch(function(error) {
                window.unityInstance.SendMessage(parsedObjectName, parsedFallback, error.message);
            });
        } catch (error) {
            window.unityInstance.SendMessage(parsedObjectName, parsedFallback, error.message);
        }
    },
    PushJSON: function (path, value, objectName, callback, fallback) {
        var parsedPath = UTF8ToString(path);
        var parsedValue = JSON.parse(UTF8ToString(value));
        var parsedObjectName = UTF8ToString(objectName);
        var parsedCallback = UTF8ToString(callback);
        var parsedFallback = UTF8ToString(fallback);

        try {
            const ref = window.firebaseRef(window.database, parsedPath);
            window.firebasePush(ref, parsedValue).then(function() {
            }).catch(function(error) {
                window.unityInstance.SendMessage(parsedObjectName, parsedFallback, error.message);
            });
        } catch (error) {
            window.unityInstance.SendMessage(parsedObjectName, parsedFallback, error.message);
        }
    },
    UpdateJSON: function (path, value, objectName, callback, fallback) {
        var parsedPath = UTF8ToString(path);
        var parsedValue = JSON.parse(UTF8ToString(value));
        var parsedObjectName = UTF8ToString(objectName);
        var parsedCallback = UTF8ToString(callback);
        var parsedFallback = UTF8ToString(fallback);

        try {
            const ref = window.firebaseRef(window.database, parsedPath);
            window.firebaseUpdate(ref, parsedValue).then(function() {
            }).catch(function(error) {
                window.unityInstance.SendMessage(parsedObjectName, parsedFallback, error.message);
            });
        } catch (error) {
            window.unityInstance.SendMessage(parsedObjectName, parsedFallback, error.message);
        }
    },
    DeleteJSON: function (path, objectName, callback, fallback) {
        var parsedPath = UTF8ToString(path);
        var parsedObjectName = UTF8ToString(objectName);
        var parsedCallback = UTF8ToString(callback);
        var parsedFallback = UTF8ToString(fallback);

        try {
            const ref = window.firebaseRef(window.database, parsedPath);
            window.firebaseSet(ref, null).then(function() {
            }).catch(function(error) {
                window.unityInstance.SendMessage(parsedObjectName, parsedFallback, error.message);
            });
        } catch (error) {
            window.unityInstance.SendMessage(parsedObjectName, parsedFallback, error.message);
        }
    }
});