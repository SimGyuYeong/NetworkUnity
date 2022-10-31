using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Monster/MonsterData")]
public class MonsterDataSO : ScriptableObject
{
    public float speed;
    public int power;
    public int hp;
    public int mp;
}
