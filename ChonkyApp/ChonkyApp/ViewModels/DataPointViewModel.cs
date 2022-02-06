using ChonkyApp.Commands;
using ChonkyApp.Models;
using ChonkyApp.Services;

using Microcharts;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace ChonkyApp.ViewModels
{
    public class DataPointViewModel : ViewModelBase
    {
        private readonly uint maxDaysInChart = 180;
        private readonly uint maxDaysInLookBackWindow = 14;

        private readonly String SodiumReachedMessage =  (String) Application.Current.Resources["SodiumGoalReachedMessage"];
        private readonly String PotassiumReachedMessage = (String)Application.Current.Resources["PotassiumGoalReachedMessage"];
        private readonly String VitaminAReachedMessage = (String)Application.Current.Resources["VitaminAGoalReachedMessage"];
        private readonly String VitaminCReachedMessage = (String)Application.Current.Resources["VitaminCGoalReachedMessage"];
        private readonly String VitaminEReachedMessage = (String)Application.Current.Resources["VitaminEGoalReachedMessage"];

        public DailyGoalDataStore DailyGoalDataStore = new DailyGoalDataStore();
        public UserDataStore UserProfileDataStore = new UserDataStore();

        bool isBusy = true;

        double prevWeightMeasurement;
        double prevFatMeasurement;

        Measurement bodyWeight;
        Measurement bodyFat;

        private ICommand saveDataPointCommand;
        private ICommand saveDailyGoalCommand;
        private ICommand calcBodyFatCommand;

        private LineChart bodyWeightChart;
        private LineChart bodyFatChart;
        private LineChart wellnessChart;

        DailyGoalEntry dailyGoalSodium;
        DailyGoalEntry dailyGoalPotassium;
        DailyGoalEntry dailyGoalVitaminA;
        DailyGoalEntry dailyGoalVitaminC;
        DailyGoalEntry dailyGoalVitaminE;

        BodyFatRange bodyFatRange;
        double bmi;
        double ffmi;
        double deltaWeight;

        String goalMessage;
        private UserProfile currentProfile;

        String bodyWeightInput;
        String bodyFatInput;

        String chestSkinfold;
        String abdomenSkinfold;
        String thighSkinfold;
        String tricepsSkinfold;
        String suprailiacSkinfold;

        PulseViewModel pulseViewModel;

        public UserProfile CurrentProfile
        {
            get => currentProfile;
            set => SetProperty(ref currentProfile, value);
        }

        public DailyGoalEntry DailyGoalSodium
        {
            get => dailyGoalSodium;
            private set
            {
                SetProperty(ref dailyGoalSodium, value);
                OnPropertyChanged(nameof(HasAchievedSodium));
                OnPropertyChanged(nameof(SpaceGoalSodiumStyle));
            }
        }

        public DailyGoalEntry DailyGoalPotassium
        {
            get => dailyGoalPotassium;
            private set
            {
                SetProperty(ref dailyGoalPotassium, value);
                OnPropertyChanged(nameof(HasAchievedPotassium));
                OnPropertyChanged(nameof(SpaceGoalPotassiumStyle));

            }
        }

        public DailyGoalEntry DailyGoalVitaminA
        {
            get => dailyGoalVitaminA;
            private set
            {
                SetProperty(ref dailyGoalVitaminA, value);
                OnPropertyChanged(nameof(HasAchievedVitaminA));
                OnPropertyChanged(nameof(SpaceGoalVitaminAStyle));

            }
        }

        public DailyGoalEntry DailyGoalVitaminC
        {
            get => dailyGoalVitaminC;
            private set
            {
                SetProperty(ref dailyGoalVitaminC, value);
                OnPropertyChanged(nameof(HasAchievedVitaminC));
                OnPropertyChanged(nameof(SpaceGoalVitaminCStyle));
            }
        }

        public DailyGoalEntry DailyGoalVitaminE
        {
            get => dailyGoalVitaminE;
            private set
            {
                SetProperty(ref dailyGoalVitaminE, value);
                OnPropertyChanged(nameof(HasAchievedVitaminE));
                OnPropertyChanged(nameof(SpaceGoalVitaminEStyle));

            }
        }

        public PulseViewModel PulseViewModel
        {
            get => pulseViewModel;
            set => SetProperty(ref pulseViewModel, value);
        }

        public Guid SodiumID
        {
            get => DailyGoalDataStore.SpaceGoalSodiumID;
        }

        public Guid PotassiumID
        {
            get => DailyGoalDataStore.SpaceGoalPotassiumID;
        }

        public Guid VitaminAID
        {
            get => DailyGoalDataStore.SpaceGoalVitaminAID;
        }

        public Guid VitaminCID
        {
            get => DailyGoalDataStore.SpaceGoalVitaminCID;
        }

        public Guid VitaminEID
        {
            get => DailyGoalDataStore.SpaceGoalVitaminEID;
        }

        private bool HasAchievedGoal(DailyGoalEntry goal)
        {
            return ((goal != null) && (goal.CreatedAt.Date == DateTime.Today));
        }

        public async Task<bool> HasAchievedGoal(Guid goal)
        {
            bool hasAchievedGoal = false;

            if (goal != null)
            {
                var goals = await DailyGoalDataStore.GetGoalEntriesOfTypeForDay(goal, DateTime.Today);
                hasAchievedGoal =  (goals.Count() > 0);

            }
            return hasAchievedGoal;
        }

        public bool HasAchievedSodium
        {
            get => HasAchievedGoal(DailyGoalSodium);
        }

        public bool HasAchievedPotassium
        {
            get => HasAchievedGoal(DailyGoalPotassium);
        }

        public bool HasAchievedVitaminA
        {
            get => HasAchievedGoal(DailyGoalVitaminA);
        }

        public bool HasAchievedVitaminC
        {
            get => HasAchievedGoal(DailyGoalVitaminC);
        }

        public bool HasAchievedVitaminE
        {
            get => HasAchievedGoal(DailyGoalVitaminE);
        }

        public Style SpaceGoalSodiumStyle
        {
            get
            {
                var styleName = HasAchievedSodium ? "SpaceGoalButtonStyle" : "SpaceGoalButtonUncheckedStyle";
                var styleVale = (Style) Application.Current.Resources[styleName];

                return styleVale;
            }
        }

        public Style SpaceGoalPotassiumStyle
        {
            get
            {
                var styleName = HasAchievedPotassium ? "SpaceGoalButtonStyle" : "SpaceGoalButtonUncheckedStyle";
                return (Style)Application.Current.Resources[styleName];
            }
        }

        public Style SpaceGoalVitaminAStyle
        {
            get
            {
                var styleName = HasAchievedVitaminA ? "SpaceGoalButtonStyle" : "SpaceGoalButtonUncheckedStyle";
                return (Style)Application.Current.Resources[styleName];
            }
        }

        public Style SpaceGoalVitaminCStyle
        {
            get
            {
                var styleName = HasAchievedVitaminC ? "SpaceGoalButtonStyle" : "SpaceGoalButtonUncheckedStyle";
                return (Style)Application.Current.Resources[styleName];
            }
        }

        public Style SpaceGoalVitaminEStyle
        {
            get
            {
                var styleName = HasAchievedVitaminE ? "SpaceGoalButtonStyle" : "SpaceGoalButtonUncheckedStyle";
                return (Style)Application.Current.Resources[styleName];
            }
        }

        public LineChart BodyWeightChart
        {
            get => bodyWeightChart;
            set => SetProperty(ref bodyWeightChart, value);
        }

        public LineChart BodyFatChart
        {
            get => bodyFatChart;
            set => SetProperty(ref bodyFatChart, value);
        }

        public LineChart AthletePulseChart
        {
            get => wellnessChart;
            set => SetProperty(ref wellnessChart, value);
        }

        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }

        public bool IsImperial
        {
            get => CurrentProfile.IsImperial;
        }

        public Measurement BodyWeight
        {
            get => bodyWeight;
            set => SetProperty(ref bodyWeight, value);
        }

        public Measurement BodyFat
        {
            get => bodyFat;
            set => SetProperty(ref bodyFat, value);
        }

        string title = "ChonkyApp";
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        public ICommand SaveDataPointCommand
        {
            get => saveDataPointCommand;
        }

        public ICommand SaveDailyGoalCommand
        {
            get => saveDailyGoalCommand;
            set => SetProperty(ref saveDailyGoalCommand, value);
        }

        public ICommand EstimateBodyFatCommand
        {
            get => calcBodyFatCommand;
            set => SetProperty(ref calcBodyFatCommand, value);
        }

        public String GoalMessage
        {
            get => goalMessage;
            set => SetProperty(ref goalMessage, value);
        }

        async Task<double> CalcBMI()
        {
            var weight = await GetAverageWeeklyWeight(true); ;
            var height = CurrentProfile.Height;

            var heightMeasurement = new Measurement(DateTime.Now, height, Unit.Centimeter);
            var bmi = Calculators.Calculators.CalculateBMI(weight, heightMeasurement);

            return bmi;
        }

        public Double BMI
        {
            get => bmi;
            set
            {
                SetProperty(ref bmi, value);
                OnPropertyChanged(BMIText);
            }
        }

        public String BMIText
        {
            get
            {
                var builder = new StringBuilder();
                if (Double.IsNaN(BMI))
                    builder.Append("?");
                else
                    builder.Append(BMI);

                return builder.ToString();
            }
        }

        public async Task<Measurement> GetAverageWeeklyWeight(bool metricize = false)
        {
            var weight = Double.NaN;

            var itemsInWeek = MeasurementDataStore.GetWeightMeasurements(7);
            if (metricize)
                itemsInWeek = MetricizeEntries(itemsInWeek);

            if (itemsInWeek.Count() > 0)
                weight = itemsInWeek.Average(e => e.Value);

            var measurement = new Measurement(DateTime.Now, weight, Unit.Kilogram);
            return measurement;
        }

        public async Task<Measurement> GetAverageWeeklyFat()
        {
            var fat = 0.0d;

            var itemsInWeek = MeasurementDataStore.GetBodyFatMeasurements(7);
            if (itemsInWeek.Count() > 0)
                fat = itemsInWeek.Average(e => e.Value);

            var measurement = new Measurement(DateTime.Now, fat, Unit.Percent);
            return measurement;
        }

        public async Task<Double> CalcDeltaWeight()
        {
            
            var dW = Double.NaN;

            var currWeekAvg = (await GetAverageWeeklyWeight(true)).Value;
            var prevWeekAvg = Double.NaN;

            var today = DateTime.Now;
            var lastWeek = today.AddDays(-7);
            var twoWeeksAgo = today.AddDays(-14);

            var itemsInWeek = MeasurementDataStore.GetWeightMeasurementsBetweenDates(twoWeeksAgo, lastWeek);
            if (itemsInWeek.Count() > 0)
            {
                itemsInWeek = MetricizeEntries(itemsInWeek);
                prevWeekAvg = itemsInWeek.Average(e => e.Value);
            
                if (currWeekAvg > 0 && prevWeekAvg > 0)
                    dW = Math.Round(currWeekAvg - prevWeekAvg, 1);
            }

            return dW;
        }

        public Double DeltaWeight
        {
            get => deltaWeight;
            set 
            { 
                SetProperty(ref deltaWeight, value);
                OnPropertyChanged(DeltaWeightText);
            }
        }

        public String DeltaWeightText
        {
            get
            {
                StringBuilder builder = new StringBuilder();

                if (Double.IsNaN(DeltaWeight))
                    builder.Append("?");
                else
                    builder.Append(String.Format("{0} kg/wk", DeltaWeight));

                return builder.ToString();
            }
        }

        async Task<Double> CalcFFMI()
        {
            var lastDay = DateTime.Today.AddDays(-7);
            var height = new Measurement(DateTime.Now, CurrentProfile.Height, CurrentProfile.IsImperial ? Unit.Inch : Unit.Centimeter);
            MeasurementConverter.TryConvert(height, height.Unit, out var heightInCm);

            var weight = await GetAverageWeeklyWeight();
            var fat = await GetAverageWeeklyFat();
            var ffmi = Double.NaN;
            if (weight.Value != 0 && height.Value != 0 && fat.Value != 0)
                ffmi = Calculators.Calculators.CalculateFFMI(weight, heightInCm, fat);

            return ffmi;
        }
        public double FFMI
        {
            get => ffmi;
            set 
            {
                SetProperty(ref ffmi, value);
                OnPropertyChanged(FFMIText);   
            }
        }

        public string FFMIText
        {
            get
            {
                var builder = new StringBuilder();

                if (Double.IsNaN(FFMI))
                    builder.Append("?");
                else
                    builder.Append(FFMI);
                
                return builder.ToString();
            }
        }

        async Task<BodyFatRange> CalcBodyFatRange()
        {
            var lastDay = DateTime.Today.AddDays(-7);
            var fat = 0.0d;
            var range = BodyFatRange.Unknown;

            var fatMeasurement = await GetAverageWeeklyFat();
            fat = fatMeasurement.Value;

            if (fat != 0 && CurrentProfile != null)
                range = Calculators.Calculators.CalculateBodyFatRange(fatMeasurement, CurrentProfile);

            return range;
        }

        public BodyFatRange BodyFatRange
        {
            get => bodyFatRange;
            set 
            { 
                SetProperty(ref bodyFatRange, value);
                OnPropertyChanged(BodyFatRangeText);
            }
        }

        public String BodyFatRangeText
        {
            get
            {
                var builder = new StringBuilder();
                if (BodyFatRange == BodyFatRange.Unknown)
                    builder.Append("?");
                else
                    builder.Append(BodyFatRange.ToString());

                return builder.ToString();
            }

        }

        public String BodyWeightInput
        {
            get => bodyWeightInput;
            set => SetProperty(ref bodyWeightInput, value);
        }

        public String BodyFatInput
        {
            get => bodyFatInput;
            set => SetProperty(ref bodyFatInput, value);
        }

        public String ChestSkinfold
        {
            get => chestSkinfold;
            set => SetProperty(ref chestSkinfold, value);
        }

        public String AbdomenSkinfold
        {
            get => abdomenSkinfold;
            set => SetProperty(ref abdomenSkinfold, value);
        }

        public String ThighSkinfold
        {
            get => thighSkinfold;
            set => SetProperty(ref thighSkinfold, value);
        }

        public String TricepsSkinfold
        {
            get => tricepsSkinfold;
            set => SetProperty(ref tricepsSkinfold, value);
        }

        public String SuprailiacSkinfold
        {
            get => suprailiacSkinfold;
            set => SetProperty(ref suprailiacSkinfold, value);
        }

        public DataPointViewModel()
        {
            CurrentProfile = UserProfileDataStore.GetCurrentProfile().Result;
            PulseViewModel = new PulseViewModel();

            bodyWeight = new Measurement(DateTime.Now, 70.0, Unit.Kilogram);
            bodyFat = new Measurement(DateTime.Now, 15.0, Unit.Percent);

            saveDataPointCommand = new AddDataPointCommand(this);
            SaveDailyGoalCommand = new SaveDailyGoalCommand(this);
            EstimateBodyFatCommand = new CalculateBodyFatCommand(); 

            BodyWeightChart = new LineChart();
            var entries = new List<ChartEntry>();
            var nullEntry = new ChartEntry(0);
            nullEntry.Color = SkiaSharp.SKColors.Purple;
            entries.Add(nullEntry);
            BodyWeightChart.BackgroundColor = SkiaSharp.SKColors.Black;
            BodyWeightChart.Entries = entries;

            BodyFatChart = new LineChart();
            var fatEntries = new List<ChartEntry>();
            var nullFatEntry = new ChartEntry(0);
            BodyFatChart.BackgroundColor = SkiaSharp.SKColors.Black;
            nullFatEntry.Color = SkiaSharp.SKColors.Purple;
            fatEntries.Add(nullFatEntry);
            BodyFatChart.Entries = fatEntries;

            AthletePulseChart = new LineChart();
            var wellnessEntries = new List<ChartEntry>();
            wellnessEntries.Add(new ChartEntry(0));
            AthletePulseChart.BackgroundColor = SkiaSharp.SKColors.Black;
            AthletePulseChart.Entries = wellnessEntries;
            var asList = wellnessEntries.ToList();
            _ = RefreshData();
        }

        private async Task RefreshData( bool refreshWeight = true, bool refreshFat = true, bool refeshSpaceGoals = true, bool refreshWellness = true)
        {
            DateTime lastDayOfLookBackWindow = DateTime.Now.AddDays(-maxDaysInLookBackWindow);
            if (refreshWeight)
            {  
                var weightEntries = MeasurementDataStore.GetWeightMeasurements(maxDaysInChart);
                if (weightEntries.Count() == 0)
                    weightEntries.Add(BodyWeight);

                RefreshChart(BodyWeightChart, weightEntries);

                var latestWeightMeasurement = weightEntries.LastOrDefault();
                BodyWeight.Value = latestWeightMeasurement.Value;
                prevWeightMeasurement = BodyWeight.Value;
                BodyWeightInput = BodyWeight.Value.ToString();
            }

            if (refreshFat)
            {
                var fatEntries = MeasurementDataStore.GetBodyFatMeasurements(maxDaysInChart);
                if (fatEntries.Count() == 0)
                    fatEntries.Add(BodyFat);

                RefreshChart(BodyFatChart, fatEntries);

                var latestFatMeasurement = fatEntries.LastOrDefault();
                BodyFat.Value = latestFatMeasurement.Value;
                prevFatMeasurement = BodyFat.Value;
                BodyFatInput = BodyFat.Value.ToString();
                BodyFatRange = await CalcBodyFatRange();
            }

            if (refeshSpaceGoals)
            {
                RefreshSpaceGoals();
            }

            if (refreshWellness)
            {
                List<ChartEntry> chartEntries = new List<ChartEntry>();
                var minMeasurement = PulseViewModel.RecentWellnessData.Min(m => m.Value);
                var maxMeasurement = PulseViewModel.RecentWellnessData.Max(m => m.Value);


                float? prevEntry = null;
                foreach (var currEntry in PulseViewModel.RecentWellnessData)
                {
                    var val = (float) currEntry.Value;
                    if (prevEntry.HasValue)
                    {
                        var computedVal = prevEntry.Value + val;
                        prevEntry = computedVal;
                        val = computedVal;
                    }
                    else
                    {
                        val = 0;
                        prevEntry = 0;
                    }

                    ChartEntry entry = new ChartEntry(val);
                    entry.Color = SkiaSharp.SKColors.Purple;
                    chartEntries.Add(entry);
                }

                if ( chartEntries.Count == 0)
                {
                    // Add a default entry so that the stupid chart lib doesn't crash.
                    chartEntries.Add(new ChartEntry(0));
                }

                AthletePulseChart.MinValue = chartEntries.Min(e => e.Value);
                AthletePulseChart.MaxValue = chartEntries.Max(e => e.Value);
                AthletePulseChart.Entries = chartEntries;
            }


            BMI = await CalcBMI();
            FFMI = await CalcFFMI();
            BodyFatRange = await CalcBodyFatRange();
            DeltaWeight = await CalcDeltaWeight();

            OnPropertyChanged();
            GenerateGoalMessage();

            void RefreshChart(LineChart chart, IEnumerable<Measurement> entries)
            {
                List<Measurement> normalizedEntries = new List<Measurement>(entries.Count());
                foreach (var entry in entries)
                {
                    Measurement normalizedEntry = null;
                    switch (entry.MeasurementType)
                    {
                        case UnitType.Length:
                            _ = MeasurementConverter.TryConvert(entry, IsImperial ? Unit.Inch : Unit.Centimeter, out normalizedEntry);
                            break;
                        case UnitType.Weight:
                            _ = MeasurementConverter.TryConvert(entry, IsImperial ? Unit.Pound : Unit.Kilogram, out normalizedEntry);
                            break;
                        default:
                            {
                                normalizedEntry = entry;
                                break;
                            }
                    }
                }

                List<ChartEntry> chartEntries = new List<ChartEntry>();
                var minMeasurement = entries.Min(m => m.Value);
                var maxMeasurement = entries.Max(m => m.Value);

                chart.MinValue = (float)minMeasurement;
                chart.MaxValue = (float)maxMeasurement;

                foreach (var currEntry in entries)
                {
                    ChartEntry entry = new ChartEntry((float)currEntry.Value);
                    entry.Color = SkiaSharp.SKColors.Purple;
                    chartEntries.Add(entry);
                }

                chart.Entries = chartEntries;
            }

            async Task RefreshSpaceGoals()
            {

                DailyGoalSodium = null;
                DailyGoalPotassium = null;
                DailyGoalVitaminA = null;
                DailyGoalVitaminC = null;
                DailyGoalVitaminE = null;

                var achievedGoals = await DailyGoalDataStore.GetGoalEntriesForDay(DateTime.Today);
                foreach (var goal in achievedGoals)
                {
                    if (goal.GoalID == DailyGoalDataStore.SpaceGoalSodiumID)
                        DailyGoalSodium = goal;
                    else if (goal.GoalID == DailyGoalDataStore.SpaceGoalPotassiumID)
                        DailyGoalPotassium = goal;
                    else if (goal.GoalID == DailyGoalDataStore.SpaceGoalVitaminAID)
                        DailyGoalVitaminA = goal;
                    else if (goal.GoalID == DailyGoalDataStore.SpaceGoalVitaminCID)
                        DailyGoalVitaminC = goal;
                    else if (goal.GoalID == DailyGoalDataStore.SpaceGoalVitaminEID)
                        DailyGoalVitaminE = goal;
                }

                OnPropertyChanged();
            }

            ChestSkinfold = "5";
            AbdomenSkinfold = "5";
            ThighSkinfold = "5";
        }

        private void GenerateGoalMessage()
        {
            GoalMessage = "";

            if (BodyFatRange == BodyFatRange.High)
                GoalMessage = "Your body fat seems a bit high. Consider cutting down a bit.";
            else if (BodyFatRange == BodyFatRange.CriticallyHigh)
                GoalMessage = "This is unhealthy. Start your cut as soon as possible.";
            else if (BodyFatRange == BodyFatRange.Low)
                GoalMessage = "There's some room to gain some weight, if you'd like.";
            else if (BodyFatRange == BodyFatRange.CrititcallyLow)
                GoalMessage = "You're significantly underweight. Start gaining as soon as possible.";
            else
            {
                var dW = DeltaWeight;

                if (dW == 0)
                    GoalMessage = "Are your numbers up to date?";
                else if (Double.IsNaN(dW))
                    GoalMessage = "I don't have enough info to give you any advice now. Keep me in the loop, OK?";
                else if (CurrentProfile.Goal == UserGoal.Gain)
                {
                    if (dW > 0.3d)
                        GoalMessage = "This rate of weight gain is unsustainable. Slow down.";
                    else if (dW > .25d)
                        GoalMessage = "You're gaining fast, but not dramatically so. Consider slowing down.";
                    else if (dW >= .15d && dW <= .25d)
                        GoalMessage = "I like what I'm seeing. Keep that scale climbing!";
                    else if (dW >= -.2d && dW < .15d)
                        GoalMessage = "Your weight is fluctuating a bit, but mostly static. Consider increasing calories.";
                    else
                        GoalMessage = "You're not gaining any weight. Eat more.";
                }
                else if (CurrentProfile.Goal == UserGoal.Lose)
                {
                    if (dW > -.12d)
                        GoalMessage = "You're not losing any weight. Consider reducing calories, or increasing activity.";
                    else if (dW < -.2d)
                        GoalMessage = "This is a nice rate of cutting. Keep going.";
                    else if (dW < -.3)
                        GoalMessage = "You're losing fast. Be careful.";
                    else if (dW < -.5)
                        GoalMessage = "This rate of weight loss is unsustainable. Slow down.";
                }
            }
        }

        public async Task SaveDataPoint()
        {
            // Validate input.
            if (!Double.TryParse(BodyWeightInput, out var newWeightVal))
                GoalMessage = "That weight doesn't look like a number. Try again.";
            else if (!Double.TryParse(BodyFatInput, out var newFatVal))
                GoalMessage = "That body fat doesn't look like a number. Try again.";
            else
            {
                BodyWeight = new Measurement(DateTime.Now, newWeightVal, BodyWeight.Unit, BodyWeight.Name);
                bool refreshWeight = (prevWeightMeasurement != BodyWeight.Value);
                if (refreshWeight)
                    _ =MeasurementDataStore.AddItem(BodyWeight);

                BodyFat = new Measurement(DateTime.Now, newFatVal, BodyFat.Unit, BodyFat.Name);
                bool refreshFat = (prevFatMeasurement != BodyFat.Value);
                if (refreshFat)
                    _ =MeasurementDataStore.AddItem(BodyFat);

                _ = RefreshData(refreshWeight, refreshFat);

            }
        }
        
        public async Task<bool> MarkGoalAsAchieved(Guid goal)
        {
            DailyGoalEntry entry = new DailyGoalEntry()
            {
                GoalID = goal
            };
            
            var isSuccess = await DailyGoalDataStore.InsertGoalEntry(entry);
            if (isSuccess)
            {
                await RefreshData(false, false, true, false);

                if (goal == SodiumID)
                    GoalMessage = SodiumReachedMessage;
                else if (goal == PotassiumID)
                    GoalMessage = PotassiumReachedMessage;
                else if (goal == VitaminAID)
                    GoalMessage = VitaminAReachedMessage;
                else if (goal == VitaminCID)
                    GoalMessage = VitaminCReachedMessage;
                else if (goal == VitaminEID)
                    GoalMessage = VitaminEReachedMessage;
                else
                    GoalMessage = "Congratulations! You reached your daily goal!";
            }
            
            return isSuccess;
        }

        public async Task<bool> MarkGoalAsUnachieved(Guid goal)
        {
            DailyGoalEntry entry = new DailyGoalEntry()
            { 
                GoalID = goal
            };

            var isSuccess = await DailyGoalDataStore.DeleteGoalEntry(entry);
            await RefreshData(false, false, true, false);
            return isSuccess;
        }

        private List<Measurement> MetricizeEntries(List<Measurement> entries)
        {
            List<Measurement> metricEntries = new List<Measurement>(entries.Count());

            foreach (var entry in entries)
            {
                Unit metricUnit;
                if (entry.MeasurementType == UnitType.Weight)
                    metricUnit = Unit.Kilogram;
                else if (entry.MeasurementType == UnitType.Length)
                    metricUnit = Unit.Centimeter;
                else
                    metricUnit = entry.Unit;

                Measurement metricEntry = entry;
                if (metricUnit != entry.Unit)
                    MeasurementConverter.TryConvert(entry, metricUnit, out metricEntry);
                metricEntries.Add(metricEntry);
            }

            return metricEntries;
        }

        public async Task<Measurement> EstimateBodyFat()
        {
            Measurement bodyFatEstimate = null;
            
            var measurements = new List<CustomMeasurement>();

            if (CurrentProfile.Sex == Sex.Male)
            {
                var isChestInputValid = Double.TryParse(ChestSkinfold, out double chestSkinfoldValue);
                var isAbdominalInputValid = Double.TryParse(AbdomenSkinfold, out double abdominalSkinfoldValue);
                var isThighInputValid = Double.TryParse(ThighSkinfold, out double thighSkinfoldValue);

                if (isChestInputValid && isAbdominalInputValid && isThighInputValid)
                {
                    var chestMeasurement = new CustomMeasurement(DateTime.Now, chestSkinfoldValue, MeasurementKindDataStore.ChestSkinfoldMeasurementKind.ID);
                    var abdominalMeasurement = new CustomMeasurement(DateTime.Now, abdominalSkinfoldValue, MeasurementKindDataStore.AbdomenSkinfoldMeasurementKind.ID);
                    var thighMeasurement = new CustomMeasurement(DateTime.Now, thighSkinfoldValue, MeasurementKindDataStore.ThighSkinfoldMeasurementKind.ID);

                    measurements.Add(chestMeasurement);
                    measurements.Add(abdominalMeasurement);
                    measurements.Add(thighMeasurement);

                    bodyFatEstimate = Calculators.Calculators.CalculateJasonPollockFatPercentage(measurements, CurrentProfile);
                }
            }
            else
            {
                var isThighInputValid = Double.TryParse(ThighSkinfold, out double thighSkinfoldValue);
                var isTricepsInputValid = Double.TryParse(TricepsSkinfold, out double tricepsSkinfoldValue);
                var isSuprailiacInputValid = Double.TryParse(SuprailiacSkinfold, out double suprailiacSkinfoldValue);

                if (isThighInputValid && isTricepsInputValid && isSuprailiacInputValid)
                {
                    var thighMeasurement = new CustomMeasurement(DateTime.Now, thighSkinfoldValue, MeasurementKindDataStore.ThighSkinfoldMeasurementKind.ID);
                    var tricepsMeasurement = new CustomMeasurement(DateTime.Now, tricepsSkinfoldValue, MeasurementKindDataStore.TricepsSkinfoldMeasurementKind.ID);
                    var suprailiacMeasurement = new CustomMeasurement(DateTime.Now, suprailiacSkinfoldValue, MeasurementKindDataStore.SuprailiumSkinfoldMeasurementKind.ID);

                    measurements.Add(thighMeasurement);
                    measurements.Add(tricepsMeasurement);
                    measurements.Add(suprailiacMeasurement);

                    bodyFatEstimate = Calculators.Calculators.CalculateJasonPollockFatPercentage(measurements, CurrentProfile);
                }
            }

            if (bodyFatEstimate != null)
            {
                var result = await App.Current.MainPage.DisplayAlert("Estimate Complete", $"You seem to be at {bodyFatEstimate.Value}% body fat.\r\nWould you like me to remember this?", "Yes (Remember)", "No (Forget)");
                if (result)
                {
                    MeasurementDataStore.AddItem(bodyFatEstimate);
                    MeasurementKindDataStore.Insert(measurements);
                    await RefreshData();
                }
            }
            else
                await App.Current.MainPage.DisplayAlert("Error", "I couldn't calculate your estimate. Try again later.", "OK");

            App.Current.MainPage.Navigation.PopAsync();

            return bodyFatEstimate;
        }
    }
}
