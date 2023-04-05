using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoardManager : MonoBehaviour
{
    public static BoardManager ShareInstance;
    public List<Sprite> prefabs = new List<Sprite>();
    public GameObject currentProp;

    public int xSize,ySize;

    public GameObject[,] props; 

    public const int MinPropstoMatch = 2;

    private Vector3 initialPosition1;
    private Vector3 initialPosition2;

    public int offset;

    public bool isShifting{get; set;} // esto quiere decir que 
    //puede asignar un valor o evaluar un valor pero solo desde la clase.
    void Start()
    {
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


    public IEnumerator FindNullProps()
    {
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
                    
                }
            }
            nullPropsCant = 0; 
        }
       StartCoroutine(FillandMatch());
        yield return new WaitForSeconds(.4f);
    }

    private IEnumerator RefillBoard(){
        Debug.Log("refill");
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
        for(int x = 0; x < xSize; x++)
        {
            for(int y = 0; y < ySize; y++)
            {
                if(props[x,y] != null){
                    if (props[x,y].GetComponent<Prop>().FindAllMatches()){
                        Debug.Log("there is a match");
                        
                        return true;
                    } 
                }
            }
        }
         return false;
    }

    private IEnumerator FillandMatch(){
        yield return new WaitForSeconds(.4f);
        Debug.Log("fill and Match");
        StartCoroutine(RefillBoard());
        yield return new WaitForSeconds(.4f);
        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.8f);
            StopCoroutine(FindNullProps());
            StartCoroutine(FindNullProps());
        }
        
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
