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