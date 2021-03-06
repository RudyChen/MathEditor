﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MathEditorView
{
    public class RowBlock
    {
        private List<BaseBlock> blocks=new List<BaseBlock>();

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

        public List<BaseBlock> Blocks
        {
            get { return blocks; }
            set { blocks = value; }
        }
    }
}
