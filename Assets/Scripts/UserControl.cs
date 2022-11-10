using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UserControl : MonoBehaviour
{
    private float MAX_HP;
    private float DROP_HP;

    private float _spd = 5.0f;

    private Vector3 _currentPos;
    public Vector3 targetPos;
    private float _currentHp;

    private GameManager gm;
    public float HP
    {
        get => _currentHp;
        set => _currentHp = value;
    }

    private Coroutine _moveCoroutine = null;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(!EventSystem.current.IsPointerOverGameObject())
            {
                Vector3 targetPos;
                targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                targetPos.z = 0f;
                this.targetPos = targetPos;

                if (_moveCoroutine != null)
                    StopCoroutine(_moveCoroutine);
                _moveCoroutine = StartCoroutine(MoveCoroutine());
            }
        }

        
        //SetTargetPos(_targetPos);
    }

    private IEnumerator MoveCoroutine()
    {
        float delta = 0f;
        while(delta <= 1f)
        {
            transform.position += ((targetPos - transform.position) * delta);
            delta += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
    }

    private void SetTargetPos(Vector3 pos)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * _spd);
        gm.SendCommand("#Move#" + targetPos.x + ',' + targetPos.y);   
    }

    public void Revive()
    {
        HP = MAX_HP;
    }

    public void DropHp(int drop)
    {
        _currentHp -= drop;
    }
}
