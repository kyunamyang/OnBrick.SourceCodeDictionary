﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace OnBrick.SourceCodeDictionary.Views
{
    public sealed partial class SettingPage : Page
    {
        //ResourceLoader _resourceLoader = ResourceLoader.GetForCurrentView();

        public string Version
        {
            get
            {
                var version = Windows.ApplicationModel.Package.Current.Id.Version;
                return String.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
            }
        }

        public string Author
        {
            get
            {
                var author = Windows.ApplicationModel.Package.Current.PublisherDisplayName;
                return author;
            }
        }

        public SettingPage()
        {
            this.InitializeComponent();
        }

        private void page_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void tbOpenSource_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ttOpenSource.IsOpen = true;
        }
    }
}