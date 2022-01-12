using NUnit.Framework;
using ChonkyApp.Models;
using System;

namespace ChonkyApp.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CmToInchTest()
        {
            Measurement cmMeasurement = new Measurement(DateTime.Now, 100, Unit.Centimeter);
            Measurement inchMeasurement;
            Assert.IsTrue(MeasurementConverter.TryConvert(cmMeasurement, Unit.Inch, out inchMeasurement));
            Assert.IsTrue(inchMeasurement.Unit == Unit.Inch);
            Assert.AreEqual(inchMeasurement.Value, 39.3700787);
        }
    }
}