using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace TracerOnWinForms
{
    public class TabPageInfo
    {
        public TreeView treeView = new TreeView();
        public Button btnRemovePage = new Button();
        public Frame frameList = new Frame();
        public string fileName = "";

        public TabPageInfo(TabControl tabControl, TabPage tabPage, Frame newFrameList, string newFileName)
        {
            tabPage.BackColor = Color.Gray;
            treeView.Parent = tabPage;
            treeView.Width = Constants.TreeViewWidth; //939
            treeView.Height = Constants.TreeViewHeight;
            btnRemovePage.Width = Constants.BtnRemoveWidth;
            btnRemovePage.Left = Constants.BtnRemoveLeft;
            btnRemovePage.Cursor = Cursors.Hand;
            btnRemovePage.Text = "X";
            btnRemovePage.Parent = treeView;
            tabControl.TabPages.Add(tabPage);
            frameList = newFrameList;
            fileName = newFileName;
        }

    }
}
