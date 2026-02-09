using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public static Timer instance;
    public int CurrentTime;

    public TextMeshProUGUI timerText;
    enum TimerType
    {
        Timer,
        CountDown
    }
    [SerializeField] TimerType timerType;
    float elapsedTime;
    [SerializeField] float remainingTime;
    bool isTimerRunning = true;

    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTimerRunning)
            if (timerType == TimerType.CountDown)
            {
                UpdateCountDown();
            }
            else
            {
                UpdateTimer();
            }
    }

    void UpdateTimer()
    {
        elapsedTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        CurrentTime = Mathf.FloorToInt(elapsedTime);
    }

    void UpdateCountDown()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
        }
        else if (remainingTime < 0)
        {
            remainingTime = 0;
            timerText.color = Color.red;
        }
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }
}
