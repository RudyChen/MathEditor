using System.Windows;
using System.Windows.Media;

namespace MathEditorView
{
    /// <summary>
    /// 文本块可以放置到任意行，所以不存行信息，
    /// 只是保留相对于行的位置信息
    /// </summary>
    public class TextBlockData:ViewModelBase
    {
        private double fontSize;

        private string style;

        private string fontFamilyName;

        private string text;

        private double rowTop;

        private double rowLeft;

        private string colorString;

        public string ColorString
        {
            get { return colorString; }
            set { colorString = value; OnPropertyChanged("ColorString"); }
        }

        public double RowLeft
        {
            get { return rowLeft; }
            set { rowLeft = value; OnPropertyChanged("RowLeft"); }
        }

        public double RowTop
        {
            get { return rowTop; }
            set { rowTop = value; OnPropertyChanged("RowTop"); }
        }

        public string Text
        {
            get { return text; }
            set { text = value; OnPropertyChanged("Text"); }
        }

        public string FontFamilyName
        {
            get { return fontFamilyName; }
            set { fontFamilyName = value; OnPropertyChanged("FontFamilyName"); }
        }

        /// <summary>
        /// Normal,Italic
        /// </summary>
        public string Style
        {
            get { return style; }
            set { style = value; OnPropertyChanged("Style"); }
        }

        public double FontSize
        {
            get { return fontSize; }
            set { fontSize = value; OnPropertyChanged("FontSize"); }
        }
    }
}