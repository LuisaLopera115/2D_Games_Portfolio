using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    private Color selectedColor = new Color(0f,0f,0f);
    public static Prop previusSelected = null;
    
    public ParticleSystem PropExplotion;
    private SpriteRenderer spriteRenderer;
    private bool isSelected = false;
    public bool isMatch;

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
        isMatch = false;
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
        if (BoardManager.ShareInstance.currentState == GmaeStates.move) {
            secondTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateTheAngle();
        }
    }
    
    private void CalculateTheAngle(){
        if (Mathf.Abs(firstTouchPosition.y-secondTouchPosition.y) > swipeLimit ||
            Mathf.Abs(firstTouchPosition.x-secondTouchPosition.x) > swipeLimit)
        {
            swipeAngle = Mathf.Atan2(secondTouchPosition.y - firstTouchPosition.y,  secondTouchPosition.x - firstTouchPosition.x) * Mathf.Rad2Deg;
            SwapProps();
            BoardManager.ShareInstance.currentState = GmaeStates.wait;
        }else
        {
            BoardManager.ShareInstance.currentState = GmaeStates.move;
        }
    }

    private void Update()
    {
        xPosTarget=xPos;
        yPosTarget=yPos;


        if(Mathf.Abs( this.transform.position.x - xPosTarget) > .01f){
            tempPos = new Vector2(xPosTarget, transform.position.y);
            this.transform.position = Vector2.Lerp(this.transform.position,tempPos, .4f);
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

        if(Mathf.Abs(yPosTarget - transform.position.y) > .01){
            tempPos = new Vector2(transform.position.x, yPosTarget);
            this.transform.position = Vector2.Lerp(this.transform.position,tempPos, .4f);
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

        BoardManager.ShareInstance.FindMatches();
        yield return new WaitForSeconds(.4f);

        if (otherProp != null)
        {
            if (!BoardManager.ShareInstance.FindMatches())
            {
                otherProp.GetComponent<Prop>().xPos = xPos;
                otherProp.GetComponent<Prop>().yPos = yPos;
                xPos = previusX;
                yPos = previusY;
                Debug.Log("nomatch");
                yield return new WaitForSeconds(.2f);
                BoardManager.ShareInstance.currentState = GmaeStates.move;
            }else {
                Debug.Log("match");
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

    public void Resetprevius() {
        previusX = xPos;
        previusY = yPos;
    }
}
