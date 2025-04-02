using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private PlayerControllerRb player;
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject gameOverUi;
    [SerializeField] private GameObject timerUi;
    [SerializeField] private GameObject creditsUi;
    [SerializeField] private GameObject BGMOptionUi;
    public Slider bgmSlider;
    public GameObject optionScreen;
    [HideInInspector] public bool isFinish;

    private void Awake()
    {
        //if (instance == null)
        //{
            //instance = this;
            //DontDestroyOnLoad(gameObject);
           
        /*}
        else
        {
            Destroy(gameObject);
        }*/
        
    }
    
    void Update()
    {
        if (isFinish == false)
        {
            ExitPopUp();
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

    public void PopUpOption()
    {
        optionScreen.SetActive(true);
    }
    
    void ExitPopUp()
    {
        if (optionScreen.activeInHierarchy && player == null )
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                optionScreen.SetActive(false);
                Debug.Log("Escape");
            }
        }
    }

    void InGameESC()
    {
        if (player != null)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (optionScreen.activeInHierarchy)
                {
                    {
                        GameResume();
                        optionScreen.SetActive(false);
                    }
                }
                else
                {
                    GamePause();
                    optionScreen.SetActive(true);
                }
            }
        }
        else
        {
            return;
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
    }

    void GameResume()
    {
        Debug.Log("Resume");
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        hud.SetActive(true);

    }

    public void GameRestart()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void InGameRestart()
    {
        SceneManager.LoadScene("Whiteboxing");
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
