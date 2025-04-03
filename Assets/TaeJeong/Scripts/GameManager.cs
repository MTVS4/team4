using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject gameOverUi;
    [SerializeField] private GameObject timerUi;
    [SerializeField] private GameObject creditsUi;
    [SerializeField] private GameObject BGMOptionUi;
    [SerializeField] private GameObject IngameOptionUi;
    public Slider bgmSlider;
    public GameObject optionScreen;
    [HideInInspector] public bool isFinish;
    public bool isPlaying;

    private void Awake()
    {
        isPlaying = true;

    }
    
    void Update()
    {
        if (isFinish == false)
        {
            InGameESC();
        }
    }

    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // 인게임 esc 눌러서 옵션창 띄우기
    public void InGameESC()
    {
        if (player != null)
        {
            bool isOptionOpen = IngameOptionUi.activeInHierarchy;

            if (Input.GetKeyDown(KeyCode.C)) 
            {
                if (!isOptionOpen) 
                {
                    IngameOptionUi.SetActive(true); 
                    GamePause(); // 게임 중지
                }
                else 
                {
                    IngameOptionUi.SetActive(false); 
                    GameResume(); // 게임 재개
                }
            }
        }
    }
    

    void GamePause()
    {
        Time.timeScale = 0;
        // 마우스 커서를 보이게 하고, 잠금 해제
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("Pause");
        hud.SetActive(false);
        isPlaying = false;
    }

    void GameResume()
    {
        Debug.Log("Resume");
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        hud.SetActive(true);
        isPlaying = true;

    }

    public void GameRestart()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void InGameRestart()
    {
        SceneManager.LoadScene("Whiteboxing");
        GameResume();
    }
    
    public void GameFinish()
    {
        gameOverUi.SetActive(true);
        timerUi.SetActive(false);
    }

    public void CreditUI()
    {
        creditsUi.SetActive(true);
    }

    public void BGMOptionUI()
    {
        BGMOptionUi.SetActive(true);
    }

    public void BGMOptionClose()
    {
        BGMOptionUi.SetActive(false);
    }

    public void OptionClose()
    {
        optionScreen.SetActive(false);
    }
}
