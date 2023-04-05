using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager ShareInstance;
    private int intLevel;

    void Start()
    {
        intLevel = 0;
        if(ShareInstance == null){
            ShareInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Level();

//        Debug.Log("Start Game, level: " + intLevel);
    }

    public void Level(bool loseWin = true){

        if (loseWin) intLevel++;
        
        if (intLevel < 5)
        {
            BoardManager.ShareInstance.xSize = 7;
            BoardManager.ShareInstance.ySize = 8;
            
            BoardManager.ShareInstance.CreateInitBoard();
        }
        else if (intLevel < 10)
        {
            BoardManager.ShareInstance.xSize = 7;
            BoardManager.ShareInstance.ySize = 9;
            
            BoardManager.ShareInstance.CreateInitBoard();
        }
        else if (intLevel < 15){
            BoardManager.ShareInstance.xSize = 7;
            BoardManager.ShareInstance.ySize = 10;
            
            BoardManager.ShareInstance.CreateInitBoard();
        }
        else if (intLevel < 20){
            BoardManager.ShareInstance.xSize = 7;
            BoardManager.ShareInstance.ySize = 11;
            
            BoardManager.ShareInstance.CreateInitBoard();
        }
        else if (intLevel < 25){
            BoardManager.ShareInstance.xSize = 7;
            BoardManager.ShareInstance.ySize = 12;
            
            BoardManager.ShareInstance.CreateInitBoard();
        }
        else if (intLevel < 30){
            BoardManager.ShareInstance.xSize = 7;
            BoardManager.ShareInstance.ySize = 13;
            
            BoardManager.ShareInstance.CreateInitBoard();
        }
    }
}
