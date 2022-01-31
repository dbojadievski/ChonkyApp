using SQLite;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ChonkyApp.Services
{
    internal abstract class SQLiteProvider
    {

        private static readonly string dbName = "chonky.db";
        private static readonly string dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), dbName);
        private static readonly SQLiteConnection database = new SQLiteConnection(dbPath);

        public static SQLiteConnection Database { get => database; }
        public static Boolean DoesTableExist(String name)
        {
            var result = false;

            try
            {
                var list = Database.Query<object>($"SELECT count(*) FROM sqlite_master WHERE type='table' AND name='{name}'");
                result = (list.Count == 1);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            return false;
        }

    }
}
