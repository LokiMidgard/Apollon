﻿using Intense.Presentation;
using System;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Apollon.Pages;
using Apollon.Presentation;

namespace Apollon
{
    public sealed partial class Shell : UserControl
    {
        public Shell()
        {
            this.InitializeComponent();

            var vm = new ShellViewModel();
            vm.TopItems.Add(new NavigationItem { Icon = "", DisplayName = "Player", PageType = typeof(PlayerPage) });
            vm.TopItems.Add(new NavigationItem { Icon = "", DisplayName = "Project Editor", PageType = typeof(ProjectPage) });
            vm.TopItems.Add(new NavigationItem { Icon = "", DisplayName = "Song Editor", PageType = typeof(Page2) });
            vm.TopItems.Add(new NavigationItem { Icon = "", DisplayName = "Page 3", PageType = typeof(Page3) });

            vm.BottomItems.Add(new NavigationItem { Icon = "", DisplayName = "Settings", PageType = typeof(SettingsPage) });

            // select the first top item
            vm.SelectedItem = vm.TopItems.First();

            this.ViewModel = vm;
        }

        public ShellViewModel ViewModel { get; private set; }

        public Frame RootFrame
        {
            get
            {
                return this.Frame;
            }
        }
    }
}
