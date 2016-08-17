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
        private List<TextBlock> blocks=new List<TextBlock>();

        private double rowTop;

        public double RowTop
        {
            get { return rowTop; }
            set { rowTop = value; }
        }

        public List<TextBlock> Blocks
        {
            get { return blocks; }
            set { blocks = value; }
        }
    }
}
