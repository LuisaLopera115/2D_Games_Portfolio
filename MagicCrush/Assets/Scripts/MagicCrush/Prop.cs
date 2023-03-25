using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    private Color selectedColor = new Color(0f,0f,0f);
    public static Prop previusSelected = null;

    private SpriteRenderer spriteRenderer;
    private bool isSelected = false;

    public int id;

    private Vector3 initialPosition1;
    private Vector3 initialPosition2;

    [SerializeField] public int xPos, yPos;
    [SerializeField] public int xPosTarget, yPosTarget;
    private Vector2 firstTouchPosition, secondTouchPosition;
    private float swipeAngle;

    private Vector2[] adjacentDirections= new Vector2[]{
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right
    };


    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
       xPosTarget = (int)transform.position.x;
       yPosTarget = (int)transform.position.x;
       xPos=xPosTarget;
       yPos=xPosTarget;
        //objective = Vector3.zero;
    }

    private void Select(){
        isSelected = true;
        spriteRenderer.color = selectedColor;
        previusSelected = GetComponent<Prop>();
        // //Debug.Log("el previus selected es: " + previusSelected.name);
    }

    private void Deselect(){
        isSelected = false; 
        spriteRenderer.color = Color.white;
        previusSelected = null;
    }

    private void OnMouseDown() {
/*
        if (spriteRenderer.sprite == null ||
         BoardManager.ShareInstance.isShifting){
            return;
        }

        if(isSelected){
            Deselect();
        }
        else{

            if (previusSelected == null){
                Select();
                
            }else{

                if (CanSwipe())
                {
                    StartCoroutine(SwapProps(previusSelected));
                    //Debug.Log(spriteRenderer.sprite);
                    
                    
                }else{
                    previusSelected.Deselect();
                    Select();
                }
            }
        }

*/

        // FOR MOVIL GAME 
        //Debug.Log("click");
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

    }

     private void OnMouseUp() {
        //Debug.Log("Unclick");
        secondTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateTheAngle();
    }
    
    private void CalculateTheAngle(){
        swipeAngle = Mathf.Atan2(secondTouchPosition.y - firstTouchPosition.y,  secondTouchPosition.x - firstTouchPosition.x) * Mathf.Rad2Deg;
        Debug.Log(swipeAngle);
    }

    private void Update()
    {
        if (objective != Vector3.zero)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, objective, 5 * Time.deltaTime);
        }
    }

    private IEnumerator SwapProps(Prop newProp){
        /*
        if (spriteRenderer.sprite == newProp.GetComponent<SpriteRenderer>().sprite)
        {
            yield return null;
        }

         float swapSpeed = 0.5f;

        initialPosition1 = newProp.GetComponent<Transform>().position;
        initialPosition2 = this.transform.position;

        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime/swapSpeed;

            newProp.GetComponent<Transform>().position = Vector3.Lerp(initialPosition1, initialPosition2, t);
            this.transform.position = Vector3.Lerp(initialPosition2, initialPosition1, t);
            yield return null;
        }*/


        
        FindAllMatches();
        previusSelected.FindAllMatches();
        previusSelected.Deselect();
        GUIManager.sharedInstance.MoveCounter--;
        
    }


    private GameObject GetNeightbor( Vector2 direction){
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position,direction);
        if (hit.collider != null)
        {
             return hit.collider.gameObject;
        }else{
            return null;
        }
    }

    private List<GameObject> GetAllNeighbors(){

        List<GameObject> neighbors = new List<GameObject>();

        foreach (Vector2 direction in adjacentDirections)
        {
            neighbors.Add(GetNeightbor(direction));
        }

        return neighbors;
    }

    private bool CanSwipe(){

        return GetAllNeighbors().Contains(previusSelected.gameObject);
    }
    
     private List<GameObject> FindMatch(Vector2 direction){

        // Debug.Log("entra al Metodo FindMacth");
        List<GameObject> findMatchingProps = new List<GameObject>();

        RaycastHit2D hit = Physics2D.Raycast(this.transform.position,direction);
        // //Debug.Log("Hit: " + hit.collider);
        // //Debug.Log("Hit Sprite: " + hit.collider.GetComponent<SpriteRenderer>().sprite);


//        Debug.Log(spriteRenderer.sprite + " --> " + hit.collider.GetComponent<SpriteRenderer>().sprite );


        while(hit.collider != null && 
        hit.collider.GetComponent<SpriteRenderer>().sprite == spriteRenderer.sprite){
            findMatchingProps.Add(hit.collider.gameObject);
            hit =  Physics2D.Raycast(hit.collider.transform.position,direction);
        }


        return findMatchingProps;
    }

    private bool ClearMatch(Vector2[] directions){

        // //Debug.Log("Entra al metodo ClearMatch");
        List<GameObject> matchingProps = new List<GameObject>();

        foreach(Vector2 direction in directions){
            matchingProps.AddRange(FindMatch(direction));
            // Debug.Log(directions.ToString());
        }

        if (matchingProps.Count >= BoardManager.MinPropstoMatch)
        {
            // //Debug.Log("hay match");
            foreach (GameObject prop in matchingProps)
            {
                //Destroy(prop.gameObject);
               prop.GetComponent<SpriteRenderer>().sprite = null;
            }
            spriteRenderer.sprite = null;

            return true;
        }else{
            return false;
        }
    }

    public void FindAllMatches(){

        //Debug.Log("Entra a metodo FindAllMatches");
        if (spriteRenderer.sprite == null) return;
       
        bool hMatch = ClearMatch(new Vector2[2]{Vector2.left,Vector2.right});
        bool vMatch = ClearMatch(new Vector2[2]{Vector2.up,Vector2.down});

        if (hMatch || vMatch){
            
            StopCoroutine(BoardManager.ShareInstance.FindNullProps());
            StartCoroutine(BoardManager.ShareInstance.FindNullProps());
        } 
    }
}
