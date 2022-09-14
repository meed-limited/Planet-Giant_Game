using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AttactFX : MonoBehaviour
{
    [SerializeField] private GameObject _fx;
    [SerializeField] private Animator _ani;
    [SerializeField] private GameObject _enir;
    [SerializeField] private AudioClip[] _sfx;
    private AudioSource _audio;
    GameManager _gm;
    bool _isblock = false;


    private void Start()
    {
        _gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _audio = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Ground") && _ani.GetBool("Attrack") == true && _isblock == false)
        {
            Debug.Log("Attack!");
            Instantiate(_fx, transform.position, Quaternion.identity);
            _gm.ReduceLife();
            _audio.clip = _sfx[1];
            _audio.Play();
            _isblock = true;
            StartCoroutine(UnBlock());
            _enir.transform.DOShakePosition(0.2f, 1f, 9, 90).OnComplete(() =>
            {
                _enir.transform.DORewind(true);
            });

        }
        else if (other.gameObject.CompareTag("Ground"))
        {
            _audio.clip = _sfx[0];
            _audio.Play();
        }
    }

    IEnumerator UnBlock()
    {
        yield return new WaitForSeconds(0.5f);
        _isblock = false;
    }
}
