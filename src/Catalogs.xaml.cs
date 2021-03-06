﻿using _4charp.Common;
using _4charp.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI;

namespace _4charp
{
    public sealed partial class Catalogs : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public Catalogs()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
        }

        private async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            /*Random rand = new Random();
            Color backColor = Color.FromArgb(255, (byte)rand.Next(255), (byte)rand.Next(255), (byte)rand.Next(255));
            Brush brush = new SolidColorBrush(backColor);
            this.back.Background = brush;*/

            LoadingBar.IsEnabled = true;
            LoadingBar.Visibility = Visibility.Visible;

            this.DefaultViewModel["Board"] = (_4chanDataBoard)e.NavigationParameter;
            var _4chanDataCatalog = await _4chanDataSource.GetCatalogdAsync((_4chanDataBoard)e.NavigationParameter);
            this.DefaultViewModel["Catalog"] = _4chanDataCatalog;

            LoadingBar.IsEnabled = false;
            LoadingBar.Visibility = Visibility.Collapsed;
        }

        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var thread = (_4chanDataCatalog)e.ClickedItem;
            this.Frame.Navigate(typeof(Thread), thread);
        }

        #region NavigationHelper registration

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}