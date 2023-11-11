using System;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;

namespace CardServer.Players
{
    /// <summary>
    /// Provides a database instance for players, with an option to save to a local file for storage
    /// </summary>
    class PlayerDatabase
    {
        /// <summary>
        /// The raw player database dictionary
        /// </summary>
        Dictionary<string, Player> Database { get; set; }

        /// <summary>
        /// An optional filename to save users to as a json structure
        /// </summary>
        string? DatabaseFileName { get; }

        /// <summary>
        /// Default player database constructor, with optional database filename to save
        /// data for persistency between server starts
        /// </summary>
        /// <param name="db_fname"></param>
        protected PlayerDatabase(string? db_fname = null)
        {
            // Initialize the database and set the filename
            Database = new();
            DatabaseFileName = db_fname;

            // Load the database
            LoadDatabase();
        }

        /// <summary>
        /// Provides the current database filename
        /// </summary>
        /// <returns>the current database filename</returns>
        public string? GetDatabaseFilename()
        {
            return DatabaseFileName;
        }

        /// <summary>
        /// Saves the database to the provided file if not null
        /// </summary>
        private void SaveDatabase()
        {
            if (!string.IsNullOrWhiteSpace(DatabaseFileName))
            {
                using StreamWriter json_writer = new(DatabaseFileName);
                json_writer.Write(JsonSerializer.Serialize(Database, Database.GetType()));
            }
        }

        /// <summary>
        /// Loads the database to the provided file if not null
        /// </summary>
        private void LoadDatabase()
        {
            if (DatabaseFileName != null)
            {
                try
                {
                    using StreamReader json_reader = new(DatabaseFileName);
                    string data = json_reader.ReadToEnd();
                    Database = (Dictionary<string, Player>?)JsonSerializer.Deserialize(data, Database.GetType()) ?? throw new NullReferenceException("unable to get player database");
                }
                catch (FileNotFoundException)
                {
                    // Do nothing if the file cannot be found to load
                }
            }
        }

        /// <summary>
        /// The static database instance for the current executable to utilize
        /// </summary>
        private static PlayerDatabase? Instance;

        /// <summary>
        /// Initializes the database with the provided database filename
        /// </summary>
        /// <param name="db_fname">the filename to use, or null if none</param>
        public static void InitDatabase(string? db_fname)
        {
            Instance = new PlayerDatabase(db_fname: db_fname);
        }

        /// <summary>
        /// Returns the player database isntance to utilize
        /// </summary>
        /// <returns></returns>
        public static PlayerDatabase? GetInstance()
        {
            return Instance;
        }

        /// <summary>
        /// Provides the player for the given name and hash if all match. Otherwise, returns null on failure
        /// </summary>
        /// <param name="username">The username to search for</param>
        /// <param name="hash">The has associated with the username</param>
        /// <returns>Player if valid username and hash found; otherwise null</returns>
        public Player? CheckPlayerNameHash(string username, string hash)
        {
            Player? p = GetPlayerForName(username);

            if (p != null && p.PaswordHash == hash)
            {
                return p;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Provides a game player for the input name if present in the player dictionary
        /// </summary>
        /// <param name="username">The username to get the name player for</param>
        /// <returns>The game player if it exists; otherwise null</returns>
        public Player? GetPlayerForName(string username)
        {
            if (Database.ContainsKey(username))
            {
                return Database[username];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Creates a new player with the provided username and hash values
        /// </summary>
        /// <param name="name">The username to create</param>
        /// <param name="hash">The hash value to associate with the username</param>
        /// <param name="save_db">Whether to call to save the database after the user is added</param>
        /// <returns>True if the user can be created; otherwise false (e.g. username already exists)</returns>
        public bool CreateNewPlayer(string name, string hash, bool save_db = true)
        {
            if (!Database.ContainsKey(name))
            {
                Database.Add(
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
        