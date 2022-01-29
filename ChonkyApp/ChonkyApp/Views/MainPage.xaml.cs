using ChonkyApp.Models;
using ChonkyApp.ViewModels;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ChonkyApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
     
        private DataPointViewModel viewModel;
        public LoginPage()
        {
            InitializeComponent();
            
            viewModel = new DataPointViewModel();
            BindingContext = viewModel;

        }

        //private void switchUnit_Toggled(object sender, ToggledEventArgs e)
        //{
        //}

        private void Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ProfilePage());
        }

        private void ButtonAddCustomMeasurements_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddCustomMeasurementPage());
        }
    }
}