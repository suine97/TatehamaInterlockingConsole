﻿using System;
using System.Windows;
using TatehamaInterlockingConsole.ViewModels;

namespace TatehamaInterlockingConsole.Views
{
    /// <summary>
    /// StationWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class StationWindow : Window
    {
        public StationWindow(StationViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            Loaded += OnLoaded;
            Closing += OnClosing;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // DataContextの初期サイズをウィンドウに設定
            if (DataContext is StationViewModel viewModel)
            {
                Width = viewModel.WindowWidth;
                Height = viewModel.WindowHeight;
            }
        }

        private void OnClosing(object sender, EventArgs e)
        {
            if (DataContext is StationViewModel viewModel)
            {
                viewModel.OnClosing();
            }
        }
    }
}