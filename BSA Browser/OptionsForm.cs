﻿using System;
using System.IO;
using System.Windows.Forms;
using BSA_Browser.Classes;
using BSA_Browser.Dialogs;
using BSA_Browser.Properties;

namespace BSA_Browser
{
    public partial class OptionsForm : Form
    {
        public OptionsForm()
        {
            InitializeComponent();

            foreach (QuickExtractPath path in Settings.Default.QuickExtractPaths)
            {
                ListViewItem item = new ListViewItem(path.Name);
                item.SubItems.Add(path.Path);
                item.SubItems.Add(path.UseFolderPath ? "Yes" : "No");
                item.Tag = path;

                lvQuickExtract.Items.Add(item);
            }

            Program.SendMessage(lvQuickExtract.Handle, 0x127, 0x10001, 0);
            Program.SendMessage(lvQuickExtract.Handle, 0x1000 + 54, 0x00010000, 0x00010000);
            Program.SetWindowTheme(lvQuickExtract.Handle, "explorer", null);
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            if (!Settings.Default.WindowStates.Contains(this.Name))
            {
                Settings.Default.WindowStates.Add(this.Name);
            }

            // Restore only columns.
            Settings.Default.WindowStates[this.Name].RestoreForm(this);
        }

        private void OptionsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.WindowStates[this.Name].SaveForm(this);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void lvQuickExtract_Enter(object sender, EventArgs e)
        {
            Program.SendMessage(lvQuickExtract.Handle, 0x127, 0x10001, 0);
        }

        private void lvQuickExtract_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.SendMessage(lvQuickExtract.Handle, 0x127, 0x10001, 0);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (QuickExtractDialog cpd = new QuickExtractDialog())
            {
                if (cpd.ShowDialog(this) == DialogResult.Cancel)
                    return;

                QuickExtractPath path = new QuickExtractPath(cpd.PathName, cpd.Path, cpd.UseFolderPath);
                ListViewItem newItem = new ListViewItem(cpd.PathName);

                newItem.SubItems.Add(cpd.Path);
                newItem.SubItems.Add(cpd.UseFolderPath ? "Yes" : "No");
                newItem.Tag = path;

                lvQuickExtract.Items.Insert(lvQuickExtract.Items.Count, newItem);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (lvQuickExtract.SelectedItems.Count == 0)
                return;

            using (QuickExtractDialog cpd = new QuickExtractDialog())
            {
                ListViewItem item = (ListViewItem)lvQuickExtract.SelectedItems[0];
                QuickExtractPath path = (QuickExtractPath)item.Tag;

                if (cpd.ShowDialog(this, path) == DialogResult.Cancel)
                    return;

                path.Name = cpd.PathName;
                path.Path = cpd.Path;
                path.UseFolderPath = cpd.UseFolderPath;

                item.Text = cpd.PathName;
                item.SubItems[1].Text = cpd.Path;
                item.SubItems[1].Text = cpd.UseFolderPath ? "Yes" : "No";
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lvQuickExtract.SelectedItems.Count == 0)
                return;

            int index = lvQuickExtract.SelectedItems[0].Index;

            lvQuickExtract.Items.RemoveAt(index);
        }

        public void SaveChanges()
        {
            Settings.Default.QuickExtractPaths.Clear();

            foreach (ListViewItem item in lvQuickExtract.Items)
            {
                Settings.Default.QuickExtractPaths.Add((QuickExtractPath)item.Tag);
            }
        }
    }
}
