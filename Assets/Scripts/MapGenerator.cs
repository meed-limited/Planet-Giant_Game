using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] GameObject[] _maps;
    [SerializeField] Transform[] _pos;
    [SerializeField] List<int> list = new List<int>();

    private void Start()
    {
        SpawnMap();
    }
    public void SpawnMap()
    {
        while (list.Count != 9)
        {
            int x = Random.Range(0, _maps.Length);
            if (!list.Contains(x))
            {
                var maps = Instantiate(_maps[x], _pos[list.Count].position, Quaternion.identity);
                maps.transform.parent = gameObject.transform;
                list.Add(x);
            }
        }
    }


}
