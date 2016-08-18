using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MathEditorView
{
    public class RowBlock
    {
        private List<TextBlockData> blocks=new List<TextBlockData>();

        private double rowTop;

        private double height;

        public double Height
        {
            get { return height; }
            set { height = value; }
        }

        public double RowTop
        {
            get { return rowTop; }
            set { rowTop = value; }
        }

        public List<TextBlockData> Blocks
        {
            get { return blocks; }
            set { blocks = value; }
        }
    }
}
