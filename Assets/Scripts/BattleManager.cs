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

    private int turnIndex = 0;


    void Awake() {
        instances = gameManager.GetComponent<InstantiateCharacters>();
    }

    void Start() {
        allCharacters = instances.AlliesList;
        allPortraits = instances.allPortraits;
    }

    public void NextTurnStep() {
        deadTemp = allCharacters.FindAll(c => c.GetComponent<AllyControl>().health <= 0);
        foreach (GameObject c in deadTemp) {
            Destroy(c);
            int itemp = allCharacters.IndexOf(c);
            GameObject portrait = GetPortrait(itemp);
            portrait.SetActive(false);
            allCharacters.Remove(c);
        }
        turnIndex = (turnIndex + 1) % allCharacters.Count;
        UpdateTurnOrder();
    }

    public void UpdateTurnOrder() {
        allCharacters.Sort((GameObject x, GameObject y) => x.GetComponent<AllyControl>().initiative.CompareTo(y.GetComponent<AllyControl>().initiative));
        allCharacters[turnIndex].GetComponent<AllyControl>().Select();
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
}