using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject _charactor;
    public int _level = 1;
    public int _health = 5;
    [SerializeField] GameObject[] _healthIcon;
    private int _spawnNum;
    public int _remind;
    private float _spawnX;
    private float _spawnZ;
    private Animator _playerAni;
    public StarterAssetsIn _input;
    private InputAction _open;
    [SerializeField] private AchievementManager _am;


    [SerializeField] TMPro.TextMeshProUGUI _remindText;
    [SerializeField] TMPro.TextMeshProUGUI _levelText;
    [SerializeField] GameObject _deadText;
    [SerializeField] GameObject QuitMenu;
    private void Awake()
    {
        _input = new StarterAssetsIn();

    }
    private void OnEnable()
    {
        _open = _input.UI.Esc;
        _open.Enable();
        _open.performed += OpenUI;


    }


    private void OnDisable()
    {
        _open.Disable();
    }

    private void Start()
    {
        _am = GameObject.FindGameObjectWithTag("AcMan").GetComponent<AchievementManager>();
        Cursor.lockState = CursorLockMode.None;
        _playerAni = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        _level = PlayerPrefs.GetInt("Level", 1);
        _spawnNum = _level * 5;
        _remind = _spawnNum;
        SpawnChar();
        _remindText.text = "Rescue: " + _remind.ToString();
 
        _levelText.text = "Level: " + _level.ToString();
    }


    private void OpenUI(InputAction.CallbackContext obj)
    {
        
        QuitMenu.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
    }


    public void GameResume()
    {
        QuitMenu.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void SpawnChar()
    {
        for (int i = 0; i<_spawnNum; i++)
        {
            _spawnX = UnityEngine.Random.Range(-107, 24);
  
            _spawnZ = UnityEngine.Random.Range(1, 128);
            var randomRotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
            Instantiate(_charactor, new Vector3(_spawnX, 0.70f, _spawnZ), randomRotation);
        }
    }

    
    public void ReduceLife()
    {
        if (_health > 0)
        {

            _health -= 1;
            UpdateLifeIcon();
            if (_health ==0)
            {
                UpdateLifeIcon();
                _deadText.SetActive(true);
                Dead();
            }
        }
    }
    
    private void UpdateLifeIcon()
    {
        _healthIcon[_health].SetActive(false);
    }
    public void ReduceCount()
    {
        _remind -= 1;
        _remindText.text = "Rescue: " + _remind.ToString();
    }
    public void LevelUp()
    {
        _level++;
        PlayerPrefs.SetInt("Level", _level);
        _am.AddAchievementProgress(2, 1);
    }

    public void Dead()
    {
        _playerAni.SetTrigger("Dead");
        GetComponent<AudioSource>().Play();
        Cursor.lockState = CursorLockMode.None;
    }
}
