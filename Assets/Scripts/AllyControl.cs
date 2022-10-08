using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyControl : MonoBehaviour
{

    public AlliesData AllyData;

    public Sprite sprite;
    public int health;
    public void Start()
    {
        if (AllyData != null) {
            LoadData(AllyData);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadData (AlliesData data) {
        sprite = data.Sprite;
    }
}
