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
    public partial class Form1 : Form
    {
        Frame frameList = new Frame();
        TreeView treeView2 = new TreeView();
        Form formNodeChange;
        Frame currentNode = new Frame();
        List<TextBox> listTB = new List<TextBox>();
        List<TabPageInfo> listTBI = new List<TabPageInfo>();
        List<string> listFileName = new List<string>();
        string fileName = "";
        int index = 0;

        public Form1()
        {
            InitializeComponent(); //Path.GetFileNameWithoutExtension(ofd.FileName)
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openNewFile();
        }

        private void openNewFile()
        {
            XmlDocument doc = new XmlDocument();
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = @"C:\Users\Александр\Documents\Visual Studio 2015\Projects\SPP\Lab2\TracerOnWinForms\TracerOnWinForms\bin\Debug";
            openFileDialog1.Filter = "XML files (*.xml)|*.xml";
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            doc.Load(openFileDialog1.FileName);
                            fileName = openFileDialog1.FileName;
                        }

                        if (!listFileName.Contains(fileName))
                        {
                            listFileName.Add(fileName);
                            foreach (XmlNode node in doc.SelectNodes(Constants.Root))
                            {
                                frameList = new Frame();
                                frameList.name = Constants.Root;
                                parse(node, ref frameList, -1);
                            }

                            TabPage tabPage = new TabPage(Path.GetFileName(openFileDialog1.FileName));
                            tabPage.Name = index.ToString();
                            index++;
                            TabPageInfo tbi = new TabPageInfo(tabControl1, tabPage, frameList, fileName);
                            listTBI.Add(tbi);
                            tbi = listTBI.Last();

                            TreeNode newNode;
                            for (int i = 0; i < frameList.childFrame.Length - 1; i++)
                            {
                                newNode = new TreeNode(frameList.childFrame[i].name);
                                newNode.Text += addAttribute(frameList.childFrame[i]);
                                buildTreeView(frameList.childFrame[i], ref newNode);
                                tbi.treeView.Nodes.Add(newNode); //treeView2
                                frameList.childFrame[i].text = newNode.Text;
                            }

                            tbi.treeView.NodeMouseDoubleClick += treeView1_NodeMouseDoubleClick;
                            tbi.btnRemovePage.Click += button1_Click_1;
                            tbi.treeView.ExpandAll(); //treeView2
                            tabPage = tabControl1.TabPages[tabControl1.TabPages.Count - 1];
                            tabControl1.SelectedTab = tabPage;
                        }
                        else
                        {
                            int ind = listFileName.IndexOf(fileName);
                            tabControl1.SelectedTab = tabControl1.TabPages[ind];
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void parse(XmlNode node, ref Frame frame, int threadId)
        {
            int i = 0;
            foreach (XmlNode node1 in node)
            {
                frame.childFrame[i] = new Frame();
                if (node1.Name == "thread")
                {
                    threadId++;
                    frame.childFrame[i].parentId = -1;
                }
                frame.childFrame[i].threadId = threadId;
                frame.childFrame[i].name = node1.Name;
                Array.Resize<Frame>(ref frame.childFrame, frame.childFrame.Length + 1);
                int j = 0;
                foreach (XmlAttribute attr in node1.Attributes)
                {
                    frame.childFrame[i].attr[j] = new MyAttribute(attr.Name, attr.Value);
                    j++;
                    Array.Resize<MyAttribute>(ref frame.childFrame[i].attr, j + 1);
                }
                parse(node1, ref frame.childFrame[i], threadId);
                i++;
            }
        }

        private void buildTreeView(Frame frame, ref TreeNode node)
        {
            TreeNode newNode;
            for(int i = 0; i < frame.childFrame.Length - 1; i++)
            {
                newNode = new TreeNode(frame.childFrame[i].name);
                newNode.Text += addAttribute(frame.childFrame[i]);
                buildTreeView(frame.childFrame[i], ref newNode);
                node.Nodes.Add(newNode);
                frame.childFrame[i].text = newNode.Text;
            }
        }

        private string addAttribute(Frame frame)
        {
            string text = "";
            text += " ( ";
            for (int j = 0; j < frame.attr.Length - 1; j++)
            {
                text += frame.attr[j].name + "=" + frame.attr[j].value + "; ";
            }
            text += ")";

            return text;
        }

        private Frame searchNode(string text, Frame node)
        {
            for(int i = 0; i < node.childFrame.Length - 1; i++)
            {
                if (text == node.childFrame[i].text)
                {
                    return node.childFrame[i];
                }
                else
                {
                    Frame node1 = searchNode(text, node.childFrame[i]);
                    if (node1 != null)
                        return node1;
                }
            }
            return null;
        }

        private void changeTime(Frame frame, int diff)
        {
            int temp = 0;
            for(int i = 0; i < frame.childFrame.Length - 1; i++)
            {
                if (!frame.childFrame[i].Equals(currentNode))
                {
                    changeTime(frame.childFrame[i], diff);
                    temp = Int32.Parse(frame.childFrame[i].attr[Constants.One].value);
                    frame.childFrame[i].attr[Constants.One].value = (temp + diff).ToString();
                }
                else
                {
                    break;
                }
            }
        }

        private void buttonCancelClick(object sender, EventArgs e)
        {
            listTB.Clear();
            formNodeChange.Close();
        }

        private void checkTextBoxValue(List<TextBox> listTB)
        {
            string tempStringForCheck = "";
            if (listTB.Count == 2)
            {
                foreach (TextBox tb in listTB)
                {
                    tempStringForCheck = Int32.Parse(tb.Text).ToString();
                }
            }
            else
            {
                tempStringForCheck = Int32.Parse(listTB[Constants.One].Text).ToString();
                tempStringForCheck = Int32.Parse(listTB[Constants.Three].Text).ToString();
            }
        }

        private void buttonSaveClick(object sender, EventArgs e)
        {
            int i = 0;
            int diff = 0;
            string tempText = currentNode.text;
            frameList = listTBI[tabControl1.SelectedIndex].frameList;
            treeView2 = listTBI[tabControl1.SelectedIndex].treeView;
            try
            {
                int oldTime = Int32.Parse(currentNode.attr[Constants.One].value);
                int newTime = Int32.Parse(listTB[Constants.One].Text);
                diff = newTime - oldTime;
                checkTextBoxValue(listTB);
                if(listTB.Count == 2)
                {
                    foreach (TextBox tb in listTB)
                    {
                        //currentNode.attr[i].value = Int32.Parse(tb.Text).ToString();
                        currentNode.attr[i].value = tb.Text;
                        i++;
                    }
                }
                else
                {
                    foreach (TextBox tb in listTB)
                    {
                        currentNode.attr[i].value = tb.Text;
                        i++;
                        /*if(i != 3)
                            currentNode.attr[i].value = tb.Text;
                        else
                            currentNode.attr[i].value = Int32.Parse(tb.Text).ToString();
                        i++;*/
                    }
                }
                
                if (!currentNode.Equals(frameList.childFrame[currentNode.threadId]))
                {
                    changeTime(frameList.childFrame[currentNode.threadId], diff);
                    int temp = Int32.Parse(frameList.childFrame[currentNode.threadId].attr[Constants.One].value);
                    frameList.childFrame[currentNode.threadId].attr[Constants.One].value = (temp + diff).ToString();
                }

                treeView2.Nodes.Clear();

                TreeNode newNode;
                for (i = 0; i < frameList.childFrame.Length - 1; i++)
                {
                    newNode = new TreeNode(frameList.childFrame[i].name);
                    newNode.Text += addAttribute(frameList.childFrame[i]);
                    buildTreeView(frameList.childFrame[i], ref newNode);
                    treeView2.Nodes.Add(newNode);
                    frameList.childFrame[i].text = newNode.Text;
                }
                treeView2.ExpandAll();

                if (!tempText.Equals(currentNode.text) && (tabControl1.SelectedTab.Text[tabControl1.SelectedTab.Text.Length - 1] != '*'))
                    tabControl1.SelectedTab.Text += "*";
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }

            listTB.Clear();
            formNodeChange.Close();
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            formNodeChange = new Form();
            formNodeChange.Show();
            currentNode = searchNode(e.Node.Text, frameList);
            
            int left = 100;
            int top = 50;
            formNodeChange.Width = Constants.FormChangeNodeWidth;
            formNodeChange.Height = Constants.FormChangeNodeHeight;
            formNodeChange.Top = Constants.FormChangeNodeTop;
            formNodeChange.Left = Constants.FormChangeNodeLeft;

            if(currentNode.attr.Length == 3)
            {
                for (int i = 0; i < currentNode.attr.Length - 1; i++)
                {
                    TextBox textBox = new TextBox();
                    Label label = new Label();
                    textBox.Left = left;
                    textBox.Top = (i + 1) * top;
                    label.Left = textBox.Left - 80;
                    label.Top = textBox.Top;
                    label.Text = currentNode.attr[i].name;
                    textBox.Name = currentNode.attr[i].name;
                    textBox.Text = currentNode.attr[i].value;
                    textBox.MaxLength = Constants.MaxLength1;
                    listTB.Add(textBox);
                    formNodeChange.Controls.Add(textBox);
                    formNodeChange.Controls.Add(label);
                }
            }
            else
            {
                for (int i = 0; i < currentNode.attr.Length - 1; i++)
                {
                    TextBox textBox = new TextBox();
                    Label label = new Label();
                    textBox.Left = left;
                    textBox.Top = (i + 1) * top;
                    label.Left = textBox.Left - 80;
                    label.Top = textBox.Top;
                    label.Text = currentNode.attr[i].name;
                    textBox.Name = currentNode.attr[i].name;
                    textBox.Text = currentNode.attr[i].value;
                    if (i == 1 || i == 3)
                        textBox.MaxLength = Constants.MaxLength1;
                    else
                        textBox.MaxLength = Constants.MaxLength2;
                    listTB.Add(textBox);
                    formNodeChange.Controls.Add(textBox);
                    formNodeChange.Controls.Add(label);
                }
            }
            
            Button buttonCancel = new Button();
            Button buttonSave = new Button();
            buttonSave.Top = Constants.BtnSaveTop;
            buttonSave.Left = Constants.BtnSaveLeft;
            buttonCancel.Top = Constants.BtnCancelTop;
            buttonCancel.Left = Constants.BtnCancelLeft;
            buttonSave.Text = "Save";
            buttonCancel.Text = "Cancel";
            buttonCancel.Click += buttonCancelClick;
            buttonSave.Click += buttonSaveClick;

            formNodeChange.Controls.Add(buttonSave);
            formNodeChange.Controls.Add(buttonCancel);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (index != 0)
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                saveFileDialog1.Filter = "XML files (*.xml)|*.xml";
                saveFileDialog1.RestoreDirectory = true;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    fileName = saveFileDialog1.FileName;
                    if (Path.GetExtension(fileName) != ".xml")
                    {
                        MessageBox.Show("This is not xml file!", "Save error!");
                        return;
                    }
                    BuildXml();
                    listTBI[tabControl1.SelectedIndex].fileName = saveFileDialog1.FileName;
                    listFileName[tabControl1.SelectedIndex] = saveFileDialog1.FileName;
                    tabControl1.SelectedTab.Text = Path.GetFileNameWithoutExtension(saveFileDialog1.FileName) + ".xml";
                }
            }
            else
            {
                MessageBox.Show("Open file!");
            }
        }

        private void BuildXml()
        {
            XDocument xmlDoc = new XDocument();
            XElement xmlRoot = new XElement("root");
            XElement xmlThread;
            XAttribute attr;
            for (int i = 0; i < frameList.childFrame.Length - 1; i++)
            {
                if (frameList.childFrame[i].parentId == -1)
                {
                    xmlThread = new XElement("thread");
                    for(int j = 0; j < frameList.childFrame[i].attr.Length - 1; j++)
                    {
                        attr = new XAttribute(frameList.childFrame[i].attr[j].name, frameList.childFrame[i].attr[j].value);
                        xmlThread.Add(attr);
                    }
                    addNodeXml(frameList.childFrame[i], ref xmlThread);
                    xmlRoot.Add(xmlThread);
                }
            }
            xmlDoc.Add(xmlRoot);
            xmlDoc.Save(fileName);
        }

        private void addNodeXml(Frame frame, ref XElement thread)
        {
            for (int i = 0; i < frame.childFrame.Length - 1; i++)
            {
                XElement xmlMethod = new XElement("method");
                XAttribute attr;
                for (int j = 0; j < frame.childFrame[i].attr.Length - 1; j++)
                {
                    attr = new XAttribute(frame.childFrame[i].attr[j].name, frame.childFrame[i].attr[j].value);
                    xmlMethod.Add(attr);
                }
                addNodeXml(frame.childFrame[i], ref xmlMethod);

                thread.Add(xmlMethod);
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            TabPage tabPage1 = new TabPage();
            
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Save file?", "Save", MessageBoxButtons.YesNoCancel);
            if(result == DialogResult.Yes)
            {
                BuildXml();
                closeTab();

            }
            else if(result == DialogResult.No)
            {
                closeTab();
            }
        }

        private void closeTab()
        {
            index--;
            int currentIndex = tabControl1.SelectedIndex;
            listTBI.RemoveAt(currentIndex);
            tabControl1.TabPages.RemoveAt(currentIndex);
            listFileName.RemoveAt(currentIndex);
            if (index != 0)
            {
                TabPage tabPage = new TabPage();
                tabPage = tabControl1.TabPages[tabControl1.TabPages.Count - 1];
                tabControl1.SelectedTab = tabPage;
            }
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (tabControl1.SelectedIndex != -1)
            {
                frameList = listTBI[tabControl1.SelectedIndex].frameList;
                treeView2 = listTBI[tabControl1.SelectedIndex].treeView;
            }
        }

        private void tabControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S && e.Control)
            {
                if (index != 0)
                {
                    fileName = listTBI[tabControl1.SelectedIndex].fileName;
                    tabControl1.SelectedTab.Text = Path.GetFileName(fileName);
                    BuildXml();
                    //MessageBox.Show("Save file!");
                }
                else
                {
                    MessageBox.Show("Open file!");
                }
            }
            else if(e.KeyCode == Keys.O && e.Control)
            {
                openNewFile();
            }
            else if(e.KeyCode == Keys.W && e.Control)
            {
                if(index != 0)
                {
                    var result = MessageBox.Show("Save file?", "Save", MessageBoxButtons.YesNoCancel);
                    if (result == DialogResult.Yes)
                    {
                        BuildXml();
                        closeTab();

                    }
                    else if (result == DialogResult.No)
                    {
                        closeTab();
                    }
                }
            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Ctrl+O - open new file;\nCtrl+W - close tab;\nCtrl+S - save file;");
        }
    }
}
