using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuSetting;
    [SerializeField] GameObject TimerObj;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] GameObject TimerTextObj;
    [SerializeField] GameObject NewPB;

    public GameObject player;
    public playerController playerScript;

    public bool isPaused;
    public bool isTimer;
    float timeScaleOrig;

    public List<WeaponStats> weaponsList = new List<WeaponStats>();
    public LevelStats levelStats;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        timeScaleOrig = Time.timeScale;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        for (int i = 0; i < weaponsList.Count; i++)
        {
            weaponsList[i].spellCheck = false;
        }

        //levelStats.isTimed1 = false;
        //levelStats.isTimed2 = false;
        //levelStats.isTimed3 = false;
        //levelStats.isTimed4 = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                StatePause();
                menuActive = menuPause;
                menuActive.SetActive(isPaused);
            }
            else if (menuActive == menuPause || menuActive == menuSetting)
                StateUnpause();
        }
    }

    public void StatePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        playerController.instance.enabled = false;
    }

    public void StateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
        playerController.instance.enabled = true;
    }

    public void Settings()
    {
        if (menuActive == menuPause)
        {
            menuActive.SetActive(false);
            menuActive = null;
            menuActive = menuSetting;
            menuActive.SetActive(true);
        }
    }

    public void Back()
    {
        if (menuActive == menuSetting)
        {
            menuActive.SetActive(false);
            menuActive = null;
            menuActive = menuPause;
            menuActive.SetActive(true);
        }
    }

    public void TimerCheck()
    {
        isTimer = !isTimer;
        TimerObj.SetActive(isTimer);
    }

    public void StateWin()
    {
        menuActive = menuWin;
        menuActive.SetActive(true);
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        int PB = playerController.instance.playerInfo.PBTime;

        if (PB == 0)
        {
            playerController.instance.playerInfo.PBTime = Timer.instance.CurrentTime;
            timerText.text = Timer.instance.timerText.text;
            NewPB.SetActive(true);
        }
        else if (Timer.instance.CurrentTime < PB)
        {
            playerController.instance.playerInfo.PBTime = Timer.instance.CurrentTime;
            timerText.text = Timer.instance.timerText.text;
            NewPB.SetActive(true);
        }
        else if (Timer.instance.CurrentTime > PB)
        {
            int minutes = Mathf.FloorToInt(PB / 60f);
            int seconds = Mathf.FloorToInt(PB % 60f);
            timerText.text = string.Format("PB {0:00}:{1:00}", minutes, seconds);
            NewPB.SetActive(false);
            TimerTextObj.transform.position 
                = new Vector3(TimerTextObj.transform.position.x - 70,
                TimerTextObj.transform.position.y, 
                TimerTextObj.transform.position.z);
        }
    }
}
