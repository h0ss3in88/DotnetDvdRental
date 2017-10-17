using System;
using System.Collections.Generic;

namespace DVDRental.Model
{
    public class Actor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime LastUpdate { get; set; }
        public List<Film> Movies { get; set; }
    }
}