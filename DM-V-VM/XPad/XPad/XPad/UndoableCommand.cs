using System;
using System.Collections.Generic;
using System.Text;

namespace XPad
{
    abstract class UndoableCommand 
    {
        public abstract void Undo();
    }
}
