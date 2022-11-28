using System.Collections.Generic;
using System.Linq;
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
    public GameObject startGameButton;
    public int enemyCount = 2;
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

    private void SpawnAlly(GameObject prefab, Vector2Int location) {
        if (AlliesList.Find((obj) => obj.GetComponent<AllyControl>().sprite == prefab.GetComponent<AllyControl>().AllyData.Sprite)) {
            Debug.LogWarning("Character already summonned.");
            return;
        }
        AlliesList.Add(Instantiate(prefab));
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
        ally.SetTilePosition(location);
        ally.instances = this;
    }

    private void SpawnEnemy(Vector2Int location) {
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
        enemy.SetTilePosition(location);
    }

    public void InitializeGame() {
        startGameButton.SetActive(false);
        this.InitializeAllies();
        this.InitializeEnemies(enemyCount);
        BattleManager.GetComponent<BattleManager>().UpdateTurnOrder();
    }

    public void InitializeAllies() {
        SpawnAlly(AllyPrefabA, new Vector2Int(1, 0));
        SpawnAlly(AllyPrefabB, new Vector2Int(0, 0));
    }

    public void InitializeEnemies(int count) {
        MapManager mapManager = this.mapManager.GetComponent<MapManager>();
        List<SelectorTile> tileList = new List<GameObject>(mapManager.map.Values)
            .Select(tile => tile.GetComponent<SelectorTile>())
            .Where(tile => tile.Location.z > 0)
            .ToList();
        for (int i = 0; i < count; i++) {
            // Choose coordinates where to spawn the enemy
            int index = Random.Range(0, tileList.Count);
            SelectorTile tile = tileList[index];
            tileList.RemoveAt(index);
            SpawnEnemy((Vector2Int) tile.Location);
        }
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
