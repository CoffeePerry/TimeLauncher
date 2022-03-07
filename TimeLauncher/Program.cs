using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using TimeLauncher.Windows;

namespace TimeLauncher {
    internal class Program {
        private static readonly string APPLICATION_NAME = Assembly.GetExecutingAssembly().GetName().Name;
        private static readonly string APPLICATION_VERSION = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        private static readonly int UI_PAUSE_MILLISECONDS = 2000;
        private static readonly string CMD_OPTION_REWRITE_CONFIG = "/rc";   // Rewrite Configs: Increment by 1 second.

        static void Main(string[] args) {
            Console.WriteLine($"- Starting {APPLICATION_NAME} (v{APPLICATION_VERSION})...");
            try {
                #if RELEASE
                if (!Privileges.IsAdmin()) {
                    Console.WriteLine("- Please, restart the program with administrative privileges.");
                } else {
                #else
                {
                #endif
                    var rewriteConfig = (args.Length == 1) && (args[0].ToLower() == CMD_OPTION_REWRITE_CONFIG);

                    var configs = new Configs();
                    configs.Filename = Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, ".conf");

                    var timeCheckpoint = new TimeCheckpoint();

                    if (File.Exists(configs.Filename)) {
                        #region Load Config
                        configs.LoadConfig();

                        if (rewriteConfig) {
                            var checkpoint = configs.Checkpoint;
                            checkpoint.AddSeconds(1);
                            configs.Checkpoint = checkpoint;

                            configs.SaveConfig();
                        }

                        Console.WriteLine("- Configuration data loaded.");
                        #endregion
                    } else {
                        #region Save Config
                        Console.WriteLine("- Starting initialization...");

                        var strProgramName = string.Empty;
                        while (true) {
                            Console.WriteLine("- Enter the executable's path to be launched: ");
                            strProgramName = Console.ReadLine();
                            if ((strProgramName.Length <= Configs.MAX_SIZE_EXECUTABLE_FILENAME_TO_LAUNCH) && (!string.IsNullOrWhiteSpace(strProgramName)))
                                break;
                        }

                        configs.ExecutableFilenameToLaunch = strProgramName;

                        var checkpoint = new SYSTEMTIME {
                            wYear = (ushort)0b11111100001,
                            wMonth = (ushort)0b1011,
                            wDay = (ushort)0b10011,
                            wHour = (ushort)0b10000,
                            wMinute = (ushort)0b101,
                            wSecond = (ushort)0b0,
                            wMilliseconds = (ushort)0b0
                        };
                        while (true) {
                            Console.WriteLine("- Enter the checkpoint year: ");
                            if (ushort.TryParse(Console.ReadLine(), out checkpoint.wYear))
                                break;
                        }
                        while (true) {
                            Console.WriteLine("- Enter the checkpoint month: ");
                            if (ushort.TryParse(Console.ReadLine(), out checkpoint.wMonth))
                                break;
                        }
                        while (true) {
                            Console.WriteLine("- Enter the checkpoint day: ");
                            if (ushort.TryParse(Console.ReadLine(), out checkpoint.wDay))
                                break;
                        }
                        while (true) {
                            Console.WriteLine("- Enter the checkpoint hours: ");
                            if (ushort.TryParse(Console.ReadLine(), out checkpoint.wHour)) {
                                checkpoint.wHour--;
                                break;
                            }
                        }
                        while (true) {
                            Console.WriteLine("- Enter the checkpoint minutes: ");
                            if (ushort.TryParse(Console.ReadLine(), out checkpoint.wMinute))
                                break;
                        }
                        while (true) {
                            Console.WriteLine("- Enter the checkpoint seconds: ");
                            if (ushort.TryParse(Console.ReadLine(), out checkpoint.wSecond))
                                break;
                        }
                        while (true) {
                            Console.WriteLine("- Enter the checkpoint milliseconds: ");
                            if (ushort.TryParse(Console.ReadLine(), out checkpoint.wMilliseconds))
                                break;
                        }
                        configs.Checkpoint = checkpoint;

                        configs.SaveConfig();

                        Console.WriteLine("- Configuration data saved.");
                        #endregion
                    }

                    timeCheckpoint.Checkpoint = configs.Checkpoint;

                    #region Checks
                    if (string.IsNullOrWhiteSpace(configs.ExecutableFilenameToLaunch))
                        throw new Exception("Executable's path to be launched is invalid");
                    if (!File.Exists(configs.ExecutableFilenameToLaunch))
                        throw new Exception($"Executable's path to be launched not found ({configs.ExecutableFilenameToLaunch})");
                    #endregion

                    #region TimeCheckpoint
                    timeCheckpoint.SetCheckpoint();
                    try {
                        var program = Process.Start(configs.ExecutableFilenameToLaunch); // TODO: Add parameters.
                        if (program == null)
                            throw new Exception($"Error in launching the executable ({configs.ExecutableFilenameToLaunch})");

                        try {
                            program.WaitForExit();
                        } catch {
                            program.Kill();
                            throw;
                        } finally {
                            program.Close();
                        }
                    } finally {
                        timeCheckpoint.ResetCheckpoint();
                    }
                    #endregion

                    Console.WriteLine("- Executable completed successfully.");
                }
            } catch (Exception ex) {
                Console.WriteLine($"- {APPLICATION_NAME} encountered a problem: {ex.Message}");
                Console.WriteLine("- Risolvere prima di riavviare l'applicativo.");
            } finally {
                Console.WriteLine($"- This application will automatically close in {UI_PAUSE_MILLISECONDS / 1000} seconds.");
                Thread.Sleep(UI_PAUSE_MILLISECONDS);
            }
        }
    }
}
