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
    private Vector3 _targetPos;
    private float _currentHp;
    public float HP
    {
        get => _currentHp;
        set => _currentHp = value;
    }

    private Coroutine _moveCoroutine = null;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(!EventSystem.current.IsPointerOverGameObject())
            {
                Vector3 targetPos;
                targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                targetPos.z = 0f;
                _targetPos = targetPos;

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
            transform.position += ((_targetPos - transform.position) * delta);
            delta += Time.deltaTime;
            yield return null;
        }
        transform.position = _targetPos;
    }

    private void SetTargetPos(Vector3 pos)
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPos, Time.deltaTime * _spd);
    }

    private void Revive()
    {
        HP = MAX_HP;
    }
}
