using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LunchSimulator.Models;
using System.Configuration;
using Npgsql;

namespace LunchSimulator.Repositories
{
    class PersonRepo
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["dbLocal"].ConnectionString;

        #region CREATE
        public static int AddPerson(Person person)
        {
            string stmt = "INSERT INTO person(name) values(@name) returning id";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    using (var command = new NpgsqlCommand(stmt, conn))
                    {
                        try
                        {
                            command.Parameters.AddWithValue("name", person.Name);
                            command.Connection = conn;
                            command.CommandText = stmt;
                            command.Prepare();
                            int id = (int)command.ExecuteScalar();
                            trans.Commit();
                            person.ID = id;
                            return id;
                        }
                        catch (PostgresException)
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
        }

        #endregion
        #region READ
        public static Person GetPersonRandom()
        {
            string stmt = "select id, name from person order by random() limit 1";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                Person person = null;
                conn.Open();

                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    //command.Parameters.AddWithValue("id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            person = new Person
                            {
                                ID = (int)reader["id"],
                                Name = (string)reader["name"],
                            };
                        }
                    }
                    return person;
                }
            }
        }
        public static IEnumerable<Person> Getpeople()
        {
            string stmt = "select id, name from person order by name";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                Person person = null;
                List<Person> people = new List<Person>();
                conn.Open();

                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            person = new Person
                            {
                                ID = (int)reader["id"],
                                Name = (string)reader["name"],
                            };
                            people.Add(person);
                        }
                    }
                    return people;
                }
            }
        }

        #endregion
        #region UPDATE
        #endregion
        #region DELETE
        public static void DeletePerson(int id)
        {
            string stmt = "DELETE FROM person WHERE id = @id";
            using (var conn = new NpgsqlConnection(connectionString))
            {
                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    conn.Open();
                    command.Parameters.AddWithValue("id", id);
                    command.ExecuteScalar();
                }
            }
        }

        #endregion

    }
}
