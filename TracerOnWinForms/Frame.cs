using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TracerOnWinForms
{
    public class Frame
    {
        public string name;
        public string text;
        public int threadId = -1;
        public int parentId = 0;
        public StackFrame frame = new StackFrame();
        public Frame[] childFrame = new Frame[1];
        public MyAttribute[] attr = new MyAttribute[1];



        public Frame()
        {
            
        }
    }

    public class MyAttribute
    {
        public string name;
        public string value;

        public MyAttribute(string newName, string newValue)
        {
            name = newName;
            value = newValue;
        }
    }
}
