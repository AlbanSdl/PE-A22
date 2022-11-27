using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public void HealthSize(float healthPoints)
    {
        transform.localScale = new Vector3(healthPoints, 1f);
    }
}
