namespace Loupedeck.WorldClock.Helpers;

using Timer = System.Timers.Timer;

public static class ClockTimer
{
    public static event EventHandler ClockTick;
    
    private static Timer _timer;
    
    public static void Init()
    {
        _timer = new Timer();
        
        DateTime currentTime = DateTime.Now;
        var secondsToWait = 60 - currentTime.Second;

        // Delay to the top of the next minute (in milliseconds)
        _timer.Interval = secondsToWait * 1000;
        
        _timer.Elapsed += (_, _) =>
        {
            ClockTick?.Invoke(null, null!);
            _timer.Interval = 60000;
        };
        _timer.AutoReset = true;
        _timer.Enabled = true;
    }
}