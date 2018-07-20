﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    [Header("InGameUI")]
    public Text countdownText;

    public GameObject inGamePanel;
    public GameObject checkPanel;

    public GameObject homeBtn;
    public GameObject coinUI;
    public GameObject gemUI;
    public GameObject scoreUI;
    //public GameObject lives;

    public Sprite livingheart;
    public Sprite deadHeart;


    [Header("OutGameUI")]
    public Text introduceText;
    public Text titleText;
    public Text recordText;
    public Text totalCoins;
    public Text totalGems; 

    public GameObject outGamePanel;
    public GameObject optionPanel;
    public GameObject characterPanel;
    public GameObject shopPanel;
    public GameObject btmPanel;

    public GameObject startBtn;
    public GameObject optionBtn;
    public GameObject musicBtn;
    public GameObject devBtn;
    public GameObject quitBtn;

    public Sprite optionDown;
    public Sprite optionUp;

    bool isDown;
    bool isMusicOn;
    bool isDev;
    bool isGameStart;


    [Header("GameOverUI")]
    public GameObject gameOverPanel;
    public GameObject boxPanel;
    public GameObject highScore;
    public Text coinText;
    public Button continueBtn;
    public Button rankingBtn;
    public Button homeBtn2;
    public GameObject skullFX;
    bool isContinueAvailable = true;
    public Text boxText;
    public Text timeText;
    

    [Header("System")]
    public GameObject blockController;
    public SpawnGrounds spawnGrounds;
    public GameObject player;
    //public GameObject enemy;
    public Camera camera;
    float meterBetBlock;
    int timeLeft = 3;
    public EnemyArm leftEnemyArm;
    public EnemyArm rightEnemyArm;
    public GameObject SoundManager;
    public GameObject lArm;
    public GameObject rArm;
    public int brokenBoxes;

    [Header("Animation")]
    Animator myCameraAnimator;
    Animator myMenuAnimator;
    Animator myGameOverAnimator;
    Animator myCharacterAnimator;
    Animator myShopAnimator;
    Animator myBoxAnimator;

    [Header("DB")]
    public int coin;
    public int gem;
    int prevCoins;
    int prevBoxes;
    float playTime;

    // Use this for initialization
    void Start()
    {
        //SoundManager = GameObject.FindGameObjectWithTag("SoundManager");
        isDown = false;
        isMusicOn = true;
        isGameStart = false;

        StartCoroutine(SoundManager.GetComponent<SoundManager>().BeginBGM());

        myCameraAnimator = camera.GetComponent<Animator>();
        myMenuAnimator = optionPanel.GetComponent<Animator>();
        myGameOverAnimator = gameOverPanel.GetComponent<Animator>();
        myCharacterAnimator = characterPanel.GetComponent<Animator>();
        myShopAnimator = shopPanel.GetComponent<Animator>();
        myBoxAnimator = boxPanel.GetComponent<Animator>();

        LoadRecord();
        //LoadSample();
    }

    void LoadRecord(){
        int score = 0;
        int loadCoin = 0;
        int loadBox = 0;
        int loadGem = 0;

        if(PlayerPrefs.HasKey("Score")){
            score = PlayerPrefs.GetInt("Score");
            loadCoin = PlayerPrefs.GetInt("Coin");
            loadBox = PlayerPrefs.GetInt("Box");
            loadGem = PlayerPrefs.GetInt("Gem");
        }
        else{
            PlayerPrefs.SetInt("Score", 0);
            PlayerPrefs.SetInt("Coin", 0);
            PlayerPrefs.SetInt("Box", 0);
            PlayerPrefs.SetInt("Gem", 0);
            //load tutorial
            SceneManager.LoadScene(1);
        }
        recordText.text = ("HIGH SCORE : " + score);
        totalCoins.text = ("" + loadCoin);
        totalGems.text = ("" + loadGem);
    }

    //void LoadSample(){
        
    //}

    void LateUpdate()
    {
        if(isDown) titleText.enabled = false;
        else titleText.enabled = true;
    }

    IEnumerator GameStart(){
        //player의 능력치를 가져옴
        player.GetComponent<Character>().SetStatus();
        spawnGrounds.playerController = player.GetComponent<PlayerController>();
        for (int i = 3; i > 0; i--){
            countdownText.text = (i + "");
            if (i == 1)
            {
                player.SetActive(true);
            }
            yield return new WaitForSeconds(1.0f);
        }
        countdownText.text = "GO!";
        yield return new WaitForSeconds(1.0f);
        countdownText.text = "";
        //enemy.SetActive(true);
        meterBetBlock = player.transform.position.x;
        myCameraAnimator.enabled = false;
        brokenBoxes = 0;
    }

    public void BtnOnStart()
    {
        isGameStart = true;
        isDown = false;
        isDev = false;
        myMenuAnimator.SetBool("Dev", false);
        myMenuAnimator.SetBool("Down", false);

        startBtn.SetActive(false);
        recordText.enabled = false;
        btmPanel.SetActive(false);
        devBtn.SetActive(false);
        quitBtn.SetActive(false);
        homeBtn.SetActive(true);
        titleText.gameObject.SetActive(false);
        introduceText.enabled = false;

        coinUI.SetActive(false);
        gemUI.SetActive(false);
        //lives.SetActive(true);

        checkPanel.SetActive(true);
        inGamePanel.SetActive(true);
        blockController.SetActive(true);
        myCameraAnimator.Play("StartMoving");
        StartCoroutine(GameStart());
        //팔 로비 애니메이션으로 대체할것
        //lArm.GetComponent<EnemyArm>().StopArm();
        //rArm.GetComponent<EnemyArm>().StopArm();
        lArm.GetComponent<EnemyArm>().player = player;
        rArm.GetComponent<EnemyArm>().player = player;
        SoundManager.GetComponent<SoundManager>().isLobbyEnd = true;
    }

    public void BtnOnOption()
    {
        if (isDown) //메뉴 내려와 있을 때 누르면
        {
            if (isGameStart)
            {
                Time.timeScale = 1;
            }
            scoreUI.SetActive(true);
            myMenuAnimator.SetBool("Down", false);
            myMenuAnimator.SetBool("Dev", false);
            myMenuAnimator.SetBool("Restart", false);
            optionBtn.GetComponent<Image>().sprite = optionDown;
            isDown = false;
            isDev = false;
        }
        else
        {
            if (isGameStart)
            {
                Time.timeScale = 0;
            }
            myMenuAnimator.SetBool("Down", true);
            optionBtn.GetComponent<Image>().sprite = optionUp;
            isDown = true;
        }

    }

    public void BtnOnMusic()
    {
        if (isMusicOn)
        {
            //sound off
            SoundManager.GetComponent<SoundManager>().Mute(true);
            musicBtn.GetComponent<Image>().color = new Color32(200, 200, 200, 255);
            isMusicOn = false;
        }
        else
        {
            //sound on
            SoundManager.GetComponent<SoundManager>().Mute(false);
            musicBtn.GetComponent<Image>().color = new Color32(255, 255, 0, 255);
            isMusicOn = true;
        }
    }

    public void BtnOnDeveloper()
    {
        if (isDev)
        {
            myMenuAnimator.SetBool("Dev", false);
            isDev = false;
        }
        else
        {
            myMenuAnimator.SetBool("Dev", true);
            isDev = true;
        }
    }
    
    public void BtnOnHome()
    {
        scoreUI.SetActive(false);
        myMenuAnimator.SetBool("Restart", true);
    }

    public void BtnOnYes()
    {
        isDown = false;
        isDev = false;
        Time.timeScale = 1;
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        SoundManager.GetComponent<SoundManager>().StopBGM();
        StartCoroutine(SoundManager.GetComponent<SoundManager>().BeginBGM());
    }
    
    public void BtnOnNo()
    {
        scoreUI.SetActive(true);
        myMenuAnimator.SetBool("Restart", false);
    }

    public void BtnOnContinue()
    {
        isContinueAvailable = false;
        myBoxAnimator.SetBool("GameOver", false);
        myGameOverAnimator.SetBool("GameOver", false);
        SoundManager.GetComponent<SoundManager>().StopBGM();
        StartCoroutine(SoundManager.GetComponent<SoundManager>().Continue());
        optionBtn.GetComponent<Button>().interactable = true;
        skullFX.SetActive(false);

        //광고시스템 추가

        leftEnemyArm.Idle();
        rightEnemyArm.Idle();
        StartCoroutine(GameStart());

        //주변 블럭 날리기
        StartCoroutine(player.GetComponent<PlayerController>().NitroShockwave(true));
        //player.GetComponent<PlayerController>().preSpeed = player.GetComponent<PlayerController>().minAutoSpeed;
        //player.GetComponent<PlayerController>().preSpeed = player.GetComponent<PlayerController>().minSpeed;

        //player.GetComponent<Animator>().Play("Idle");
        //player.GetComponent<PlayerController>().live = 1;
        //player.GetComponent<PlayerController>().checkDead = false;
        //player.GetComponent<PlayerController>().speed += player.GetComponent<PlayerController>().minSpeed;
        //player.GetComponent<PlayerController>().damagedSpeed = 1.0f;
        player.GetComponent<PlayerController>().Restart();
        LiveUp();
    }

    public void BtnOnRanking()
    {
        //랭킹시스템 추가
    }

    public void LiveUp()
    {
        //int heartNum = player.GetComponent<PlayerController>().live;
        //string temp = "Live" + heartNum.ToString();
        //GameObject heart = GameObject.Find(temp);
        //heart.GetComponent<Image>().sprite = livingheart;
    }

    public void LiveDown()
    {
        //int heartNum = player.GetComponent<PlayerController>().live;
        //string temp = "Live" + (heartNum + 1).ToString();
        //GameObject heart = GameObject.Find(temp);
        //heart.GetComponent<Image>().sprite = deadHeart;
    }

    public void GameOver()
    {
        if(!isContinueAvailable){
            continueBtn.GetComponent<Image>().sprite = deadHeart;
            continueBtn.interactable = false;
        }
        optionBtn.GetComponent<Button>().interactable = false;
        //int curScore = (int)player.transform.position.x + 20;
        int curScore = scoreUI.GetComponent<Score>().score;
        int prevScore = PlayerPrefs.GetInt("Score");

        int prevGem = PlayerPrefs.GetInt("Gem");
        int curGem = prevGem + gem;
        PlayerPrefs.SetInt("Gem", curGem);

        coinText.text = ("" + coin);
        boxText.text = ("" + (brokenBoxes + prevBoxes));
        playTime = Time.time;
        int min = (int)playTime / 60;
        int sec = (int)playTime - min * 60;
        float res = playTime - min*60 - sec;
        res = (int)(res * 100);
        Debug.Log(min +", "+sec+", "+res);

        string m, s, r;
        if (min < 10)
        {
            m = "0";
            m += min.ToString();
        } else m = min.ToString();
        if (sec < 10)
        {
            s = "0";
            s += sec.ToString();
        } else s = sec.ToString();
        if (res < 10)
        {
            r = "0";
            r += res.ToString();
        }
        else r = res.ToString();
        timeText.text = ("" + m + "' " + s + "'' " + r);

        if (isContinueAvailable)
        {
            int prevC = PlayerPrefs.GetInt("Coin");
            prevC += coin;
            PlayerPrefs.SetInt("Coin", prevC);
            prevCoins = coin;

            int prevB = PlayerPrefs.GetInt("Box");
            prevB += brokenBoxes;
            PlayerPrefs.SetInt("Box", prevB);
            prevBoxes = brokenBoxes;
        }
        else
        {
            int prevC = PlayerPrefs.GetInt("Coin");
            prevC += (coin - prevCoins);
            PlayerPrefs.SetInt("Coin", prevC);

            int prevB = PlayerPrefs.GetInt("Box");
            prevB += brokenBoxes;
            PlayerPrefs.SetInt("Box", prevB);
        }

        if(curScore >= prevScore){
            PlayerPrefs.SetInt("Score", curScore);
            highScore.SetActive(true);
        }else{
            highScore.SetActive(false);
        }
        PlayerPrefs.Save();

        myBoxAnimator.SetBool("GameOver", true);
        myGameOverAnimator.SetBool("GameOver", true);
        SoundManager.GetComponent<SoundManager>().StopBGM();
        StartCoroutine(SoundManager.GetComponent<SoundManager>().GameOver());
        skullFX.SetActive(true);
    }

    public void BtnOnCharacter()
    {
        myCharacterAnimator.SetBool("isOpen", true);
    }

    public void BtnOnShop()
    {
        myShopAnimator.SetBool("isOpen", true);
    }

    public void BtnOnMain()
    {
        myShopAnimator.SetBool("isOpen", false);
    }

    public void BtnOnQuit()
    {
        Application.Quit();
    }
}