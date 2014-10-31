﻿using System;
using System.Windows.Forms;
using FolderSelect;
using KSPModAdmin.Core.Model;
using KSPModAdmin.Core.Utils;

namespace KSPModAdmin.Core.Views
{
    /// <summary>
    /// Dialog to choose a source and destination folder.
    /// </summary>
    public partial class frmDestFolderSelection : frmBase
    {
        #region Members

        private ModNode m_SourceNode = null;

        #endregion

        #region Properties

        /// <summary>
        /// Sets the available destination pahts.
        /// </summary>
        public string[] DestFolders
        {
            set
            {
                if (value == null)
                    cbDestination.Items.Clear();
                else
                {
                    cbDestination.Items.Add(new DestInfo("Other folder ...", ""));
                    foreach (string path in value)
                    {
                        int index = path.LastIndexOf("\\");
                        if (index >= 0)
                        {
                            string name = path.Substring(index);
                            name = name[0] + name[1].ToString().ToUpper() + name.Substring(2);
                            cbDestination.Items.Add(new DestInfo(name, path));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the available source folders.
        /// </summary>
        public ModNode[] SrcFolders
        {
            set
            {
                if (value == null)
                    CB_Source.Items.Clear();
                else
                {
                    if (value.Length > 0)
                    {
                        m_SourceNode = value[0];
                        foreach (ModNode child in value)
                            AddSrcFolder(child);

                        cbListFoldersOnly.Checked = !m_SourceNode.IsFile;
                        SrcFolder = m_SourceNode.Text;
                    }
                }
            }
        }
        
        /// <summary>
        /// Gets the selected destination folder.
        /// </summary>
        public string DestFolder
        {
            get
            {
                string result = string.Empty;
                if (cbDestination.SelectedIndex >= 0)
                    result = ((DestInfo)cbDestination.SelectedItem).Fullpath;

                return result.ToLower();
            }
            set
            {
                foreach (DestInfo entry in cbDestination.Items)
                {
                    if (entry.Name.ToLower() == value.ToLower())
                    {
                        cbDestination.SelectedItem = entry;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the selected source folder.
        /// </summary>
        public string SrcFolder
        {
            get
            {
                ModNode result = null;
                if (CB_Source.SelectedIndex >= 0)
                    result = (ModNode)CB_Source.SelectedItem;

                if (result != null)
                    return result.Name;
                else
                    return string.Empty;
            }
            set
            {
                foreach (ModNode entry in CB_Source.Items)
                {
                    if (entry.Text == value)
                        CB_Source.SelectedItem = entry;
                }
            }
        }

        /// <summary>
        /// Gets a value that indicates wether the content of the source should be copied or the selected source itself.
        /// </summary>
        public bool CopyContent { get { return rbCopyContentToDestination.Checked; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the DestFolderSelection class.
        /// </summary>
        public frmDestFolderSelection()
        {
            InitializeComponent();
        }

        #endregion

        #region Eventhandling

        /// <summary>
        /// Handels the Click event of the btnOK.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cbDestination.SelectedIndex >= 0 && CB_Source.SelectedIndex >= 0)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                if (cbDestination.SelectedIndex == -1)
                    MessageBox.Show(Messages.MSG_SELECT_DEST);
                else if (CB_Source.SelectedIndex == -1)
                    MessageBox.Show(Messages.MSG_SELECT_SCOURCE);
            }
        }

        /// <summary>
        /// Handels the Click event of the btnCancel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Handels the SelectedIndexChanged event of the CB_Dest.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CB_Dest_SelectedIndexChanged(object sender, EventArgs e)
        {
            // "Other folder ..." selected?
            if (cbDestination.SelectedIndex == 0)
            {
                FolderSelectDialog dlg = new FolderSelectDialog();
                dlg.Title = Messages.MSG_DESTINATION_FOLDER_SELECTION_TITLE;
                dlg.InitialDirectory = KSPPathHelper.GetPath(KSPPaths.KSPRoot);
                if (dlg.ShowDialog(this.Handle))
                {
                    string dest = dlg.FileName;
                    string destName = dest.Substring(dest.LastIndexOf("\\"));
                    cbDestination.Items.Add(new DestInfo(destName, dlg.FileName));
                    cbDestination.SelectedIndex = cbDestination.Items.Count - 1;
                }
                else
                    cbDestination.SelectedIndex = -1;

                //FolderBrowserDialog dlg = new FolderBrowserDialog();
                //dlg.Description = "Select a destination folder.";
                //dlg.SelectedPath = KSPPathHelper.GetPath(KSPPaths.KSPRoot);
                //if (dlg.ShowDialog() == DialogResult.OK)
                //{
                //    string dest = dlg.SelectedPath;
                //    string destName = dest.Substring(dest.LastIndexOf("\\"));
                //    cbDestination.Items.Add(new DestInfo(destName, dlg.SelectedPath));
                //    cbDestination.SelectedIndex = cbDestination.Items.Count - 1;
                //}
                //else
                //    cbDestination.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Handels the CheckedChanged event of the CB_ListFoldersOnly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CB_ListFoldersOnly_CheckedChanged(object sender, EventArgs e)
        {
            CB_Source.Items.Clear();
            SrcFolders = new ModNode[] { m_SourceNode };
        }

        #endregion

        #region Private

        /// <summary>
        /// Adds a source folder to the CB_Source for the passed node and all its childs.
        /// </summary>
        /// <param name="node">The node to add as source folder.</param>
        /// <param name="depth">The depth of the recursive call.</param>
        /// <note>Recursive function!</note>
        private void AddSrcFolder(ModNode node, int depth = 0)
        {
            if (!cbListFoldersOnly.Checked || (cbListFoldersOnly.Checked && !node.IsFile))
                CB_Source.Items.Add(node);

            foreach (ModNode child in node.Nodes)
                if (!cbListFoldersOnly.Checked || (cbListFoldersOnly.Checked && !child.IsFile))
                    AddSrcFolder(child, depth + 1);
        }

        #endregion
    }

    /// <summary>
    /// Class for destination paths.
    /// </summary>
    public class DestInfo
    {
        /// <summary>
        /// Display name of the destination path.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The full destination path.
        /// </summary>
        public string Fullpath { get; set; }


        /// <summary>
        /// Creates a instance of the DestInfo class.
        /// </summary>
        /// <param name="name">The display name of the destination path.</param>
        /// <param name="fullpath">The full destination path.</param>
        public DestInfo(string name, string fullpath)
        {
            Name = name;
            Fullpath = fullpath;
        }


        /// <summary>
        /// Returns the display name of the destination path.
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return Name; }
    }
}
