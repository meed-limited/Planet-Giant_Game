using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicControl : MonoBehaviour
{
    [SerializeField] private AudioClip[] _musicList;
    private AudioSource _music;
    // Start is called before the first frame update
    void Start()
    {
        _music = GetComponent<AudioSource>();   
    }


    private void OnLevelWasLoaded()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2 || SceneManager.GetActiveScene().buildIndex == 3)
        {
            ChangeMusic();
        }
    }
    private void ChangeMusic()
    {
        _music.clip = _musicList[Random.Range(0, _musicList.Length)];
        _music.Play();
    }
}
