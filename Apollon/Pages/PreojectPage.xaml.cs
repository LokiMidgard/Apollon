using System;
using Windows.UI.Xaml.Controls;

namespace Apollon.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PreojectPage : Page
    {
        public PreojectPage()
        {
            this.InitializeComponent();
            Init();
        }

        private async void Init()
        {
            if (App.CurrentProject == null)
                App.SetProject(new Presentation.Music.ProjectViewModel());

            this.DataContext = await App.CurrentProject;
        }
    }
}
