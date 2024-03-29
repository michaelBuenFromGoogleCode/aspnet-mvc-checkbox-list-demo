﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AspNetMvcCheckboxList.Models;

namespace AspNetMvcCheckboxList.ViewsModels
{
    public class MovieInputViewModel
    {
        public string MessageToUser { get; set; }

        public Movie TheMovie { get; set; }
        
        public List<Genre> GenreSelections{ get; set; }

        public List<int> SelectedGenres { get; set; }
    }
}