using UnityEngine;
using UnityEngine.SceneManagement;

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

    // TODO: Refator into a struct/data class
    int funds = 0;
    public void AddFunds(int funds)
    {
        this.funds = funds;
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
            DontDestroyOnLoad(this);
            Debug.Log("GameManager set through Awake");
        }
        else if (GameManager.instance != this)
        {
            Debug.Log("Replacing old GameManager with the scene GameManager.");
            Destroy(GameManager.instance.gameObject);
            instance = this;
            DontDestroyOnLoad(this);
        }
        else 
        {
            Debug.Log("GameManager already initialized.");
        }
    }

    public void LoadScenebyName(string name)
    {
        SceneManager.LoadScene(name);
    }
}
