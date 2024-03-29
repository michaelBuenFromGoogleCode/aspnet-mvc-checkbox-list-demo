﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace AspNetMvcCheckboxList.Models
{
    public class Movie
    {
        public virtual int MovieId { get; set; }
        
        [   Required, Display(Name="Title")
        ]   public virtual string MovieName { get; set; }
        
        [   Required, Display(Name="Description")
        ]   public virtual string MovieDescription { get; set; }
        
        [   Required, Display(Name="Year Released"), Range(1900,9999)
        ]   public virtual int? YearReleased { get; set; }
        
        public virtual byte[] Version { get; set; }

        
        public virtual IList<Genre> Genres { get; set; }               
    }
}