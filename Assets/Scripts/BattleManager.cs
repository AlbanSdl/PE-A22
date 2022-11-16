using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public GameObject gameManager;
    InstantiateCharacters instances;
    public List<GameObject> allCharacters;
    public List<GameObject> deadTemp;
    GameObject[] allPortraits;
    public AllyControl ally;

    private int enemiesAlive = 1;
    private int turnIndex = 0;


    void Awake() {
        instances = gameManager.GetComponent<InstantiateCharacters>();
    }

    void Start() {
        allCharacters = instances.CharacterList;
        allPortraits = instances.allPortraits;
    }

    public void NextTurnStep() {
        deadTemp = allCharacters.FindAll(c => (c.GetComponent<AllyControl>()?.health ?? 0) <= 0 && (c.GetComponent<EnemyControl>()?.health ?? 0) <= 0);
        foreach (GameObject c in deadTemp) {
            AllyControl ally = c.GetComponent<AllyControl>();
            Destroy(c);
            int itemp = allCharacters.IndexOf(c);
            GameObject portrait = GetPortrait(itemp);
            portrait.SetActive(false);
            allCharacters.Remove(c);
            instances.AlliesList.Remove(c); // Make sure it is removed from the proper List
            instances.EnemiesList.Remove(c); // Make sure it is removed from the proper List
            // Remove player from Enemy "memory"
            if (ally != null) foreach (GameObject enemy in instances.EnemiesList)
                enemy.GetComponent<EnemyControl>().Memory.Remove(ally);
        }
        foreach (GameObject e in allCharacters) {
            if (e.GetComponent<EnemyControl>() != null) {
                enemiesAlive ++;
            }
        }
        if (enemiesAlive == 0) {
            Debug.Log("Vous avez terrassé toute l'armée adverse !");
        }
        enemiesAlive = 0;
        turnIndex = (turnIndex + 1) % allCharacters.Count;
        UpdateTurnOrder();
    }

    public void UpdateTurnOrder() {
        allCharacters.Sort((GameObject x, GameObject y) => (
            x.GetComponent<AllyControl>()?.initiative ?? x.GetComponent<EnemyControl>()!.initiative).CompareTo(
                y.GetComponent<AllyControl>()?.initiative ?? y.GetComponent<EnemyControl>()!.initiative));
        AllyControl ally = allCharacters[turnIndex].GetComponent<AllyControl>();
        if (ally != null) ally.Select();
        else allCharacters[turnIndex].GetComponent<EnemyControl>().Select();

        for (int i = 0; i < allCharacters.Count; i++) {
            GameObject portrait = GetPortrait(i);
            portrait.SetActive(i >= turnIndex);
            if (portrait != null) {
                portrait.GetComponent<RectTransform>().localPosition = new Vector3(
                    (i - turnIndex) * 100,
                    portrait.GetComponent<RectTransform>().localPosition.y,
                    portrait.GetComponent<RectTransform>().localPosition.z
                );
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

    public void Attack() {
        this.ally.Attack();
    }

    public void Defend() {
        this.ally.Defend();
    }

    public void Wait() {
        this.ally.Wait();
    }
}