using NUnit.Framework;

/// <summary>Tests for <see cref="TimerManager.FormatTime"/>, the pure MM:SS formatting logic.</summary>
public class TimerManagerTests
{
    [Test]
    public void FormatTime_Zero_ReturnsZeroZero()
    {
        Assert.AreEqual("00 : 00", TimerManager.FormatTime(0f));
    }

    [Test]
    public void FormatTime_UnderOneMinute_PadsSecondsWithZero()
    {
        Assert.AreEqual("00 : 05", TimerManager.FormatTime(5f));
    }

    [Test]
    public void FormatTime_OverOneMinute_ComputesMinutesAndSeconds()
    {
        Assert.AreEqual("01 : 05", TimerManager.FormatTime(65f));
    }

    [Test]
    public void FormatTime_OverAnHour_DoesNotRollOverIntoHours()
    {
        // 3661 seconds = 61 minutes, 1 second. Minutes are never capped/converted to hours.
        Assert.AreEqual("61 : 01", TimerManager.FormatTime(3661f));
    }

    [Test]
    public void FormatTime_FractionalSeconds_FloorsRatherThanRounds()
    {
        // 59.9s should read as 59s, not round up to a new minute.
        Assert.AreEqual("00 : 59", TimerManager.FormatTime(59.9f));
    }
}
