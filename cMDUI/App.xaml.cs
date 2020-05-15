using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using customMD;

namespace WpfApp1{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application{
        public static CSS css = new CSS();
        public static CodeBlockPage code_block_page;
        public static TitlePage title_page;
    }
}