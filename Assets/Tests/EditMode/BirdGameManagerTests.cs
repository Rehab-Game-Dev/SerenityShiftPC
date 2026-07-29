using NUnit.Framework;
using UnityEngine;

/// <summary>Tests for the bird-catch counting logic in <see cref="BirdGameManager"/>.</summary>
public class BirdGameManagerTests
{
    private GameObject testObject;
    private BirdGameManager birdGameManager;

    [SetUp]
    public void SetUp()
    {
        testObject = new GameObject("BirdGameManagerTestObject");
        birdGameManager = testObject.AddComponent<BirdGameManager>();
        birdGameManager.totalBirds = 3;
    }

    [TearDown]
    public void TearDown()
    {
        if (testObject != null) Object.DestroyImmediate(testObject);
    }

    [Test]
    public void BirdCaught_IncrementsCaughtCount()
    {
        birdGameManager.BirdCaught();

        Assert.AreEqual(1, birdGameManager.caughtCount);
    }

    [Test]
    public void BirdCaught_CalledUntilThreshold_CaughtCountMatchesTotal()
    {
        for (int i = 0; i < birdGameManager.totalBirds; i++)
        {
            birdGameManager.BirdCaught();
        }

        Assert.AreEqual(birdGameManager.totalBirds, birdGameManager.caughtCount);
    }

    [Test]
    public void BirdCaught_PastThreshold_KeepsCountingRatherThanCapping()
    {
        for (int i = 0; i < birdGameManager.totalBirds + 2; i++)
        {
            birdGameManager.BirdCaught();
        }

        Assert.AreEqual(birdGameManager.totalBirds + 2, birdGameManager.caughtCount);
    }
}
