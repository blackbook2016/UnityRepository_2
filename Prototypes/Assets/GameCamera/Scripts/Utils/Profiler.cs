// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System.Collections.Generic;
using System.Diagnostics;

namespace RG_GameCamera.Utils
{
    static public class Profiler
    {
        private static readonly Dictionary<string, Stopwatch> timeSegments = new Dictionary<string, Stopwatch>();

        static public void Start(string key)
        {
            Stopwatch timer = null;

            if (timeSegments.TryGetValue(key, out timer))
            {
                timer.Reset();
                timer.Start();
            }
            else
            {
                timer = new Stopwatch();
                timer.Start();
                timeSegments.Add(key, timer);
            }
        }

        static public void Stop(string key)
        {
            timeSegments[key].Stop();
        }

        static public string[] GetResults()
        {
            var result = new string[timeSegments.Count];
            var i = 0;

            foreach (var timeSegment in timeSegments)
            {
                var milliseconds = timeSegment.Value.ElapsedMilliseconds;
                var microseconds = timeSegment.Value.ElapsedTicks / (Stopwatch.Frequency / (1000L * 1000L));

                result[i++] = timeSegment.Key + " " + milliseconds + " [ms] | " + microseconds + " [us]";
            }

            return result;
        }
    }
}
