using MainView.Mock;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MainView
{
    public partial class Form1 : Form, IView
    {

        public Form1()
        {
            InitializeComponent();
            dgvWebsites.DataSource = this.websites;
            SetDGVStyle();

        }


        private bool IsCellValueChanged = false;
        private bool IsAddingNewItem = false;

        private BindingList<IWebsite> websites = new BindingList<IWebsite>();

       public void AddMany(IEnumerable<IWebsite> newWebsites)
        {
            foreach (IWebsite newWebsite in newWebsites)
            {
                websites.Add(newWebsite);
            }
        }

        public void Add(IWebsite newWebsite)
        {
            websites.Add(newWebsite);
        }

        public void Remove(int index)
        {
            websites.RemoveAt(index);
        }

        public event ContextMenuEventHandler ContextMenuEvent;
        private delegate void RefreshAll();

        public void SetDGVStyle()
        {
            dgvWebsites.Columns["Name"].Width = (int)(dgvWebsites.Width * 0.20);
            dgvWebsites.Columns["URL"].Width = (int)(dgvWebsites.Width * 0.35);
            dgvWebsites.Columns["CheckInterval"].Width = (int)(dgvWebsites.Width * 0.20);
            dgvWebsites.Columns["IsOnline"].Visible = false;
            dgvWebsites.Columns["ChecksCount"].Width = (int)(dgvWebsites.Width * 0.23);

        }

        private void tsmiAdd_Click(object sender, EventArgs e)
        {
            if (ContextMenuEvent == null || !ContextMenuEvent.Invoke(EventType.BeginAdding, new ContextMenuEventArgs()).IsSuccessfull)
            {
                return;
            }

            dgvWebsites[0, dgvWebsites.Rows.Count - 1].Selected = true;
            IsAddingNewItem = true;
            dgvWebsites.BeginEdit(true);
        }

        public void DataUpdate()
        {
            for (int ix = 0; ix < websites.Count; ix++)
            {
                if (websites[ix].IsOnline)
                {
                    dgvWebsites.Rows[ix].DefaultCellStyle.BackColor = Color.LightGreen;
                    dgvWebsites.Rows[ix].DefaultCellStyle.SelectionBackColor = Color.Green;
                }
                else
                {
                    dgvWebsites.Rows[ix].DefaultCellStyle.BackColor = Color.IndianRed;
                    dgvWebsites.Rows[ix].DefaultCellStyle.SelectionBackColor = Color.Red;
                }
            }

            if (!InvokeRequired)
            {
                dgvWebsites.Refresh();
            }
            else
            {
                Invoke(new RefreshAll(RefreshDgv), new object[] { });
            }
        }

        public void RefreshDgv()
        {
            dgvWebsites.Refresh();
        }


        private void dgvWebsites_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.RowIndex > -1)
                {
                    dgvWebsites.Rows[e.RowIndex].Selected = true;
                    cmsMain.Show(Cursor.Position);
                }
            }
        }

        private void tsmiDelete_Click(object sender, EventArgs e)
        {
            int index = dgvWebsites.CurrentCell.RowIndex;
            ValidationResult vr = ContextMenuEvent.Invoke(EventType.Remove, new ContextMenuEventArgs()
            {
                RowIndex = index
            });
            if (!vr.IsSuccessfull) ShowErrorMessage(vr.Message, "Error!");
        }

        private void ShowErrorMessage(string message, string header)
        {
            MessageBox.Show(message, header, MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private void dgvWebsites_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (!IsAddingNewItem && !IsCellValueChanged) return;

            int colIndex = e.ColumnIndex;
            int rowIndex = e.RowIndex;
            IWebsite website = websites[rowIndex];
            ValidationResult vr = ContextMenuEvent.Invoke(EventType.RowValidating, new ContextMenuEventArgs()
            {
                RowIndex = rowIndex,
                ColumnIndex = colIndex,
                Website = website
            });

            if (!vr.IsSuccessfull)
            {
                ShowErrorMessage(vr.Message, "Error!");
                e.Cancel = true;                
            }
            else
            {
                IsAddingNewItem = false;
                IsCellValueChanged = false;
            }
        }

        private void dgvWebsites_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            IsCellValueChanged = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            ContextMenuEvent.Invoke(EventType.Exit, new ContextMenuEventArgs());
        }
    }
}
