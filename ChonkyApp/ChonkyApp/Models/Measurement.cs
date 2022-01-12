using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ChonkyApp.Models
{
    public class ConversionException : Exception
    {
        public ConversionException() { }
        public ConversionException(string message) : base(message) { }

        public ConversionException(String message, Exception inner) : base(message, inner) { }
    }

    public enum Unit 
    {
        Centimeter  = 0,
        Inch        = 1,
        Kilogram    = 2,
        Pound       = 3,
        Second      = 4,
        Percent     = 5
    }

    public enum UnitType
    {
        Length      = 0,
        Weight      = 1,
        Time        = 2,
        Relative    = 3
    }

    public class UnitEntry: BaseModel
    {
        private Unit unit;
        private UnitType unitType;

        public Unit Unit 
        { 
            get => unit;
            set { SetProperty(ref unit, value); }
         } 


        public UnitType UnitType
        {
            get => unitType;
            set => SetProperty(ref unitType, value);
        }
    }

    static class UnitHelper
    {
        public static UnitType GetUnitType(this Unit input)
        {
            switch (input)
            {
                case Unit.Centimeter:
                case Unit.Inch:
                    return UnitType.Length;
                case Unit.Kilogram:
                case Unit.Pound:
                    return UnitType.Weight;
                case Unit.Second:
                    return UnitType.Time;
                case Unit.Percent:
                    return UnitType.Relative;
                default:
                    throw new NotImplementedException(
                        String.Format("Unit {0} type not implemented.", input.ToString())
                    );
            }
        }

        public static Boolean IsCompatibleWith(this Unit self, in Unit other)
        {
            UnitType typeSelf = self.GetUnitType();
            UnitType typeOther = other.GetUnitType();

            return (typeSelf == typeOther);
        }
    }

    public class Measurement : BaseModel
    {
        private double value;
        private Unit unit;

        [SQLite.Column("Value")]
        public Double Value
        {
            get => value;
            set => SetProperty(ref this.value, value);
        }

        [SQLite.Column("Unit")]
        public Unit Unit
        {
            get => unit;
            set => SetProperty(ref unit, value);
        }

        [SQLite.Column("MeasurementType")]
        public UnitType MeasurementType
        {
            get => unit.GetUnitType();
        }
        

        public Measurement()
        {
        }

        public Measurement(DateTime recordedAt, Double value = 0, Unit unit = Unit.Centimeter)
        {
            CreatedAt   = recordedAt;
            Unit        = unit;
            Value       = value;
        }

        public override string ToString()
        {
            return String.Format("{0:F1}", Value);
        }
    }

    public static class MeasurementConverter
    {
        private static Double KilogramToPound(in Double kilograms)
        {
            return (kilograms * 2.2d);
        }

        private static Double PoundToKilogram(in Double pounds)
        {
            return (pounds * 0.45359237);
        }

        private static Double CentimeterToInch(in Double centimetres)
        {
            return (centimetres * 0.393700787);
        }

        private static Double InchToCentimeter(in Double inches)
        {
            return (inches * 2.54);
        }

        private static Measurement ConvertLength(in Measurement from, in Unit to)
        {
            Measurement result = null;
            if (from.Unit == to)
                result = from;
            else if (from.Unit == Unit.Centimeter && to == Unit.Inch)
                result = new Measurement(from.CreatedAt, CentimeterToInch(from.Value), Unit.Inch);
            else if (from.Unit == Unit.Inch && to == Unit.Centimeter)
                result = new Measurement(from.CreatedAt, InchToCentimeter(from.Value), Unit.Centimeter);

            return result;
        }

        public static Boolean TryConvert(Measurement input, Unit type, out Measurement output)
        {
            Boolean result = input.Unit.IsCompatibleWith(type);
            UnitType measurementType = type.GetUnitType();
            if (result)
            {
                switch (measurementType)
                {
                    case UnitType.Length:
                        output = ConvertLength(input, type);
                        break;
                    case UnitType.Weight:
                    case UnitType.Time:
                    default:
                        throw new NotImplementedException();
                }
                output = new Measurement(input.CreatedAt, input.Value, input.Unit);
            }
            else
                output = null;

            return result;
        }
        public static Measurement Convert(Measurement input, Unit unit)
        {
            Measurement result = null;
            Boolean isConverted = TryConvert(input, unit, out result);

            return !isConverted
                ? throw new ConversionException(String.Format("Error converting unit {0} to {1}", input.Unit.ToString(), unit.ToString()))
                : result;
        }
    }
}
