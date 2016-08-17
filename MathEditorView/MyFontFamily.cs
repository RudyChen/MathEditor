using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MathEditorView
{
    public class MyFontFamily:ViewModelBase
    {
        private string fontName;

        private FontFamily fontFamily;
              
        public FontFamily FontFamilyEntity
        {
            get { return fontFamily; }
            set { fontFamily = value; OnPropertyChanged("FontFamilyEntity"); }
        }

        public string FontName
        {
            get { return fontName; }
            set { fontName = value; OnPropertyChanged("FontName"); }
        }
    }
}
