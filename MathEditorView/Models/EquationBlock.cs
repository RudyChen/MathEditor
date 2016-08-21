using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathEditorView
{
   public class EquationBlock:BaseBlock
    {
        private List<BaseBlock> blocks=new List<BaseBlock>();

        private EquationType type;

        public EquationType CurrentEquationType
        {
            get { return type; }
            set { type = value; }
        }


        public List<BaseBlock> Blocks
        {
            get { return blocks; }
            set { blocks = value; }
        }

    }
}
