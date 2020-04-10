﻿using System;
using System.Collections.Generic;
using System.Text.Json;

namespace CardServer.Players
{
    class PlayerDatabase
    {
        Dictionary<string, Player> database;
        string db_fname;

        protected PlayerDatabase() : this(db_fname: null)
        {
            // Empty Constructor
        }

        protected PlayerDatabase(string db_fname=null)
        {
            database = new Dictionary<string, Player>();
            this.db_fname = db_fname;

            LoadDatabase();
        }

        private void SaveDatabase()
        {
            if (db_fname != null)
            {
                List<Player> player_list = new List<Player>();
                foreach (Player p in database.Values)
                {
                    player_list.Add(p);
                }

                System.IO.StreamWriter json_writer = new System.IO.StreamWriter(db_fname);
                json_writer.Write(JsonSerializer.Serialize(database, database.GetType()));
                json_writer.Close();
            }
        }

        private void LoadDatabase()
        {
            if (db_fname != null)
            {
                List<Player> player_list = new List<Player>();
                System.Xml.Serialization.XmlSerializer xml = new System.Xml.Serialization.XmlSerializer(player_list.GetType());

                try
                {
                    System.IO.StreamReader json_reader = new System.IO.StreamReader(db_fname);
                    string data = json_reader.ReadToEnd();
                    database = (Dictionary<string, Player>)JsonSerializer.Deserialize(data, database.GetType());
                }
                catch (System.IO.FileNotFoundException)
                {
                    // Do nothing
                }
            }
        }

        private static PlayerDatabase db = new PlayerDatabase(db_fname: "users.json");

        public static PlayerDatabase GetInstance()
        {
            return db;
        }

        public Player GetPlayerForName(string username)
        {
            if (database.ContainsKey(username))
            {
                return database[username];
            }
            else
            {
                return null;
            }
        }

        public bool NewPlayer(string name, string hash, bool save_db=true)
        {
            if (!database.ContainsKey(name))
            {
                database.Add(
                    key: name,
                    value: new Player(
                        name: name,
                        password_hash: hash));
                if (save_db) SaveDatabase();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}