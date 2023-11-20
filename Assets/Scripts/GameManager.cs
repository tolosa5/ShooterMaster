using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameObject deathPanel, winPanel;

    [SerializeField] AudioClip[] sfx;
    AudioSource aS;
    Vector3 playerStartPosition;

    [HideInInspector] public int timesLoaded;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);

        aS = GetComponent<AudioSource>();
    }


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        deathPanel.SetActive(false);
        winPanel.SetActive(false);
    }

    public void DeathPanelActivate()
    {
        deathPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void DeathPanelDeactivate()
    {
        deathPanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void Win()
    {
        Debug.Log("Win activada");
        //toda l apesca de ganar y tal
        winPanel.SetActive(true);
        Time.timeScale = 0f;
        WinPanel();
    }

    public void WinPanel()
    {
        
    }
}
