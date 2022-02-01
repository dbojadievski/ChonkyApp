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
    public partial class NavigationPage : TabbedPage
    {
        private PulseViewModel pulseViewModel;
        
        public NavigationPage()
        {
            InitializeComponent();
            pulseViewModel = new PulseViewModel();
            this.BindingContext = pulseViewModel;
        }
    }
}