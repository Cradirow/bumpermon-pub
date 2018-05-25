﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArm : MonoBehaviour
{

    [Header("SYSTEM")]
    public float changedPosition;
    public GameObject player;
    public float speed;
    private float movingTime;
    public int randomSeed;
    public GameObject child;
    public GameObject EnemyController;

    public float xOffset;
    public float zOffset;

    [Header("Action")]
    public GameObject mine;
    public GameObject life;
    public GameObject coin;
    public GameObject nitro;
    public GameObject shield;
    public GameObject item;
    public bool posRecoverEnd = true;

    private Animator myAnimator;

    private IEnumerator moveCoroutine;
    // Use this for initialization
    void Start()
    {
        myAnimator = child.GetComponent<Animator>();
        Random.InitState(randomSeed);
        moveCoroutine = MoveArmTemp();
        StartCoroutine(moveCoroutine);
    }

    // Update is called once per frame
    void Update()
    {
        speed = player.GetComponent<PlayerController>().speed;
        transform.Translate(new Vector3(0.1f, 0, 0) * speed * Time.deltaTime);
        transform.Translate(new Vector3(changedPosition, 0, 0) * Time.deltaTime);
    }

    public void PlaceMine(Vector3 minePosition)
    {
        StopCoroutine(moveCoroutine);
        changedPosition = 0.0f;
        int itemType;
        itemType = Random.Range(1, 100);
        if (itemType < 20)
        {
            //코인
            itemType = 3;
        }
        else if (20 < itemType && itemType < 40)
        {
            //마인
            itemType = 1;
        }
        else if (40 < itemType && itemType < 60)
        {
            //니트로
            itemType = 4;
        }
        else if (60 < itemType && itemType < 80)
        {
            //하트
            itemType = 2;
        }
        else
        {
            //쉴드
            itemType = 5;
        }
        StartCoroutine(Mine(minePosition, itemType));
    }

    public IEnumerator MoveArmTemp()
    {
        while (true)
        {
            movingTime = Random.Range(1, 3);
            changedPosition = 5.0f;
            yield return new WaitForSeconds(movingTime);
            changedPosition = -5.0f;
            yield return new WaitForSeconds(movingTime);
            changedPosition = 0.0f;
        }
    }

    public IEnumerator Mine(Vector3 minePosition, int type)
    {
        switch (type)
        {
            case 1:
                item = mine;
                break;
            case 2:
                item = life;
                break;
            case 3:
                item = coin;
                break;
            case 4:
                item = nitro;
                break;
            case 5:
                item = shield;
                break;
        }

        Vector3 armPosition = new Vector3(minePosition.x, transform.position.y, transform.position.z);
        //마인 설치 지점까지 이동(오차 1)
        while (Vector3.Distance(transform.position, armPosition) >= 1.0f)
        {
            changedPosition = 20.0f;
            yield return null;
        }
        //공격 동안 멈추기(3초간)
        changedPosition = -0.1f * speed;
        //이동 후 애니메이션 결정
        if (minePosition.z == 0)
        {
            myAnimator.SetTrigger("Lane2");
        }
        else
        {
            myAnimator.SetTrigger("Lane1");
        }
        //3초 카운트 (애니메이션 이벤트로 대체 고려)
        float timeCount = 0;
        bool endCountAnimationTime = false;
        while (!endCountAnimationTime)
        {
            timeCount += Time.deltaTime;
            if (timeCount >= 3)
            {
                endCountAnimationTime = true;
                break;
            }
            yield return null;
        }
        //마인 생성
        GameObject newMine;
        newMine = Instantiate(item, minePosition, new Quaternion(0, 0, 0, 0));
        newMine.SetActive(true);
        //위치 회복
        changedPosition = 0.0f;
        posRecoverEnd = false;
        if (!posRecoverEnd)
        {
            StartCoroutine(ArmSpeedRecovery());
        }
        while (!posRecoverEnd)
        {
            yield return null;
        }
        //회복 완료했으면 정상화
        EnemyController.GetComponent<EnemyController>().lArmJustMove = true;
        EnemyController.GetComponent<EnemyController>().rArmJustMove = true;
        StartCoroutine(moveCoroutine);
    }

    public IEnumerator ArmSpeedRecovery()
    {
        xOffset = 10.0f;
        xOffset += Random.Range(0, speed / 10);
        while (Vector3.Distance(transform.position, new Vector3(player.transform.position.x + xOffset, transform.position.y, transform.position.z)) > 1.0f)
        {
            if (transform.position.x > player.transform.position.x + xOffset)
            {
                //changedPosition = -15.0f - speed / 100;
                changedPosition = -0.1f * speed - 20.0f;
            }
            else
            {
                //changedPosition = 15.0f + speed / 100;
                changedPosition = 0.1f * speed + 20.0f;
            }
            yield return null;
        }
        changedPosition = 0.0f;
        posRecoverEnd = true;
    }

    public void Idle(){
        myAnimator.Play("Idle");
    }

    public void Laugh(){
        myAnimator.Play("Laugh");
    }

    public void Surprise(){
        myAnimator.Play("Surprise");
    }
}
