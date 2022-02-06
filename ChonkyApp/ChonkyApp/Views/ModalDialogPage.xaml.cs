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
    public partial class ModalDialogPage : ContentPage
    {
        DataPointViewModel viewModel;
        public ModalDialogPage(DataPointViewModel vm)
        {
            viewModel = vm;
            BindingContext = viewModel;
            InitializeComponent();
        }
    }
}