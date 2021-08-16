﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MicroscopeGUI
{
    public partial class FactorInputBox : Window
    {
        static readonly Regex NumRegex = new Regex("[^0-9.,]+"); // Regex that only allows numbers, a dot and a comma
        public bool Aborted = true;

        public FactorInputBox()
        {
            InitializeComponent();

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            InputBox.Focus();
        }

        // Checks if the input in the box is numeric and prevents it, if not
        private void CheckIfNumeric(object sender, TextCompositionEventArgs e) =>
            e.Handled = NumRegex.IsMatch(e.Text);

        private void SubmitClick(object sender, RoutedEventArgs e) =>
            SubmitForm();

        private void InputKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SubmitForm();
        }

        private void AbortClick(object sender, RoutedEventArgs e) =>
            Close();

        void SubmitForm()
        {
            // Won't submit until there is something in the InputBox
            if (InputBox.Text != string.Empty)
            {
                // Replacing the ',' with a '.' since the double.Parse()-function only accepts a '.' 
                InputBox.Text.Replace(',', '.');
                Aborted = false;
                Close();
            }
        }
    }
}
