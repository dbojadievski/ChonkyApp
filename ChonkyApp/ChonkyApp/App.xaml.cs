using ChonkyApp.Services;
using ChonkyApp.Views;

using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ChonkyApp
{
    public partial class App : Application
    {
        private async Task InitializeDatabase()
        {
            //TODO(Constantine): Move to a new DB Initializer class.
            await UnitDataStore.Initialize();
            MeasurementDataStore.Initialize();
            await MeasurementKindDataStore.Initialize();
            await DailyGoalDataStore.Initialize();
        }

        public App()
        {
            
            InitializeComponent();
            InitializeDatabase();

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
