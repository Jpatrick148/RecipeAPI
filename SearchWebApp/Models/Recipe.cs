using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SearchWebApp.Models
{
    public class Recipe
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Calories { get; set; }
        public int TotalIngrediance { get; set; }
        public int TotalCookTime { get; set; }

    }

    public class RecipeContext : DbContext
    {
        public DbSet<Recipe> Recipes { get; set; }
    }
}