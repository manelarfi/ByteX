using UnityEngine;

// Base class for managing a static instance of a MonoBehaviour
public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour {
    public static T Instance { get; private set; }

    protected virtual void Awake() {
        // If an instance already exists, destroy the new object
        if (Instance != null) {
            Debug.LogWarning($"[StaticInstance] An instance of {typeof(T).Name} already exists. Destroying the new instance.");
            Destroy(gameObject);
            return;
        }

        Instance = this as T; // Assign this instance
    }

    protected virtual void OnApplicationQuit() {
        Instance = null;
        Destroy(gameObject);
    }
}

// Singleton class to ensure only one instance of T exists
public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour {
    protected override void Awake() {
        // Check and destroy duplicates in the base class
        base.Awake();
    }
}

// Persistent singleton class for objects that persist across scenes
public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour {
    protected override void Awake() {
        // Check and destroy duplicates in the base class
        base.Awake();

        // Ensure this object is not destroyed when loading new scenes
        if (Instance == this) {
            DontDestroyOnLoad(gameObject);
        }
    }
}
