using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization.Settings;
using static AppSettings;

namespace Ale.Patches
{
    [HarmonyPatch(typeof(RecipeManager))]
    public class BaseRecipePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("LoadGame")]
        public static void Postfix(ref RecipeManager __instance)
        {
            RecipeDataHub recipeDataHub = Resources.FindObjectsOfTypeAll<RecipeDataHub>().FirstOrDefault();

            FieldInfo itemDictInfo = AccessTools.Field(typeof(RecipeManager), "_recipeData");
            Dictionary<ushort, RecipeData> itemDictValue = itemDictInfo.GetValue(__instance) as Dictionary<ushort, RecipeData>;

            Dictionary<ushort, RecipeData> keyValuePairs = new Dictionary<ushort, RecipeData>();

            keyValuePairs = itemDictValue;

            List<RecipeData> recipeDatas = recipeDataHub.recipeData.ToList();

            foreach (RecipeData data in CustomRecipes.CustomRecipesList)
            {
                
                recipeDatas.Add(data);
                keyValuePairs.Add(data.id, data);
            }

            itemDictValue = keyValuePairs;
            recipeDataHub.recipeData = recipeDatas.ToArray();
        }

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void Postfix(RecipeManager __instance)
        {
            FieldInfo RecipeFieldInfo = AccessTools.Field(typeof(RecipeManager), "_recipeIdByProductItemDataId");
            Dictionary<ushort, ushort> RecipeIDValue = RecipeFieldInfo.GetValue(__instance) as Dictionary<ushort,ushort>;

            FieldInfo RecipeDataInfo = AccessTools.Field(typeof(RecipeManager), "_recipeData");
            Dictionary<ushort, RecipeData> RecipeDataValue = RecipeDataInfo.GetValue(__instance) as Dictionary<ushort, RecipeData>;

            FieldInfo RecipeByRecipeInfo = AccessTools.Field(typeof(RecipeManager), "_recipeIdByRecipeItemDataId");
            Dictionary<ushort, ushort> RecipeByRecipeValue = RecipeFieldInfo.GetValue(__instance) as Dictionary<ushort, ushort>;




            byte b = 0;
            while ((int)b < __instance.recipeDataHub.recipeData.Length)
            {
                if (!RecipeDataValue.ContainsKey(__instance.recipeDataHub.recipeData[(int)b].id))
                {
                    RecipeDataValue.Add(__instance.recipeDataHub.recipeData[(int)b].id, __instance.recipeDataHub.recipeData[(int)b]);
                }
                else
                {
                    //Debug.LogError(string.Format("RecipeDataHub.Awake ERROR: Duplicate recipe data id {0}", __instance.recipeDataHub.recipeData[(int)b].id));
                }
                if (!RecipeIDValue.ContainsKey(__instance.recipeDataHub.recipeData[(int)b].itemData.id))
                {
                    RecipeIDValue.Add(__instance.recipeDataHub.recipeData[(int)b].itemData.id, __instance.recipeDataHub.recipeData[(int)b].id);
                } else
                {
                    //Debug.LogError("Already There");
                }
                if (__instance.recipeDataHub.recipeData[(int)b].recipeCollectibleItemData != null)
                {
                    RecipeByRecipeValue.Add(__instance.recipeDataHub.recipeData[(int)b].recipeCollectibleItemData.id, __instance.recipeDataHub.recipeData[(int)b].id);
                }
                b += 1;
            }

        }
    }

    [HarmonyPatch(typeof(RecipeAdvUI))]
    public class BaseRecipeUIPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("UpdRecipes")]
        public static void Postfix(ref RecipeAdvUI __instance)
        {
            FieldInfo avaliableItemInfo = AccessTools.Field(typeof(RecipeAdvUI), "_availableItems");
            List<RecipeData> recipeDatas = avaliableItemInfo.GetValue(__instance) as List<RecipeData>;

            FieldInfo LockedItemInfo = AccessTools.Field(typeof(RecipeAdvUI), "_lockedItems");
            List<RecipeData> LockedItemData = LockedItemInfo.GetValue(__instance) as List<RecipeData>;

            FieldInfo listViewInfo = AccessTools.Field(typeof(RecipeAdvUI), "recipeListView");
            Transform listViewTransform = listViewInfo.GetValue(__instance) as Transform;

            FieldInfo RecipleListElementsInfo = AccessTools.Field(typeof(RecipeAdvUI), "_recipeListElements");
            Dictionary<ushort, ListElement> RecipleListValues = RecipleListElementsInfo.GetValue(__instance) as Dictionary<ushort, ListElement>;

            MethodInfo onRecipeClickFunc = typeof(RecipeAdvUI).GetMethod("OnRecipeClick", BindingFlags.NonPublic | BindingFlags.Instance);

            Action<ushort> onRecipeClickDelegate = (Action<ushort>)onRecipeClickFunc.CreateDelegate(typeof(Action<ushort>), __instance);






            // Clear the existing list elements
            foreach (KeyValuePair<ushort, ListElement> keyValuePair in RecipleListValues)
            {
                PlayerUI.Instance.DespawnListElement(keyValuePair.Value);
            }
            RecipleListValues.Clear();

            // Add available items first
            foreach (RecipeData data in recipeDatas)
            {
                string itemName = "";
                try
                {
                    if (CustomRecipes.CustomRecipesList == null)
                    {
                        UnityEngine.Debug.Log("NULL LIST");
                    }
                    foreach (RecipeData data1 in CustomRecipes.CustomRecipesList)
                    {
                        
                        if (data1.id == data.id)
                        {
                            itemName = data1.name;
                            UnityEngine.Debug.Log(data.ingredients.Keys.Count);
                            break;
                        }
                    }
                } catch (Exception ex)
                {
                    foreach (RecipeData data2 in CustomRecipes.CustomRecipesList)
                    {
                        UnityEngine.Debug.Log(data2);
                    }
                    Debug.Log(ex);
                }

                if (string.IsNullOrEmpty(itemName))
                {
                    itemName = LocalizationSettings.StringDatabase.GetLocalizedString("ItemData", data.itemData.name, null, FallbackBehavior.UseProjectSettings, Array.Empty<object>());
                }

                ListElement listElement = PlayerUI.Instance.SpawnListElement(listViewTransform);
                listElement.Set(data.itemData.icon, itemName, "", true);
                listElement.SetNew(!__instance.recipesLastSeen.Contains(data.id));
                listElement.SetSelectCallback(onRecipeClickDelegate, data.id);

                RecipleListValues.Add(data.id, listElement);
            }

            // Add locked items next
            foreach (RecipeData recipeData2 in LockedItemData)
            {
                string itemName = "";
                foreach (RecipeData data1 in CustomRecipes.CustomRecipesList)
                {
                    if (data1.id == recipeData2.id)
                    {
                        itemName = data1.name;
                        //UnityEngine.Debug.Log(recipeData2.ingredients.Keys.Count);
                        break;
                    }
                }

                if (string.IsNullOrEmpty(itemName))
                {
                    itemName = LocalizationSettings.StringDatabase.GetLocalizedString("ItemData", recipeData2.itemData.name, null, FallbackBehavior.UseProjectSettings, Array.Empty<object>());
                }

                ListElement listElement2 = PlayerUI.Instance.SpawnListElement(listViewTransform);
                string localizedString2 = LocalizationSettings.StringDatabase.GetLocalizedString("ItemData", recipeData2.itemData.name, null, FallbackBehavior.UseProjectSettings, Array.Empty<object>());
                listElement2.Set(recipeData2.itemData.icon, localizedString2, (recipeData2.levelDependant == 0) ? "" : ((int)(recipeData2.levelDependant + 1)).ToString(), false);
                listElement2.SetSelectCallback(onRecipeClickDelegate, recipeData2.id);
                RecipleListValues.Add(recipeData2.id, listElement2);
            }





        }
        [HarmonyPostfix]
        [HarmonyPatch("SortItems")]
        public static void Postfix(ref RecipeAdvUI __instance, byte s = 0) 
        {
            FieldInfo avaliableItemInfo = AccessTools.Field(typeof(RecipeAdvUI), "_availableItems");
            List<RecipeData> recipeDatas = avaliableItemInfo.GetValue(__instance) as List<RecipeData>;

            FieldInfo LockedItemInfo = AccessTools.Field(typeof(RecipeAdvUI), "_lockedItems");
            List<RecipeData> LockedItemData = LockedItemInfo.GetValue(__instance) as List<RecipeData>;


            //foreach (RecipeData data in LockedItemData)
            //    UnityEngine.Debug.Log($"The locked items are: {data}");


            //foreach (var lockedItem in LockedItemData)
            //{
            //    if (lockedItem != null)
            //    {
            //        UnityEngine.Debug.Log(lockedItem.name);
            //    }
            //}
            //foreach (RecipeData recipeData2 in RecipeManager.Instance.recipeDataHub.recipeData)
            //{
            //    if (recipeData2.type - RecipeData.Type.Boiled == s && recipeData2.showInRecipes && (!recipeData2.isAlchemicalRecipe || RecipeManager.Instance.secretRecipes.Contains(recipeData2.id)))
            //    {
            //        if (RecipeManager.Instance.IsRecipeOpen(recipeData2.id))
            //        {
            //            foreach (RecipeData recipe in CustomRecipes.CustomRecipesList)
            //            {
            //                if (recipeData2.id == recipe.id)
            //                {
            //                    UnityEngine.Debug.Log($"It was added {recipe.name}");
            //                }
            //            }
            //        }
            //        else
            //        {
            //            foreach (RecipeData recipe in CustomRecipes.CustomRecipesList)
            //            {
            //                if (recipeData2.id == recipe.id)
            //                {
            //                    UnityEngine.Debug.Log($"It wasn't added {recipe.name}");
            //                }
            //                if (GameStatus.Instance.level.Value >= recipe.levelDependant)
            //                {
            //                    UnityEngine.Debug.Log("Meets Second statement");
            //                }
            //            }
            //        }
            //    }
            //}
        }

        [HarmonyPrefix]
        [HarmonyPatch("AddRLERecursively")]
        public static bool Prefix(RecipeAdvUI __instance, ref ushort recipeId, byte level = 0)
        {
            FieldInfo itemListViewRLE = AccessTools.Field(typeof(RecipeAdvUI), "itemListView");
            Transform transform = itemListViewRLE.GetValue(__instance) as Transform;

            MethodInfo GetRLEFunc = typeof(RecipeAdvUI).GetMethod("GetRLE", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo GetRLEFuncRecur = typeof(RecipeAdvUI).GetMethod("AddRLERecursively", BindingFlags.NonPublic | BindingFlags.Instance);

            ItemData key;
            IngredientListElement rle;

            foreach (KeyValuePair<ItemData, byte> keyValuePair in RecipeManager.Instance.GetRecipeDataById(recipeId).ingredients)
            {
                key = keyValuePair.Key;
                rle = GetRLEFunc.Invoke(__instance, new object[] { transform.transform}) as IngredientListElement;

                // Check if the item is in your custom items list
                string localizedString = "";
                foreach (var Item in CustomItems.Items)
                {
                    if (Item.id == key.id) // Replace `CustomItems.IsCustomItem` with your custom logic
                    {
                        // Use the raw name if it's a custom item
                        localizedString = key.name;
                        //UnityEngine.Debug.Log($"Name Provided Is: {Item.name} ");
                        break;
                    }
                    else
                    {
                        // Use the localized name for non-custom items
                        localizedString = LocalizationSettings.StringDatabase.GetLocalizedString("ItemData", key.name, null, FallbackBehavior.UseProjectSettings, Array.Empty<object>());
                    }
                }
                
                if (string.IsNullOrEmpty(localizedString))
                {
                    localizedString = "Placeholder";
                }

                // Set the ingredient list element
                rle.Set(key.icon, localizedString, keyValuePair.Value.ToString(), level);

                // If the item is a product of another recipe, add its ingredients directly (without recursion)
                RecipeData recipeData;
                if (RecipeManager.Instance.GetRecipeDataByProductItemDataId(key.id, out recipeData))
                {
                    foreach (KeyValuePair<ItemData, byte> nestedKeyValuePair in recipeData.ingredients)
                    {
                        ItemData nestedKey = nestedKeyValuePair.Key;
                        UnityEngine.Debug.Log($"{nestedKey.name} and Type: {nestedKey.type}");
                        IngredientListElement nestedRle = GetRLEFunc.Invoke(__instance, new object[] { transform.transform }) as IngredientListElement;

                        string nestedLocalizedString = "";
                        foreach (var custom in CustomItems.Items)
                        {
                            if (custom.id == nestedKey.id) // Replace `CustomItems.IsCustomItem` with your custom logic
                            {
                                // Use the raw name if it's a custom item
                                nestedLocalizedString = nestedKey.name;
                                break;
                            }
                            else
                            {
                                // Use the localized name for non-custom items
                                nestedLocalizedString = LocalizationSettings.StringDatabase.GetLocalizedString("ItemData", nestedKey.name, null, FallbackBehavior.UseProjectSettings, Array.Empty<object>());
                            }
                        }

                        Debug.Log(nestedLocalizedString);
                        if (string.IsNullOrEmpty(nestedLocalizedString))
                        {
                            nestedLocalizedString = "Placeholder";
                        }

                        // Set the nested ingredient list element
                        nestedRle.Set(nestedKey.icon, nestedLocalizedString, nestedKeyValuePair.Value.ToString(), (byte)(level + 1));
                    }
                }
            }
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch("UpdItems")]
        public static void Postfix(RecipeAdvUI __instance, ref ushort recipeId)
        {
            //UnityEngine.Debug.Log("Clled UpdItemns");
        }


    }


}
