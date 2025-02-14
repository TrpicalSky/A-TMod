using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using UnityEngine;
using TMPro;
using System.Management.Instrumentation;


namespace Ale.Patches
{
    public class ObjDef
    {
        public static void PrintObjectDefinition(object obj)
        {
            Type type = obj.GetType();

            UnityEngine.Debug.Log($"Type: {type.FullName}");

            // Print fields
            UnityEngine.Debug.Log("\nFields:");
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (var field in fields)
            {
                UnityEngine.Debug.Log($"{field.Name} ({field.FieldType}): {field.GetValue(obj)}");
            }

            // Print properties
            UnityEngine.Debug.Log("\nProperties:");
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (var property in properties)
            {
                UnityEngine.Debug.Log($"{property.Name} ({property.PropertyType}): {property.GetValue(obj)}");
            }

            // Print methods
            UnityEngine.Debug.Log("\nMethods:");
            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (var method in methods)
            {
                UnityEngine.Debug.Log($"{method.Name} - Return type: {method.ReturnType}");
            }

            // Print constructors
            UnityEngine.Debug.Log("\nConstructors:");
            ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (var constructor in constructors)
            {
                UnityEngine.Debug.Log($"{constructor.Name} - Parameters: {string.Join(", ", constructor.GetParameters().Select(p => p.ToString()))}");
            }
        }
    }

    
    public class ItemPatches
    {
        internal static ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("AddMoneyPatch");

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameStatus), "TryRemoveMoney")]
        public static void Prefix(ref GameStatus __instance ,ulong m)
        {
            __instance.AddMoney(10000);
            Logger.LogMessage($"[AddMoneyPatch] Prefix triggered for TryRemoveMoney with money: {m}");
        }
    }

    [HarmonyPatch(typeof(ShopUI), "SpawnShopItemUI")]
    public class ItemsAdded
    {






        internal static ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("ItemsAdded");
        
        [HarmonyPostfix]
        public static void PostFix(ref ShopUI __instance, ref ShopItemUI __result, ref Transform parent)
        {

            FieldInfo uiPool = AccessTools.Field(typeof(ShopUI), "shopItemUIPool");
            var uiPoolList = uiPool.GetValue(__instance) as List<ShopItemUI>;

            FieldInfo shopItemUIInfo = AccessTools.Field(typeof(ShopUI), "shopItemUI");
            var shopItemUIOrg = shopItemUIInfo.GetValue(__instance) as ShopItemUI;

            //FieldInfo itemDataField = typeof(ShopItemUI).GetField("_itemData", BindingFlags.NonPublic | BindingFlags.Instance);
            //var itemData = itemDataField.GetValue(shopItemUIOrg) as ItemData;


            foreach (ShopItemUI shopItemUI in uiPoolList)
            {
                FieldInfo itemDataField = typeof(ShopItemUI).GetField("_itemData", BindingFlags.NonPublic | BindingFlags.Instance);
                var itemData = itemDataField.GetValue(shopItemUI) as ItemData;

                foreach (ItemData item in CustomItems.Items)
                {
                    try
                    {
                        if (itemData == null)
                            continue;

                        if (item.id == itemData.id)
                        {
                            TMP_Text titleText = shopItemUI.transform.Find("Title (TMP)")?.GetComponent<TMP_Text>();
                            if (titleText != null)
                            {
                                titleText.text = item.name;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogException(ex);
                    }
                }
            }
        }
        
    }

    [HarmonyPatch(typeof(ShopUI), "SortItems")]
    class SortItemPatch
    {
        // Specify the name of the private method to patch
        [HarmonyPatch("SortItems")]
        [HarmonyPostfix]  // Use a Postfix or Prefix depending on when you want to run your code
        public static void Postfix(ItemManager __instance)
        {
            foreach (ItemData itemData2 in ItemManager.Instance.itemDataHub.itemData)
            {
                if (itemData2.shopItem)
                {
                    //UnityEngine.Debug.Log(itemData2.name);
                }
            }
        }
    }

    [HarmonyPatch(typeof(PlayerUI))]
    class PlayerUIPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch("OnItemMouseOver", new Type[] {typeof(ItemData), typeof(Vector3)})]
        public static void Postfix(ref PlayerUI __instance, ref ItemData itemData, ref Vector3 pos)
        {
            foreach (ItemData item in CustomItems.Items)
            {
                if (item.id == itemData.id)
                {
                    FieldInfo itemNameInfo = AccessTools.Field(typeof(PlayerUI), "itemDescrNameText");
                    var itemNameValue = itemNameInfo.GetValue(__instance) as TextMeshProUGUI;

                    FieldInfo itemDescInfo = AccessTools.Field(typeof(PlayerUI), "itemDescrText");
                    var itemDescValue = itemDescInfo.GetValue(__instance) as TextMeshProUGUI;

                    FieldInfo itemActionField = AccessTools.Field(typeof(PlayerUI), "itemDescrActionRT");
                    var itemActionDescValue = itemActionField.GetValue(__instance) as RectTransform;


                    itemNameValue.text = itemData.name;
                    itemDescValue.text = itemData.itemDescription;
                    itemActionDescValue.gameObject.SetActive(false);
                }
            }

        }

    }

    [HarmonyPatch(typeof(PlayerUI), "OnItemMouseOver")]
    class PlayerUIPatch
    {
        [HarmonyPatch("OnItemMouseOver", new Type[] {typeof(Item), typeof(ItemData), typeof(Vector3), typeof(bool)})]
        [HarmonyPostfix]
        public static void Postfix(ref PlayerUI __instance ,ref Item item, ref ItemData itemData, ref Vector3 pos, bool hideUseDescr = false)
        {
            foreach (ItemData itemData2 in CustomItems.Items)
            {
                if (itemData2.id == itemData.id)
                {
                    FieldInfo itemNameInfo = AccessTools.Field(typeof(PlayerUI), "itemDescrNameText");
                    var itemNameValue = itemNameInfo.GetValue(__instance) as TextMeshProUGUI;

                    FieldInfo itemDescInfo = AccessTools.Field(typeof(PlayerUI), "itemDescrText");
                    var itemDescValue = itemDescInfo.GetValue(__instance) as TextMeshProUGUI;

                    FieldInfo itemActionField = AccessTools.Field(typeof(PlayerUI), "itemDescrActionRT");
                    var itemActionDescValue = itemActionField.GetValue(__instance) as RectTransform;

                    itemNameValue.text = itemData.name;
                    itemDescValue.text = itemData.itemDescription;
                    itemActionDescValue.gameObject.SetActive(false);
                }
            }
        }

        [HarmonyPatch("OnSelectedHandItemData")]
        [HarmonyPostfix]
        public static void Postfix(ref PlayerUI __instance ,ref uint itemDataId)
        {
            FieldInfo selectedItemInfo = AccessTools.Field(typeof(PlayerUI), "selectedItemNameText");
            TextMeshProUGUI selectedItemValue = selectedItemInfo.GetValue(__instance) as TextMeshProUGUI;

            ItemData itemData;

            if (ItemManager.Instance.GetItemData(itemDataId, out itemData))
            {
                foreach (ItemData i in CustomItems.Items)
                {
                    if (i.id == itemDataId)
                    {
                        selectedItemValue.text = i.name;
                    }
                }
            }
        }
    }
    [HarmonyPatch(typeof(TMP_Text), "set_text")]
    public static class TMPTextPatch
    {
        static void Prefix(TMP_Text __instance, string value)
        {
            if (__instance.gameObject.name.Contains("SelectedItemName")) // Only track the Title TMP
            {
                Debug.Log($"Title TMP changed to: {value}");
                Debug.Log($"StackTrace:\n{System.Environment.StackTrace}");
            }
        }
    }

    [HarmonyPatch(typeof(ItemManager))]
    class PrivateMethodPatch
    {
        // Specify the name of the private method to patch
        [HarmonyPatch("LoadGame")]
        [HarmonyPrefix]  // Use a Postfix or Prefix depending on when you want to run your code
        public static void Prefix(ItemManager __instance)
        {
            ItemDataHub itemHub = Resources.FindObjectsOfTypeAll<ItemDataHub>().FirstOrDefault();


            FieldInfo itemDictInfo = AccessTools.Field(typeof(ItemManager), "_itemData");
            Dictionary<uint, ItemData> itemDictValue = itemDictInfo.GetValue(__instance) as Dictionary<uint, ItemData>;

            Dictionary<uint, ItemData> keyValuePairs = new Dictionary<uint, ItemData>();

            keyValuePairs = itemDictValue;

            if (itemHub == null)
            {
                UnityEngine.Debug.Log("ItemDataHub not found!");
                return;
            }



            List<ItemData> itemlist = itemHub.itemData.ToList();

            foreach (ItemData item in CustomItems.Items)
            {
                itemlist.Add(item);
                keyValuePairs.Add(item.id, item);
            }

            itemHub.itemData = itemlist.ToArray();

            foreach (ItemData item in itemHub.itemData)
            {
                UnityEngine.Debug.Log($"{item.ToString()} and Type: {item.type}");
            }

            itemDictValue = keyValuePairs;


            InterceptedItems.InterceptedItemDatas.interceptedDatas = itemlist;
            CustomRecipes.Init();


        }
    }
}
