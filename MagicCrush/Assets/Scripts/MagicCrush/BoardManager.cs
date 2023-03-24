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
                GameObject newProp = Instantiate(currentProp,
                 new Vector3(startX + (offset.x*x), 
                            startY + (offset.y*y),
                            0),
                            currentProp.transform.rotation);

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
        for(int x = 0; x < xSize; x++)
        {
            for(int y = 0; y < ySize; y++)
            {
                if(props[x,y].GetComponent<SpriteRenderer>().sprite == null)
                {
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

        List<SpriteRenderer> renderes = new List<SpriteRenderer>();
        int nullProps = 0;

        for(int y = yStart; y < ySize; y++)
        {
            SpriteRenderer spriteRenderer = props[x, y].GetComponent<SpriteRenderer>();
            if (spriteRenderer.sprite == null)
            {
                nullProps++;
            }
            renderes.Add(spriteRenderer);
        }

        for (int i = 0; i < nullProps; i++)
        {
            GUIManager.sharedInstance.Score += 10;

            yield return new WaitForSeconds(shiftDelay);
            for(int j = 0; j < renderes.Count - 1; j++)
            {
                renderes[j].sprite = renderes[j + 1].sprite;
                renderes[j + 1].sprite = GetNewProp(x, ySize-1);;
            }
        }

        isShifting = false;
    }

    private Sprite GetNewProp(int x, int y)
    {
        List<Sprite> possibleProps = new List<Sprite>();
        possibleProps.AddRange(prefabs);

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
