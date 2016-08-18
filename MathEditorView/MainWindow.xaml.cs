using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MathEditorView
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel mainViewModel = new MainWindowViewModel();
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = mainViewModel;
        }

        private double inputOffsetY = 0;

        private PageBlock currentPage=new PageBlock();

        public PageBlock CurrentPage
        {
            get { return currentPage; }
            set { currentPage = value; }
        }

        private RowBlock currentRow=new RowBlock() { RowTop=10};

        public RowBlock CurrentRow
        {
            get { return currentRow; }
            set { currentRow = value; }
        }

        private bool isEquationSelected = false;

        private void ControlButton_Clicked(object sender, RoutedEventArgs e)
        {
            ButtonBase commandBtn = sender as ButtonBase;
            var viewModel = this.DataContext as MainWindowViewModel;
            if (commandBtn.Name == "exponentialButton")
            {
                //System.Windows.Input.InputLanguageManager.SetInputLanguage()
                //InputLanguageManager.SetInputLanguage(caretTextBox, CultureInfo.CreateSpecificCulture("zh-Hans"));
                              
                inputOffsetY = caretTextBox.FontSize * 0.3;
                var lineOfssetX = AcceptInputText(0.0, inputOffsetY);
                SetCaretLocation(lineOfssetX, 0);
            }
            else if (commandBtn.Name == "restoreButton")
            {
                var lineOfssetX = AcceptInputText(0, 0);
                SetCaretLocation(lineOfssetX, inputOffsetY);
            }
            else if (commandBtn.Name== "equationToggleButton")
            {
                var equationBtn = sender as ToggleButton;
                if (equationBtn.IsChecked==true)
                {
                    isEquationSelected = true;
                    viewModel.ChangeSelectedFontFamily("Times New Roman");
                    caretTextBox.FontStyle = FontStyles.Italic;
                }
                else
                {
                    isEquationSelected = false;
                    viewModel.ChangeSelectedFontFamily("宋体");
                    caretTextBox.FontStyle = FontStyles.Normal;
                }

                var lineOfssetX = AcceptInputText(0, 0);
                SetCaretLocation(lineOfssetX, inputOffsetY);

            }
        }

        private void SetCaretLocation(double x, double y)
        {
            var oldCaretLeft = Canvas.GetLeft(caretTextBox);
            var oldCaretTop = Canvas.GetTop(caretTextBox);

            Canvas.SetLeft(caretTextBox, oldCaretLeft + x);
            Canvas.SetTop(caretTextBox, oldCaretTop + y);
        }

        private TextBlockData GenerateTextBlockData(double rowTop,double rowTopOffsetY)
        {
            TextBlockData textBlockData = new TextBlockData();
            textBlockData.Text= caretTextBox.Text;
            textBlockData.FontSize= caretTextBox.FontSize;

           


            return textBlockData;
        }

        private double AcceptInputText(double lineOffsetX, double lineOffsetY)
        {
            var viewModel = this.DataContext as MainWindowViewModel;
            TextBlock inputedTextBlock = new TextBlock();
            inputedTextBlock.Text = caretTextBox.Text;
            inputedTextBlock.FontSize = caretTextBox.FontSize;           
            inputedTextBlock.FontFamily = viewModel.SelectedFontFamily.FontFamilyEntity;
            inputedTextBlock.FontStyle = FontStyles.Italic; //isEquationSelected ? FontStyles.Normal : FontStyles.Italic;           
            FormattedText formatted = new FormattedText(inputedTextBlock.Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(inputedTextBlock.FontFamily.ToString()), inputedTextBlock.FontSize, inputedTextBlock.Foreground);

            var oldCaretLeft = Canvas.GetLeft(caretTextBox);
            var oldCaretTop = Canvas.GetTop(caretTextBox);

            editorCanvas.Children.Add(inputedTextBlock);

            Canvas.SetLeft(inputedTextBlock, oldCaretLeft + lineOffsetX);
            Canvas.SetTop(inputedTextBlock, oldCaretTop + lineOffsetY);
            caretTextBox.Text = string.Empty;

            return formatted.WidthIncludingTrailingWhitespace;

        }

        private void editorCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            caretTextBox.Focus();
        }



        private void caretTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var currentLanguage = System.Windows.Input.InputLanguageManager.Current.CurrentInputLanguage;



            var obj = currentLanguage;
        }

        private bool IsChinese(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;

            text = text.Trim();

            foreach (char c in text)
            {
                if (c > 0x80) return true;
            }

            return false;
        }

        public int IsNumeric(string str)
        {
            int i;
            if (str != null && System.Text.RegularExpressions.Regex.IsMatch(str, @"^-?\d+(\.\d+)?$"))
                i = int.Parse(str);
            else
                i = -1;
            return i;
        }
    }
}
