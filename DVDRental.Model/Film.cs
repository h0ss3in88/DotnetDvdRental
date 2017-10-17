using System;
using System.Collections.Generic;

namespace DVDRental.Model
{
    public class Film
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ReleaseYear { get; set; }
        public int Length { get; set; }
        public decimal Price { get; set; }
        public DateTime LastUpdate { get; set; }
        public IList<Actor> Actors { get; set; }
    }
}