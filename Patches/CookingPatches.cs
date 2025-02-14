using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ale.Patches
{
    [HarmonyPatch(typeof(CookUI))]
    public class CookingPot
    {
        [HarmonyPatch("UpdRecipes")]
        [HarmonyPrefix]
        public static void Prefix(CookUI __instance)
        {
            FieldInfo recipes = AccessTools.Field(typeof(CookUI), "_recipes");
            List<RecipeData> data = recipes.GetValue(__instance) as List<RecipeData>;

            foreach (RecipeData dataItem in CustomRecipes.CustomRecipesList)
            {
                if (dataItem.id == CustomRecipes.LiquidCrackMalt.id && !data.Contains(dataItem))
                {
                    data.Add(dataItem);
                }
            }

        }
    }
}
