﻿using PanGu;
using PanGu.Dict;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DictManage
{
    public partial class FormMain : Form
    {
        WordDictionary _WordDict = null;
        String m_DictFileName;
        string _Version = "";

        private int Count
        {
            get
            {
                if (_WordDict != null)
                {
                    return _WordDict.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        public FormMain()
        {
            InitializeComponent();
        }

        private void ShowCount()
        {
            labelCount.Text = Count.ToString();
        }


        private void openBinDictFile13ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialogDict.RestoreDirectory = true;
            openFileDialogDict.FileName = "Dict.dct";
            openFileDialogDict.Filter = "Dictionay file|*.dct";

            if (openFileDialogDict.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DateTime old = DateTime.Now;
                    _WordDict = new WordDictionary();
                    _WordDict.Load(openFileDialogDict.FileName, out _Version);

                    TimeSpan s = DateTime.Now - old;
                    statusStrip.Items[0].Text = s.TotalMilliseconds.ToString() + "ms";
                }
                catch (Exception e1)
                {
                    MessageBox.Show(String.Format("Can not open dictionary, errmsg:{0}", e1.Message));
                    return;
                }

                panelMain.Enabled = true;
                m_DictFileName = openFileDialogDict.FileName;
                this.Text = "V" + _Version + " " + openFileDialogDict.FileName;
                ShowCount();
            }

        }

        private void saveBinDictFile13ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_WordDict == null)
            {
                return;
            }

            saveFileDialogDict.RestoreDirectory = true;
            saveFileDialogDict.FileName = "Dict.dct";
            saveFileDialogDict.Filter = "Dictionay file|*.dct";

            if (saveFileDialogDict.ShowDialog() == DialogResult.OK)
            {
                FormInputDictVersion frmInputDictVersion = new FormInputDictVersion();
                frmInputDictVersion.Version = _Version;
                if (frmInputDictVersion.ShowDialog() == DialogResult.OK)
                {
                    _WordDict.Save(saveFileDialogDict.FileName,
                        frmInputDictVersion.Version);
                }
            }

        }


        private void buttonSearch_Click(object sender, EventArgs e)
        {
            if (textBoxSearch.Text.Trim() == "")
            {
                return;
            }

            List<SearchWordResult> result = _WordDict.Search(textBoxSearch.Text.Trim());

            result.Sort();

            listBoxList.Items.Clear();

            foreach (SearchWordResult word in result)
            {
                listBoxList.Items.Add(word);
            }

            label.Text = "符合条件的记录数:" + result.Count.ToString();
        }

        private void listBoxList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = listBoxList.SelectedIndex;

            if (index < 0)
            {
                return;
            }

            object obj = listBoxList.Items[index];
            SearchWordResult word = (SearchWordResult)obj;

            textBoxWord.Text = word.Word.Word;
            numericUpDownFrequency.Value = (decimal)word.Word.Frequency;
            posCtrl.Pos = (int)word.Word.Pos;
        }

        private void textBoxWord_TextChanged(object sender, EventArgs e)
        {
            String word = textBoxWord.Text.Trim();
            if (word == "")
            {
                buttonUpdate.Enabled = false;
                buttonInsert.Enabled = false;
                buttonDelete.Enabled = false;
                return;
            }

            WordAttribute selWord = _WordDict.GetWordAttr(word);
            if (selWord != null)
            {
                buttonUpdate.Enabled = true;
                buttonInsert.Enabled = false;
                buttonDelete.Enabled = true;
                numericUpDownFrequency.Value = (decimal)selWord.Frequency;
                posCtrl.Pos = (int)selWord.Pos;
            }
            else
            {
                buttonUpdate.Enabled = false;
                buttonInsert.Enabled = true;
                buttonDelete.Enabled = false;
                numericUpDownFrequency.Value = 0;
                posCtrl.Pos = 0;

            }
        }

        private void buttonInsert_Click(object sender, EventArgs e)
        {
            _WordDict.InsertWord(textBoxWord.Text.Trim(), (double)numericUpDownFrequency.Value, (POS)posCtrl.Pos);
            MessageBox.Show("增加成功", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ShowCount();
            textBoxWord_TextChanged(sender, e);
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            _WordDict.UpdateWord(textBoxWord.Text.Trim(), (double)numericUpDownFrequency.Value, (POS)posCtrl.Pos);
            MessageBox.Show("修改成功", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ShowCount();
            textBoxWord_TextChanged(sender, e);
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确认删除改单词?", "删除提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information)
                == DialogResult.Yes)
            {
                _WordDict.DeleteWord(textBoxWord.Text.Trim());
                MessageBox.Show("删除成功", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            ShowCount();
            textBoxWord_TextChanged(sender, e);
        }

        private void BatchInsert(String fileName, String encoder)
        {
            String content = PanGu.Framework.File.ReadFileToString(fileName, Encoding.GetEncoding(encoder));

            String[] words = PanGu.Framework.Regex.Split(content, @"\r\n");

            bool allUse = false;
            WordAttribute lstWord = null;

            foreach (String word in words)
            {
                if (word == null)
                {
                    continue;
                }

                if (word.Trim() == "")
                {
                    continue;
                }

                string[] strs = word.Split(new char[] { '|' });

                if (strs.Length == 3)
                {
                    try
                    {
                        POS pos = (POS)int.Parse(strs[1].Substring(2, strs[1].Length - 2),
                            System.Globalization.NumberStyles.HexNumber);
                        double frequency = double.Parse(strs[2]);
                        string w = strs[0].Trim();
                        _WordDict.InsertWord(w, frequency, pos);
                        continue;
                    }
                    catch
                    {
                    }
                }


                FormBatchInsert frmBatchInsert = new FormBatchInsert();

                if (!allUse || lstWord == null)
                {
                    frmBatchInsert.Word.Word = word.Trim();

                    if (frmBatchInsert.ShowDialog() == DialogResult.OK)
                    {
                        lstWord = frmBatchInsert.Word;
                        allUse = frmBatchInsert.AllUse;
                        _WordDict.InsertWord(lstWord.Word, lstWord.Frequency, lstWord.Pos);
                    }
                }
                else
                {
                    lstWord.Word = word.Trim();
                    _WordDict.InsertWord(lstWord.Word, lstWord.Frequency, lstWord.Pos);
                }
            }
        }


        private void buttonBatchInsert_Click(object sender, EventArgs e)
        {
            openFileDialogDict.RestoreDirectory = true;
            openFileDialogDict.FileName = "*.txt";
            openFileDialogDict.Filter = "Text files|*.txt";

            if (openFileDialogDict.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    FormEncoder frmEncoder = new FormEncoder();
                    if (frmEncoder.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }

                    BatchInsert(openFileDialogDict.FileName, frmEncoder.Encoding);
                    MessageBox.Show("批量增加成功,注意只有保存字典后,导入的单词才会生效!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ShowCount();
                }
                catch (Exception e1)
                {
                    MessageBox.Show(e1.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ListWordsByPos(int pos)
        {
            List<SearchWordResult> result = _WordDict.SearchByPos((POS)pos);

            result.Sort();

            listBoxList.Items.Clear();

            foreach (SearchWordResult word in result)
            {
                listBoxList.Items.Add(word);
            }

            label.Text = "符合条件的记录数:" + result.Count.ToString();

        }

        private void ListWordsByLength(int length)
        {
            List<SearchWordResult> result = _WordDict.SearchByLength(length);

            result.Sort();

            listBoxList.Items.Clear();

            foreach (SearchWordResult word in result)
            {
                listBoxList.Items.Add(word);
            }

            label.Text = "符合条件的记录数:" + result.Count.ToString();

        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormFind frmFind = new FormFind();

            frmFind.ShowDialog();

            switch (frmFind.Mode)
            {
                case FormFind.SearchMode.None:
                    listBoxList.Items.Clear();
                    break;

                case FormFind.SearchMode.ByPos:
                    ListWordsByPos(frmFind.POS);
                    break;

                case FormFind.SearchMode.ByLength:
                    ListWordsByLength(frmFind.Length);
                    break;
            }

        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialogText.ShowDialog() == DialogResult.OK)
            {
                StringBuilder str = new StringBuilder();

                foreach (object text in listBoxList.Items)
                {
                    str.AppendLine(text.ToString());
                }

                PanGu.Framework.File.WriteString(saveFileDialogText.FileName, str.ToString(), Encoding.UTF8);
                MessageBox.Show("Save OK!");
            }
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            buttonSearch_Click(sender, e);
        }

        private void OpenAsTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialogDict.RestoreDirectory = true;
            openFileDialogDict.FileName = "Dict.txt";
            openFileDialogDict.Filter = "Dictionay file|*.txt";

            if (openFileDialogDict.ShowDialog() == DialogResult.OK)
            {
                _Version = "";

                try
                {
                    DateTime old = DateTime.Now;
                    _WordDict = new WordDictionary();

                    _WordDict.Load(openFileDialogDict.FileName, true, out _Version);

                    TimeSpan s = DateTime.Now - old;
                    statusStrip.Items[0].Text = s.TotalMilliseconds.ToString() + "ms";
                }
                catch (Exception e1)
                {
                    MessageBox.Show(String.Format("Can not open dictionary, errmsg:{0}", e1.Message));
                    return;
                }

                panelMain.Enabled = true;
                m_DictFileName = openFileDialogDict.FileName;
                this.Text = "V" + _Version + " " + openFileDialogDict.FileName;
                ShowCount();
            }
        }

        private void SaveAsTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_WordDict == null)
            {
                return;
            }

            saveFileDialogDict.RestoreDirectory = true;
            saveFileDialogDict.FileName = "Dict.txt";
            saveFileDialogDict.Filter = "Dictionay file|*.txt";

            if (saveFileDialogDict.ShowDialog() == DialogResult.OK)
            {
                _WordDict.SaveToText(saveFileDialogDict.FileName);
            }
        }
    }
}
