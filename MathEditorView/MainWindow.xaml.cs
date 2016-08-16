using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
        public MainWindow()
        {
            InitializeComponent();
        }

        private double inputOffsetY = 0;

        private void ControlButton_Clicked(object sender, RoutedEventArgs e)
        {
            Button commandBtn = sender as Button;

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
        }

        private void SetCaretLocation(double x, double y)
        {
            var oldCaretLeft = Canvas.GetLeft(caretTextBox);
            var oldCaretTop = Canvas.GetTop(caretTextBox);

            Canvas.SetLeft(caretTextBox, oldCaretLeft + x);
            Canvas.SetTop(caretTextBox, oldCaretTop + y);
        }

        private double AcceptInputText(double lineOffsetX, double lineOffsetY)
        {
            TextBlock inputedTextBlock = new TextBlock();
            inputedTextBlock.Text = caretTextBox.Text;
            inputedTextBlock.FontSize = caretTextBox.FontSize;
            inputedTextBlock.FontStyle = caretTextBox.FontStyle;
            inputedTextBlock.FontFamily = caretTextBox.FontFamily;
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
