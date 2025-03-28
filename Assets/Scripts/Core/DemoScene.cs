using UnityEngine;

public class DemoScene : MonoBehaviour
{
    const string LOG_TAG = "DemoScene";

    private void Start()
    {
        SmartLog.Info(LOG_TAG, "Demo scene started.");
        GameManager.Instance.Init();
    }
}