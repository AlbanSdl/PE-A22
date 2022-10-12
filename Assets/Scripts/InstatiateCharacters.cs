using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InstatiateCharacters : MonoBehaviour
{
    AllyControl AllyData;
    public GameObject mapManager;
    public GameObject AllyPrefabA;
    public GameObject AllyPrefabB;
    public int N;
    public List<GameObject> AlliesList;
    private int index;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnA() {
        AlliesList.Add(Instantiate(AllyPrefabA));
        index = AlliesList.Count-1;
        AllyControl ally = AlliesList[index].GetComponent<AllyControl>();
        ally.Start();
        ally.mapManager = this.mapManager;
        AlliesList[index].GetComponent<SpriteRenderer>().sprite = AlliesList[index].GetComponent<AllyControl>().sprite;
    }

    public void SpawnB() {
        AlliesList.Add(Instantiate(AllyPrefabB));
        index = AlliesList.Count-1;
        AllyControl ally = AlliesList[index].GetComponent<AllyControl>();
        ally.Start();
        ally.mapManager = this.mapManager;
        AlliesList[index].GetComponent<SpriteRenderer>().sprite = AlliesList[index].GetComponent<AllyControl>().sprite;
    }
}
