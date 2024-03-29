﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AspNetMvcCheckboxList.ViewsModels;

using NHibernate.Linq;
using AspNetMvcCheckboxList.Models;
using NHibernate;



namespace AspNetMvcCheckboxList.Controllers
{
    public class MovieController : Controller
    {

        public ViewResult Index()
        {
            using (var s = Mapper.GetSessionFactory().OpenSession())
            {
                return View(s.Query<Movie>().OrderBy(x => x.MovieName).ToList());
            }
        }


        public ActionResult Delete(int id)
        {
            if (Request.HttpMethod == "POST")
            {
                using (var s = Mapper.GetSessionFactory().OpenSession())
                using (var tx = s.BeginTransaction())
                {
                    s.Delete(s.Load<Movie>(id));
                    tx.Commit();                    
                }
                return RedirectToAction("Index");
            }
            else
            {
                // prompt
                return View(Mapper.GetSessionFactory().OpenSession().Get<Movie>(id));
            }

        }

        public ViewResult Input(int id = 0)
        {
            using (var s = Mapper.GetSessionFactory().OpenSession())
            {
                var movie = id != 0 ? s.Get<Movie>(id) : new Movie { Genres = new List<Genre>() };
                return View(new MovieInputViewModel
                {
                    TheMovie = movie,
                    GenreSelections = s.Query<Genre>().OrderBy(x => x.GenreName).ToList(),
                    SelectedGenres = movie.Genres.Select(x => x.GenreId).ToList()                    
                });
            }
        }

        [HttpPost]
        public ActionResult Save(MovieInputViewModel input)
        {
            using (var s = Mapper.GetSessionFactory().OpenSession())
            using (var tx = s.BeginTransaction())
            {
                try
                {
                    bool isNew = input.TheMovie.MovieId == 0;

                    input.SelectedGenres = input.SelectedGenres ?? new List<int>();
                    input.GenreSelections = s.Query<Genre>().OrderBy(x => x.GenreName).ToList();

                    input.TheMovie.Genres = new List<Genre>();
                    foreach (int g in input.SelectedGenres)
                        input.TheMovie.Genres.Add(s.Load<Genre>(g));


                    s.SaveOrUpdate(input.TheMovie); // Primary key(MovieId) is automatically set with SaveOrUpdate, and the row version (Version) field too.

                    /* Use this if you want dynamic update(not all columns are set, only those that are changed),
                     * making NH similar to EF. Note: remove s.SaveOrUpdate, leave tx.Commit only
                    var m = s.Load<Movie>(input.TheMovie.MovieId);
                    UpdateModel(m, "TheMovie");*/

                    tx.Commit();






                    ModelState.Remove("TheMovie.MovieId");

                    // No need to remove TheMovie.Version, ASP.NET MVC is not preserving the ModelState of variables with byte array type.
                    // Hence, with byte array, the HiddenFor will always gets its value from the model, not from the ModelState
                    // ModelState.Remove("TheMovie.Version"); 



                    input.MessageToUser = string.Format("Saved. {0}", isNew ? "ID is " + input.TheMovie.MovieId : "");

                }
                catch (StaleObjectStateException)
                {
                    ModelState.AddModelError(string.Empty,
                        "The record you attempted to edit was already modified by another user since you last loaded it. Open the latest changes on this record");
                }
                               
            }
            
            return View("Input", input);
        }

    }
}
