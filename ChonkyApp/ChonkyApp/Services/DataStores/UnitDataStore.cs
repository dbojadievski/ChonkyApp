using ChonkyApp.Models;
using ChonkyApp.Services;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

public class UnitDataStore
{
    private const String TABLE_NAME = "UnitEntry";

    private static bool CreateTableUnit()
    {
        var wasCreated = SQLiteProvider.Database.CreateTable<UnitEntry>() == SQLite.CreateTableResult.Created;
        return wasCreated;
    }

    public static async Task<UnitEntry> GetForUnit(Unit unit)
    {
        UnitEntry result = null;
        try
        {
            var entries = SQLiteProvider.Database.Query<UnitEntry>("SELECT * FROM UnitEntry WHERE Unit = ?", unit);
            result = entries.FirstOrDefault();

        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.TraceError(ex.Message);
        }

        return result;
    }

    public static async Task<UnitEntry> GetForUnitType(UnitType type)
    {
        UnitEntry result = null;

        try
        {
            var entries = SQLiteProvider.Database.Query<UnitEntry>("SELECT * FROM UnitEntry WHERE UnitType = ?", type);
            result = entries.FirstOrDefault();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.TraceError(ex.Message);
        }

        return result;
    }

    public static async Task<Boolean> Clear()
    {
        var result = false;

        try
        {
            var numAffected = SQLiteProvider.Database.DeleteAll<UnitEntry>();
            result = true;
        }
        catch (Exception ex)
        {
            Trace.TraceError(ex.Message);
        }
        return result;
    }

    private static async Task InsertMissingDefaultData()
    {
        var insertList = new List<UnitEntry>();
        foreach (var x in Enum.GetValues(typeof(Unit)))
        {
            Unit asUnit = (Unit)x;
            var entry = new UnitEntry
            {
                Unit = asUnit,
                UnitType = asUnit.GetUnitType()
            };
            
            if (await GetForUnit(asUnit) == null)
                insertList.Add(entry);
        }


        if (insertList.Count > 0)
        {
            Trace.TraceInformation($"Table {TABLE_NAME} is missing default data, attempting to insert.");
            var result = await InsertRange(insertList);
            if (result)
                Trace.TraceInformation($"Successfully inserted missing default data for table {TABLE_NAME}.");
            else
                Trace.TraceError($"Failed to insert missing default data for table {TABLE_NAME}.");
        }
    }

    public static async Task Initialize()
    {
        CreateTableUnit();
        await InsertMissingDefaultData();
    }
    public static async Task<Boolean> Insert(UnitEntry entry)
    {
        var isSuccess = false;
        try
        {
            var numAffected =  await Task.Run( () => SQLiteProvider.Database.Insert(entry));
            isSuccess = (numAffected == 1);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.TraceError(ex.Message);
        }

        return isSuccess;
    }

    public static async Task<Boolean> InsertRange(IEnumerable<UnitEntry> entries)
    {
        var isSuccess = false;

        try
        {
            var numAffected = await Task.Run(() => SQLiteProvider.Database.InsertAll(entries));
            isSuccess = (numAffected == entries.Count());
        }
        catch (Exception ex)
        {
            Trace.TraceError(ex.Message);
        }

        return isSuccess;
    }

    public static async Task<Boolean> Update(UnitEntry entry)
    {
        var isSuccess = false;

        try
        {
            var numAffected = await Task.Run(() => SQLiteProvider.Database.Delete(entry));
            isSuccess = (numAffected == 1);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.TraceError(ex.Message);
        }

        return isSuccess;
    }
}