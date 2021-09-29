using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ClonePad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _unsavedChanges = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void WordWrap_Click(object sender, RoutedEventArgs e)
        {
            Editor.TextWrapping = WordWrap.IsChecked ? TextWrapping.WrapWithOverflow : TextWrapping.NoWrap;

            UpdateLayout();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var sizeSelector = (MenuItem)sender;
            Editor.FontSize = double.Parse(sizeSelector.Tag as string);
            
            foreach (var item in FontSizeSelectors.Items)
            {
                if (item is not MenuItem) continue;
                ((MenuItem)item).IsChecked = false;
            }

            sizeSelector.IsChecked = true;

            UpdateLayout();
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFile();
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            if (_unsavedChanges)
            {
                MessageBoxResult result = MessageBox.Show("Did you want to save before starting a new file?", "Unsaved Changes", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    SaveFile();
                }
            }
            Editor.Text = string.Empty;
            _unsavedChanges = false;
            UpdateLayout();
        }

        private void SaveFile()
        {
            var saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, Editor.Text);
            }
        }

        private async void Open_Click(object sender, RoutedEventArgs e)
        {
            if (_unsavedChanges)
            {
                MessageBoxResult result = MessageBox.Show("Did you want to save before opening a new file?", "Unsaved Changes", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    SaveFile();
                }
            }
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (openFileDialog.ShowDialog() == true)
            {
                Editor.Text = await File.ReadAllTextAsync(openFileDialog.FileName);
            }
            _unsavedChanges = false;
            UpdateLayout();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_unsavedChanges)
            {
                MessageBoxResult result = MessageBox.Show("Did you want to save before exiting?", "Unsaved Changes", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    SaveFile();
                }
            }
        }

        private void Editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            _unsavedChanges = true;
        }

        private void About_Screen(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("ClonePad v1.0 \nSimple text editing app \nWirtten by Nicolas Gerhardt", "About",MessageBoxButton.OK, MessageBoxImage.Information);

        }
    }
}
