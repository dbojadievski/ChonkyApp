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
    public partial class MainPage : ContentPage
    {
     
        private DataPointViewModel viewModel;
        public MainPage()
        {
            InitializeComponent();
            
            viewModel = new DataPointViewModel();
            BindingContext = viewModel;
        }


        private void ProfilePageButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ProfilePage());
        }

        private void ButtonAddCustomMeasurements_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddCustomMeasurementPage());
        }

        private void HeartbeatButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new PulseView());
        }

        private void ButtonEstimate_Clicked(object sender, EventArgs e)
        {
            var page = new ModalDialogPage(viewModel);
            Navigation.PushModalAsync(page);
        }
    }
}