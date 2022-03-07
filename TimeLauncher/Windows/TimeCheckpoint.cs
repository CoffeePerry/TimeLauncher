using System;
using System.Diagnostics;

namespace TimeLauncher.Windows {
    internal class TimeCheckpoint {
        private Stopwatch millisecondsTimer;
        private DateTime currentDateTime;
        private SYSTEMTIME currentTime;
        private SYSTEMTIME checkpoint;
        internal SYSTEMTIME Checkpoint {
            get => checkpoint;
            set => checkpoint = value;
        }

        internal TimeCheckpoint() {
            millisecondsTimer = new Stopwatch();
            currentTime = new SYSTEMTIME();
            checkpoint = new SYSTEMTIME {
                wYear = (ushort)0b11111100001,
                wMonth = (ushort)0b1011,
                wDay = (ushort)0b10011,
                wHour = (ushort)0b10000,
                wMinute = (ushort)0b101,
                wSecond = (ushort)0b0,
                wMilliseconds = (ushort)0b0
            };
        }

        internal void SetCheckpoint() {
            Time.GetSystemTime(ref currentTime);

            currentDateTime = new DateTime(
                (int)currentTime.wYear, (int)currentTime.wMonth, (int)currentTime.wDay,
                (int)currentTime.wHour, (int)currentTime.wMinute, (int)currentTime.wSecond, (int)currentTime.wMilliseconds
            );

            Time.SetSystemTime(ref checkpoint);

            millisecondsTimer.Start();
        }

        internal void ResetCheckpoint() {
            millisecondsTimer.Stop();

            currentDateTime = currentDateTime.AddMilliseconds((double)millisecondsTimer.ElapsedMilliseconds);
            currentTime.wYear = (ushort)currentDateTime.Year;
            currentTime.wDay = (ushort)currentDateTime.Day;
            currentTime.wHour = (ushort)currentDateTime.Hour;
            currentTime.wMinute = (ushort)currentDateTime.Minute;
            currentTime.wSecond = (ushort)currentDateTime.Second;
            currentTime.wMilliseconds = (ushort)currentDateTime.Millisecond;

            Time.SetSystemTime(ref currentTime);
        }
    }
}
