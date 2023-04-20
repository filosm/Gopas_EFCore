using System;

namespace Gopas.Entities;

public class Duration
{
    public TimeSpan Time { get; set; }

    public Duration(int ms)
    {
        Time = TimeSpan.FromMilliseconds(ms);
    }
}
