using System;
using System.Windows;
using customMD;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace WpfApp1{
    public partial class GenerationWindow : Window{
        public GenerationWindow(){
            InitializeComponent();
        }

        private void BackToStyle(object sender, RoutedEventArgs e){
            MainWindow mw = new MainWindow();
            App.css = new CSS();
            Close();
            mw.Show();
        }

        private void Generate(object sender, RoutedEventArgs e){
            string markdown_file_path;
            string generate_dir;
            string config_file_path;
            if (GWChooseMarkdownFileLabel.Content.ToString() == "Choose Markdown File"){
                markdown_file_path = null;
            }
            else{
                markdown_file_path = GWChooseMarkdownFileLabel.Content.ToString();
            }

            if (GWChooseDirLabel.Content.ToString() == "Choose Generation Dir"){
                generate_dir = null;
            }
            else{
                generate_dir = GWChooseDirLabel.Content.ToString();
            }

            if (GWChooseConfigFileLabel.Content.ToString() == "Choose Config File"){
                config_file_path = null;
            }
            else{
                config_file_path = GWChooseConfigFileLabel.Content.ToString();
            }

            Console.WriteLine(markdown_file_path);

            new Generator(markdown_file_path, App.css, generate_dir, config_file_path).Generate();
        }

        private void ChooseGenerationDir(object sender, RoutedEventArgs e){
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok){
                GWChooseDirLabel.Content = dialog.FileName;
            }
        }

        private void ChooseMarkdownFile(object sender, RoutedEventArgs e){
            var dialog = new OpenFileDialog();
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            dialog.Filter = "Markdown Files|*.md";
            var d = dialog.ShowDialog();
            if (d == null) return;
            if ((bool) d){
                GWChooseMarkdownFileLabel.Content = dialog.FileName;
            }
        }

        private void ChooseConfigFile(object sender, RoutedEventArgs e){
            var dialog = new OpenFileDialog();
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            dialog.Filter = "Markdown Files|*.md";
            var d = dialog.ShowDialog();
            if (d == null) return;
            if ((bool) d){
                GWChooseConfigFileLabel.Content = dialog.FileName;
            }
        }
    }
}