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
using System.Xml.XPath;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {            
            InitializeComboBox();
            InitializeComponent();
        }

        internal System.Windows.Forms.ComboBox comboBox1;

        OpenFileDialog openFileDialog1 = new OpenFileDialog();

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.DefaultExt = "*.xml";  // Initialize the OpenFileDialog to look for XML files.
            openFileDialog1.Filter = "XML Files |*.xml|All Files (*.*)|*.*"; // file types, that will be allowed to upload
            openFileDialog1.Title = "Select a XML File";
            openFileDialog1.Multiselect = false; // allow/deny user to upload more than one file at a time
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                String path = openFileDialog1.FileName; // get name of file
                // reads the file
                using (StreamReader reader = new StreamReader(new FileStream(path, FileMode.Open), new UTF8Encoding()))
                {
                    reader.Close();
                    XmlDocument doc = new XmlDocument();
                    doc.Load(path);

                    //Indentation of XML file
                    StringBuilder builder = new StringBuilder();
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Indent = true,
                        IndentChars = "  ",
                        NewLineChars = "\r\n",
                        NewLineHandling = NewLineHandling.Replace
                    };
                    using (XmlWriter writer = XmlWriter.Create(builder, settings))
                    {
                        doc.Save(writer);
                    }
                    InitializeComboBox();
                    richTextBox1.AppendText(builder.ToString());
                    
                    //Application.DoEvents();
                }
            }
        }
               
        // This method initializes the combo box, binding through a dictionary
        private void InitializeComboBox()
        {
            List<Exception> exception = new List<Exception>();
            comboBox1 = new System.Windows.Forms.ComboBox();

            try
            {
                Dictionary<string, string> xPathCollection = new Dictionary<string, string>();

                xPathCollection.Add("0", "/employees/employee[1]");  //Selects the first element that is the child of the head element.
                xPathCollection.Add("1", "/employees/employee[last()]");  //Selects the last element that is the child of the head element
                xPathCollection.Add("2", "/employees/employee[last()-1]");  //Selects the last but one element that is the child of the head element
                xPathCollection.Add("3", "/employees/employee[position()<3]");  //Selects the first two elements that are children of the head element
                xPathCollection.Add("4", "/employees/*");  //Selects all the child element nodes of the head element
                //xPathCollection.Add("5", "//title[@*]");  //Selects all title elements which have at least one attribute of any kind

                comboBox1.DataSource = new BindingSource(xPathCollection, null);
                comboBox1.DisplayMember = "Value";
                comboBox1.ValueMember = "Key";

                this.comboBox1.Location = new System.Drawing.Point(470, 0);
                this.comboBox1.IntegralHeight = false;
                this.comboBox1.MaxDropDownItems = 10;
                this.comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
                this.comboBox1.Name = "ComboBox1";
                this.comboBox1.Size = new System.Drawing.Size(440, 81);
                this.comboBox1.TabIndex = 1;
                this.Controls.Add(this.comboBox1);

                // Associate the event-handling method with the SelectedIndexChanged event.
                this.comboBox1.SelectedIndexChanged += new System.EventHandler(comboBox1_SelectedIndexChanged);
            }

            catch (NullReferenceException e)
            {
                exception.Add(e);
                MessageBox.Show(e.Message);
            }

            catch (ArgumentException e)
            {
                exception.Add(e);
                MessageBox.Show(e.Message);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<Exception> exceptions = new List<Exception>();
            ComboBox comboBox = (ComboBox)sender;

            try
            {

                this.Activate();
                string fileName = openFileDialog1.FileName;
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileName);
                System.IO.FileStream fileStream = fileInfo.OpenRead();
                XPathDocument doc = new XPathDocument(fileName);
                XPathNavigator nav = doc.CreateNavigator();

                string xPathSelected = (string)comboBox.SelectedValue;
                int xPathKeySelected = comboBox.SelectedIndex;

                //MessageBox.Show(xPathSelected);

                XPathExpression expr;
                XPathNodeIterator iterator;

                if (xPathSelected.Equals("0"))
                {
                    // Compile a standard XPath expression
                    expr = nav.Compile("/employees/employee[1]");
                    iterator = nav.Select(expr);
                }
                else if (xPathSelected.Equals("1"))
                {
                    expr = nav.Compile("/employees/employee[last()]");
                    iterator = nav.Select(expr);
                }

                else if (xPathSelected.Equals("2"))
                {
                    expr = nav.Compile("employees/employee[last()-1]");
                    iterator = nav.Select(expr);
                }

                else if (xPathSelected.Equals("3"))
                {
                    expr = nav.Compile("/employees/employee[position()<3]");
                    iterator = nav.Select(expr);
                }

                else
                {
                    expr = nav.Compile("/employees/*");
                    iterator = nav.Select(expr);
                }

                // Iterate on the node set
                richTextBox2.Clear();
                while (iterator.MoveNext())
                {
                    XPathNavigator nav2 = iterator.Current.Clone();
                    richTextBox2.AppendText(nav2.OuterXml);
                }
            }
            catch (InvalidCastException ex1)
            {
                exceptions.Add(ex1);
                MessageBox.Show(ex1.Message);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}