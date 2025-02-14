using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using BepInEx;


namespace Ale.Patches
{
    public class CustomItems
    {
        public static ItemData Syrup { get; private set; }

        public static ItemData CrackSyrupBarrelData { get; private set; }
        public static ItemData LiquidCrackMaltData { get; private set; }


        public static List<ItemData> Items { get; private set; }

        public static Sprite LoadSprite(string fileName)
        {
            string path = Path.Combine(Paths.PluginPath, "AlePlugin/Images", fileName);
            UnityEngine.Debug.Log(path);
            if (!File.Exists(path))
            {
                Debug.LogError($"Image not found: {path}");
                return null;
            }

            byte[] imageData = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            if (!texture.LoadImage(imageData))
            {
                Debug.LogError("Failed to load image.");
                return null;
            }

            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

        public static void Init()
        {
            Items = new List<ItemData>();

            Syrup = ScriptableObject.CreateInstance<ItemData>();
            CrackSyrupBarrelData = ScriptableObject.CreateInstance<ItemData>();
            LiquidCrackMaltData = ScriptableObject.CreateInstance<ItemData>();


            {
                Syrup.id = 542;
                Syrup.price = 250;
                Syrup.shopItem = true;
                Syrup.itemDescription = "Syrup for Liquid Crack";
                Syrup.buyByOne = false;
                Syrup.maxStack = 20;
                Syrup.name = "Crack Syrup";
                Syrup.type = ItemData.Type.Material;
                Syrup.questDependant = 0;
                Syrup.levelDependant = 0;
                Syrup.useDescription = "Used for making Liquid Crack";
                Syrup.icon = CustomItems.LoadSprite("Maple-Syrup.png");
            }

            {
                CrackSyrupBarrelData.name = "Liquid Crack Barrel";
                CrackSyrupBarrelData.id = 550;
                CrackSyrupBarrelData.icon = CustomItems.LoadSprite("CrackSyrupAle.png");
                CrackSyrupBarrelData.questDependant = 0;
                CrackSyrupBarrelData.levelDependant = 0;
                CrackSyrupBarrelData.type = ItemData.Type.Material;
                CrackSyrupBarrelData.maxCharge = 10;
                CrackSyrupBarrelData.maxStack = 1;
                CrackSyrupBarrelData.price = 1;
                CrackSyrupBarrelData.playerCantSell = true;
                CrackSyrupBarrelData.playerCantDrop = false;
            }

            {
                LiquidCrackMaltData.name = "Liquid Crack Malt";
                LiquidCrackMaltData.id = 520;
                LiquidCrackMaltData.icon = CustomItems.LoadSprite("CrackSyrupMalt.png");
                LiquidCrackMaltData.questDependant = 0;
                LiquidCrackMaltData.levelDependant = 0;
                LiquidCrackMaltData.type = ItemData.Type.Material;
                LiquidCrackMaltData.maxCharge = 10;
                LiquidCrackMaltData.maxStack = 1;
                LiquidCrackMaltData.price = 1;
                LiquidCrackMaltData.playerCantSell = true;
                LiquidCrackMaltData.playerCantDrop = false;
            }

            Items.Add(CrackSyrupBarrelData);
            Items.Add(Syrup);
            Items.Add(LiquidCrackMaltData);
        }
    }
}
