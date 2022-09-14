using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class EnemyNavMesh : MonoBehaviour
{
    private NavMeshAgent _navAgent;
    private Animator _ani;
    private PlayerDetec _playerDe;
    [SerializeField] private Transform _movePos;
    private GameManager _gm;
    private bool _isBlocked = false;
    void Start()
    {
        _navAgent = GetComponent<NavMeshAgent>();
        _ani = GetComponent<Animator>();
        _gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        int index = PlayerPrefs.GetInt("CharSelected", 0);
        _movePos = GameObject.Find("PlayerCameraRoot" + index).transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (_navAgent.enabled == true)
        {
            _navAgent.destination = _movePos.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Environment"))
        {
            //Debug.Log("touch map");
            _playerDe = other.gameObject.GetComponent<PlayerDetec>();
            if (_playerDe._isHere == true)
            {
                _navAgent.enabled = false;
                _ani.SetBool("Attrack", true);
                //Debug.Log("Attack true");
                if (_isBlocked == false)
                {
                //_gm.ReduceLife();
                _isBlocked = true;
                StartCoroutine(UnBlocked());
                }
            }
            
        }

    }

    private IEnumerator UnBlocked()
    {
        yield return new WaitForSeconds(2f);
        _isBlocked = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Environment"))
        {
            _playerDe = other.gameObject.GetComponent<PlayerDetec>();
            if (_playerDe._isHere == true)
            {
                _navAgent.enabled = false;
                _ani.SetBool("Attrack", true);
                //Debug.Log("Attack true");
                if (_isBlocked == false)
                {
                    //_gm.ReduceLife();
                    _isBlocked = true;
                    StartCoroutine(UnBlocked());
                }
            }
            else if (_playerDe._isHere == false)
            {
                _navAgent.enabled = true;
                _ani.SetBool("Attrack", false);
                //Debug.Log("Attack false");

            }
        }
    }
}
