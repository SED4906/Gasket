using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinVi
{
    public partial class Gasket : Form
    {
        public Gasket()
        {
            InitializeComponent();
        }
        public Gasket(String openfile)
        {
            InitializeComponent();
            try
            {
                savedFileName = openfile;
                groupBuffer.Text = savedFileName;
                var sr = new StreamReader(openfile);
                bufferTextBox.Text = sr.ReadToEnd();
            }
            catch (SecurityException ex)
            {
                MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
            $"Details:\n\n{ex.StackTrace}");
            }
        }

        private String savedFileName = "";
        private String search_string = "";

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Are you sure you want to discard the current buffer?", "New file?", MessageBoxButtons.YesNo);
            if(res == DialogResult.Yes)
            {
                bufferTextBox.Clear();
            }
        }
        
        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveAsFileDialog.ShowDialog() == DialogResult.OK)
            {
                Stream filestream;
                if ((filestream = saveAsFileDialog.OpenFile()) != null)
                {
                    savedFileName = saveAsFileDialog.FileName;
                    groupBuffer.Text = savedFileName;
                    filestream.Write(Encoding.UTF8.GetBytes(bufferTextBox.Text));
                    filestream.Close();
                }
                else
                {
                    MessageBox.Show("Error saving file.", "Error");
                }
            }
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    savedFileName = openFileDialog.FileName;
                    groupBuffer.Text = savedFileName;
                    var sr = new StreamReader(openFileDialog.FileName);
                    bufferTextBox.Text = sr.ReadToEnd();
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                $"Details:\n\n{ex.StackTrace}");
                }
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Are you sure you want to exit?", "Exit?", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                File.WriteAllText(savedFileName, bufferTextBox.Text);
            }
            catch
            {
                MessageBox.Show("Error saving file.", "Error");
            }
        }

        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bufferTextBox.Undo();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bufferTextBox.Cut();
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bufferTextBox.Copy();
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bufferTextBox.Paste();
        }

        private void AllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bufferTextBox.SelectAll();
        }

        private void LineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int selstart = bufferTextBox.SelectionStart;
            while (selstart > 0)
                if(bufferTextBox.Text[--selstart] == '\n') break;
            if(selstart - 1 > 0)
                selstart++;
            int selend = selstart;
            while (selend < bufferTextBox.Text.Length-1)
                if (bufferTextBox.Text[++selend] == '\n') break;
            if (selend + 1 == bufferTextBox.Text.Length) selend++;
            bufferTextBox.Select(selstart, selend-selstart);
        }

        private void WordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int selstart = bufferTextBox.SelectionStart;
            while (selstart > 0)
                if (bufferTextBox.Text[--selstart] == ' ' || bufferTextBox.Text[selstart] == '\n') break;
            if (selstart - 1 > 0)
                selstart++;
            int selend = selstart;
            while (selend < bufferTextBox.Text.Length - 1)
                if (bufferTextBox.Text[++selend] == ' ' || bufferTextBox.Text[selend] == '\n') break;
            if (selend + 1 == bufferTextBox.Text.Length) selend++;
            bufferTextBox.Select(selstart, selend - selstart);
        }

        private void TextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            search_string = queryTextBox.Text;
            int findex = bufferTextBox.Text.IndexOf(queryTextBox.Text);
            if (findex > -1)
            {
                bufferTextBox.SelectionStart = findex;
                bufferTextBox.SelectionLength = queryTextBox.Text.Length;
            }
            else
            {
                MessageBox.Show("Not found.");
            }
        }

        private void CharacterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            search_string = queryTextBox.Text.Substring(0,1);
            int findex = bufferTextBox.Text.IndexOf(queryTextBox.Text[0]);
            if (findex > -1)
            {
                bufferTextBox.SelectionStart = findex;
                bufferTextBox.SelectionLength = 1;
            }
            else
            {
                MessageBox.Show("Not found.");
            }
        }

        private void WordToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            int fxindex = 0;
            while (fxindex < queryTextBox.Text.Length)
            {
                if (queryTextBox.Text[fxindex] == ' ') { break; }
                fxindex++;
            }
            if (fxindex == 0) return;
            search_string = queryTextBox.Text.Substring(0, fxindex);
            int findex = bufferTextBox.Text.IndexOf(queryTextBox.Text.Substring(0,fxindex));
            if (findex > -1)
            {
                bufferTextBox.SelectionStart = findex;
                bufferTextBox.SelectionLength = 1;
            }
            else
            {
                MessageBox.Show("Not found.");
            }
        }

        private void NextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bufferTextBox.SelectionStart + 1 >= bufferTextBox.Text.Length)
            {
                MessageBox.Show("End of file reached.");
                return;
            }
            int findex = bufferTextBox.Text.IndexOf(search_string, bufferTextBox.SelectionStart + 1);
            if (findex > -1)
            {
                bufferTextBox.SelectionStart = findex;
                bufferTextBox.SelectionLength = search_string.Length;
            }
            else
            {
                MessageBox.Show("Not found.");
            }
        }

        private void PreviousToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bufferTextBox.SelectionStart - 1 < 0)
            {
                MessageBox.Show("Start of file reached.");
                return;
            }
            int findex = bufferTextBox.Text.LastIndexOf(search_string, bufferTextBox.SelectionStart - 1);
            if (findex > -1)
            {
                bufferTextBox.SelectionStart = findex;
                bufferTextBox.SelectionLength = search_string.Length;
            }
            else
            {
                MessageBox.Show("Not found.");
            }
        }

        private void ReplaceTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (search_string == null || queryTextBox.Text == "") return;
            bufferTextBox.Text = bufferTextBox.Text.Replace(search_string, queryTextBox.Text);
        }

        private void WordToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (search_string == null || queryTextBox.Text == "") return;
            int findex = 0;
            while(findex < queryTextBox.Text.Length)
            {
                if (queryTextBox.Text[findex] == ' ') { break; }
                findex++;
            }
            if (findex == 0) return;
            bufferTextBox.Text = bufferTextBox.Text.Replace(search_string, queryTextBox.Text.Substring(0,findex));
        }

        private void CharacterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (search_string == null || queryTextBox.Text == "") return;
            bufferTextBox.Text = bufferTextBox.Text.Replace(search_string, queryTextBox.Text.Substring(0,1));
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Gasket, the uninteresting editor.\nWritten by SED in no time at all.\n--- 4906.org ---","About Gasket");
        }

        private void Gasket_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.ApplicationExitCall) return;
            DialogResult res = MessageBox.Show("Are you sure you want to exit?", "Exit?", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                Application.Exit();
            }
            else e.Cancel = true;
        }
    }
}
