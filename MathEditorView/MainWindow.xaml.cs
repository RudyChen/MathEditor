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

        private PageBlock currentPage = new PageBlock();

        public PageBlock CurrentPage
        {
            get { return currentPage; }
            set { currentPage = value; }
        }

        private RowBlock currentRow = new RowBlock() { RowTop = 10 };

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
                var lineOfssetX = AcceptEnglishInputText(0.0, inputOffsetY);
                SetCaretLocation(lineOfssetX, 0);
            }
            else if (commandBtn.Name == "restoreButton")
            {
                var lineOfssetX = AcceptEnglishInputText(0, 0);
                SetCaretLocation(lineOfssetX, inputOffsetY);
            }
            else if (commandBtn.Name == "equationToggleButton")
            {
                var equationBtn = sender as ToggleButton;
                if (equationBtn.IsChecked == true)
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

                var lineOfssetX = AcceptEnglishInputText(0, 0);
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

        private TextBlockData GenerateTextBlockData(double rowTop, double rowTopOffsetY)
        {
            TextBlockData textBlockData = new TextBlockData();
            textBlockData.Text = caretTextBox.Text;
            textBlockData.FontSize = caretTextBox.FontSize;




            return textBlockData;
        }

        private void editorCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            caretTextBox.Focus();

            Point mouseDownPoint = e.GetPosition(editorCanvas);
            bool isHalfHeadContain = false;
            var existElement = GetMostNearCaretElement(mouseDownPoint,ref isHalfHeadContain);

            if (null != existElement)
            {
                SetCaretPositionAndSize(mouseDownPoint, existElement,isHalfHeadContain);
            }
        }

        private void caretTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ////接收输入的时候拆分中文字，分成单个的统一处理
            var currentLanguage = System.Windows.Input.InputLanguageManager.Current.CurrentInputLanguage;

            double lineOffsetX = 0;

            if (IsChinese(e.Text))
            {
                lineOffsetX = AcceptChineseInputText(0, 0);
            }
            else
            {
                lineOffsetX = AcceptEnglishInputText(0, 0);
            }

            SetCaretLocation(lineOffsetX, 0);

            var obj = currentLanguage;
        }

        private bool IsChinese(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;

            text = text.Trim();

            foreach (char c in text)
            {
                if (c < 0x301E) return false;
            }

            return true;
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

        private void editorCanvas_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {

                RemoveTailItem();
            }
        }

        private void RemoveTailItem()
        {
            if (!string.IsNullOrEmpty(caretTextBox.Text))
            {
                caretTextBox.Text = string.Empty;
            }
            else
            {
                FrameworkElement beforeCaretElement = null;
                //找到光标之前的文本，移除文本
                beforeCaretElement = GetCaretBeforeElement();

                if (beforeCaretElement is TextBlock)
                {
                    var beforeTextBlock = beforeCaretElement as TextBlock;
                    editorCanvas.Children.Remove(beforeCaretElement);
                    SetCaretLocation(-beforeCaretElement.ActualWidth, 0);
                }
            }
        }

        /// <summary>
        /// 查询插字符前面的额文本元素
        /// </summary>
        /// <returns>文本元素</returns>
        private FrameworkElement GetCaretBeforeElement()
        {
            var children = LogicalTreeHelper.GetChildren(editorCanvas);
            var caretLeft = Canvas.GetLeft(caretTextBox);
            var caretTop = Canvas.GetTop(caretTextBox);
            var fontSize = caretTextBox.FontSize;
            FrameworkElement beforeElement = null;
            foreach (FrameworkElement element in children)
            {
                var elementLeft = Canvas.GetLeft(element);
                var elementTop = Canvas.GetTop(element);
                Rect elementRect = new Rect(elementLeft, elementTop, element.ActualWidth, element.ActualHeight);
                if (elementRect.Contains(new Point(caretLeft - element.ActualWidth / 6, caretTop + element.ActualHeight / 6)))
                {
                    beforeElement = element;
                    return beforeElement;
                }
            }

            return null;
        }

        private void SetCaretPositionAndSize(Point mouseDownPoint, FrameworkElement element,bool isSetBefore)
        {
            var elementLeft = Canvas.GetLeft(element);
            var elementTop = Canvas.GetTop(element);

            if (isSetBefore)
            {
                Canvas.SetLeft(caretTextBox, elementLeft -1);
            }
            else
            {
                Canvas.SetLeft(caretTextBox, elementLeft + element.ActualWidth);
            }
            
            Canvas.SetTop(caretTextBox, elementTop);

            if (element is TextBlock)
            {
                var textElement = element as TextBlock;
                caretTextBox.FontSize = textElement.FontSize;
            }
        }

        private FrameworkElement GetElementByPoint(Point point)
        {
            FrameworkElement frameworkElement = null;
            var children = LogicalTreeHelper.GetChildren(editorCanvas);
            foreach (FrameworkElement element in children)
            {
                var elementLeft = Canvas.GetLeft(element);
                var elementTop = Canvas.GetTop(element);
                Rect elementRect = new Rect(elementLeft, elementTop, element.ActualWidth + 1, element.ActualHeight);
                if (elementRect.Contains(point))
                {
                    frameworkElement = element;
                    return frameworkElement;
                }
            }

            return null;
        }

        /// <summary>
        /// 汉子点击设置插字符最佳位置
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private FrameworkElement GetMostNearCaretElement(Point point,ref bool isHalfHeadContain)
        {
            FrameworkElement frameworkElement = null;
            var children = LogicalTreeHelper.GetChildren(editorCanvas);

            Point judgePoint = new Point();
            foreach (FrameworkElement element in children)
            {
                var elementLeft = Canvas.GetLeft(element);
                var elementTop = Canvas.GetTop(element);
                judgePoint = new Point(elementLeft+element.ActualWidth*0.8, elementTop);
                Rect elementRect = new Rect(elementLeft, elementTop, element.ActualWidth + 1, element.ActualHeight);
                
                Rect halfTailElementRect = new Rect(elementLeft + element.ActualWidth / 2, elementTop, element.ActualWidth / 2 + 1, element.ActualHeight);
                if (elementRect.Contains(point) && halfTailElementRect.Contains(point))
                {
                    frameworkElement = element;
                    isHalfHeadContain = false;
                    return frameworkElement;
                }
                else if (elementRect.Contains(point) && !halfTailElementRect.Contains(point))
                {
                    frameworkElement = element;                  
                    isHalfHeadContain = true;
                    break;
                }
            }

            return frameworkElement;
        }

        private double AcceptEnglishInputText(double lineOffsetX, double lineOffsetY)
        {
            var viewModel = this.DataContext as MainWindowViewModel;
            TextBlock inputedTextBlock = new TextBlock();
            inputedTextBlock.Text = caretTextBox.Text;
            inputedTextBlock.FontSize = caretTextBox.FontSize;
            inputedTextBlock.FontFamily = viewModel.SelectedFontFamily.FontFamilyEntity;
            inputedTextBlock.FontStyle = isEquationSelected ? FontStyles.Normal : FontStyles.Italic;
            FormattedText formatted = new FormattedText(inputedTextBlock.Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(inputedTextBlock.FontFamily.ToString()), inputedTextBlock.FontSize, inputedTextBlock.Foreground);

            var oldCaretLeft = Canvas.GetLeft(caretTextBox);
            var oldCaretTop = Canvas.GetTop(caretTextBox);

            editorCanvas.Children.Add(inputedTextBlock);

            Canvas.SetLeft(inputedTextBlock, oldCaretLeft + lineOffsetX);
            Canvas.SetTop(inputedTextBlock, oldCaretTop + lineOffsetY);
            caretTextBox.Text = string.Empty;

            return formatted.WidthIncludingTrailingWhitespace;

        }

        private double AcceptChineseInputText(double lineOffsetX, double lineOffsetY)
        {
            var viewModel = this.DataContext as MainWindowViewModel;

            double allWidth = 0;
            foreach (var item in caretTextBox.Text)
            {
                TextBlock inputedTextBlock = new TextBlock();
                inputedTextBlock.Text = item.ToString();
                inputedTextBlock.FontSize = caretTextBox.FontSize;
                inputedTextBlock.FontFamily = viewModel.SelectedFontFamily.FontFamilyEntity;
                inputedTextBlock.FontStyle = isEquationSelected ? FontStyles.Normal : FontStyles.Italic;
                FormattedText formatted = new FormattedText(inputedTextBlock.Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(inputedTextBlock.FontFamily.ToString()), inputedTextBlock.FontSize, inputedTextBlock.Foreground);

                var oldCaretLeft = Canvas.GetLeft(caretTextBox);
                var oldCaretTop = Canvas.GetTop(caretTextBox);

                editorCanvas.Children.Add(inputedTextBlock);

                Canvas.SetLeft(inputedTextBlock, oldCaretLeft + allWidth + lineOffsetX);
                Canvas.SetTop(inputedTextBlock, oldCaretTop + lineOffsetY);

                allWidth += formatted.WidthIncludingTrailingWhitespace;
            }

            caretTextBox.Text = string.Empty;

            return allWidth;

        }
    }
}