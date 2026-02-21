using UnityEngine;
using UnityEngine.SceneManagement;
using Character;

public class GameManager : MonoBehaviour
{
    // static == class level, aka GameManager.Instance
    static private GameManager instance = null;

    // We want to make GameManager Instance READ ONLY
    static public GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("GameManager");
                instance = go.AddComponent<GameManager>();
                DontDestroyOnLoad(go);
                Debug.Log("Create a new GameManager.");
            }
            return instance;
        }
        // Do not make a set! READ ONLY!
    }
    public int funds = 0;
    private Vector3 savedPlayerPosition;
    private bool hasSavedPlayerPosition = false;
    private bool restorePlayerPositionOnNextLoad = false;
    private int savedDayCount = 1;
    private bool isSceneProxy = false;
    
    public void AddFunds(int amnt)
        {
            this.funds += amnt;
        }
    public int getFunds()
        {
            return this.funds;
        }
    void Awake()
    {
        if (GameManager.instance == null)
        {
            // Keep this as close to the top of Awake as possible to avoid
            // multiple instancing
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameManager set through Awake");
        }
        else if (GameManager.instance != this)
        {
            isSceneProxy = true;
            Debug.Log("Scene GameManager acting as proxy to persistent instance.");
        }
        else 
        {
            Debug.Log("GameManager already initialized.");
        }
    }

    void OnEnable()
    {
        if (isSceneProxy)
        {
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        if (isSceneProxy)
        {
            return;
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!restorePlayerPositionOnNextLoad || !hasSavedPlayerPosition)
        {
            return;
        }

        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player == null)
        {
            player = FindAnyObjectByType<PlayerController>();
        }

        if (player != null)
        {
            player.transform.position = savedPlayerPosition;
        }

        restorePlayerPositionOnNextLoad = false;
    }

    public void SavePlayerPosition(Vector3 position)
    {
        if (isSceneProxy && instance != null && instance != this)
        {
            instance.SavePlayerPosition(position);
            return;
        }

        savedPlayerPosition = position;
        hasSavedPlayerPosition = true;
    }

    public void RestoreSavedPlayerPositionOnNextSceneLoad()
    {
        if (isSceneProxy && instance != null && instance != this)
        {
            instance.RestoreSavedPlayerPositionOnNextSceneLoad();
            return;
        }

        restorePlayerPositionOnNextLoad = true;
    }

    public void LoadScenebyName(string name)
    {
        if (isSceneProxy && instance != null && instance != this)
        {
            instance.LoadScenebyName(name);
            return;
        }

        SceneManager.LoadScene(name);
    }

    public void ExitStoreToScene(string returnSceneName)
    {
        if (isSceneProxy && instance != null && instance != this)
        {
            instance.ExitStoreToScene(returnSceneName);
            return;
        }

        if (string.IsNullOrWhiteSpace(returnSceneName))
        {
            Debug.LogError("GameManager.ExitStoreToScene called with empty scene name.");
            return;
        }

        RestoreSavedPlayerPositionOnNextSceneLoad();
        LoadScenebyName(returnSceneName);
    }

    public void SaveDayCount(int day)
    {
        if (isSceneProxy && instance != null && instance != this)
        {
            instance.SaveDayCount(day);
            return;
        }

        savedDayCount = Mathf.Max(1, day);
    }

    public int GetSavedDayCount()
    {
        if (isSceneProxy && instance != null && instance != this)
        {
            return instance.GetSavedDayCount();
        }

        return savedDayCount;
    }
}
