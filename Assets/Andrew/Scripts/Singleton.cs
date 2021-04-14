using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Component
{
	#region Variables
	public static T Instance { get; private set; }
	#endregion

	#region Unity Event Functions
	protected virtual void Awake()
	{
		if (Instance == null)
		{
			Instance = this as T;
			//DontDestroyOnLoad(this);
		}
		else
		{
			//Destroy(gameObject);
		}
	}
	#endregion
}

// Warning: There is a problem when using OnLevelWasLoaded() and DontDestroyOnLoad() together
// Reloading a scene doesn't reset the object that is set to don't destroy on load.
