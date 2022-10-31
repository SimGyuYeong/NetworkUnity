using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField]
    private MonsterDataSO _monsterDataSO;

    public float speed;
    public int power;
    public int hp;
    public int mp;

    private void Start()
    {
        speed = _monsterDataSO.speed;
        power = _monsterDataSO.power;
        hp = _monsterDataSO.hp;
        mp = _monsterDataSO.mp;
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
