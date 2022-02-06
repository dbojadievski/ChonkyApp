using ChonkyApp.Models;

using System;
using System.Collections.Generic;
using System.Text;

using System.Diagnostics;
using System.Linq;

namespace ChonkyApp.Calculators
{
    public static class Calculators
    {

        public static Double CalculateBMI(Measurement weight, Measurement height)
        {
            Double bmi = 0.0d;

            Debug.Assert(weight != null);
            Debug.Assert(weight.MeasurementType == UnitType.Weight);

            Debug.Assert(height != null);
            Debug.Assert(height.MeasurementType == UnitType.Length);

            if ((weight.MeasurementType == UnitType.Weight) && (height.MeasurementType == UnitType.Length))
            {
                MeasurementConverter.TryConvert(weight, Unit.Kilogram, out Measurement weightInKg);
                MeasurementConverter.TryConvert(height, Unit.Centimeter, out Measurement heightInCms);

                bmi = (((weightInKg.Value / heightInCms.Value) / heightInCms.Value)) * 10000;
                bmi = Math.Round(bmi, 1);
            }
            return bmi;
        }

        public static Double CalculateFatFreeMass(Measurement weight, Measurement bodyFat)
        {
            Double fatFreeMass = 0;

            if (weight.MeasurementType == UnitType.Weight && bodyFat.Unit == Unit.Percent)
            {
                MeasurementConverter.TryConvert(weight, Unit.Kilogram, out Measurement weightInKg);
                fatFreeMass = weightInKg.Value * (1 - (bodyFat.Value / 100));
                fatFreeMass = Math.Round(fatFreeMass, 1);
            }

            return fatFreeMass;
        }

        public static Double CalculateFFMI(Measurement weight, Measurement height, Measurement bodyFat)
        {
            Double ffmi = 0.0d;

            Debug.Assert(weight != null);
            Debug.Assert(weight.MeasurementType == UnitType.Weight);

            Debug.Assert(bodyFat != null);
            Debug.Assert(bodyFat.Unit == Unit.Percent);

            if (weight.MeasurementType == UnitType.Weight && bodyFat.Unit == Unit.Percent && height.MeasurementType == UnitType.Length)
            {
                var fatFreeMass = CalculateFatFreeMass(weight, bodyFat);

                MeasurementConverter.TryConvert(weight, Unit.Kilogram, out Measurement weightInKg);
                MeasurementConverter.TryConvert(height, Unit.Centimeter, out Measurement heightInCm);

                var heightInMeters = heightInCm.Value * .01;
                ffmi = fatFreeMass / (heightInMeters * heightInMeters);
                ffmi = Math.Round(ffmi, 1);
            }

            return ffmi;
        }

        public static BodyFatRange CalculateBodyFatRange(Measurement bodyFatPercentage, UserProfile user)
        {
            var range = BodyFatRange.Unknown;

            switch (user.Sex)
            {
                case Sex.Male:
                    range = CalculateBodyFatRangeForMen(bodyFatPercentage, user);
                    break;
                case Sex.Female:
                    range = CalculateBodyFatRangeForWomen(bodyFatPercentage, user);
                    break;
            }

            return range;

            BodyFatRange CalculateBodyFatRangeForMen(Measurement fatPercentage, UserProfile profile)
            {
                var fatRange = BodyFatRange.Unknown;

                if (fatPercentage.Unit == Unit.Percent && profile.Sex == Sex.Male && fatPercentage.Value > 0)
                {
                    var percentage = fatPercentage.Value;
                    if (percentage >= 25)
                        fatRange = BodyFatRange.CriticallyHigh;
                    else if (percentage >= 20)
                        fatRange = BodyFatRange.High;
                    else if (percentage >= 14)
                        fatRange = BodyFatRange.Normal;
                    else if (percentage >= 10)
                        fatRange = BodyFatRange.Low;
                    else
                        fatRange = BodyFatRange.CrititcallyLow;
                }

                return fatRange;
            }

            BodyFatRange CalculateBodyFatRangeForWomen(Measurement fatPercentage, UserProfile profile)
            {
                var fatRange = BodyFatRange.Unknown;

                if (fatPercentage.Unit == Unit.Percent && profile.Sex == Sex.Female && fatPercentage.Value > 0)
                {
                    var percentage = fatPercentage.Value;
                    if (percentage >= 31)
                        fatRange = BodyFatRange.CriticallyHigh;
                    else if (percentage >= 25)
                        fatRange = BodyFatRange.High;
                    else if (percentage >= 20)
                        fatRange = BodyFatRange.Normal;
                    else if (percentage >= 15)
                        fatRange = BodyFatRange.Low;
                    else
                        fatRange = BodyFatRange.CrititcallyLow;
                }

                return fatRange;
            }
        }

        public static Measurement CalculateJasonPollockFatPercentage(IEnumerable<CustomMeasurement> sites, UserProfile profile)
        {
            // First, make sure all the sites are what they're supposed to be.
            Measurement bodyFatMeasurement = null;
            var bodyFat = 0d;
            {
                {
                    var density = 0d;

                    if (profile.Sex == Sex.Male)
                    {
                        var ageFactor = (0.0002574 * profile.AgeInYears);

                        var chest = sites.Where(site => site.MeasurementKindID == Services.MeasurementKindDataStore.ChestSkinfoldMeasurementKind.ID).FirstOrDefault();
                        var abdomen = sites.Where(site => site.MeasurementKindID == Services.MeasurementKindDataStore.AbdomenSkinfoldMeasurementKind.ID).FirstOrDefault();
                        var thigh = sites.Where(site => site.MeasurementKindID == Services.MeasurementKindDataStore.ThighSkinfoldMeasurementKind.ID).FirstOrDefault();

                        if (chest == null || abdomen == null || thigh == null)
                            throw new ArgumentException("Missing data to calculate Jason-Pollock equation for male.");
                        
                        
                        var sum = chest.Value + abdomen.Value + thigh.Value;
                        var sumSquared = (sum * sum);

                        density = (1.10938d - (0.0008267d * sum) + (0.0000016d * sumSquared) - ageFactor);
                    }
                    else if (profile.Sex == Sex.Female)
                    {
                        var ageFactor = (0.0001392 * profile.AgeInYears);

                        var thigh = sites.Where(site => site.MeasurementKindID == Services.MeasurementKindDataStore.ThighSkinfoldMeasurementKind.ID).FirstOrDefault();
                        var triceps = sites.Where(site => site.MeasurementKindID == Services.MeasurementKindDataStore.TricepsSkinfoldMeasurementKind.ID).FirstOrDefault();
                        var suprailium = sites.Where(site => site.MeasurementKindID == Services.MeasurementKindDataStore.SuprailiumSkinfoldMeasurementKind.ID).FirstOrDefault();

                        if (thigh == null || triceps == null || suprailium == null)
                            throw new ArgumentException("Missing data to calculate Jason-Pollock equation for female.");

                        var sum = thigh.Value + triceps.Value + suprailium.Value;
                        var sumSquared = (sum * sum);

                        density = (1.0994921d - (0.0009929d * sum) + (0.0000023d * sumSquared) - ageFactor);
                    }

                    // Siri formula for body fat % estimation from density.
                    bodyFat = Math.Truncate(((495d / density) - 450d));
                    bodyFatMeasurement = new Measurement(DateTime.Now, bodyFat, Unit.Percent, "Body fat");
                }
            }

            return bodyFatMeasurement;
        }

    }
}
