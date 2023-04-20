using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CauldronManager : MonoBehaviour
{
    public static CauldronManager ShareInstance;
    public GameObject[] ingredients;
    public GameObject[] cauldrons;
    public Sprite EmptyCircle;
    int idx;

    void Start()
    {
        deledeCualdrons();
        if(ShareInstance == null){
            ShareInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        idx = -1;
    }
    
     public void InitIngredients(){

        //ingredients = null;
        //cauldrons = null;
        Debug.Log("reset the array for cauldrons and ingredients");
        
        ingredients = GameObject.FindGameObjectsWithTag("Ingredient");
        cauldrons = GameObject.FindGameObjectsWithTag("Cauldron");

        for (int i = 0; i < ingredients.Length; i++)
        {
            if (ingredients[i] != null)
            {
                Debug.Log(ingredients[i]);
            }
        }
        
        Debug.Log(" finish reseting the array for cauldrons and ingredients");
        idx = -1;
         
         for (int i = 0; i < ingredients.Length; i++)
         {
            idx = Random.Range(0,BoardManager.ShareInstance.ingredienstSprites.Length);
//            Debug.Log(idx);

            Sprite sprite = BoardManager.ShareInstance.ingredienstSprites[idx];
            ingredients[i].GetComponent<Image>().sprite = sprite;
            ingredients[i].GetComponent<IngredientProp>().circle.color = new Color(1f,
                                                                            .7f,
                                                                            .7f,
                                                                            1f);
            ingredients[i].GetComponent<IngredientProp>().id = idx;
         }

         Debug.Log(" random INGREDIENTS");
    }

    public void CompareIngredientsWhitMatch(){

        for (int i = 0; i < ingredients.Length; i++)
        {
            if (ingredients[i] != null)
            {
                if (ingredients[i].GetComponent<IngredientProp>().id == BoardManager.ShareInstance.MatchId)
                {
                    ingredients[i].GetComponent<IngredientProp>().circle.color = new Color (.4f,1,.4f, 0.5f);
                    BoardManager.ShareInstance.MatchId = -1;
                    ingredients[i].GetComponent<IngredientProp>().completed = true;
                    ingredients[i].GetComponent<IngredientProp>().id = -1;
                    GUIManager.sharedInstance.Score += 50;

                    foreach (var cauldron in cauldrons)
                    {
                        cauldron.GetComponent<Cauldron>().ChechifComplete();
                    }
                }
            }
        }
    }

    public void RefillCauldron(){
        StartCoroutine(RefillCauldronCo());
    }

    private IEnumerator RefillCauldronCo(){

        yield return new WaitForSeconds(3f);

        for (int i = 0; i < ingredients.Length; i++)
        {
            if (ingredients[i].GetComponent<Image>().sprite == EmptyCircle)
            {
                idx = Random.Range(0,BoardManager.ShareInstance.ingredienstSprites.Length);
                Sprite sprite = BoardManager.ShareInstance.ingredienstSprites[idx];

                ingredients[i].GetComponent<Image>().sprite = sprite;
                ingredients[i].GetComponent<IngredientProp>().id = idx;

                ingredients[i].GetComponent<IngredientProp>().circle.color = new Color (1,.7f,.7f);
                BoardManager.ShareInstance.MatchId = -1;
                ingredients[i].GetComponent<IngredientProp>().completed = false;

            }
        }
    }

    public void ResetIngredient(){
        for (int i = 0; i < ingredients.Length; i++)
        {
            ingredients[i].GetComponent<Image>().sprite = EmptyCircle;
            ingredients[i].GetComponent<IngredientProp>().circle.color = new Color(ingredients[i].GetComponent<IngredientProp>().circle.color.r,
                                                                            ingredients[i].GetComponent<IngredientProp>().circle.color.g,
                                                                            ingredients[i].GetComponent<IngredientProp>().circle.color.b,
                                                                            0f);
        }
    }


    public void InitCauldron(){
        StartCoroutine(InitCauldronCO());
    }
    public IEnumerator InitCauldronCO(){

        deledeCualdrons();
        yield return new WaitForSeconds (0.4f);
        Debug.Log("INIT CAULDRON");
        GameObject newProp = Instantiate(BoardManager.ShareInstance.world.levels[BoardManager.ShareInstance.currentLevel].CauldronSection,
                                new Vector2(0,0),Quaternion.identity);
        newProp.transform.parent = this.transform;
        newProp.GetComponent<RectTransform>().position = new Vector3(540,250);
        Debug.Log("---- "+ newProp.name);
        Debug.Log("ready all CAULDRON");
        yield return new WaitForSeconds (0.4f);
        InitIngredients();
    }

    private void deledeCualdrons(){

        Debug.Log("RESET");

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Object.Destroy(transform.GetChild(i).gameObject);
        }

    }
}
