using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InstantiateCharacters : MonoBehaviour
{
    AllyControl AllyData;
    public GameObject mapManager;
    public GameObject AllyPrefabA;
    public GameObject AllyPrefabB;
    public int N;
    public List<GameObject> AlliesList;
    private int index;
    GameObject[] allPortraits;

    void Awake()
    {
        allPortraits = GameObject.FindGameObjectsWithTag("Portrait");
        foreach (GameObject portrait in allPortraits) {
            Debug.Log(portrait);
            portrait.SetActive(false);
        }
    }

    void Update()
    {
        
    }

    public void SpawnA() {
        AlliesList.Add(Instantiate(AllyPrefabA));
        index = AlliesList.Count-1;
        AllyControl ally = AlliesList[index].GetComponent<AllyControl>();
        ally.Awake();
        ally.mapManager = this.mapManager;
        AlliesList[index].GetComponent<SpriteRenderer>().sprite = AlliesList[index].GetComponent<AllyControl>().sprite;
        ally.Select();
        ShowPortrait(index);
    }

    public void SpawnB() {
        AlliesList.Add(Instantiate(AllyPrefabB));
        index = AlliesList.Count-1;
        AllyControl ally = AlliesList[index].GetComponent<AllyControl>();
        ally.Awake();
        ally.mapManager = this.mapManager;
        AlliesList[index].GetComponent<SpriteRenderer>().sprite = AlliesList[index].GetComponent<AllyControl>().sprite;
        ally.Select();
        ShowPortrait(index);
    }

    public void ShowPortrait(int ind) {
    foreach (GameObject portrait in allPortraits) {
            if (AlliesList[ind].GetComponent<SpriteRenderer>().sprite == portrait.GetComponent<Image>().sprite) {
                portrait.SetActive(true);
                break;
            }
        }
    }
}
