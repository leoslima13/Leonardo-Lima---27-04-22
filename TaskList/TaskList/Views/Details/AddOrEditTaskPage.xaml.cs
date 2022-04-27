using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TaskList.Views.Details
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddOrEditTaskPage : ContentPage
    {
        public AddOrEditTaskPage()
        {
            InitializeComponent();
        }
    }
}