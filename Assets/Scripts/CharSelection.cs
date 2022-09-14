using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharSelection : MonoBehaviour
{
    private GameObject[] _charList;
    private int index = 0;
    public GameObject loaderUI;
    public Slider progressSlider;
    [SerializeField] GameObject _lockText;
    [SerializeField] GameObject _startButton;

    private void Start()
    {
        index = PlayerPrefs.GetInt("CharSelected", 0);
        PlayerPrefs.SetInt("Ac0", 1);
        Debug.Log(index);
        _charList = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            _charList[i] = transform.GetChild(i).gameObject;
        }

        foreach (GameObject go in _charList)
        {
            go.SetActive(false);
        }

        if (_charList[index])
        {
            _charList[index].SetActive(true);
        }
    }

    public void ToggleLeft()
    {
        _charList[index].SetActive(false);
        index--;
        if(index < 0)
        {
            index = _charList.Length - 1;
        }

        _charList[index].transform.position = new Vector3(0, 4.5f, -7.1f);
        if (_charList[index] && PlayerPrefs.GetInt("Ac" + index.ToString()) == 1)
        {
            _lockText.SetActive(false);
            _charList[index].SetActive(true);
            _startButton.SetActive(true);
        }

        if (PlayerPrefs.GetInt("Ac" + index.ToString()) == 0 || !PlayerPrefs.HasKey("Ac" + index.ToString()))
        {
            _lockText.SetActive(true);
            _startButton.SetActive(false);
        }
    }

    public void ToggleRight()
    {
        _charList[index].SetActive(false);

        index++;
        Debug.Log("index++");
        if (index > _charList.Length-1)
        {
            index = 0;
        }
        _charList[index].transform.position = new Vector3(0, 4.5f, -7.1f);
        if (_charList[index] && PlayerPrefs.GetInt("Ac" + index.ToString()) == 1)
        {
            _lockText.SetActive(false);
            _charList[index].SetActive(true);
            _startButton.SetActive(true);
        }
        if (PlayerPrefs.GetInt("Ac" + index.ToString()) == 0 || !PlayerPrefs.HasKey("Ac" + index.ToString()))
        {
            _lockText.SetActive(true);
            _startButton.SetActive(false);

        }
    }

    public void NextScene()
    {
        PlayerPrefs.SetInt("CharSelected", index);
        StartCoroutine(LoadScene_coroutine());


        
    }
    public IEnumerator LoadScene_coroutine()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            yield return new WaitForSeconds(0.7f);
        }
        progressSlider.value = 0;
        loaderUI.SetActive(true);

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        asyncOperation.allowSceneActivation = false;
        float progress = 0;
        while (!asyncOperation.isDone)
        {
            progress = Mathf.MoveTowards(progress, asyncOperation.progress, Time.deltaTime);
            progressSlider.value = progress;
            if (progress >= 0.9f)
            {
                progressSlider.value = 1;
                asyncOperation.allowSceneActivation = true;
            }
                yield return null;
        }
    }
}
