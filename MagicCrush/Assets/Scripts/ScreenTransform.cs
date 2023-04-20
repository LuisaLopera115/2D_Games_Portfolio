using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTransform : MonoBehaviour
{
    public int padding = 2;
    public float aspectRadio = 0.625f;

    void Start()
    {
        RePositionCamera(5f -1, 5f -1);
    }

    public void RePositionCamera(float x , float y){
        Vector3 newPos = new Vector3 (x/2, (y/2) - 2, -10f);
        this.transform.position = newPos;
//        Debug.Log(x + " -- " + y);
        if (BoardManager.ShareInstance != null)
        {
            if (BoardManager.ShareInstance.xSize >= BoardManager.ShareInstance.ySize)
            {
                Camera.main.orthographicSize = (BoardManager.ShareInstance.xSize/2 + padding) / aspectRadio;
            }else
            {
                Camera.main.orthographicSize = BoardManager.ShareInstance.ySize/2 + padding;
            }
        }
        
    }
}
