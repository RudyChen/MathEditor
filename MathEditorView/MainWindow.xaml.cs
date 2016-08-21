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
        /// <summary>
        /// 公式输入栈
        /// </summary>
        private Stack<EquationType> inputTypeStack = new Stack<EquationType>();

        /// <summary>
        /// 切换输入下一部分插字符坐标
        /// </summary>
        private Stack<Point> caretJumpStack = new Stack<Point>();



        /// <summary>
        /// 块子元素
        /// </summary>
        public int InputedCount { get; set; }

        MainWindowViewModel mainViewModel = new MainWindowViewModel();
        public MainWindow()
        {
            InitializeComponent();
            string guid = Guid.NewGuid().ToString();
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

        public BaseBlock CurrentInputBlock { get; set; }
        private void ControlButton_Clicked(object sender, RoutedEventArgs e)
        {

            double caretLeft = Canvas.GetLeft(caretTextBox);
            double caretTop = Canvas.GetTop(caretTextBox);

            ButtonBase commandBtn = sender as ButtonBase;
            var viewModel = this.DataContext as MainWindowViewModel;
            if (commandBtn.Name == "exponentialButton")
            {
                //System.Windows.Input.InputLanguageManager.SetInputLanguage()
                //InputLanguageManager.SetInputLanguage(caretTextBox, CultureInfo.CreateSpecificCulture("zh-Hans"));

                ///指数输入
                inputTypeStack.Push(EquationType.Exponenttial);
                InputedCount = 0;
                viewModel.ChangeSelectedFontFamily("Times New Roman");
                caretTextBox.FontStyle = FontStyles.Italic;
                CurrentInputBlock = null;

                //inputOffsetY = caretTextBox.FontSize * 0.3;
                //var lineOfssetX = AcceptEnglishInputText(0.0, inputOffsetY);
                //SetCaretLocation(lineOfssetX, 0);
            }
            else if (commandBtn.Name == "nextPartButton")
            {
                if (inputTypeStack.Count > 0)
                {
                    EquationType inputType = inputTypeStack.Peek();
                    switch (inputType)
                    {
                        case EquationType.EquationChar:
                            

                            
                            break;
                        case EquationType.Exponenttial:
                            var offsetY = viewModel.SelectedFontSize * 0.3;
                            if (InputedCount == 1)
                            {
                                //跳出当前栈的输入模块
                                inputTypeStack.Pop();
                                exponentialButton.IsChecked = false;
                                SetCaretLocation(0, offsetY);
                                //todo:重设插字符位置，获取输入宽度，设置
                                CurrentInputBlock = null;

                            }
                            else
                            {
                                InputedCount++;
                               
                                caretJumpStack.Push(new Point(caretLeft, caretTop));
                                SetCaretLocation(0, -offsetY);
                            }
                            break;
                        case EquationType.Fraction:
                            break;
                        default:
                            break;
                    }
                }

            }
            else if (commandBtn.Name == "equationToggleButton")
            {
                var equationBtn = sender as ToggleButton;
                if (equationBtn.IsChecked == true)
                {
                    inputTypeStack.Push(EquationType.EquationChar);                   
                    viewModel.ChangeSelectedFontFamily("Times New Roman");                   
                    CurrentInputBlock = null;
                    InputedCount = 0;
                }
                else
                {                   
                    viewModel.ChangeSelectedFontFamily("宋体");
                }
            }
        }

        private void SetCaretLocation(double offsetX, double offsetY)
        {
            var oldCaretLeft = Canvas.GetLeft(caretTextBox);
            var oldCaretTop = Canvas.GetTop(caretTextBox);

            Canvas.SetLeft(caretTextBox, oldCaretLeft + offsetX);
            Canvas.SetTop(caretTextBox, oldCaretTop + offsetY);
        }

        private void editorCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            caretTextBox.Focus();

            Point mouseDownPoint = e.GetPosition(editorCanvas);
            bool isHalfHeadContain = false;
            var existElement = GetMostNearCaretElement(mouseDownPoint, ref isHalfHeadContain);

            if (null != existElement)
            {
                SetCaretPositionAndSize(mouseDownPoint, existElement, isHalfHeadContain);
            }
        }

        private void caretTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var viewModel = this.DataContext as MainWindowViewModel;
            ////接收输入的时候拆分中文字，分成单个的统一处理
            var currentLanguage = InputLanguageManager.Current.CurrentInputLanguage;

            double caretLeft = Canvas.GetLeft(caretTextBox);
            double caretTop = Canvas.GetTop(caretTextBox);

            var fontStyle = FontStyles.Normal;
            if (inputTypeStack.Count != 0 && IsLowercaseLetter(e.Text))
            {
                fontStyle = FontStyles.Italic;
            }

            MathFont font = GetFont(viewModel, fontStyle);
            double lineOffsetX = 0;

            if (inputTypeStack.Count != 0)
            {
                EquationType inputType = inputTypeStack.Peek();
                switch (inputType)
                {
                    case EquationType.EquationChar:
                        if (null==CurrentInputBlock)
                        {
                            CurrentInputBlock = new EquationBlock() { BlockID = Guid.NewGuid().ToString(), CurrentEquationType = EquationType.EquationChar };
                        }
                        break;
                    case EquationType.Exponenttial:
                        if (null==CurrentInputBlock)
                        {
                            CurrentInputBlock = new EquationBlock() { BlockID = Guid.NewGuid().ToString(), CurrentEquationType = EquationType.Exponenttial };
                        }                                           
                        break;
                    case EquationType.Fraction:
                        break;
                    default:
                        break;
                }
            }
            else
            {
                CurrentInputBlock = new MathTextBlock();
            }
            var obj = caretTextBox.Text;
            if (IsChinese(e.Text))
            {
                viewModel.ChangeSelectedFontFamily("宋体");
                font = GetFont(viewModel, FontStyles.Normal);

                lineOffsetX = AcceptChineseInputText(e.Text, caretLeft, caretTop, font);
            }
            else
            {
                lineOffsetX = AcceptEnglishInputText(e.Text, caretLeft, caretTop, font);
            }

            SetCaretLocation(lineOffsetX, 0);
            e.Handled = true;
            caretTextBox.Text = string.Empty;
        }

        private MathFont GetFont(MainWindowViewModel viewModel, FontStyle fontStyle)
        {
           var font= new MathFont() { FontFamily = viewModel.SelectedFontFamily.FontFamilyEntity, FontSize = viewModel.SelectedFontSize, Foreground = (Color)ColorConverter.ConvertFromString(viewModel.SelectedColor), FontStyle = fontStyle };
            return font;
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

        private void SetCaretPositionAndSize(Point mouseDownPoint, FrameworkElement element, bool isSetBefore)
        {
            var elementLeft = Canvas.GetLeft(element);
            var elementTop = Canvas.GetTop(element);

            if (isSetBefore)
            {
                Canvas.SetLeft(caretTextBox, elementLeft - 1);
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
        private FrameworkElement GetMostNearCaretElement(Point point, ref bool isHalfHeadContain)
        {
            FrameworkElement frameworkElement = null;
            var children = LogicalTreeHelper.GetChildren(editorCanvas);

            Point judgePoint = new Point();
            foreach (FrameworkElement element in children)
            {
                var elementLeft = Canvas.GetLeft(element);
                var elementTop = Canvas.GetTop(element);
                judgePoint = new Point(elementLeft + element.ActualWidth * 0.8, elementTop);
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

        private double AcceptEnglishInputText(string text, double caretOffsetX, double caretOffsetY, MathFont font)
        {
            MathTextBlock mathBlock = new MathTextBlock();

            TextBlock inputedTextBlock = new TextBlock();
            inputedTextBlock.Uid = Guid.NewGuid().ToString();
            inputedTextBlock.Text = text;
            inputedTextBlock.FontFamily = font.FontFamily;
            inputedTextBlock.FontSize = font.FontSize;
            inputedTextBlock.Foreground = new SolidColorBrush(font.Foreground);
            inputedTextBlock.FontStyle = font.FontStyle;

            mathBlock.Block = inputedTextBlock;

            editorCanvas.Children.Add(inputedTextBlock);
            Canvas.SetLeft(inputedTextBlock, caretOffsetX);
            Canvas.SetTop(inputedTextBlock, caretOffsetY);

            if (CurrentInputBlock is EquationBlock)
            {
                var equationBlock = CurrentInputBlock as EquationBlock;
                mathBlock.EquationId = CurrentInputBlock.BlockID;
                mathBlock.PartIndex = InputedCount;
                equationBlock.Blocks.Add(mathBlock);
                

            }
            else if (CurrentInputBlock is MathTextBlock)
            {
                CurrentInputBlock = mathBlock;
                CurrentRow.Blocks.Add(CurrentInputBlock);
            }

            

            FormattedText formatted = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(font.FontFamily.ToString()), font.FontSize, new SolidColorBrush(font.Foreground));

            return formatted.WidthIncludingTrailingWhitespace;
        }

        private double AcceptChineseInputText(string text, double caretOffsetX, double caretOffsetY, MathFont font)
        {
            double allWidth = 0;
            foreach (var item in text)
            {
                MathTextBlock mathBlock = new MathTextBlock();

                TextBlock inputedTextBlock = new TextBlock();
                inputedTextBlock.Uid = Guid.NewGuid().ToString();
                inputedTextBlock.Text = item.ToString();
                inputedTextBlock.FontFamily = font.FontFamily;
                inputedTextBlock.FontSize = font.FontSize;
                inputedTextBlock.Foreground = new SolidColorBrush(font.Foreground);
                inputedTextBlock.FontStyle = font.FontStyle;

                mathBlock.Block = inputedTextBlock;
                CurrentRow.Blocks.Add(mathBlock);

                editorCanvas.Children.Add(inputedTextBlock);
                Canvas.SetLeft(inputedTextBlock, caretOffsetX + allWidth);
                Canvas.SetTop(inputedTextBlock, caretOffsetY);

                FormattedText formatted = new FormattedText(item.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(font.FontFamily.ToString()), font.FontSize, new SolidColorBrush(font.Foreground));
                allWidth += formatted.WidthIncludingTrailingWhitespace;
            }

            return allWidth;
        }

        private bool IsLowercaseLetter(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            text = text.Trim();

            foreach (char c in text)
            {
                if (c < 0x0061 || c > 0x007a)
                {
                    return false;
                }
            }

            return true;
        }

        private bool isNumber(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            text = text.Trim();
            foreach (char item in text)
            {
                if (item < 0x0030 || item > 0x0040)
                {
                    return false;
                }
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
    }
}