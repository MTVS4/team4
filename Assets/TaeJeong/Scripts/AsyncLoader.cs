using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AsyncLoader : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject startScreen;
    
    [SerializeField] private Slider loadingSlider;

    public void LoadLevel(string levelToLoad)
    {
        startScreen.SetActive(false);
        loadingScreen.SetActive(true);

        StartCoroutine(LoadLevelASync(levelToLoad));
    }

    IEnumerator LoadLevelASync(string levelToLoad)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelToLoad);
        loadOperation.allowSceneActivation = false;

        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            loadingSlider.value = progressValue;
            Debug.Log("로딩 진행률: " + progressValue);
            
            if (loadOperation.progress >= 0.9f)
            {
                loadingSlider.value = 1f;
                loadOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

}
