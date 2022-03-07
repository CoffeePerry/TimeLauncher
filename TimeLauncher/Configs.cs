using System;
using System.IO;
using System.Text;
using System.Reflection;
using TimeLauncher.Windows;

namespace TimeLauncher {
    internal class Configs {
        private readonly string DEFAULT_CONFIG_FILENAME = $"{Assembly.GetExecutingAssembly().GetName().Name}.conf";
        internal static readonly int MAX_SIZE_EXECUTABLE_FILENAME_TO_LAUNCH = 255;

        internal string Filename { get; set; }
        private string executableFilenameToLaunch;
        internal string ExecutableFilenameToLaunch {
            get => executableFilenameToLaunch;
            set => SetExecutableFilenameToLaunch(value);
        }
        private SYSTEMTIME checkpoint;
        internal SYSTEMTIME Checkpoint {
            get => checkpoint;
            set => checkpoint = value;
        }

        internal Configs() {
            Filename = DEFAULT_CONFIG_FILENAME;
            executableFilenameToLaunch = string.Empty;
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

        private void SetExecutableFilenameToLaunch(string value) {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));
            if (value.Length > MAX_SIZE_EXECUTABLE_FILENAME_TO_LAUNCH)
                throw new ArgumentException("Executable Filename to Open is too long", nameof(value));
            if (!File.Exists(value))
                throw new ArgumentException("Executable Filename to Open not found", nameof(value));
            executableFilenameToLaunch = value;
        }

        internal void LoadConfig() {
            if (string.IsNullOrWhiteSpace(Filename))
                throw new ArgumentNullException(nameof(Filename));
            if (!File.Exists(Filename))
                throw new ArgumentException("Configuration File not found", nameof(Filename));

            var programName = new char[MAX_SIZE_EXECUTABLE_FILENAME_TO_LAUNCH];
            byte[][] confDat = {
                Encoding.UTF8.GetBytes(programName),
                BitConverter.GetBytes(checkpoint.wYear),
                BitConverter.GetBytes(checkpoint.wMonth),
                BitConverter.GetBytes(checkpoint.wDay),
                BitConverter.GetBytes(checkpoint.wHour),
                BitConverter.GetBytes(checkpoint.wMinute),
                BitConverter.GetBytes(checkpoint.wSecond),
                BitConverter.GetBytes(checkpoint.wMilliseconds)
            };
            using (var fs = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.None))
                for (var i = 0; i < confDat.Length; ++i)
                    fs.Read(confDat[i], 0, confDat[i].Length);

            ExecutableFilenameToLaunch = Encoding.UTF8.GetString(confDat[0]).TrimEnd('\0');

            checkpoint.wYear = BitConverter.ToUInt16(confDat[1], 0);
            checkpoint.wMonth = BitConverter.ToUInt16(confDat[2], 0);
            checkpoint.wDay = BitConverter.ToUInt16(confDat[3], 0);
            checkpoint.wHour = BitConverter.ToUInt16(confDat[4], 0);
            checkpoint.wMinute = BitConverter.ToUInt16(confDat[5], 0);
            checkpoint.wSecond = BitConverter.ToUInt16(confDat[6], 0);
            checkpoint.wMilliseconds = BitConverter.ToUInt16(confDat[7], 0);
        }

        internal void SaveConfig() {
            if (string.IsNullOrWhiteSpace(Filename))
                throw new ArgumentNullException(nameof(Filename));

            var programName = new char[MAX_SIZE_EXECUTABLE_FILENAME_TO_LAUNCH];
            if ((executableFilenameToLaunch.Length <= programName.Length) && (!string.IsNullOrWhiteSpace(executableFilenameToLaunch))) {
                for (var i = 0; i < executableFilenameToLaunch.Length; ++i)
                    programName[i] = executableFilenameToLaunch[i];
                for (var i = executableFilenameToLaunch.Length + 1; i < programName.Length; ++i)
                    programName[i] = '\0';
            }

            byte[][] confDat = {
                Encoding.UTF8.GetBytes(programName),
                BitConverter.GetBytes(checkpoint.wYear),
                BitConverter.GetBytes(checkpoint.wMonth),
                BitConverter.GetBytes(checkpoint.wDay),
                BitConverter.GetBytes(checkpoint.wHour),
                BitConverter.GetBytes(checkpoint.wMinute),
                BitConverter.GetBytes(checkpoint.wSecond),
                BitConverter.GetBytes(checkpoint.wMilliseconds)
            };
            if (File.Exists(Filename))
                File.Delete(Filename);
            using (var fs = new FileStream(Filename, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                for (var i = 0; i < confDat.Length; ++i)
                    fs.Write(confDat[i], 0, confDat[i].Length);

            if (string.IsNullOrWhiteSpace(executableFilenameToLaunch))
                throw new ArgumentNullException(nameof(ExecutableFilenameToLaunch));
        }
    }
}
