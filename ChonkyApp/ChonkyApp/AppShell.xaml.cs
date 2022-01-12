using ChonkyApp.ViewModels;
using ChonkyApp.Views;

using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace ChonkyApp
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
        }

    }
}
