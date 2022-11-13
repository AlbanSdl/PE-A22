using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InstantiateCharacters : MonoBehaviour
{
    AllyControl AllyData;
    public GameObject mapManager;
    public GameObject gameManager;
    public GameObject[] tempBattleMenu;
    public GameObject[] battleMenu;
    public GameObject AllyPrefabA;
    public GameObject AllyPrefabB;
    public int N;
    public List<GameObject> AlliesList;
    private int index;
    internal GameObject[] allPortraits;
    public GameObject BattleManager;

    void Awake()
    {
        tempBattleMenu = GameObject.FindGameObjectsWithTag("BattleMenu");
        foreach (GameObject menu in tempBattleMenu) {
            menu.SetActive(false);
        } 
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
        if (AlliesList.Find((obj) => obj.GetComponent<AllyControl>().sprite == AllyPrefabA.GetComponent<AllyControl>().AllyData.Sprite)) {
            Debug.LogWarning("Character already summonned");
            return;
        }
        AlliesList.Add(Instantiate(AllyPrefabA));
        index = AlliesList.Count-1;
        AllyControl ally = AlliesList[index].GetComponent<AllyControl>();
        ally.Awake();
        ally.battleMenu = tempBattleMenu;
        ally.mapManager = this.mapManager;
        ally.gameManager = this.gameManager;
        AlliesList[index].GetComponent<SpriteRenderer>().sprite = AlliesList[index].GetComponent<AllyControl>().sprite;
        ShowPortrait(index);
        BattleManager.GetComponent<BattleManager>().UpdateTurnOrder();
    }

    public void SpawnB() {
        if (AlliesList.Find((obj) => obj.GetComponent<AllyControl>().sprite == AllyPrefabB.GetComponent<AllyControl>().AllyData.Sprite)) {
            Debug.LogWarning("Character already summonned.");
            return;
        }
        AlliesList.Add(Instantiate(AllyPrefabB));
        index = AlliesList.Count-1;
        AllyControl ally = AlliesList[index].GetComponent<AllyControl>();
        ally.gameManager = this.gameManager;
        ally.Awake();
        ally.mapManager = this.mapManager;
        AlliesList[index].GetComponent<SpriteRenderer>().sprite = AlliesList[index].GetComponent<AllyControl>().sprite;
        ShowPortrait(index);
        BattleManager.GetComponent<BattleManager>().UpdateTurnOrder();
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
