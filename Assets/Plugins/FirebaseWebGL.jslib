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
            window.unityInstance.SendMessage(parsedObjectName, parsedFallback, "There was an error: " + error.message);
        }
    },
    GetAndAdd: function (path, objectName, callback, fallback) {
        var parsedPath = UTF8ToString(path);
        var parsedObjectName = UTF8ToString(objectName);
        var parsedCallback = UTF8ToString(callback);
        var parsedFallback = UTF8ToString(fallback);

        try {
            const ref = window.firebaseRef(window.database, parsedPath);
            window.firebaseTransaction(window.database, ref, function(currentData) {
                if (currentData.exists()) {
                    var currentValue = currentData.val(); // Use .val() to get the current value
                    if (!isNaN(currentValue)) {
                        currentData.ref.set(currentValue + 1, function(error) {
                            if (error) {
                                window.unityInstance.SendMessage(parsedObjectName, parsedFallback, error.message);
                            } else {
                                window.unityInstance.SendMessage(parsedObjectName, parsedCallback, "");
                            }
                        });
                    } else {
                        currentData.ref.set(1, function(error) {
                            if (error) {
                                window.unityInstance.SendMessage(parsedObjectName, parsedFallback, error.message);
                            } else {
                                window.unityInstance.SendMessage(parsedObjectName, parsedCallback, "");
                            }
                        });
                    }
                } else {
                    // If the data doesn't exist, set it to 1
                    currentData.ref.set(1, function(error) {
                        if (error) {
                            window.unityInstance.SendMessage(parsedObjectName, parsedFallback, error.message);
                        } else {
                            window.unityInstance.SendMessage(parsedObjectName, parsedCallback, "");
                        }
                    });
                }
                // Be sure to return a value or your transaction will fail.
                return null;
            });
        } catch (error) {
            window.unityInstance.SendMessage(parsedObjectName, parsedFallback, "There was an error: " + error.message);
        }
    },
    ModifyNumberWithTransaction: function (path, objectName, callback, fallback) {
        var parsedPath = UTF8ToString(path);
        var parsedObjectName = UTF8ToString(objectName);
        var parsedCallback = UTF8ToString(callback);
        var parsedFallback = UTF8ToString(fallback);
        console.log("Path: " + parsedPath + " name " + parsedObjectName +" callback "+parsedCallback+" fallback "+ parsedFallback);
        try {
            const ref = window.firebaseRef(window.database, parsedPath);
            console.log("ref: " + ref);
            console.log("database: " + window.database);
            window.firebaseTransaction(window.database, ref, function(currentData) {
                console.log("currentdata: "+ currentData.snapshot.val() );
                if (currentData.snapshot.exists()) {
                    var currentValue = currentData.snapshot.val();
                    if (!isNaN(currentValue)) {
                        currentData.snapshot.ref.set(currentValue + 1);
                    } else {
                        currentData.snapshot.ref.set(1);
                    }
                }
            }).then(function(unused) {
                window.unityInstance.SendMessage(parsedObjectName, parsedCallback, "Success: transaction run in " + parsedPath);
            });

        } catch (error) {
            window.unityInstance.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }
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
                window.unityInstance.SendMessage(parsedObjectName, parsedCallback, "");
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
                window.unityInstance.SendMessage(parsedObjectName, parsedCallback, "");
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
                window.unityInstance.SendMessage(parsedObjectName, parsedCallback, "");
            }).catch(function(error) {
                window.unityInstance.SendMessage(parsedObjectName, parsedFallback, error.message);
            });
        } catch (error) {
            window.unityInstance.SendMessage(parsedObjectName, parsedFallback, error.message);
        }
    }
});