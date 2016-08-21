using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MathEditorView
{
   public class MathTextBlock:BaseBlock
    {
        private TextBlock block;

        private string equationId;

        private int partIndex;

        public int PartIndex
        {
            get { return partIndex; }
            set { partIndex = value; }
        }


        public string EquationId
        {
            get { return equationId; }
            set { equationId = value; }
        }

        public TextBlock Block
        {
            get { return block; }
            set { block = value; }
        }

    }
}
