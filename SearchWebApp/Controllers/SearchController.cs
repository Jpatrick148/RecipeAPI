using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using SearchWebApp.Models;

namespace SearchWebApp.Controllers
{
    public class SearchController : Controller
    {
        private RecipeContext db = new RecipeContext();

        public ActionResult UserInput()
        {
            return View();
        }

        public ActionResult Search(string userSearches)
        {
            
            HttpWebRequest request = WebRequest.CreateHttp("https://api.edamam.com/search?app_id=fe7c1a1d&app_key=5304c08e1fe3221883096e724828bf89" + "&q=" + userSearches);
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();

            StreamReader rd = new StreamReader(response.GetResponseStream());
            string data = rd.ReadToEnd();
            
            JObject SearchResultJSON = JObject.Parse(data);
            List<JToken> listR = SearchResultJSON["hits"].ToList(); // EDIT JSON PATH HERE

            List<Recipe> output = new List<Recipe>();
            Session["UsersSearchedList"] = output;

            for (int i = 0; i < listR.Count ; i++)
            {
                Recipe rp = new Recipe();
                rp.Name = listR[i]["recipe"]["label"].ToString();
                rp.Calories = (int)listR[i]["recipe"]["calories"];
                rp.Description = listR[i]["recipe"]["image"].ToString();
                rp.TotalIngrediance = listR[i]["recipe"]["ingredients"].Count();
                rp.TotalCookTime = (int) listR[i]["recipe"]["totalTime"];

                output.Add(rp);
            }

            return View(output);
        }

        public ActionResult Favorites(string IsFavorite)
        {
            List<Recipe> output = (List<Recipe>) Session["UsersSearchedList"];
            foreach (Recipe r in output)
            {
                if (IsFavorite != null)
                {
                    db.Recipes.Add(r);
                    db.SaveChanges();
                    ;
                    return View(db.Recipes);
                }
            }

            return RedirectToAction("Search");
        }
        


        // GET: Search
        public ActionResult Index()
        {
            return View(db.Recipes.ToList());
        }

        // GET: Search/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipe recipe = db.Recipes.Find(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            return View(recipe);
        }

        // GET: Search/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Search/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Description,Calories,TotalIngrediance,TotalCookTime")] Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                db.Recipes.Add(recipe);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(recipe);
        }

        // GET: Search/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipe recipe = db.Recipes.Find(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            return View(recipe);
        }

        // POST: Search/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Description,Calories,TotalIngrediance,TotalCookTime")] Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                db.Entry(recipe).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(recipe);
        }

        // GET: Search/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipe recipe = db.Recipes.Find(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            return View(recipe);
        }

        // POST: Search/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Recipe recipe = db.Recipes.Find(id);
            db.Recipes.Remove(recipe);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
