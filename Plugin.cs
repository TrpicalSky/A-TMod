using Ale.Patches;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using HarmonyLib.Tools;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace Ale;

[BepInPlugin("net.trpicalsky.plugins.newThings", "New Thing Mod", "1.0.0.0")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    private static string pluginGUID = "net.trpicalsky.plugins.newThings";

    
    private void Awake()
    {
        

        HarmonyFileLog.Enabled = true;
        // Plugin startup logic
        Harmony harmony = new Harmony(pluginGUID);
        harmony.PatchAll();
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        CustomItems.Init();
        //RecipeItemDatas.Init();



    }

    private void Start()
    {
        CustomItems.Init();
        
    }
}




