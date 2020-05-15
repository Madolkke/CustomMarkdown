using System.Windows.Controls;
using customMD;

namespace WpfApp1{
    public partial class TitlePage : Page{
        public TitlePage(){
            InitializeComponent();
            App.title_page = this;
        }

        public ElementSelector GetStyle(){
            ElementSelector elementSelector = new ElementSelector(MarkType.h1);
            elementSelector.addTarget(MarkType.h2);
            elementSelector.addTarget(MarkType.h3);
            elementSelector.addTarget(MarkType.h4);
            elementSelector.addTarget(MarkType.h5);
            elementSelector.addTarget(MarkType.h6);
            if (TitleAlignRbRight.IsChecked != null && (bool) TitleAlignRbRight.IsChecked){
                elementSelector.addStyle("text-align", "right");
            }
            if (TitleAlignRbLEFT.IsChecked != null && (bool) TitleAlignRbLEFT.IsChecked){
                elementSelector.addStyle("text-align", "left");
            }
            if (TitleAlignRbCENTER.IsChecked != null && (bool) TitleAlignRbCENTER.IsChecked){
                elementSelector.addStyle("text-align", "center");
            }
            elementSelector.addStyle("color", this.TitleTextColorValue.Text);
            return elementSelector;
        }
    }
}