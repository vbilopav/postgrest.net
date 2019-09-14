﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;

namespace PostgExecute.Net
{
    public partial class Postg
    {
        public static IEnumerable<IDictionary<string, object>> Read(string connection, string command)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                return pg.Read(command);
            }
        }

        public static IEnumerable<IDictionary<string, object>> Read(string connection, string command, params object[] parameters)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                return pg.Read(command, parameters);
            }
        }

        public static IEnumerable<IDictionary<string, object>> Read(string connection, string command, 
            Action<NpgsqlParameterCollection> parameters)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                return pg.Read(command, parameters);
            }
        }

        public static void Read(string connection, string command,
            Action<IDictionary<string, object>> results)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                pg.Read(command, results);
            }
        }
        public static void Read(string connection, string command,
            Action<IDictionary<string, object>> results, params object[] parameters)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                pg.Read(command, results);
            }
        }

        public static void Read(string connection, string command,
            Action<IDictionary<string, object>> results, Action<NpgsqlParameterCollection> parameters)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                pg.Read(command, results);
            }
        }

        public static void Read(string connection, string command, 
            Func<IDictionary<string, object>, bool> results)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                pg.Read(command, results);
            }
        }
        public static void Read(string connection, string command, 
            Func<IDictionary<string, object>, bool> results, params object[] parameters)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                pg.Read(command, results);
            }
        }

        public static void Read(string connection, string command, 
            Func<IDictionary<string, object>, bool> results, Action<NpgsqlParameterCollection> parameters)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                pg.Read(command, results);
            }
        }
    }
}
