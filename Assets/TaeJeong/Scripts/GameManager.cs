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
    public float time;
    public Slider bgmSlider;
    private AudioSource audioSource;
    public GameObject optionScreen;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    void Start()
    {
        
    }

    void Update()
    {
        time += Time.deltaTime;
        ExitPopUp();
        InGameESC();
    }
    
    public void SoundControl()
    {
        audioSource.volume = bgmSlider.value;
    }
    
    public void StartScene()
    {
        SceneManager.LoadScene("GameScene");
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
}
