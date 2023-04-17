using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombsManager : MonoBehaviour
{
    public static BombsManager ShareInstance;
    public List<GameObject> FindedMatcches = new List<GameObject>();

    void Start()
    {
        if(ShareInstance == null){
            ShareInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void FindMatches(){
        StartCoroutine(FindMatchesCo());
    }

    public IEnumerator FindMatchesCo(){

        yield return new WaitForSeconds(.2f);

        Debug.Log("FindMatchesCo");

        for (int x = 0; x < BoardManager.ShareInstance.xSize; x++)
        {
            for (int y = 0; y < BoardManager.ShareInstance.ySize; y++)
            {
                GameObject currentProp =  BoardManager.ShareInstance.props[x,y];
                // horizontal
                if (currentProp != null)
                {
                    if (x > 0 && x <  BoardManager.ShareInstance.xSize - 1)
                    {
                        GameObject leftProp =  BoardManager.ShareInstance.props[x - 1,y];
                        GameObject rightProp =  BoardManager.ShareInstance.props[x + 1,y];

                        if (leftProp != null && rightProp != null)
                        {
                            if (currentProp.GetComponent<Prop>().id == leftProp.GetComponent<Prop>().id &&
                            currentProp.GetComponent<Prop>().id == rightProp.GetComponent<Prop>().id)
                            {
                                if (!FindedMatcches.Contains(currentProp)) FindedMatcches.Add(currentProp);
                        
                                if (!FindedMatcches.Contains(leftProp)) FindedMatcches.Add(leftProp);

                                if (!FindedMatcches.Contains(rightProp)) FindedMatcches.Add(rightProp);
                            }
                        }
                    }
                
                    // vertical
                    if (y > 0 && y <  BoardManager.ShareInstance.ySize - 1)
                    {
                            GameObject upProp =  BoardManager.ShareInstance.props[x,y + 1];
                            GameObject downProp =  BoardManager.ShareInstance.props[x,y - 1];
                        if (upProp != null && downProp != null)
                        {
                            if (currentProp.GetComponent<Prop>().id == upProp.GetComponent<Prop>().id &&
                            currentProp.GetComponent<Prop>().id == downProp.GetComponent<Prop>().id)
                            {
                                if (!FindedMatcches.Contains(currentProp)) FindedMatcches.Add(currentProp);
                        
                                if (!FindedMatcches.Contains(upProp)) FindedMatcches.Add(upProp);

                                if (!FindedMatcches.Contains(downProp)) FindedMatcches.Add(downProp);
                            }
                        }
                    }
                }
            }
        }
     }
}
