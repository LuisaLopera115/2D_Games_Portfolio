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

    private Vector3 initialPosition1, tempPos;
    private Vector3 initialPosition2;

    [SerializeField] public int xPos, yPos;
    [SerializeField] public int xPosTarget, yPosTarget;
    private Vector2 firstTouchPosition, secondTouchPosition;
    private float swipeAngle;
    private float swipeLimit = 1f;
    public GameObject otherProp; 
    
    public int previusX , previusY;

    private Vector2[] adjacentDirections= new Vector2[]{
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right
    };


    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        if (BoardManager.ShareInstance.currentState == GmaeStates.move) firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

     private void OnMouseUp() {
        //Debug.Log("Unclick");
        secondTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateTheAngle();
    }
    
    private void CalculateTheAngle(){
        if (Mathf.Abs(firstTouchPosition.y-secondTouchPosition.y) > swipeLimit )
        {
            swipeAngle = Mathf.Atan2(secondTouchPosition.y - firstTouchPosition.y,  secondTouchPosition.x - firstTouchPosition.x) * Mathf.Rad2Deg;
            SwapProps();
        }
    }

    private void Update()
    {
        xPosTarget=xPos;
        yPosTarget=yPos;


        if(Mathf.Abs( this.transform.position.x - xPosTarget) > .1){
            tempPos = new Vector2(xPosTarget, transform.position.y);
            this.transform.position = Vector2.Lerp(this.transform.position,tempPos, .1f);
            if (BoardManager.ShareInstance.props[xPos,yPos] != this.gameObject)
            {
                 BoardManager.ShareInstance.props[xPos,yPos] = this.gameObject;
                 //Debug.Log("se repposicionan los props"); 
            }
        }else{
            tempPos = new Vector2(xPosTarget, transform.position.y);
            transform.position = tempPos;
            //Debug.Log("se repposicionan los props");    
           
        }

        if(Mathf.Abs(yPosTarget - transform.position.y) > .1){
            tempPos = new Vector2(transform.position.x, yPosTarget);
            this.transform.position = Vector2.Lerp(this.transform.position,tempPos, .1f);
            if (BoardManager.ShareInstance.props[xPos,yPos] != this.gameObject)
            {
                 BoardManager.ShareInstance.props[xPos,yPos] = this.gameObject;
            }
        }else{
            tempPos = new Vector2(transform.position.x, yPosTarget);
            transform.position = tempPos;
        }
    }

    private void SwapProps(){

       // Debug.Log("SwapProps");

        if (swipeAngle > -45 && swipeAngle <= 45 && xPos < (BoardManager.ShareInstance.xSize-1)) 
        {
            //swipe rigth
            otherProp = BoardManager.ShareInstance.props[xPos + 1, yPos];
            Resetprevius();
            otherProp.GetComponent<Prop>().xPos --;
            xPos +=1;
        } 
        else if ((swipeAngle > 135 || swipeAngle <= -135) && xPos > 0)  
        {
            //Debug.Log("swipe left");
            //swipe left
            otherProp = BoardManager.ShareInstance.props[xPos - 1, yPos];
            Resetprevius();
            otherProp.GetComponent<Prop>().xPos ++;
            xPos -=1;
            
        } 
        else if(swipeAngle > 45 && swipeAngle < 135 && yPos < (BoardManager.ShareInstance.ySize-1)) 
        {
            //Debug.Log("swipe up");
            //swipe up
            otherProp = BoardManager.ShareInstance.props[xPos, yPos + 1];
            Resetprevius();
            otherProp.GetComponent<Prop>().yPos --;
            yPos +=1; 
            
        }
        else if(swipeAngle < -45 && swipeAngle >= -135 && yPos > 0 )
        {
            //Debug.Log("swipe down");
            //swipe down
            otherProp = BoardManager.ShareInstance.props[xPos, yPos - 1];
            Resetprevius();
            otherProp.GetComponent<Prop>().yPos ++;
            yPos -=1;
        }
        StartCoroutine(CheckMove());
    }

     private IEnumerator CheckMove(){
        Debug.Log("nomatch");
        yield return new WaitForSeconds(.4f);
        if (otherProp != null)
        {
            if (!FindAllMatches() &&  !otherProp.GetComponent<Prop>().FindAllMatches())
            {
                otherProp.GetComponent<Prop>().xPos = xPos;
                otherProp.GetComponent<Prop>().yPos = yPos;
                xPos = previusX;
                yPos = previusY;
            }else {
                Debug.Log("match");
                StopCoroutine(BoardManager.ShareInstance.FindNullProps());
                StartCoroutine(BoardManager.ShareInstance.FindNullProps());
            }
            otherProp = null;   
        }
     }


    private GameObject GetNeightbor( Vector2 direction){
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position,direction);
        if (hit.collider != null)
        {
             return hit.collider.gameObject;
        }else{
            return null;}
        
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
//                Debug.Log(prop.GetComponent<SpriteRenderer>().sprite.name);
                prop.GetComponent<SpriteRenderer>().sprite=null;
                Destroy(BoardManager.ShareInstance.props[prop.GetComponent<Prop>().xPos, prop.GetComponent<Prop>().yPos]);
                BoardManager.ShareInstance.props[prop.GetComponent<Prop>().xPos, prop.GetComponent<Prop>().yPos] = null;
            }
            Destroy(BoardManager.ShareInstance.props[xPos, yPos] );
            BoardManager.ShareInstance.props[xPos, yPos] = null;

            return true;
        }else{
            return false;
        }
    }

    public bool FindAllMatches(){

       
        //Debug.Log("Entra a metodo FindAllMatches");
        if (spriteRenderer.sprite == null) return false;
       
        bool hMatch = ClearMatch(new Vector2[2]{Vector2.left,Vector2.right});
        bool vMatch = ClearMatch(new Vector2[2]{Vector2.up,Vector2.down});

        if (hMatch || vMatch){

            return true;
            
        } else {
            return false;
        }
    }

    public void Resetprevius() {
//        Debug.Log("reset previus");
        previusX = xPos;
        previusY = yPos;
    }
}
