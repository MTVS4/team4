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
        // 자동 활성화를 막아 로딩 진행률을 명확히 확인할 수 있게 합니다.
        loadOperation.allowSceneActivation = false;

        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            loadingSlider.value = progressValue;
            Debug.Log("로딩 진행률: " + progressValue);

            // 로딩이 0.9에 도달하면, 씬 전환을 진행할 준비가 된 것입니다.
            if (loadOperation.progress >= 0.9f)
            {
                Debug.Log("로딩 완료! 이제 씬 전환 가능");
                loadingSlider.value = 1f;
                // 원하는 추가 처리 후(예: 로딩 애니메이션, 버튼 클릭 등) allowSceneActivation을 true로 전환
                loadOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

}
