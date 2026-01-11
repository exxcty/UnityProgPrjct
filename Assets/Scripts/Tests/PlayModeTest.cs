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

        float initialHealth = health.CurrentHealth;
        health.TakeDamage(10f);

        Assert.AreEqual(initialHealth - 10f, health.CurrentHealth);

        Object.Destroy(playerGO);
    }
}