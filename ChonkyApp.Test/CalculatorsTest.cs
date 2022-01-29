using ChonkyApp.Models;

using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChonkyApp.Test
{
    [TestFixture]
    public class CalculatorsTest
    {
        [Test]
        public void BMITest()
        {
            Measurement weight = new Measurement(DateTime.Now, 16.9, Unit.Kilogram);
            Measurement height = new Measurement(DateTime.Now, 105.4, Unit.Centimeter);

            var bmi = Calculators.Calculators.CalculateBMI(weight, height);
            Assert.AreEqual(bmi, 15.2);
        }

        [Test]
        public void FFMITest()
        {
            Measurement weight = new Measurement(DateTime.Now, 93, Unit.Kilogram);
            Measurement height = new Measurement(DateTime.Now, 183, Unit.Centimeter);
            Measurement bodyFat = new Measurement(DateTime.Now, 17.4, Unit.Percent);

            var ffmi = Calculators.Calculators.CalculateFFMI(weight, height, bodyFat);
            Assert.AreEqual(ffmi, 22.9);
        }

    }
}
