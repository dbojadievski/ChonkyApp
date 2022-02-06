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
    public class PulseViewModel : ViewModelBase
    {
        private const String MESSAGE_PULSE_PROMPT = "How are you feeling today?\r\nBe honest.";
        private const String MESSAGE_PULSE_DATA_ENTERED = "Nice! This'll help me understand you better.";
        private const String MESSAGE_PULSE_DATA_STILL_FRESH = "We have some good data already.\r\n Come back tomorrow.";
        private const String MESSAGE_PULSE_DATA_FIELD_SKIPPED = "Looks like you missed some questions.";
        private const String MESSAGE_PULSE_DATA_INSERT_FAILED = "Uh-on.\r\nI couldn't save your answers.";
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

        Boolean isPulseStale;

        List<CustomMeasurement> recentWellnessData;
        private IEnumerable<CustomMeasurement> sleepData;
        private IEnumerable<CustomMeasurement> moodData;
        private IEnumerable<CustomMeasurement> stressData;
        private IEnumerable<CustomMeasurement> sorenessData;
        private IEnumerable<CustomMeasurement> nutritionData;

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
            set => SetProperty(ref mood, value);
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

        public String GoalMessage
        {
            get => statusMessage;
            set => SetProperty(ref statusMessage, value);
        }

        public IEnumerable<CustomMeasurement> SleepData
        {
            get => sleepData;
            set => SetProperty(ref sleepData, value);
        }

        public IEnumerable<CustomMeasurement> MoodData
        {
            get => moodData;
            set => SetProperty(ref moodData, value);
        }

        public IEnumerable<CustomMeasurement> SorenessData
        {
            get => sorenessData;
            set => SetProperty(ref sorenessData, value);
        }

        public IEnumerable<CustomMeasurement> StressData
        {
            get => stressData;
            set => SetProperty(ref stressData, value);
        }

        public IEnumerable<CustomMeasurement> NutritionData
        {
            get => nutritionData;
            set => SetProperty(ref nutritionData, value);
        }

        public List<CustomMeasurement> RecentWellnessData
        {
            get => recentWellnessData;
            set => SetProperty(ref recentWellnessData, value);
        }

        private void LoadDaily()
        {
            var wellnessItemCutoffTime = DateTime.Today;
            var dailyItems = sleepData.Where(dt => dt.CreatedAt >= wellnessItemCutoffTime);
            
            IsPulseStale = (dailyItems == null || dailyItems.Count() == 0);
        }

        private void ClearValues()
        {
            Sleep = -2;
            Mood = -2;
            Soreness = -2;
            Stress = -2;
            Nutrition = -2;
        }

        private async Task RefreshData(Boolean onSave = false)
        {
            ClearValues();

            var recentCutoff = DateTime.Now.AddDays(7 * 8 * -1);
            SleepData = MeasurementKindDataStore.Find(MeasurementKindDataStore.SleepMeasurementKind, recentCutoff);
            AverageSleep = SleepData.Count() == 0 ? 0d : sleepData.Average(e => e.Value);

            MoodData = MeasurementKindDataStore.Find(MeasurementKindDataStore.MoodMeasurementKind, recentCutoff);
            AverageMood = MoodData.Count() == 0 ? 0d : moodData.Average(e => e.Value);

            SorenessData = MeasurementKindDataStore.Find(MeasurementKindDataStore.SorenessMeasurementKind, recentCutoff);
            AverageSoreness = SorenessData.Count() == 0 ? 0d : sorenessData.Average(e => e.Value);

            StressData = MeasurementKindDataStore.Find(MeasurementKindDataStore.StressMeasurementKind, recentCutoff);
            AverageStress = StressData.Count() == 0 ? 0d : stressData.Average(e => e.Value);

            NutritionData = MeasurementKindDataStore.Find(MeasurementKindDataStore.NutritionMeasurementKind, recentCutoff);
            AverageNutrition = NutritionData.Count() == 0 ? 0d: nutritionData.Average(e => e.Value);

            RecentWellnessData = new List<CustomMeasurement>();

            var numEntries = SleepData.Count();
            for (var i = 0; i < numEntries; i++)
            {
                var sleepEntry = SleepData.ElementAt(i);
                var moodEntry = MoodData.ElementAt(i);
                var sorenessEntry = SorenessData.ElementAt(i);
                var stressEntry = StressData.ElementAt(i);
                var nutritionEntry = NutritionData.ElementAt(i);

                var avg = ((sleepEntry.Value + moodEntry.Value + sorenessEntry.Value + stressEntry.Value + nutritionEntry.Value) / 5d);
                RecentWellnessData.Add(new CustomMeasurement(sleepEntry.CreatedAt, avg, Guid.Empty));
            }

            LoadDaily();

            if (!onSave)
            {
                if (IsPulseStale)
                    GoalMessage = MESSAGE_PULSE_PROMPT;
                else
                    GoalMessage = MESSAGE_PULSE_DATA_STILL_FRESH;
            }
        }

        public void RecordEntry()
        {
            if (IsPulseStale)
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
                        GoalMessage = MESSAGE_PULSE_DATA_ENTERED;
                        RefreshData(true);
                    }
                    else
                    {
                        GoalMessage = MESSAGE_PULSE_DATA_INSERT_FAILED;
                    }
                }
                else
                {
                    // Pop dialog something is missing.
                    GoalMessage = MESSAGE_PULSE_DATA_FIELD_SKIPPED;
                }
            }
            else
            {
                GoalMessage = MESSAGE_PULSE_DATA_STILL_FRESH;
            }
        }

        public PulseViewModel()
        {
            SaveEntryCommand = new RecordPulseEntryCommand();
            RefreshData();
        }
    }
}
