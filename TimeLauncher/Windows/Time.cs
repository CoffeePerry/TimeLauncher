using System;
using System.Runtime.InteropServices;

namespace TimeLauncher.Windows {
    internal struct SYSTEMTIME {
        internal ushort wYear;
        internal ushort wMonth;
        internal ushort wDayOfWeek;
        internal ushort wDay;
        internal ushort wHour;
        internal ushort wMinute;
        internal ushort wSecond;
        internal ushort wMilliseconds;

        internal void FromDateTime(DateTime dateTime) {
            wYear = (ushort)dateTime.Year;
            wMonth = (ushort)dateTime.Month;
            wDayOfWeek = (ushort)dateTime.DayOfWeek;
            wDay = (ushort)dateTime.Day;
            wHour = (ushort)dateTime.Hour;
            wMinute = (ushort)dateTime.Minute;
            wSecond = (ushort)dateTime.Second;
            wMilliseconds = (ushort)dateTime.Millisecond;
        }

        internal void AddMilliseconds(double milliseconds = 1.0) {
            FromDateTime(new DateTime((int)wYear, (int)wMonth, (int)wDay, (int)wHour, (int)wMinute, (int)wSecond, (int)wMilliseconds).AddMilliseconds(milliseconds));
        }

        internal void AddSeconds(double seconds = 1.0) {
            FromDateTime(new DateTime((int)wYear, (int)wMonth, (int)wDay, (int)wHour, (int)wMinute, (int)wSecond, (int)wMilliseconds).AddSeconds(seconds));
        }

        internal void AddMinutes(double minutes = 1.0) {
            FromDateTime(new DateTime((int)wYear, (int)wMonth, (int)wDay, (int)wHour, (int)wMinute, (int)wSecond, (int)wMilliseconds).AddMinutes(minutes));
        }

        internal void AddHours(double hours = 1.0) {
            FromDateTime(new DateTime((int)wYear, (int)wMonth, (int)wDay, (int)wHour, (int)wMinute, (int)wSecond, (int)wMilliseconds).AddHours(hours));
        }

        internal void AddMonths(int months = 1) {
            FromDateTime(new DateTime((int)wYear, (int)wMonth, (int)wDay, (int)wHour, (int)wMinute, (int)wSecond, (int)wMilliseconds).AddMonths(months));
        }

        internal void AddYears(int years = 1) {
            FromDateTime(new DateTime((int)wYear, (int)wMonth, (int)wDay, (int)wHour, (int)wMinute, (int)wSecond, (int)wMilliseconds).AddYears(years));
        }
    };

    internal static class Time {
        [DllImport("kernel32.dll")]
        internal extern static void GetSystemTime(ref SYSTEMTIME IpSystemTime);
        [DllImport("kernel32.dll")]
        internal extern static uint SetSystemTime(ref SYSTEMTIME IpSystemTime);
    }
}
