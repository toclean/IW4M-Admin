﻿
using System;
using SharedLibraryCore;
using System.Threading.Tasks;
using System.IO;
using SharedLibraryCore.Objects;
using System.Reflection;

namespace IW4MAdmin.Application
{
    public class Program
    {
        static public double Version { get; private set; }
        static public ApplicationManager ServerManager = ApplicationManager.GetInstance();
        public static string OperatingDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + Path.DirectorySeparatorChar;

        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", OperatingDirectory);
            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.BelowNormal;

            Version = Assembly.GetExecutingAssembly().GetName().Version.Major + Assembly.GetExecutingAssembly().GetName().Version.Minor / 10.0f;

            Console.WriteLine("=====================================================");
            Console.WriteLine(" IW4M ADMIN");
            Console.WriteLine(" by RaidMax ");
            Console.WriteLine($" Version {Version}");
            Console.WriteLine("=====================================================");

            try
            {
                CheckDirectories();

                ServerManager = ApplicationManager.GetInstance();
                ServerManager.Init().Wait();
                Task.Run(() => ServerManager.Start());

                Task.Run(() =>
                {
                    String userInput;
                    Player Origin = ServerManager.GetClientService().Get(1).Result.AsPlayer();

                    do
                    {
                        userInput = Console.ReadLine();

                        if (userInput?.ToLower() == "quit")
                            ServerManager.Stop();

                        if (ServerManager.Servers.Count == 0)
                            return;

                        Origin.CurrentServer = ServerManager.Servers[0];
                        Event E = new Event(Event.GType.Say, userInput, Origin, null, ServerManager.Servers[0]);
                        ServerManager.Servers[0].ExecuteEvent(E);
                        Console.Write('>');

                    } while (ServerManager.Running);

                    Console.WriteLine("Shutdown complete");
                });

                WebfrontCore.Program.Init(ServerManager);
            }

            catch (Exception e)
            {
                Console.WriteLine($"Fatal Error during initialization: {e.Message}");
                while(e.InnerException != null)
                {
                    e = e.InnerException;
                    Console.WriteLine($"Inner exception: {e.Message}");
                }
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }

        static void CheckDirectories()
        {
            string curDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + Path.DirectorySeparatorChar;

            if (!Directory.Exists($"{curDirectory}Plugins"))
                Directory.CreateDirectory($"{curDirectory}Plugins");
        }
    }
}