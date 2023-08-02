Build the 'Firebase connection Example' to see the results and how to use the firebase connection.
Make sure you play the build on a online server. The communication between Firebase and WebGL will not work otherwise.

NOTE!! When building the WebGL project, make sure the following script is added to the 'index.html' file:
- After the 'var unityInstance = UnityLoader.instantiate("unityContainer", "Build/www.json", {onProgress: UnityProgress});' add:
	'window.unityInstance = unityInstance;'

- After </body> add:
<script type="module">
    // Import the functions you need from the SDKs you need
    import { initializeApp } from "https://www.gstatic.com/firebasejs/9.22.0/firebase-app.js";
    import { getAnalytics } from "https://www.gstatic.com/firebasejs/9.22.0/firebase-analytics.js";
    import { getDatabase, ref, onValue, set, push, update, onChildAdded, onChildChanged, onChildRemoved, runTransaction } from "https://www.gstatic.com/firebasejs/9.22.0/firebase-database.js";
    import { getFirestore, collection, doc, getDocs, getDoc} from "https://www.gstatic.com/firebasejs/9.22.0/firebase-firestore.js";
    import { getAuth, onAuthStateChanged } from "https://www.gstatic.com/firebasejs/9.22.0/firebase-auth.js";
    import { getStorage } from "https://www.gstatic.com/firebasejs/9.22.0/firebase-storage.js";

    // Your web app's Firebase configuration
    const firebaseConfig = {
        apiKey: "AIzaSyBesjP8FCNGDcg0Pt_fKW9m88GPWr76b4k",
        authDomain: "fir-webgl-1b296.firebaseapp.com",
        databaseURL: "https://fir-webgl-1b296-default-rtdb.europe-west1.firebasedatabase.app",
        projectId: "fir-webgl-1b296",
        storageBucket: "fir-webgl-1b296.appspot.com",
        messagingSenderId: "945978901286",
        appId: "1:945978901286:web:a3edbde57e7910ef3af7e7",
        measurementId: "G-T8DKME89QK"
    };
    // Initialize Firebase
    const firebase = initializeApp(firebaseConfig);
    const analytics = getAnalytics(firebase);
    const database = getDatabase(firebase);
    const firestore = getFirestore(firebase);
    const auth = getAuth(firebase);
    const storage = getStorage(firebase);

    const firebaseRef = ref;
    const firebaseTransaction = runTransaction;
    const firebaseOnValue = onValue;
    const firebaseSet = set;
    const firebasePush = push;
    const firebaseUpdate = update;
    const firebaseOnChildAdded = onChildAdded;
    const firebaseOnChildChanged = onChildChanged;
    const firebaseOnChildRemoved = onChildRemoved;

    const firebaseOnAuthStateChanged = onAuthStateChanged;
    const firestoreCol = collection;
    const firestoreGetDocs = getDocs;
    const firestoreDoc = doc;
    const firestoreGetDoc = getDoc;

    // Attach firebase to window object to use it in global scope
    window.firebase = firebase;
    window.database = database;
    window.storage = storage;
    window.auth = auth;
    window.firestore = firestore;

    window.firebaseRef = firebaseRef;
    window.firebaseOnValue = firebaseOnValue;
    window.firebaseSet = firebaseSet;
    window.firebasePush = firebasePush;
    window.firebaseUpdate = firebaseUpdate;
    window.firebaseOnAuthStateChanged = firebaseOnAuthStateChanged;
    window.firebaseOnChildAdded = firebaseOnChildAdded;
    window.firebaseOnChildChanged = firebaseOnChildChanged;
    window.firebaseOnChildRemoved = firebaseOnChildRemoved;
    window.firebaseTransaction = firebaseTransaction;

    window.firestoreGetDocs = firestoreGetDocs;
    window.firestoreCol = firestoreCol;
    window.firestoreDoc = firestoreDoc;
    window.firestoreGetDoc = firestoreGetDoc;
</script>
