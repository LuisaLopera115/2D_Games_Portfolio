using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GmaeStates
{
    wait, 
    move
} 

public class BoardManager : MonoBehaviour
{
    public GmaeStates currentState = GmaeStates.move;
    
    public static BoardManager ShareInstance;
    public List<Sprite> prefabs = new List<Sprite>();
    public GameObject currentProp;
    public List<GameObject> FindedMatcches = new List<GameObject>();
    public GameObject PropExplotion;
    public bool matchAterreilling = false;

    public int xSize,ySize;

    public GameObject[,] props; 

    public const int MinPropstoMatch = 2;

    private Vector3 initialPosition1;
    private Vector3 initialPosition2;

    public int MatchId;

    public int offset;

    

    public bool isShifting{get; set;} // esto quiere decir que 
    //puede asignar un valor o evaluar un valor pero solo desde la clase.
    void Start()
    {
        MatchId = -1;
        offset = 3;
        if(ShareInstance == null){
            ShareInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CreateInitBoard(){

//        Debug.Log("Init Pos " + gameObject.transform.position);
        
//        Debug.Log("SE RESETEA TODO EL JUEGO");
        DestroyAll("prop");

        CauldronManager.ShareInstance.InitIngredients();

        props = new GameObject[xSize, ySize];
        float startX = this.transform.position.x;
        float startY = this.transform.position.y;
        int idx = -1;
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                Vector2 tempPosition = new Vector2(x,  y + offset );
                GameObject newProp = Instantiate(currentProp,tempPosition,Quaternion.identity);
                newProp.name = string.Format("Prop[{0}][{1}]", x,y);
                newProp.GetComponent<Prop>().xPos = x;
                newProp.GetComponent<Prop>().yPos= y;
                newProp.GetComponent<Prop>().Resetprevius();
                do
                {
                    idx = Random.Range(0,prefabs.Count);
                } while ((x>0 && idx == props[x-1,y].GetComponent<Prop>().id) ||
                        (y>0 && idx == props[x,y-1].GetComponent<Prop>().id) );


                Sprite sprite = prefabs[idx];
                newProp.GetComponent<SpriteRenderer>().sprite = sprite;
                newProp.GetComponent<Prop>().id= idx;
                
                newProp.transform.parent = this.transform;

                props[x,y] = newProp;
            }
        }

//       Debug.Log("Final Pos " + gameObject.transform.position);
    } 



    public bool FindMatches(){
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
                                currentProp.GetComponent<Prop>().isMatch= true;
                                leftProp.GetComponent<Prop>().isMatch= true;
                                rightProp.GetComponent<Prop>().isMatch= true;
                                matchAterreilling = true;
                                MatchId = currentProp.GetComponent<Prop>().id;
                                Debug.Log("id " +  MatchId);
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
                                currentProp.GetComponent<Prop>().isMatch= true;
                                upProp.GetComponent<Prop>().isMatch= true;
                                downProp.GetComponent<Prop>().isMatch= true;
                                matchAterreilling = true;
                                MatchId = currentProp.GetComponent<Prop>().id;
                                Debug.Log("id " +  MatchId);
                            }
                        }
                    }
                }
            }
        }

        
        
        if (matchAterreilling)
        {
            matchAterreilling = false;
            CauldronManager.ShareInstance.CompareIngredientsWhitMatch();
            StartCoroutine(NullMatches());
            Debug.Log("FindMatchesCo");
            return true;
        }else{
            return false;
        }
     }

    public IEnumerator NullMatches(){

        Debug.Log("NullMatches");

        for (int x = 0; x < BoardManager.ShareInstance.xSize; x++)
        {
            for (int y = 0; y < BoardManager.ShareInstance.ySize; y++)
            {
                if (props[x,y] != null){

                    if (props[x,y].GetComponent<Prop>().isMatch)
                    {
                        GameObject Particle = Instantiate(PropExplotion,props[x,y].transform.position,Quaternion.identity);
                        Destroy(Particle, .5f);
                        Destroy(props[x,y]);
                        props[x, y] = null;
                    }
                }
            }
        }
        yield return new WaitForSeconds(.1f);
        StopCoroutine(BoardManager.ShareInstance.FindNullProps());
        StartCoroutine(BoardManager.ShareInstance.FindNullProps());
    }


    public IEnumerator FindNullProps()
    {   
        yield return new WaitForSeconds(.2f);
        int nullPropsCant = 0;
         Debug.Log("FindNullProps");
        for(int x = 0; x < xSize; x++)
        {
            for(int y = 0; y < ySize; y++)
            {
                if(props[x,y] == null)
                {
//                    Debug.Log("null in X: " + x + "Y: " + y);
                    Destroy(props[x,y]);
                    props[x,y] = null;
                    nullPropsCant ++;
                    //yield return StartCoroutine(MakePropsFall(x, y));
                }else if (nullPropsCant > 0){
                    props[x,y].GetComponent<Prop>().yPos -= nullPropsCant;
                    props[x,y].GetComponent<Prop>().Resetprevius();
                    props[x,y] = null; // este no lo entendi bien
                   // Debug.Log("CAEN EN CASCADA");
                }
            }
            nullPropsCant = 0; 
        }
        yield return new WaitForSeconds(.2f);
        currentState = GmaeStates.move;
        StartCoroutine(FillandMatch());
    }

    private IEnumerator RefillBoard(){

        
//        Debug.Log("refill");
        int idx = -1;
        for(int x = 0; x < xSize; x++)
        {
            for(int y = 0; y < ySize; y++)
            {
                if(props[x,y] == null)
                {
//                    Debug.Log("null in X: " + x + "Y: " + y);
                    Vector2 tempPos = new Vector2 (x,y + offset);
                    GameObject newProp = Instantiate(currentProp,tempPos,Quaternion.identity);
                    newProp.name = string.Format("Prop[{0}][{1}]", x,y);
                    newProp.GetComponent<Prop>().xPos = x;
                    newProp.GetComponent<Prop>().yPos= y;
                    newProp.GetComponent<Prop>().Resetprevius();


                do{
                    idx = Random.Range(0,prefabs.Count);
                } while ((x>0 && idx == props[x-1,y].GetComponent<Prop>().id) ||
                        (y>0 && idx == props[x,y-1].GetComponent<Prop>().id) );


                    Sprite sprite = prefabs[idx];
                    newProp.GetComponent<SpriteRenderer>().sprite = sprite;
                    newProp.GetComponent<Prop>().id= idx;
                    
                    newProp.transform.parent = this.transform;

                    props[x,y] = newProp;
                }
            }
        }
        yield return new WaitForSeconds(.4f);
    }

    private bool MatchesOnBoard(){

        int matchCounter = 0; 
        if (FindMatches()){
            matchCounter ++;
            return true;
        } else return false;
    }

    private IEnumerator FillandMatch(){

        Debug.Log("FillandMatch");

        yield return new WaitForSeconds(.4f);
//        Debug.Log("fill and Match");
        StartCoroutine(RefillBoard());
        
        yield return new WaitForSeconds(.4f);
        while (MatchesOnBoard())
        {
            Debug.Log("MatchesOnBoard()");
            yield return new WaitForSeconds(.4f);
        }
        yield return new WaitForSeconds(.5f);
        currentState = GmaeStates.move;
    }

    void DestroyAll(string tag)
    {
        GameObject[] Props = GameObject.FindGameObjectsWithTag(tag);

        for(int i=0; i< Props. Length; i++)
        {
            Destroy(Props[i]);
        }
    }

     
}


