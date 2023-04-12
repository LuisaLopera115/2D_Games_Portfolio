using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauldronManager : MonoBehaviour
{
    public static CauldronManager ShareInstance;
    public GameObject[] ingredients;
    public GameObject[] cauldrons;
    public List<Sprite> prefabs = new List<Sprite>();

    int idx;

    void Start()
    {
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

        ingredients = GameObject.FindGameObjectsWithTag("Ingredient");
        cauldrons = GameObject.FindGameObjectsWithTag("Cauldron");
        
        idx = -1;
         
         for (int i = 0; i < ingredients.Length; i++)
         {
            idx = Random.Range(0,prefabs.Count);
            Sprite sprite = prefabs[idx];
            ingredients[i].GetComponent<SpriteRenderer>().sprite = sprite;
            ingredients[i].GetComponent<IngredientProp>().id = idx;
         }
    }

    public void CompareIngredientsWhitMatch(){

        for (int i = 0; i < ingredients.Length; i++)
        {
            if (ingredients[i] != null)
            {
                if (ingredients[i].GetComponent<IngredientProp>().id == BoardManager.ShareInstance.MatchId)
                {
                    ingredients[i].GetComponent<IngredientProp>().circle.color = new Color (80/255,255/255,80/255, 0.5f);
                    BoardManager.ShareInstance.MatchId = -1;
                    ingredients[i].GetComponent<IngredientProp>().completed = true;
                    ingredients[i].GetComponent<IngredientProp>().id = -1;

                    foreach (var cauldron in cauldrons)
                    {
                        cauldron.GetComponent<Cauldron>().ChechifComplete();
                    }
                }
            }
        }
    }

    public void RefillCauldron(){

        for (int i = 0; i < ingredients.Length; i++)
        {
            if (ingredients[i].GetComponent<SpriteRenderer>().sprite == null)
            {
                idx = Random.Range(0,prefabs.Count);
                Sprite sprite = prefabs[idx];

                ingredients[i].GetComponent<SpriteRenderer>().sprite = sprite;
                ingredients[i].GetComponent<IngredientProp>().id = idx;

                ingredients[i].GetComponent<IngredientProp>().circle.color = new Color (255/255,203/255,203/255, 0.5f);
                BoardManager.ShareInstance.MatchId = -1;
                ingredients[i].GetComponent<IngredientProp>().completed = false;

            }
        }
    }
}
