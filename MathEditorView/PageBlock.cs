using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathEditorView
{
   public class PageBlock
    {
        private List<RowBlock> rowBlocks=new List<RowBlock>();

        public List<RowBlock> RowBlocks
        {
            get { return rowBlocks; }
            set { rowBlocks = value; }
        }
    }
}
