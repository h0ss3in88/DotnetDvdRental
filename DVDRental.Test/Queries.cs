using System.Collections.Generic;
using DVDRental.Data;
using DVDRental.Data.Infrustructure;
using DVDRental.Data.Queries;
using DVDRental.Model;
using Xunit;

namespace DVDRental.Test
{
    public class Queries
    {
        public IRunner _runner;
        public ActorQuery _query;
        public Queries()
        {
            _runner = new Runner("server=localhost;user id=Hussein;database=dvdrental;password=123456;");
            _query = new ActorQuery(_runner);
        }
        [Fact(DisplayName = "All Actors")]
        public void GetAllActors()
        {
            var result = _query.GetAll();
            Assert.NotNull(result);
            Assert.IsType(typeof(List<Actor>),result);
        }

        [Fact(DisplayName = "Actor Counts")]
        public void GetCount()
        {
            var result = _query.Count();
            Assert.Equal(200,result);
        }

        [Fact(DisplayName = "Actor with Films")]
        public void GetActorsByFilms()
        {
            var result = _query.GetActorWithMovies(1);
            Assert.NotNull(result);
            Assert.Equal("Penelope",result.FirstName);
            Assert.Equal("Guiness",result.LastName);
            Assert.Equal(19,result.Movies.Count);
        }
    }
}