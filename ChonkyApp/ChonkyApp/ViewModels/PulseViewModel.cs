using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ChonkyApp.Models;
using ChonkyApp.Services;
using System.Windows.Input;
using ChonkyApp.Commands;

using Xamarin.Forms;
using ChonkyApp.Views;

namespace ChonkyApp.ViewModels
{
    public class PulseViewModel: ViewModelBase
    {
        Double averageWellness;

        Double averageSleep;
        Double averageMood;
        Double averageSoreness;
        Double averageStress;
        Double averageNutrition;

        Double sleep;
        Double mood;
        Double soreness;
        Double stress;
        Double nutrition;

        Boolean isSaveEnabled;
        Boolean isPulseStale;

        private String statusMessage;
        private ICommand saveEntryCommand;

        public Double AverageWellness
        {
            get => averageWellness;
            set => SetProperty(ref averageWellness, value);
        }

        public Double AverageSleep
        {
            get => averageSleep;
            set => SetProperty(ref averageSleep, value);
        }

        public Double AverageMood
        {
            get => averageMood;
            set => SetProperty(ref averageMood, value);
        }

        public Double AverageSoreness
        {
            get => averageSoreness;
            set => SetProperty(ref averageSoreness, value);
        }

        public Double AverageStress
        {
            get => averageStress;
            set => SetProperty(ref averageStress, value);
        }

        public double AverageNutrition
        {
            get => averageNutrition;
            set => SetProperty(ref averageNutrition, value);
        }

        public Double Sleep
        {
            get => sleep;
            set => SetProperty(ref sleep, value);
        }

        public Double Mood
        {
            get => mood;
            set => SetProperty (ref mood, value);
        }
        public Double Soreness
        {
            get => soreness;
            set => SetProperty(ref soreness, value);
        }

        public Double Stress
        {
            get => stress;
            set => SetProperty(ref stress, value);
        }
        public Double Nutrition
        {
            get => nutrition;
            set => SetProperty(ref nutrition, value);
        }

        public ICommand SaveEntryCommand
        {
            get => saveEntryCommand;
            set => SetProperty(ref saveEntryCommand, value);
        }

        public Boolean IsSaveEnabled
        {
            get => (Sleep >= -2 && Mood > -2 && Soreness > -2 && Stress > -2 && Nutrition > -2);
            set { }
        }

        public Boolean IsPulseStale
        {
            get => isPulseStale;
            set => SetProperty(ref isPulseStale, value);
        }

        public String StatusMessage
        {
            get => statusMessage;
            set => SetProperty(ref statusMessage, value);
        }

        private void LoadDaily()
        {
            var wellnessItemCutoffTime = DateTime.Now.AddHours(-8);

            // Since the items 
            var items = MeasurementKindDataStore.FindWellnessItems(wellnessItemCutoffTime);
            IsPulseStale = (items == null || items.Count() == 0);
        }

        private void ClearValues()
        {
            Sleep = -2;
            Mood = -2;
            Soreness = -2;
            Stress = -2;
            Nutrition = -2;
        }

        private async Task RefreshData()
        {
            StatusMessage = "How do you feel today?" + "\r\n" + "Be honest";

            LoadDaily();
            ClearValues();

            var sleepEntries = MeasurementKindDataStore.Find(MeasurementKindDataStore.SleepMeasurementKind);
            AverageSleep = sleepEntries.Count() == 0 ? 0d : sleepEntries.Average(e => e.Value);

            var moodEntries = MeasurementKindDataStore.Find(MeasurementKindDataStore.MoodMeasurementKind);
            AverageMood = moodEntries.Count() == 0 ? 0d : moodEntries.Average(e => e.Value);

            var sorenessEntries = MeasurementKindDataStore.Find(MeasurementKindDataStore.SorenessMeasurementKind);
            AverageSoreness = sorenessEntries.Count() == 0 ? 0d : sorenessEntries.Average(e => e.Value);

            var stressEntries = MeasurementKindDataStore.Find(MeasurementKindDataStore.StressMeasurementKind);
            AverageStress = stressEntries.Count() == 0 ? 0d : stressEntries.Average(e => e.Value);

            var nutritionEntries = MeasurementKindDataStore.Find(MeasurementKindDataStore.NutritionMeasurementKind);
            AverageNutrition = nutritionEntries.Count() == 0 ? 0d: nutritionEntries.Average(e => e.Value);
        }

        public void RecordEntry()
        {
            if (IsSaveEnabled)
            {
                var sleepMeasurement = new CustomMeasurement(DateTime.Now, Sleep, MeasurementKindDataStore.SleepMeasurementKind.ID);
                var moodMeasurement = new CustomMeasurement(DateTime.Now, Mood, MeasurementKindDataStore.MoodMeasurementKind.ID);
                var sorenessMeasurement = new CustomMeasurement(DateTime.Now, Soreness, MeasurementKindDataStore.SorenessMeasurementKind.ID);
                var stressMeasurement = new CustomMeasurement(DateTime.Now, Stress, MeasurementKindDataStore.StressMeasurementKind.ID);
                var nutritionMeasurement = new CustomMeasurement(DateTime.Now, Nutrition, MeasurementKindDataStore.NutritionMeasurementKind.ID);

                var measurements = new List<CustomMeasurement>(5);
                measurements.Add(sleepMeasurement);
                measurements.Add(moodMeasurement);
                measurements.Add(sorenessMeasurement);
                measurements.Add(stressMeasurement);
                measurements.Add(nutritionMeasurement);
            
                var didInsert = MeasurementKindDataStore.Insert(measurements);
                if (didInsert)
                {
                    // Do this.
                }
                else
                {
                }
            }
            else
            {
                // Pop dialog something is missing.
                StatusMessage = "Looks like you missed some questions.";
            }
        }

        public PulseViewModel()
        {
            SaveEntryCommand = new RecordPulseEntryCommand();
            RefreshData();
        }
    }
}
