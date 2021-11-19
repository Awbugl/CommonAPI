﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using BepInEx;
using BepInEx.Logging;
using CommonAPI.ShotScene;
using CommonAPI.Systems;
using crecheng.DSPModSave;
using HarmonyLib;
using UnityEngine;

[module: UnverifiableCode]
#pragma warning disable 618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore 618

namespace CommonAPI
{
    /// <summary>
    /// Plugin class of Common API. Entry point
    /// </summary>
    [BepInPlugin(GUID, NAME, VERSION)]
    public class CommonAPIPlugin : BaseUnityPlugin, IModCanSave
    {
        public const string ID = "common-api";
        public const string GUID = "dsp.common-tools." + ID;
        public const string NAME = "DSP Common API";
        
        public const string VERSION = "1.0.0";

        internal static HashSet<string> LoadedSubmodules;
        internal static Harmony harmony;
        internal static ManualLogSource logger;
        internal static ResourceData resource;
        internal static Action onIntoOtherSave;
        
        public static Dictionary<string, Registry> registries = new Dictionary<string, Registry>();
        public static readonly Version buildFor = GameVersionUtil.GetVersion(0, 8, 23, 9808);

        public static bool iconShotMenuEnabled;
        public static KeyCode openIconShotMenuButton;
        

        void Awake()
        {
            logger = Logger;

            UnityThread.initUnityThread();

            iconShotMenuEnabled = Config.Bind("General", "enableIconShotMenu", false, "Is Icon shot menu enabled. It is useful for mod developers, because it allows to create consistent icons.").Value;

            openIconShotMenuButton = Config.Bind("General", "OpenIconShotMenuButton", KeyCode.F6, "Button used to open special Icon shot menu. It is useful for mod developers, because it allows to create consistent icons.").Value;
            
            string pluginfolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            resource = new ResourceData(ID, "CommonAPI", pluginfolder);
            resource.LoadAssetBundle("commonapi");
            
            harmony = new Harmony(GUID);
            
            var pluginScanner = new PluginScanner();
            var submoduleHandler = new APISubmoduleHandler(buildFor, Logger);
            LoadedSubmodules = submoduleHandler.LoadRequested(pluginScanner);
            pluginScanner.ScanPlugins();

            LoadSaveOnLoad.Init();
            harmony.PatchAll(typeof(VFPreloadPatch));
            
            logger.LogInfo("Common API is initialized!");
        }

        private void Update()
        {
            if (iconShotMenuEnabled && DSPGame.MenuDemoLoaded && Input.GetKeyDown(openIconShotMenuButton))
            {
                GeneratorSceneController.LoadIconGeneratorScene();
            }
        }

        internal static void CheckIfUsedOnRightGameVersion() {
            var buildId = GameConfig.gameVersion;

            if (buildFor == buildId)
                return;

            // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
            logger.LogWarning($"This version of CommonAPI was built for build id \"{buildFor.ToFullString()}\", you are running \"{buildId.ToFullString()}\".");
            logger.LogWarning("Should any problems arise, please check for a new version before reporting issues.");
        }
        
        /// <summary>
        /// Return true if the specified submodule is loaded.
        /// </summary>
        /// <param name="submodule">nameof the submodule</param>
        public static bool IsSubmoduleLoaded(string submodule) {
            if (LoadedSubmodules == null) {
                logger.LogWarning("IsLoaded called before submodules were loaded, result may not reflect actual load status.");
                return false;
            }
            return LoadedSubmodules.Contains(submodule);
        }

        public void Import(BinaryReader r)
        {
            
            r.ReadInt32();
            
            while (true)
            {
                if (r.ReadByte() == 0) break;

                string key = r.ReadString();

                if (registries.ContainsKey(key))
                {
                    r.ReadInt64();
                    registries[key].Import(r);
                }
                else
                {
                    long len = r.ReadInt64();
                    r.ReadBytes((int)len);
                }
            }
            
            CustomStarSystem.InitOnLoad();
            CustomStarSystem.Import(r);

            CustomPlanetSystem.InitOnLoad();
            CustomPlanetSystem.Import(r);
        }

        public void Export(BinaryWriter w)
        {
            w.Write(0);

            foreach (var kv in registries)
            {
                w.Write((byte)1);
                w.Write(kv.Key);
                MemoryStream stream = new MemoryStream();
                BinaryWriter tw = new BinaryWriter(stream);
                kv.Value.Export(tw);
                w.Write(stream.Length);
                w.Write(stream.ToArray());
            }

            w.Write((byte)0);
            
            CustomStarSystem.Export(w);
            CustomPlanetSystem.Export(w);
        }

        public void IntoOtherSave()
        {
            onIntoOtherSave?.Invoke();

            CustomStarSystem.InitOnLoad();
            CustomPlanetSystem.InitOnLoad();
        }
    }

    [HarmonyPatch]
    public static class VFPreloadPatch
    {
        [HarmonyPatch(typeof(VFPreload), "InvokeOnLoadWorkEnded")]
        [HarmonyPostfix]
        public static void OnMainMenuOpen()
        {
            CommonAPIPlugin.CheckIfUsedOnRightGameVersion();
        }
    }
}