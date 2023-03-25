using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoardManager : MonoBehaviour
{
    public static BoardManager ShareInstance;
    public List<Sprite> prefabs = new List<Sprite>();
    public GameObject currentProp;

    public int xSize,ySize;

    private GameObject[,] props; 

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
                /*
                GameObject newProp = Instantiate(currentProp,
                 new Vector3(startX + (offset.x*x), 
                            startY + (offset.y*y),
                            0),
                            currentProp.transform.rotation);

                newProp.name = string.Format("Prop[{0}][{1}]", x,y);
*/
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
        //Debug.Log("FindNullProps");
        for(int x = 0; x < xSize; x++)
        {
            for(int y = 0; y < ySize; y++)
            {
                if(props[x,y].GetComponent<SpriteRenderer>().sprite == null)
                {
                    Debug.Log("en X: " + x + " Y: " + y + "es null");
                    yield return StartCoroutine(MakePropsFall(x, y));
                    break;
                }
            }
        }


        for(int x = 0; x < xSize; x++)
        {
            for(int y = 0; y < ySize; y++)
            {
                props[x, y].GetComponent<Prop>().FindAllMatches();
            }
        }

    }

    private IEnumerator MakePropsFall(int x, int yStart,
                                        float shiftDelay = 0.05f)
    {

        

        isShifting = true;

        List<GameObject> nullProps = new List<GameObject>();

        
        int nullPropsCant = 0;

        for(int y = yStart; y < ySize; y++)
        {
            Transform RowProps = props[x, y].GetComponent<Transform>();

            if (RowProps.GetComponent<SpriteRenderer>().sprite == null)
            {
                nullPropsCant++;
                GUIManager.sharedInstance.Score += 10;
                nullProps.Add(RowProps.gameObject);

                
                yield return null;
            }
        }

        if (nullPropsCant>0){

                for (int i = 0; i < nullPropsCant; i++)
                {
                    List<Transform> NonNullRowProps = new List<Transform>();

                    RaycastHit2D hit = Physics2D.Raycast(nullProps[i].transform.position,Vector2.up);

                    while(hit.collider != null && hit.collider.gameObject.tag == "prop"){
                        NonNullRowProps.Add(hit.collider.transform);
                        Debug.Log(hit.collider.GetComponent<SpriteRenderer>().sprite);
                        hit =  Physics2D.Raycast(hit.collider.transform.position,Vector2.up);
                    }


                }

        }
        
        /*
        for (int i = 0; i < props_.Count; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(props_[i].transform.position,Vector2.up);

            while(hit.collider != null && hit.collider.gameObject.tag == "prop"){

                float swapSpeed = 0.2f;

                initialPosition1 = props_[i].transform.position;
                initialPosition2 = hit.collider.GetComponent<Transform>().position;

                float t = 0.0f;
                while (t < 1.0f)
                {
                    t += Time.deltaTime/swapSpeed;

                    props_[i].GetComponent<Transform>().position = Vector3.Lerp(initialPosition1, initialPosition2, t);
                    hit.collider.GetComponent<Transform>().position = Vector3.Lerp(initialPosition2, initialPosition1, t);
                    yield return null;
                }

                hit =  Physics2D.Raycast(hit.collider.transform.position,Vector2.up);
            }

        }

        for (int i = 0; i < props_.Count; i++)
        {
            props_[i].GetComponent<SpriteRenderer>().sprite = GetNewProp();
        }
        */
        isShifting = false;
    

    }

    private Sprite GetNewProp()
    {
        List<Sprite> possibleProps = new List<Sprite>();
        possibleProps.AddRange(prefabs);
        /*
        if(x>0)
        {
            possibleProps.Remove(
                props[x - 1, y].GetComponent<SpriteRenderer>().sprite);
        }
        if (x < xSize - 1)
        {
            possibleProps.Remove(
                props[x + 1, y].GetComponent<SpriteRenderer>().sprite);
        }
        if (y > 0)
        {
            possibleProps.Remove(
                props[x, y - 1].GetComponent<SpriteRenderer>().sprite);
        }
*/
        return possibleProps[Random.Range(0, possibleProps.Count)];
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
