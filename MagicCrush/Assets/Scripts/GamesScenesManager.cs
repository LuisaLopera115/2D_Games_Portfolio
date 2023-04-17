using UnityEngine;
using UnityEngine.SceneManagement;

public class GamesScenesManager : MonoBehaviour
{
    public GameObject gamesPanel;
    public void MagicCrush(){
        SceneManager.LoadScene("MagicCrush");
    }

    public void GamesPanel(){
        gamesPanel.SetActive(true);
    }

}
