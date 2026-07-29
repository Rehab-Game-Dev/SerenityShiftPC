using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class SequenceCheckerTests
{
    private static readonly List<string> CorrectSequence = new List<string>
    { "Do", "Mi", "Fa", "Sol", "Fa", "Mi", "Re", "Do" };

    // --- Pure logic: SequenceChecker.IsSequencePrefixMatch ---

    [Test]
    public void IsSequencePrefixMatch_EmptyPlayed_ReturnsTrue()
    {
        Assert.IsTrue(SequenceChecker.IsSequencePrefixMatch(new List<string>(), CorrectSequence));
    }

    [Test]
    public void IsSequencePrefixMatch_CorrectPartialPrefix_ReturnsTrue()
    {
        var played = new List<string> { "Do", "Mi", "Fa" };
        Assert.IsTrue(SequenceChecker.IsSequencePrefixMatch(played, CorrectSequence));
    }

    [Test]
    public void IsSequencePrefixMatch_FullCorrectSequence_ReturnsTrue()
    {
        Assert.IsTrue(SequenceChecker.IsSequencePrefixMatch(CorrectSequence, CorrectSequence));
    }

    [Test]
    public void IsSequencePrefixMatch_WrongNoteAtStart_ReturnsFalse()
    {
        var played = new List<string> { "Sol" };
        Assert.IsFalse(SequenceChecker.IsSequencePrefixMatch(played, CorrectSequence));
    }

    [Test]
    public void IsSequencePrefixMatch_WrongNoteMidway_ReturnsFalse()
    {
        var played = new List<string> { "Do", "Mi", "Re" }; // third note should be "Fa"
        Assert.IsFalse(SequenceChecker.IsSequencePrefixMatch(played, CorrectSequence));
    }

    [Test]
    public void IsSequencePrefixMatch_PlayedLongerThanCorrect_ReturnsFalse()
    {
        var played = new List<string> { "Do", "Mi", "Fa", "Sol", "Fa", "Mi", "Re", "Do", "Do" };
        Assert.IsFalse(SequenceChecker.IsSequencePrefixMatch(played, CorrectSequence));
    }

    // --- Instance behavior: SequenceChecker.NotePressed (wrong-note path only - the
    // success path starts a coroutine, which needs Play Mode rather than Edit Mode) ---

    private GameObject testObject;

    [TearDown]
    public void TearDown()
    {
        if (testObject != null) Object.DestroyImmediate(testObject);
    }

    [Test]
    public void NotePressed_WrongNote_LeavesPuzzleUnsolved()
    {
        testObject = new GameObject("SequenceCheckerTestObject");
        SequenceChecker checker = testObject.AddComponent<SequenceChecker>();

        checker.NotePressed("Sol"); // wrong first note

        Assert.IsFalse(checker.IsPuzzleSolved);
    }

    [Test]
    public void NotePressed_PartialCorrectSequence_DoesNotSolvePuzzleYet()
    {
        testObject = new GameObject("SequenceCheckerTestObject");
        SequenceChecker checker = testObject.AddComponent<SequenceChecker>();

        checker.NotePressed("Do");
        checker.NotePressed("Mi");
        checker.NotePressed("Fa");

        Assert.IsFalse(checker.IsPuzzleSolved);
    }
}
