﻿using MelonLoader;
using MelonLoader.Utils;
using Newtonsoft.Json;
using System.Collections;
using System.Net;
using System.Reflection;
using System.Web;
using UnityEngine;
using WickerREST;

namespace Wicker
{
    public static class BuildInfo
    {
        public const string Name = "WickerREST";
        public const string Description = "WickerREST";
        public const string Author = "Skrip";
        public const string Version = "0.91.0";
        public const string DownloadLink = "";
    }

    public class WickerServer : MelonMod
    {
        internal const string userDataPath   = "WickerREST";
        internal const string resourcesPath  = "WickerREST/resources";

        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private MelonPreferences_Category?      modCategory;
        private MelonPreferences_Entry<int>?    listeningPort;
        private MelonPreferences_Entry<int>?    debugLevel;

        // Instance of this class
        private static WickerServer?            instance;

        // Singleton pattern
        public static WickerServer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WickerServer();
                }
                return instance;
            }
            private set
            {
                instance = value;
            }
        }

        public override void OnInitializeMelon()
        {
            Instance = this;

            var configDirectoryPath = Path.Combine(MelonEnvironment.UserDataDirectory, userDataPath);
            Directory.CreateDirectory(configDirectoryPath);

            var resourceDirectoryPath = Path.Combine(MelonEnvironment.UserDataDirectory, resourcesPath);
            Directory.CreateDirectory(resourceDirectoryPath);

            modCategory = MelonPreferences.CreateCategory("WickerREST");
            modCategory.SetFilePath(Path.Combine(configDirectoryPath, "WickerREST.cfg"));

            listeningPort = modCategory.CreateEntry("ListeningPort", 6103, description: "Port server will listen on");
            debugLevel = modCategory.CreateEntry("DebugLevel", 0, description: "Debug level for logging (0: None, 1: Raised, 2: Verbose)");

            if (!File.Exists(Path.Combine(configDirectoryPath, "WickerREST.cfg")))
            {
                MelonPreferences.Save();
            }

            //Verify listening port is valid, otherwise set to default 6103. Notify it was reset
            if (listeningPort.Value < 1 || listeningPort.Value > 65535)
            {
                listeningPort.Value = 6103;
                LogMessage("Listening port was invalid. Reset to default 6103.");
                MelonPreferences.Save();
            }

            //Verify debug level is valid, otherwise set to default 0. Notify it was reset
            if (debugLevel.Value < 0 || debugLevel.Value > 2)
            {
                debugLevel.Value = 0;
                LogMessage("Debug level was invalid. Reset to default 0.");
                MelonPreferences.Save();
            }

            var discoveryThread = new Thread(() =>
            {
                Commands.Instance.DiscoverHandlersAndVariables(); // Discover command handlers and game variables
            });
            discoveryThread.Start();

            WickerNetwork.Instance.StartServer(listeningPort.Value, cancellationTokenSource);

            LoggerInstance.WriteLine(37);
            LogMessage($"Server initialized on port {listeningPort.Value}");
            LogMessage($"Navigate to: http://localhost:{listeningPort.Value}/");
            LoggerInstance.WriteLine(37);
        }

        internal IEnumerator ExecuteOnMainThread(Action action)
        {
            action.Invoke();
            yield return null;
        }

        public void LogMessage(string message, int requiredDebugLevel = 0)
        {
            // Check if current debug level allows logging this message
            if ((debugLevel == null && requiredDebugLevel == 0) 
                    || (debugLevel != null && debugLevel.Value >= requiredDebugLevel))
            {
                LoggerInstance.Msg(message);
            }
        }

        public override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            WickerNetwork.Instance.StopServer();
        }
    }
}

