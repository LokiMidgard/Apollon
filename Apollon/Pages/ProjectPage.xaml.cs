using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Apollon.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProjectPage : Page
    {
        public ProjectPage()
        {
            this.InitializeComponent();
            Init();
        }

        private async void Init()
        {
            var projectViewModel = await (App.CurrentProject ?? Task.FromResult<Presentation.Music.ProjectViewModel>(null));
            if (projectViewModel == null)
            {
                projectViewModel = new Presentation.Music.ProjectViewModel();
                App.SetProject(projectViewModel);
            }

            this.DataContext = projectViewModel;
        }
    }
}
