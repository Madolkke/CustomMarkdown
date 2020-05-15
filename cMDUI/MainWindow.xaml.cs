using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        void OpenMenu(object sender, RoutedEventArgs e){
            this.StyleDrawer.IsLeftDrawerOpen = true;
        }

        private void GoToGenerationPage(object sender, RoutedEventArgs e){
            GenerationWindow gw = new GenerationWindow();
            Console.WriteLine(App.code_block_page == null);
            Console.WriteLine(App.title_page == null);
            if (App.code_block_page != null) App.css.addSelector(App.code_block_page.GetStyle());
            if (App.title_page != null) App.css.addSelector(App.title_page.GetStyle());
            Console.WriteLine(App.css.ToString());
            gw.Show();
            this.Close();
        }

        private void ToTITLE2(object sender, RoutedEventArgs e){
            this.PageFrame.Source = new Uri("pack://application:,,,/TitlePage.xaml");
        }

        private void ToSplitter(object sender, RoutedEventArgs e){
            this.PageFrame.Source = new Uri("pack://application:,,,/CodeBlockPage.xaml");
        }
    }
}