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
    public GameObject EnemyPrefab;
    public int N;
    public List<GameObject> AlliesList;
    public List<GameObject> EnemiesList;
    public List<GameObject> CharacterList;
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
        CharacterList.Add(ally.gameObject);
        ally.Awake();
        ally.battleMenu = tempBattleMenu;
        ally.mapManager = this.mapManager;
        ally.gameManager = this.gameManager;
        ally.instances = this;
        AlliesList[index].GetComponent<SpriteRenderer>().sprite = AlliesList[index].GetComponent<AllyControl>().sprite;
        ShowPortrait(index);
        BattleManager.GetComponent<BattleManager>().UpdateTurnOrder();
        ally.instances = this;
    }

    public void SpawnB() {
        if (AlliesList.Find((obj) => obj.GetComponent<AllyControl>().sprite == AllyPrefabB.GetComponent<AllyControl>().AllyData.Sprite)) {
            Debug.LogWarning("Character already summonned.");
            return;
        }
        AlliesList.Add(Instantiate(AllyPrefabB));
        index = AlliesList.Count-1;
        AllyControl ally = AlliesList[index].GetComponent<AllyControl>();
        CharacterList.Add(ally.gameObject);
        ally.gameManager = this.gameManager;
        ally.Awake();
        ally.battleMenu = tempBattleMenu;
        ally.mapManager = this.mapManager;
        ally.instances = this;
        AlliesList[index].GetComponent<SpriteRenderer>().sprite = AlliesList[index].GetComponent<AllyControl>().sprite;
        ShowPortrait(index);
        BattleManager.GetComponent<BattleManager>().UpdateTurnOrder();
        ally.instances = this;
    }

    public void SpawnEnemy() {
        EnemiesList.Add(Instantiate(EnemyPrefab));
        index = EnemiesList.Count-1;
        EnemyControl enemy = EnemiesList[index].GetComponent<EnemyControl>();
        CharacterList.Add(enemy.gameObject);
        enemy.Awake();
        enemy.mapManager = this.mapManager;
        enemy.gameManager = this.gameManager;
        enemy.instances = this;
        enemy.GetComponent<SpriteRenderer>().sprite = enemy.sprite;
        // ShowPortrait(index);
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
