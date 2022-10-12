using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public GameObject gameManager;
    InstantiateCharacters instances;
    public List<GameObject> allCharacters;
    GameObject[] allPortraits;


    void Awake() {
        instances = gameManager.GetComponent<InstantiateCharacters>();
        allPortraits = GameObject.FindGameObjectsWithTag("Portrait");
    }

    void Start()
    {
        allCharacters = instances.AlliesList;

    }

    void Update()
    {
        
    }

    void TurnOrder() {
            allCharacters.Sort((GameObject x, GameObject y) => x.GetComponent<AllyControl>().initiative.CompareTo(y.GetComponent<AllyControl>().initiative));
            for (int i = 0; i < allCharacters.Count; i++) {
                GameObject portrait = GetPortrait(i);
                if (portrait != null) {
                    portrait.GetComponent<RectTransform>().position = new Vector3(portrait.GetComponent<RectTransform>().position.x-100*i, portrait.GetComponent<RectTransform>().position.y, portrait.GetComponent<RectTransform>().position.z);
                }
            }
        }

    public GameObject GetPortrait(int ind) {
        foreach (GameObject portrait in allPortraits) {
            if (allCharacters[ind].GetComponent<SpriteRenderer>().sprite == portrait.GetComponent<Image>().sprite) {
                return portrait;
            }
        }
        return null;
    }
}