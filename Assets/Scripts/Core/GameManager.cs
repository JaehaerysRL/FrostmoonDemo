using UnityEngine;

public class GameManager : MonoBehaviour
{
    const string LOG_TAG = "GameManager";
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Init()
    {
        SmartLog.Info(LOG_TAG, "GameManager initialized.");
    }
}