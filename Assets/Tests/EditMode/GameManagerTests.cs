using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Tests for the NPC-catch counting and phase-transition threshold in <see cref="GameManager"/>.
/// All UI/bird references are left unassigned (null) - GameManager guards every one of them,
/// so this exercises the counting logic in isolation without needing to wire up a scene.
/// </summary>
public class GameManagerTests
{
    private GameObject testObject;
    private GameManager gameManager;

    [SetUp]
    public void SetUp()
    {
        testObject = new GameObject("GameManagerTestObject");
        gameManager = testObject.AddComponent<GameManager>();
        gameManager.totalNPCs = 3;
    }

    [TearDown]
    public void TearDown()
    {
        if (testObject != null) Object.DestroyImmediate(testObject);
    }

    [Test]
    public void NPCCaught_IncrementsCaughtCount()
    {
        gameManager.NPCCaught();

        Assert.AreEqual(1, gameManager.caughtCount);
    }

    [Test]
    public void NPCCaught_CalledRepeatedly_AccumulatesCount()
    {
        gameManager.NPCCaught();
        gameManager.NPCCaught();

        Assert.AreEqual(2, gameManager.caughtCount);
    }

    [Test]
    public void NPCCaught_BelowThreshold_DoesNotThrowWithUnassignedReferences()
    {
        Assert.DoesNotThrow(() => gameManager.NPCCaught());
    }

    [Test]
    public void NPCCaught_ReachingThreshold_DoesNotThrowWithUnassignedReferences()
    {
        // Catches enough NPCs to cross totalNPCs, triggering the UI-swap/bird-activation
        // branch. Should still be safe since birdManager/birdSpawner/UI fields are null.
        Assert.DoesNotThrow(() =>
        {
            for (int i = 0; i < gameManager.totalNPCs; i++)
            {
                gameManager.NPCCaught();
            }
        });

        Assert.AreEqual(gameManager.totalNPCs, gameManager.caughtCount);
    }
}
