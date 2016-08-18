using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MathEditorView
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<MyFontFamily> fonts;
        private ObservableCollection<int> fontSizeList;
        private MyFontFamily selectedFontFamily;
        private ObservableCollection<string> foregrounds;
        private int selectedFontSize;
        private string selectedColor;

        public MainWindowViewModel()
        {
            List<MyFontFamily> myFontList = new List<MyFontFamily>();

            myFontList = GetAllFonts();

            fonts = new ObservableCollection<MyFontFamily>(myFontList);
            fontSizeList = new ObservableCollection<int>() { 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 };
            SelectedFontSize = fontSizeList.ToList().Find(p => p == 16);
            foregrounds = new ObservableCollection<string>() { "Black", "Gray", "Green", "Blue", "Yellow","Orange" };
            selectedColor = foregrounds.First();            
            selectedFontFamily = myFontList.Find(p => p.FontName == "宋体");
        }

        public void ChangeSelectedFontFamily(string fontName)
        {
            SelectedFontFamily = fonts.ToList().Find(p => p.FontName == fontName);
        }

        public string SelectedColor
        {
            get { return selectedColor; }
            set { selectedColor = value; OnPropertyChanged("SelectedColor"); }
        }

        public ObservableCollection<string> Foregrounds
        {
            get { return foregrounds; }
            set { foregrounds = value; OnPropertyChanged("Foregrounds"); }
        }

        public int SelectedFontSize
        {
            get { return selectedFontSize; }
            set { selectedFontSize = value; OnPropertyChanged("SelectedFontSize"); }
        }

        public ObservableCollection<int> FontSizeList
        {
            get { return fontSizeList; }
            set { fontSizeList = value; OnPropertyChanged("FontSizeList"); }
        }

        public MyFontFamily SelectedFontFamily
        {
            get { return selectedFontFamily; }
            set { selectedFontFamily = value; OnPropertyChanged("SelectedFontFamily"); }
        }

        public ObservableCollection<MyFontFamily> AllFonts
        {
            get { return fonts; }
            set { fonts = value; OnPropertyChanged("AllFonts"); }
        }

        private List<MyFontFamily> GetAllFonts()
        {
            List<MyFontFamily> myFontList = new List<MyFontFamily>();
            var fontCollection = Fonts.GetFontFamilies(@"c:\Windows\Fonts");

            if (fontCollection != null)
            {
                foreach (var item in fontCollection)
                {
                    var name = string.Empty;
                    var languageItem = System.Windows.Markup.XmlLanguage.GetLanguage("zh-cn");
                    if (item.FamilyNames.Keys.Contains(languageItem))
                    {
                        name = item.FamilyNames[languageItem];
                    }
                    else
                    {
                        var enLanguageItem = System.Windows.Markup.XmlLanguage.GetLanguage("en-us");
                        name = item.FamilyNames[enLanguageItem];
                    }
                    var myFontItem = new MyFontFamily() { FontFamilyEntity = item, FontName = name };

                    myFontList.Add(myFontItem);
                }
            }

            return myFontList;
        }
    }
}
