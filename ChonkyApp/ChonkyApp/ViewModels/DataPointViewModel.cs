using ChonkyApp.Commands;
using ChonkyApp.Models;
using ChonkyApp.Services;

using Microcharts;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace ChonkyApp.ViewModels
{
    public class DataPointViewModel : INotifyPropertyChanged
    {
        public MeasurementDataStore DataStore = new MeasurementDataStore();

        bool isBusy = true;
        bool isImperial = true;

        double prevWeightMeasurement;
        double prevFatMeasurement;

        Measurement bodyWeight;
        Measurement bodyFat;

        private ICommand saveDataPointCommand;

        private LineChart bodyWeightChart;
        private LineChart bodyFatChart;

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

        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }

        public bool IsImperial
        {
            get => isImperial;
            set => SetProperty(ref isImperial, value);
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

        public DataPointViewModel()
        {                                                                                                              
            bodyWeight = new Measurement(DateTime.Now, 93.0, Unit.Kilogram);
            bodyFat = new Measurement(DateTime.Now, 17.4, Unit.Percent);

            saveDataPointCommand = new AddDataPointCommand(this);

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

            _ = RefreshChartEntries();
        }

        private async Task RefreshChartEntries( bool refreshWeight = true, bool refreshFat = true)
        {
            void RefreshChart(LineChart chart, IEnumerable<Measurement> entries)
            {
                List<ChartEntry> chartEntries = new List<ChartEntry>();
                var minMeasurement = entries.Min(m => m.Value);
                var maxMeasurement = entries.Max(m => m.Value);

                chart.MinValue = (float) minMeasurement;
                chart.MaxValue = (float) maxMeasurement;

                foreach (var currEntry in entries)
                {
                    ChartEntry entry = new ChartEntry((float)currEntry.Value);
                    entry.Color = SkiaSharp.SKColors.Purple;
                    chartEntries.Add(entry);
                }

                chart.Entries = chartEntries;
            }

            if (refreshWeight)
            {  
                var weightEntries = await DataStore.GetWeightMeasurements();
                RefreshChart(BodyWeightChart, weightEntries);

                var latestWeightMeasurement = weightEntries.LastOrDefault();
                BodyWeight.Value = latestWeightMeasurement.Value;
                prevWeightMeasurement = BodyWeight.Value;
            }

            if (refreshFat)
            {
                var fatEntries = await DataStore.GetBodyFatMeasurements();
                RefreshChart(BodyFatChart, fatEntries);

                var latestFatMeasurement = fatEntries.LastOrDefault();
                BodyFat.Value = latestFatMeasurement.Value;
                prevFatMeasurement = BodyFat.Value;
            }
        }

        public async Task SaveDataPoint()
        {
            BodyWeight.CreatedAt = DateTime.Now;
            BodyFat.CreatedAt = DateTime.Now;

            bool refreshWeight = (prevWeightMeasurement != BodyWeight.Value);
            bool refreshFat = (prevFatMeasurement != BodyFat.Value);

            if (refreshWeight)
                _ =DataStore.AddItemAsync(BodyWeight);

            if (refreshFat)
                _ =DataStore.AddItemAsync(BodyFat);

            _ = RefreshChartEntries(refreshWeight, refreshFat);
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
