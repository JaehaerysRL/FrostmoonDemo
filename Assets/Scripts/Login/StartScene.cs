using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class StartScene : MonoBehaviour
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Slider _progressSlider;

    private AsyncOperationHandle<SceneInstance> _loadSceneOperation;

    private void Start()
    {
        _startButton.onClick.AddListener(OnStartButtonClicked);
        _startButton.gameObject.SetActive(true);
        _progressSlider.gameObject.SetActive(false);
        // TODO core system initialization
    }

    private void OnStartButtonClicked()
    {
        _startButton.gameObject.SetActive(false);
        _progressSlider.gameObject.SetActive(true);
        _loadSceneOperation = Addressables.LoadSceneAsync("DemoScene", LoadSceneMode.Single);
    }

    private void Update()
    {
        if (_loadSceneOperation.IsValid() && !_loadSceneOperation.IsDone)
        {
            _progressSlider.value = _loadSceneOperation.PercentComplete;
        }
    }
}
