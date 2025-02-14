using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ale.Patches
{
    public static class CustomRecipes
    {
        public static RecipeData LiquidCrackMalt {  get; private set; }

        public static RecipeData LiquidCrackBarrel { get; private set; }

        public static List<RecipeData> CustomRecipesList { get; private set; }

        public static void Init()
        {
            RecipeIngredientsData.Init();
            
            
            

            CustomRecipesList = new List<RecipeData>();

            LiquidCrackMalt = ScriptableObject.CreateInstance<RecipeData>();
            LiquidCrackBarrel = ScriptableObject.CreateInstance<RecipeData>();

            {
                LiquidCrackMalt.cookingTime = 100.0f;
                LiquidCrackMalt.showInRecipes = true;
                LiquidCrackMalt.showInMenu = false;
                LiquidCrackMalt.isOpenWorldRecipe = false;
                LiquidCrackMalt.id = 520;
                LiquidCrackMalt.type = RecipeData.Type.Ingredient;
                LiquidCrackMalt.name = "Liquid Crack Malt";
                LiquidCrackMalt.itemData = CustomItems.LiquidCrackMaltData;
                LiquidCrackMalt.ingredients = RecipeIngredientsData.CrackSyrupMaltIngredients;
                LiquidCrackMalt.ingredientsByStages = null;
                LiquidCrackMalt.levelDependant = 0;
                LiquidCrackMalt.questDependant = 0;
            }

            {
                LiquidCrackBarrel.cookingTime = 200.0f;
                LiquidCrackBarrel.showInRecipes = true;
                LiquidCrackBarrel.showInMenu = false;
                LiquidCrackBarrel.isOpenWorldRecipe = false;
                LiquidCrackBarrel.id = 550;
                LiquidCrackBarrel.type = RecipeData.Type.Drink;
                LiquidCrackBarrel.name = "Liquid Crack Barrel";
                LiquidCrackBarrel.itemData = CustomItems.CrackSyrupBarrelData;
                LiquidCrackBarrel.ingredients = RecipeIngredientsData.CrackSyrupBarrelIngredients;
                LiquidCrackBarrel.levelDependant = 0;
                LiquidCrackBarrel.questDependant = 0;
            }

            

            CustomRecipesList.Add(LiquidCrackBarrel);
            CustomRecipesList.Add(LiquidCrackMalt);
        }
    }

    public static class RecipeIngredientsData
    {
        public static RecipeData.ItemDataByte CrackSyrupMaltIngredients { get; private set; }
        public static RecipeData.ItemDataByte CrackSyrupBarrelIngredients { get; private set; }

        public static void Init()
        {

            {
                CrackSyrupMaltIngredients = new RecipeData.ItemDataByte();
                ItemData waterBucket = InterceptedItems.InterceptedItemDatas.interceptedDatas.First(i => i.name == "ItemDataNameWaterBucket");
                CrackSyrupMaltIngredients.Add(waterBucket, 10);
                CrackSyrupMaltIngredients.Add(CustomItems.Syrup, 15);
            }

            {
                CrackSyrupBarrelIngredients = new RecipeData.ItemDataByte();
                CrackSyrupBarrelIngredients.Add(CustomItems.LiquidCrackMaltData, 1);
                CrackSyrupBarrelIngredients.Add(CustomItems.Syrup, 5);
            }

            
        }

    }
}
