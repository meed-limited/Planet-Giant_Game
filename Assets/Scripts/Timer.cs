using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{

    public float _timeRemaining = 300;
    private bool _timerIsRunning = false;
    [SerializeField]
    private TextMeshProUGUI _timeText;
    [SerializeField]
    private GameObject _end;
    private GameManager _gm;
    public int _currentSelected;
    public float _totalspeedboosted;
    public float _totaljumpboosted;
    public int _totaltimeboosted;

    private void Start()
    {
        _gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
    public void CountDown()
    {
        // Starts the timer automatically
        _timerIsRunning = true;

    }
    void Update()
    {
        if (_timerIsRunning)
        {
            if (_timeRemaining > 0)
            {
                _timeRemaining -= Time.deltaTime;
                DisplayTime(_timeRemaining);
            }
            else
            {
                //_fire.SetActive(false);
                _timeRemaining = 0;
                _timerIsRunning = false;
                _gm.Dead();
                _end.SetActive(true);

            }
        }
    }
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        _timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}