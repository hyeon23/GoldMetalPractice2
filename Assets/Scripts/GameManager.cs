using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //StageManager
    //1. Score
    //2. Scene Movement    

    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;
    public GameObject[] stages;

    public Image[] UIHealth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject UIRestartButton;

    private void Update()
    {
        UIPoint.text = (stagePoint + totalPoint).ToString();

    }

    public void NextStage()
    {
        //Change Stage
        if(stageIndex < stages.Length - 1)
        {
            stages[stageIndex].SetActive(false);
            stageIndex++;
            stages[stageIndex].SetActive(true);

            //Calculate Score
            totalPoint += stagePoint;
            stagePoint = 0;

            //Rearrange Position
            PlayerReposition();

            UIStage.text = "STAGE" + (stageIndex + 1);
        }
        else
        {//Game Ending
            //Player Control Lock
            Time.timeScale = 0;
            //Result UI
            Debug.Log("게임 클리어.");
            //Restart Button UI
            Text buttonText = UIRestartButton.GetComponentInChildren<Text>();
            buttonText.text = "Game Clear!";
            UIRestartButton.SetActive(true);
        }
        

    }

    public void HealthDown()
    {
        if(health > 1)
        {
            health--;
            UIHealth[health].color = new Color(1, 0, 0, 0.4f);
        }
        else
        {
            //Player Die Effect
            Time.timeScale = 0;
            player.OnDie();
            //Result UI
            Debug.Log("죽었습니다.");
            //Retry Button UI
            UIRestartButton.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //Player Reposition
            if(health > 1)
            {
                PlayerReposition();
            }
            //Health Down
            HealthDown();
        }
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(0, 0, -1);
        player.VelocityZero();
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);//현재 씬 로드
    }
}
