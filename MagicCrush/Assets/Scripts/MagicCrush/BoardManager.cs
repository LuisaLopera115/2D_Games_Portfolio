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
    public ScreenTransform ScreenTransform;

    public World world;

    public int currentLevel = 0;

    public GmaeStates currentState = GmaeStates.move;
    
    public static BoardManager ShareInstance;
    public Sprite[] prefabs;
    public GameObject currentProp;
    public List<GameObject> FindedMatcches = new List<GameObject>();
    public GameObject PropExplotion;
    public bool matchAterreilling = false;

    public int xSize,ySize;

    public GameObject[,] props; 

    public int[] scoreGoals = new int[3];

    public const int MinPropstoMatch = 2;

    private Vector3 initialPosition1;
    private Vector3 initialPosition2;

    public int MatchId;

    public int offset;

    public Sprite[] ingredienstSprites;
    public int moveCounter;
    
    public int strikeValue = 5; // when you have an extra match while falling props you get extra bonus 

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

        CreateInitBoard();
        ScreenTransform = FindObjectOfType<ScreenTransform>();
    }

    public void CreateInitBoard() {
       StartCoroutine(CreateInitBoardCo());
    }
    public IEnumerator CreateInitBoardCo(){

        Debug.Log("reset old props");

        DestroyAll("prop");

        Debug.Log("Info colection");

        PutLevelInfo();
        
        Debug.Log("Info colected");

        yield return new WaitForSeconds(.5f);
        
        if (ScreenTransform != null)
        {
            ScreenTransform.RePositionCamera(xSize - 1, ySize - 1);
        }

        CauldronManager.ShareInstance.InitCauldron();

        yield return new WaitForSeconds(.5f);
        
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
                    idx = Random.Range(0,prefabs.Length);
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
//                                Debug.Log("id " +  MatchId);
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
//                                Debug.Log("id " +  MatchId);
                            }
                        }
                    }
                }
            }
        }

        
        
        if (matchAterreilling)
        {
            matchAterreilling = false;
            GUIManager.sharedInstance.Score += 30;
            GUIManager.sharedInstance.MoveCounter --;
            CauldronManager.ShareInstance.CompareIngredientsWhitMatch();
            StartCoroutine(NullMatches());
            //Debug.Log("FindMatchesCo");
            return true;
        }else{
            return false;
        }
     }

    public IEnumerator NullMatches(){

        //Debug.Log("NullMatches");

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
//         Debug.Log("FindNullProps");
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
//                  Debug.Log("null in X: " + x + "Y: " + y);
                    Vector2 tempPos = new Vector2 (x,y + offset);
                    GameObject newProp = Instantiate(currentProp,tempPos,Quaternion.identity);
                    newProp.name = string.Format("Prop[{0}][{1}]", x,y);
                    newProp.GetComponent<Prop>().xPos = x;
                    newProp.GetComponent<Prop>().yPos= y;
                    newProp.GetComponent<Prop>().Resetprevius();


                do{
                    idx = Random.Range(0,prefabs.Length);
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

       // Debug.Log("FillandMatch");

        yield return new WaitForSeconds(.4f);
//        Debug.Log("fill and Match");
        StartCoroutine(RefillBoard());
        
        yield return new WaitForSeconds(.4f);
        while (MatchesOnBoard())
        {
           // Debug.Log("MatchesOnBoard()");
            GUIManager.sharedInstance.Score += 20;
            yield return new WaitForSeconds(.4f);
        }

        if (IsDeadLock())
        {
            Debug.Log("DEAD");
        }else{
            Debug.Log("HAY MATCH");
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

    public void PutLevelInfo(){

        
         if (world != null)
        {
            if(world.levels[currentLevel] != null){

                Debug.Log("Info colection Funtion");

                xSize = world.levels[currentLevel].xSice;
                ySize = world.levels[currentLevel].ySice;

                prefabs = world.levels[currentLevel].propsSprites;

                moveCounter = world.levels[currentLevel].moves;
                
                scoreGoals = world.levels[currentLevel].scoreGoals;

                ingredienstSprites = world.levels[currentLevel].ingredienstSprites;

                if (ScreenTransform != null)
                {
                    ScreenTransform.RePositionCamera(BoardManager.ShareInstance.xSize - 1,BoardManager.ShareInstance.ySize - 1);
                }
            }
        }
    }

    private void SwitchProps(int row, int column, Vector2 direction){
        //ebug.Log("Swipea");
        // here board is gonna change position to check if theres is matche if not its a deadlock
        GameObject holder = props[row + (int) direction.x, column + (int) direction.y];
        props[row + (int) direction.x, column + (int) direction.y] = props[row,column];
        props[row,column] = holder;
    }

    private bool checkForMatch(){

        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                if (props[i,j] != null)
                {
                    if (i < xSize - 2)
                    {
                        if (props[i + 1, j].GetComponent<Prop>().id == props[i, j].GetComponent<Prop>().id &&
                            props[i + 2, j].GetComponent<Prop>().id == props[i, j].GetComponent<Prop>().id)
                        {
                            return true;
                        }
                    }
                    
                    if (j < ySize - 2)
                    {
                        if (props[i, j + 1].GetComponent<Prop>().id == props[i, j].GetComponent<Prop>().id &&
                            props[i, j + 2].GetComponent<Prop>().id == props[i, j].GetComponent<Prop>().id)
                        {
                            return true;
                        }
                    }
                    
                }
            }
        }
        return false;
    }

    private bool SwitchAndCheck(int column, int row, Vector2 derection){

        SwitchProps(column,row,derection);
        if (checkForMatch())
        {
            SwitchProps(column,row,derection);
            return true;
        }
        SwitchProps(column,row,derection);
            return false;
        
    }

    private bool IsDeadLock(){

    for (int i = 0; i < xSize; i++)
    {
        for (int j = 0; j < ySize; j++)
        {
            if (props[i,j] != null)
            {
                if (i < xSize - 1)
                {
                    if (SwitchAndCheck(i,j,Vector2.right))
                    {
                        return false;
                    }
                }
                if (j < ySize - 1)
                {
                    if (SwitchAndCheck(i,j,Vector2.up))
                    {
                        return false;
                    }
                }
            }
        }
    }
    return true;
}

}


