
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public Text movesText, scoreText;
    private int moveCounter;
    private int score;

    public GameObject winPanel, gameOverPanel;

    public int Score
    {
        get{return score; }
        set
        {
            score = value;
            scoreText.text = "SCORE: " + score;
        }
    }

    public int MoveCounter
    {
        get { return moveCounter; }
        set
        {
            moveCounter = value;
            movesText.text = "MOVES: " + moveCounter;

            if (moveCounter == 0)
            {
                if(Score > 100){
                    StartCoroutine(WinGame());
                }else{
                    StartCoroutine(GameOver());
                }
                
                
            }
        }
    }

    public static GUIManager sharedInstance;
    // Start is called before the first frame update
    void Start()
    {

        if (sharedInstance == null)
        {
            sharedInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        score = 0;
        moveCounter = 5;
        movesText.text = "MOVES: " + moveCounter;
        scoreText.text = "SCORE: " + score;
    }

    private IEnumerator GameOver(){
        
        yield return new WaitUntil(() => !BoardManager.ShareInstance.isShifting);
        yield return new WaitForSeconds(3f);
        Debug.Log("Game Over");
        PanelManager("Game Over");
        score = 0;
        moveCounter = 5;
        movesText.text = "MOVES: " + moveCounter;
        scoreText.text = "SCORE: " + score;
        // game over panel
    }

    private IEnumerator WinGame(){
        
        yield return new WaitUntil(() => !BoardManager.ShareInstance.isShifting);
        yield return new WaitForSeconds(3f);
        Debug.Log("Win Game");
        score = 0;
        moveCounter = 5;
        movesText.text = "MOVES: " + moveCounter;
        scoreText.text = "SCORE: " + score;

        // game win panel
        PanelManager("Reset");
        PanelManager("Game Win");
    }

    private void NextLevelButton(){

    }

    private void MenuButton(){
        
    }

    public void PanelManager(string Panel){
        
        switch (Panel)
        {
            case "Game Over": 
                gameOverPanel.SetActive(true);
            break;

            case "Game Win": 
                winPanel.SetActive(true);

            break;

            case "Reset": 
                winPanel.SetActive(false);
                gameOverPanel.SetActive(false);

            break;

            default:
            break;
        }
    }
}
