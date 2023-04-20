
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public Image scoraBar;
    public Text movesText, scoreText;
    public Image panel;
    
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
        get { return BoardManager.ShareInstance.moveCounter; }
        set
        {
            BoardManager.ShareInstance.moveCounter = value;
            movesText.text = "MOVES: " + BoardManager.ShareInstance.moveCounter;

            if (BoardManager.ShareInstance.moveCounter == 0)
            {
                if(Score > BoardManager.ShareInstance.scoreGoals[BoardManager.ShareInstance.scoreGoals.Length - 1]){
                    BoardManager.ShareInstance.currentLevel ++;
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
        if (BoardManager.ShareInstance != null)
        {
            movesText.text = "MOVES: " + BoardManager.ShareInstance.moveCounter;
            scoreText.text = "SCORE: " + score;
        }
        
    }

    private void Update() {
        UpdateBar();
    }

    private void UpdateBar(){

        if (scoraBar != null)
        {
            int length = BoardManager.ShareInstance.scoreGoals.Length - 1;
            scoraBar.fillAmount = (float) score / (float) BoardManager.ShareInstance.scoreGoals[length];
        }
    }

    // Panel Manager

    private IEnumerator GameOver(){
        
        yield return new WaitUntil(() => !BoardManager.ShareInstance.isShifting);
        yield return new WaitForSeconds(3f);
        Debug.Log("Game Over");
        PanelManager("Game Over");
        score = 0;
        //BoardManager.ShareInstance.moveCounter = 5;
        movesText.text = "MOVES: " + BoardManager.ShareInstance.moveCounter;
        scoreText.text = "SCORE: " + score;
        // game over panel
    }

    private IEnumerator WinGame(){
        
        yield return new WaitUntil(() => !BoardManager.ShareInstance.isShifting);
        yield return new WaitForSeconds(3f);
        Debug.Log("Win Game");
        score = 0;
        movesText.text = "MOVES: " + BoardManager.ShareInstance.moveCounter;
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
                panel.enabled = true;
            break;

            case "Game Win": 
                winPanel.SetActive(true);
                panel.enabled = true;

            break;

            case "Reset": 
                winPanel.SetActive(false);
                gameOverPanel.SetActive(false);
                panel.enabled = false;

            break;

            default:
            break;
        }
    }
}
