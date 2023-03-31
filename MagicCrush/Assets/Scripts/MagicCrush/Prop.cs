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
    public GameObject otherProp; 
    private bool reverseSwipe = false;
    public bool fall = false;

    private Vector2[] adjacentDirections= new Vector2[]{
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right
    };


    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // track the position if the current prop and also init their targets
       xPosTarget = (int)transform.position.x;
       yPosTarget = (int)transform.position.y;
       xPos=xPosTarget;
       yPos=yPosTarget;
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
        SwapProps();
    }
    
    private void CalculateTheAngle(){
        swipeAngle = Mathf.Atan2(secondTouchPosition.y - firstTouchPosition.y,  secondTouchPosition.x - firstTouchPosition.x) * Mathf.Rad2Deg;
        //Debug.Log(swipeAngle);
    }

    private void Update()
    {
        xPosTarget=xPos;
        yPosTarget=yPos;
    if(fall){

        if(Mathf.Abs(xPosTarget - transform.position.x) > .1){
            tempPos = new Vector2(xPosTarget, transform.position.y);
            transform.position = Vector2.Lerp(transform.position,tempPos, .4f);
        }else{
            tempPos = new Vector2(xPosTarget, transform.position.y);
            transform.position = tempPos;
            BoardManager.ShareInstance.props[xPos,yPos] = this.gameObject;

        }

        if(Mathf.Abs(yPosTarget - transform.position.y) > .1){
            tempPos = new Vector2(transform.position.x, yPosTarget);
            transform.position = Vector2.Lerp(transform.position,tempPos, .4f);
        }else{
            initialPosition1 = new Vector2(transform.position.x, yPosTarget);
            transform.position = tempPos;
            BoardManager.ShareInstance.props[xPos,yPos] = this.gameObject;

        }
    }
    }

    private void SwapProps(){

       // Debug.Log("SwapProps");

        if (swipeAngle > -45 && swipeAngle <= 45 && xPos < (BoardManager.ShareInstance.xSize-1)) 
        {
            //swipe rigth
            otherProp = BoardManager.ShareInstance.props[xPos + 1, yPos];
            StartCoroutine(StartSwapPropsMovement(otherProp, 'x', true));
        } 
        else if ((swipeAngle > 135 || swipeAngle <= -135) && xPos > 0)  
        {
            //Debug.Log("swipe left");
            //swipe left
            otherProp = BoardManager.ShareInstance.props[xPos - 1, yPos];
            StartCoroutine(StartSwapPropsMovement(otherProp, 'x', false));
            
        } 
        else if(swipeAngle > 45 && swipeAngle < 135 && yPos < (BoardManager.ShareInstance.ySize-1)) 
        {
            //Debug.Log("swipe up");
            //swipe up
            otherProp = BoardManager.ShareInstance.props[xPos, yPos + 1];
            //StartCoroutine(StartSwapPropsMovement(otherProp));
            StartCoroutine(StartSwapPropsMovement(otherProp, 'y', true));
                
            
        }
        else if(swipeAngle < -45 && swipeAngle >= -135 && yPos > 0 )
        {
            //Debug.Log("swipe down");
            //swipe down
            otherProp = BoardManager.ShareInstance.props[xPos, yPos - 1];
            //StartCoroutine(StartSwapPropsMovement(otherProp));
            StartCoroutine(StartSwapPropsMovement(otherProp, 'y', false));
                
            
        }
    }

     private IEnumerator StartSwapPropsMovement(GameObject otherProp, char xy, bool plus){
        
        //Debug.Log("other pos: " + otherProp.transform.position + "sprite: " + otherProp.GetComponent<SpriteRenderer>().sprite );

        float swapSpeed = 0.5f;

        initialPosition1 = otherProp.GetComponent<Transform>().position;
        initialPosition2 = this.transform.position;

        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime/swapSpeed;

            otherProp.GetComponent<Transform>().position = Vector3.Lerp(initialPosition1, initialPosition2, t);
            this.transform.position = Vector3.Lerp(initialPosition2, initialPosition1, t);
            
            yield return null;
        }

        //Debug.Log("Swipe 1");

        if(!FindAllMatches() &&  !otherProp.GetComponent<Prop>().FindAllMatches() ){
            Debug.Log("funciona");

            reverseSwipe = true;
            t = 0.0f;
            while (t < 1.0f)
            {
                t += Time.deltaTime/swapSpeed;

                otherProp.GetComponent<Transform>().position = Vector3.Lerp(initialPosition2, initialPosition1, t);
                this.transform.position = Vector3.Lerp(initialPosition1, initialPosition2, t);
                
                yield return null;
            }
            BoardManager.ShareInstance.props[xPos,yPos] = this.gameObject;
            BoardManager.ShareInstance.props[otherProp.GetComponent<Prop>().xPos ,otherProp.GetComponent<Prop>().yPos] = otherProp.gameObject;
        } else
        {
            if (xy == 'x' && plus){
                otherProp.GetComponent<Prop>().xPos --;
                xPos +=1;
            }else if (xy == 'x' && !plus){
                otherProp.GetComponent<Prop>().xPos ++;
                xPos -=1;
            }else if (xy == 'y' && plus){
//                Debug.Log("up");
                otherProp.GetComponent<Prop>().yPos --;
                yPos +=1;
            }else if(xy == 'y' && !plus){
          //      Debug.Log("down");
                otherProp.GetComponent<Prop>().yPos ++;
                yPos -=1;
            }

            BoardManager.ShareInstance.props[xPos,yPos] = this.gameObject;
            BoardManager.ShareInstance.props[otherProp.GetComponent<Prop>().xPos ,otherProp.GetComponent<Prop>().yPos] = otherProp.gameObject;

            StopCoroutine(BoardManager.ShareInstance.FindNullProps());
            StartCoroutine(BoardManager.ShareInstance.FindNullProps());
        }
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
}
