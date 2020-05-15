using System.Windows.Controls;
using customMD;

namespace WpfApp1{
    public partial class CodeBlockPage : Page{
        public CodeBlockPage(){
            InitializeComponent();
            App.code_block_page = this;
        }

        public ElementSelector GetStyle(){
            ElementSelector elementSelector = new ElementSelector(MarkType.p);
            elementSelector.addStyle("font-size", CodeBlockFontSize.Text);
            elementSelector.addStyle("margin-left", CodeBlockMarginLeft.Text);
            elementSelector.addStyle("margin-top", CodeBlockMarginTop.Text);
            elementSelector.addStyle("margin-right", CodeBlockMarginRight.Text);
            elementSelector.addStyle("margin-bottom", CodeBlockMarginBottom.Text);
            if (CodeBlockAlignRbCenter.IsChecked != null && (bool) CodeBlockAlignRbCenter.IsChecked){
                elementSelector.addStyle("text-align", "center");
            }
            if (CodeBlockAlignRbLeft.IsChecked != null && (bool) CodeBlockAlignRbLeft.IsChecked){
                elementSelector.addStyle("text-align", "left");
            }
            if (CodeBlockAlignRbRight.IsChecked != null && (bool) CodeBlockAlignRbRight.IsChecked){
                elementSelector.addStyle("text-align", "right");
            }
            return elementSelector;
        }
    }
}