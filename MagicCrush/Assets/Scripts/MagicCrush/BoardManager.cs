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

    public bool isShifting{get; set;} // esto quiere decir que 
    //puede asignar un valor o evaluar un valor pero solo desde la clase.
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

    public void CreateInitBoard(Vector2 offset){

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
                Vector2 tempPosition = new Vector2(x,y);
                GameObject newProp = Instantiate(currentProp,tempPosition,Quaternion.identity);
                newProp.name = string.Format("Prop[{0}][{1}]", x,y);
                
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
                if(props[x,y].GetComponent<SpriteRenderer>().sprite == null)
                {
                    Debug.Log("null in X: " + x + "Y: " + y);
                    Destroy(props[x,y]);
                    props[x,y] = null;
                    nullPropsCant ++;
                    //yield return StartCoroutine(MakePropsFall(x, y));
                }else if (nullPropsCant > 0){
                    props[x,y].GetComponent<Prop>().fall= true;
                    props[x,y].GetComponent<Prop>().yPos -= nullPropsCant;
                }
            }
            Debug.Log(nullPropsCant);
            nullPropsCant =0; 
        }

        yield return new WaitForSeconds(.4f);

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
