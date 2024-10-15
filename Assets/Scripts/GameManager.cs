using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public bool paused = false;
    public bool dead = false;
    private bool showingControlls = false;
    public bool onMainMenu = false;
    public bool onBoss = false;

    public Image healthBar;
    public Image manaBar;
    public Image swordLifeTime;

    public Image bossHealthBar;


    public GameObject pauseIndicator;
    public GameObject resume;
    public GameObject mainMenu;
    public GameObject quit;
    public GameObject start;
    public GameObject controlls;
    public GameObject hideControlls;
    public GameObject controllsText;
    public GameObject retryStuff;
    public GameObject scroll1;
    public GameObject scroll2;
    public GameObject scroll3;
    public GameObject sign1;



    public PlayerController player;

    public BossController boss;

    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {



        if (!onMainMenu)
        {
            healthBar.fillAmount = (float)player.health / (float)player.maxHealth;
            manaBar.fillAmount = (float)player.mana / (float)player.maxMana;
            swordLifeTime.fillAmount = (float)player.meleeActiveTimer / (float)15;
        }

        if (onBoss)
        {
            bossHealthBar.fillAmount = (float)boss.health / 50;
        }


        if (Input.GetButtonDown("Cancel") && !dead)
        {
            if (!paused)
            {
                Time.timeScale = 0;
                pauseIndicator.SetActive(true);
                resume.SetActive(true);
                mainMenu.SetActive(true);
                quit.SetActive(true);
                controlls.SetActive(true);
                paused = true;
            }

            else
            {
                Time.timeScale = 1;

                pauseIndicator.SetActive(false);
                resume.SetActive(false);
                mainMenu.SetActive(false);
                quit.SetActive(false);
                controlls.SetActive(false);
                hideControlls.SetActive(false);
                controllsText.SetActive(false);
                scroll1.SetActive(false);
                scroll2.SetActive(false);
                scroll3.SetActive(false);
                sign1.SetActive(false);
                paused = false;
            }
        }

    }

    public void resumeGame()
    {
        Time.timeScale = 1;

        pauseIndicator.SetActive(false);
        resume.SetActive(false);
        mainMenu.SetActive(false);
        quit.SetActive(false);
        controlls.SetActive(false);
        controllsText.SetActive(false);
        hideControlls.SetActive(false);
        paused = false;
    }

    public void returnToMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
        paused = false;
    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void startGame()
    {
        SceneManager.LoadScene(1);
    }

    public void showControlls()
    {
        controllsText.SetActive(true);
        showingControlls = true;
    }


    public void hideControllsText()
    {
        controllsText.SetActive(false);
        showingControlls = false;
    }

    public void loadLevel(int v)
    {
        if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        else
            returnToMenu();
    }

    public void death()
    {
        dead = true;
        Time.timeScale = 0;
        retryStuff.SetActive(true);
        paused = true;
    }

    public void win()
    {
        SceneManager.LoadScene("Win");
    }

    public void retry()
    {
        retryStuff.SetActive(false);
        player.death();
        Time.timeScale = 1;
        paused = false;
        dead = false;
    }

    public void retryLvl()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void scroll()
    {
        Time.timeScale = 0;
        paused = true;
        scroll1.SetActive(true);
    }

    public void scroll_2()
    {
        Time.timeScale = 0;
        paused = true;
        scroll2.SetActive(true);
    }

    public void scroll_3()
    {
        Time.timeScale = 0;
        paused = true;
        scroll3.SetActive(true);
    }

    public void sign()
    {
        Time.timeScale = 0;
        paused = true;
        sign1.SetActive(true);
    }
}
