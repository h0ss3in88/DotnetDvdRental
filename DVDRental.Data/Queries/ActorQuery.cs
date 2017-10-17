using System;
using System.Collections.Generic;
using System.Linq;
using DVDRental.Data.Infrustructure;
using DVDRental.Model;

namespace DVDRental.Data.Queries
{
    public class ActorQuery
    {
        private readonly IRunner _runner;

        public ActorQuery(IRunner runner)
        {
            _runner = runner;
        }

        public IList<Actor> GetAll()
        {
            var sql =
                $"select actor_id as Id,first_name as FirstName,last_name as LastName,last_update as LastUpdate from actor;";
            var result = _runner.Execute<Actor>(sql, null);
            return result.ToList();
        }

        public Actor GetById(int id)
        {
            var sql =
                $"select actor_id as Id,first_name as FirstName,last_name as LastName from actor where actor_id=@0";
            var result = _runner.ExecuteSingle<Actor>(sql, new Object[] {id});
            return result;
        }

        public int Count()
        {
            var result = 0;
            var sql = $"select count(1) from actor;";
            var reader = _runner.OpenReader(sql, null);
            if (reader.Read())
            {
                result = System.Convert.ToInt32(reader[0]);
            }
            return result;
        }

        public Actor GetActorWithMovies(int id)
        {
            var actor = new Actor();
            var actorSql =
                @"SELECT actor.actor_id AS Id ,actor.first_name as FirstName ,actor.last_name as LastName , actor.last_update as LastUpdate FROM actor WHERE actor_id=@0;";
            var filmSql =
                @"SELECT film.film_id AS id , film.title AS Title,film.description AS Description , film.release_year AS ReleaseYear,film.length AS Length ,film.last_update AS LastUpdate,film.replacement_cost AS Price FROM film INNER JOIN film_actor ON film.film_id = film_actor.film_id WHERE film_actor.actor_id =@0;";
            var reader = _runner.OpenReader(actorSql + filmSql, new object[] {id});
            if (reader.Read())
            {
                actor = reader.ToSingle<Actor>();
                reader.NextResult();
                actor.Movies = new List<Film>(reader.ToList<Film>());
            }
            return actor;
        }
    }
}