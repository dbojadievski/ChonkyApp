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
    public partial class PulseView : ContentPage
    {
        PulseViewModel viewModel;
        public PulseView()
        {
            InitializeComponent();
            
            viewModel = new PulseViewModel();
            this.BindingContext = viewModel;
        }
    }
}