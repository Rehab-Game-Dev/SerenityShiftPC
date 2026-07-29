using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

/// <summary>Tests for the random bird-pool activation logic in <see cref="BirdManager"/>.</summary>
public class BirdManagerTests
{
    private GameObject root;
    private BirdManager birdManager;

    [SetUp]
    public void SetUp()
    {
        root = new GameObject("BirdManagerTestRoot");
        birdManager = root.AddComponent<BirdManager>();
    }

    [TearDown]
    public void TearDown()
    {
        if (root != null) Object.DestroyImmediate(root); // also destroys the child bird GameObjects
    }

    private List<GameObject> CreateBirdPool(int count)
    {
        var birds = new List<GameObject>();
        for (int i = 0; i < count; i++)
        {
            GameObject bird = new GameObject($"Bird{i}");
            bird.transform.SetParent(root.transform);
            bird.SetActive(false); // mirrors the pool's normal starting state (deactivated by BirdManager.Start)
            birds.Add(bird);
        }
        return birds;
    }

    [Test]
    public void ActivateBirds_PoolLargerThanAmount_ActivatesExactlyAmountToSpawn()
    {
        birdManager.birdsInScene = CreateBirdPool(10);
        birdManager.amountToSpawn = 4;

        birdManager.ActivateBirds();

        int activeCount = birdManager.birdsInScene.Count(b => b.activeSelf);
        Assert.AreEqual(4, activeCount);
    }

    [Test]
    public void ActivateBirds_PoolSmallerThanAmount_ActivatesEntirePoolWithoutError()
    {
        birdManager.birdsInScene = CreateBirdPool(3);
        birdManager.amountToSpawn = 10;

        Assert.DoesNotThrow(() => birdManager.ActivateBirds());

        int activeCount = birdManager.birdsInScene.Count(b => b.activeSelf);
        Assert.AreEqual(3, activeCount);
    }

    [Test]
    public void ActivateBirds_DoesNotActivateTheSameBirdTwice()
    {
        birdManager.birdsInScene = CreateBirdPool(5);
        birdManager.amountToSpawn = 5;

        birdManager.ActivateBirds();

        // With amountToSpawn == pool size, every bird should end up active exactly once.
        Assert.IsTrue(birdManager.birdsInScene.All(b => b.activeSelf));
    }
}
