﻿using System;

namespace HHW.Service
{
    public static class TimeHelper
    {
        private static readonly long epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;

        public static long ClientNow()
        {
            return (DateTime.UtcNow.Ticks - epoch) / 10000;
        }

        public static long ClientNowSeconds()
        {
            return (DateTime.UtcNow.Ticks - epoch) / 10000000;
        }

        public static long Now()
        {
            return ClientNow();
        }
    }
}
