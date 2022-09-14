using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class Goal : MonoBehaviour
{
    private GameManager _gm;
    [SerializeField] private GameObject _finishtext;
    private MoralisHandling _mh;
    private int _lv1Rewardclaimed;
    private int _lv2Rewardclaimed;
    private int _lv3Rewardclaimed;


    private void Start()
    {
        _gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _mh = GameObject.FindGameObjectWithTag("GameManager").GetComponent<MoralisHandling>();
        _lv1Rewardclaimed = PlayerPrefs.GetInt("Lv1Rewardclaimed", 0);
        _lv2Rewardclaimed = PlayerPrefs.GetInt("Lv2Rewardclaimed", 0);
        _lv3Rewardclaimed = PlayerPrefs.GetInt("Lv3Rewardclaimed", 0);

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _gm._remind <= 3)
        {
            _gm.LevelUp();
            _finishtext.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            if (PlayerPrefs.GetInt("Level", 1) == 1 && _lv1Rewardclaimed == 0)
            {
                _mh.ClaimRewardAsync();
                PlayerPrefs.SetInt("Lv1Rewardclaimed", 1);
            }
            else if (PlayerPrefs.GetInt("Level", 1) == 2 && _lv2Rewardclaimed == 0)
            {
                _mh.ClaimRewardAsync();
                PlayerPrefs.SetInt("Lv2Rewardclaimed", 1);
            }
            else if (PlayerPrefs.GetInt("Level", 1)==3 && _lv3Rewardclaimed == 0)
            {
                _mh.ClaimRewardAsync();
                PlayerPrefs.SetInt("Lv3Rewardclaimed", 1);
            }
        }
    }
}
