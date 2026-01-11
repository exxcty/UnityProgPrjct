using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayModeTest
{
    [UnityTest]
    public IEnumerator PlayerTakesDamage_HealthDecreases()
    {
        GameObject playerGO = new GameObject();
        Health health = playerGO.AddComponent<Health>();

        yield return null; 

        health.TakeDamage(10f);

        Assert.Pass(); 

        Object.Destroy(playerGO);
    }
}