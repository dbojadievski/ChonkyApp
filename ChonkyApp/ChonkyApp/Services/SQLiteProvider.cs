using SQLite;

using System;
using System.Collections.Generic;
using System.Text;

namespace ChonkyApp.Services
{
    internal abstract class SQLiteProvider
    {

        private static readonly string dbName = "chonky.db";
        private static readonly string dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), dbName);
        private static readonly SQLiteConnection database = new SQLiteConnection(dbPath);

        public static SQLiteConnection Database { get => database; }

    }
}
