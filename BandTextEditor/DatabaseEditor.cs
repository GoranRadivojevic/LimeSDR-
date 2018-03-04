using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;
using System.IO;

namespace DatabaseEditor
{
    public partial class DatabaseEditorClass : Form
    {
        #region variable

        bool open_table = false;
        bool save_table = false;
        string app_name = "";
        bool database_changed = false;

        #endregion

        #region constructor/load/closing

        public DatabaseEditorClass()
        {
            InitializeComponent();
            float dpi = this.CreateGraphics().DpiX;
            float ratio = dpi / 96.0f;
            string font_name = this.Font.Name;
            float size = 8.25f / ratio;
            System.Drawing.Font new_font = new System.Drawing.Font(font_name, size);
            this.Font = new_font;
            DB.Init();
            RestoreState();
            app_name = this.Text;
            string[] s = DB.FileName.Split('\\');
            this.Text += "  " + s[s.Length-1];

            try
            {
                string[] tables;
                DB.GetTableNames(out tables);
                comboDatabaseTable.Items.Clear();
                foreach (string str in tables)
                    comboDatabaseTable.Items.Add(str);
                comboDatabaseTable.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void LoadForm(object sender, EventArgs e)
        {
            try
            {
                dataGridView.RowHeadersWidth = 35;
                dataGridView.DataSource = DB.ds.Tables[0];
                dataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCellsExceptHeader);
                float dpi = this.CreateGraphics().DpiX;
                float ratio = dpi / 96.0f;
                string font_name = this.dataGridView.Font.Name;
                float size = 9.0f / ratio;
                System.Drawing.Font new_font = new System.Drawing.Font(font_name, size);
                dataGridView.DefaultCellStyle.Font = new Font(font_name, size, FontStyle.Regular);
                comboDatabaseTable_SelectedIndexChanged(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        #endregion

        #region Save/Restore

        public void SaveState()
        {
            try
            {
                ArrayList a = new ArrayList();

                a.Add("window_top/" + this.Top.ToString());		// save form positions
                a.Add("window_left/" + this.Left.ToString());
                a.Add("window_width/" + this.Width.ToString());
                a.Add("window_height/" + this.Height.ToString());

                DB.SaveVars("Options", ref a);		            // save the values to the DB

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in LOG SaveState function!\n" + ex.ToString());
            }
        }

        public void RestoreState()
        {
            try
            {
                ArrayList a = DB.GetVars("Options");
                a.Sort();

                // restore saved values to the controls
                foreach (string s in a)
                {
                    string[] vals = s.Split('/');
                    string name = vals[0];
                    string val = vals[1];

                    if (s.StartsWith("window_top"))
                    {
                        int top = Int32.Parse(vals[1]);
                        this.Top = top;
                    }
                    else if (s.StartsWith("window_left"))
                    {
                        int left = Int32.Parse(vals[1]);
                        this.Left = left;
                    }
                    else if (s.StartsWith("window_width"))
                    {
                        int width = Int32.Parse(vals[1]);
                        this.Width = width;
                    }
                    else if (s.StartsWith("window_height"))
                    {
                        int height = Int32.Parse(vals[1]);
                        this.Height = height;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        #endregion

        #region buttons events

        private void btnExit_Click(object sender, EventArgs e)
        {
            try
            {
                if (database_changed)
                {
                    DialogResult = MessageBox.Show("Do you wish to save it?", "Database is changed!", MessageBoxButtons.YesNo);

                    if (DialogResult == DialogResult.No)
                        this.Close();
                    else if (DialogResult == DialogResult.Yes)
                    {
                        string[] s = DB.FileName.Split('\\');
                        string str = s[s.Length - 1];

                        if (str == "band_database.xml")
                            SaveState();

                        DB.Update();
                        this.Close();
                    }
                }
                else
                {
                    string[] s = DB.FileName.Split('\\');
                    string str = s[s.Length - 1];

                    if (str == "band_database.xml")
                        SaveState();

                    DB.Update();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                DataColumn[] col = DB.ds.Tables[comboDatabaseTable.Text].PrimaryKey;
                int index = dataGridView.SelectedRows[0].Index + 1;
                DataRow dr = DB.ds.Tables[comboDatabaseTable.Text].NewRow();
                DataRow r = DB.ds.Tables[comboDatabaseTable.Text].NewRow();
                DataRow[] rows = DB.ds.Tables[comboDatabaseTable.Text].Select();

                for (int i = 0; i < DB.ds.Tables[comboDatabaseTable.Text].Columns.Count; i++)
                    r[i] = rows[index-1][i];

                if (col.Length > 0)
                {
                    Type n = col[0].DataType;

                    if (n == typeof(string))
                        r[col[0].ColumnName] = "";
                    else if (n == typeof(short))
                        r[col[0].ColumnName] = 0;
                    else if (n == typeof(int))
                        r[col[0].ColumnName] = 0;
                    else if (n == typeof(long))
                        r[col[0].ColumnName] = 0;
                    else if (n == typeof(double))
                        r[col[0].ColumnName] = 0.0;
                    else if (n == typeof(float))
                        r[col[0].ColumnName] = 0.0f;
                    else
                        r[col[0].ColumnName] = 0;
                }

                DB.ds.Tables[comboDatabaseTable.Text].Rows.InsertAt(r, index);
                DB.ds.Tables[comboDatabaseTable.Text].DefaultView.Sort = "";

                if (!database_changed)
                {
                    this.Text += "*";
                    database_changed = true;
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView.Rows.RemoveAt(dataGridView.SelectedRows[0].Index);

                if (!database_changed)
                {
                    this.Text += "*";
                    database_changed = true;
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView.CurrentCell = dataGridView[0, Math.Max(dataGridView.CurrentRow.Index - 1, 0)];
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView.CurrentCell = dataGridView[0, Math.Min(dataGridView.CurrentRow.Index + 1,
                    dataGridView.Rows.Count - 2)];
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView.CurrentCell = dataGridView[0, 0];
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView.CurrentCell = dataGridView[0, dataGridView.Rows.Count - 2];
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string[] s = DB.FileName.Split('\\');
                string str = s[s.Length - 1];

                if (str == "band_database.xml")
                    SaveState();

                foreach (string text in comboDatabaseTable.Items)
                {
                    DB.SortTable(text);
                }

                DB.Update();
                this.Text = this.Text.Replace('*', ' ');
                database_changed = false;
                string q = DB.ds.Tables[comboDatabaseTable.Text].Columns[0].ToString();
                DB.ds.Tables[comboDatabaseTable.Text].DefaultView.Sort = q;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        #endregion

        #region misc functions

        private void comboDatabaseTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                dataGridView.DataSource = DB.ds.Tables[comboDatabaseTable.Text];

                if (DB.SortTable(comboDatabaseTable.Text))
                {
                    if (!database_changed)
                    {
                        this.Text += "*";
                        database_changed = true;
                    }
                }

                DB.ds.Tables[comboDatabaseTable.Text].DefaultView.Sort = 
                    DB.ds.Tables[comboDatabaseTable.Text].Columns[0].ToString();

                switch (dataGridView.ColumnCount)
                {
                    case 4:
                        dataGridView.Columns[0].Width = 95;
                        dataGridView.Columns[1].Width = 95;
                        dataGridView.Columns[2].Width = 205;
                        dataGridView.Columns[3].Width = 40;
                        break;

                    case 3:
                        dataGridView.Columns[0].Width = 95;
                        dataGridView.Columns[1].Width = 95;
                        dataGridView.Columns[2].Width = 205;
                        break;

                    case 2:
                        dataGridView.Columns[0].Width = 150;
                        dataGridView.Columns[1].Width = 200;
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        #endregion

        #region Import/Export

        private void btnOpenDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                if (database_changed)
                {
                    DialogResult = MessageBox.Show("Do you wish to save it?", "Database is changed!", MessageBoxButtons.YesNo);

                    if (DialogResult == DialogResult.No)
                    {
                        database_changed = false;
                    }
                    else if (DialogResult == DialogResult.Yes)
                    {
                        string[] s = DB.FileName.Split('\\');
                        string str = s[s.Length - 1];

                        if (str == "band_database.xml")
                            SaveState();

                        DB.Update();
                    }
                }

                open_table = false;
                openFileDialog.InitialDirectory = Application.StartupPath;
                openFileDialog.ShowReadOnly = true;
                openFileDialog.Title = "Open database";
                openFileDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void btnExportDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                save_table = false;
                saveFileDialog.InitialDirectory = Application.StartupPath;
                saveFileDialog.Title = "Export database";
                saveFileDialog.FileName = "";
                saveFileDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void btnImportTable_Click(object sender, EventArgs e)
        {
            try
            {
                open_table = true;
                openFileDialog.InitialDirectory = Application.StartupPath;
                openFileDialog.ShowReadOnly = true;
                openFileDialog.Title = "Import database table";
                openFileDialog.ShowDialog();   
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void btnExportTable_Click(object sender, EventArgs e)
        {
            try
            {
                save_table = true;
                saveFileDialog.InitialDirectory = Application.StartupPath;
                saveFileDialog.Title = "Export database table";
                saveFileDialog.FileName = comboDatabaseTable.Text;
                saveFileDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void btnTableRemove_Click(object sender, EventArgs e)
        {
            try
            {
                string table = comboDatabaseTable.Text;
                if (DB.RemoveTable(table))
                {
                    comboDatabaseTable.Items.Remove(comboDatabaseTable.Text);

                    if (comboDatabaseTable.Items.Count > 0)
                        comboDatabaseTable.SelectedIndex = 0;

                    if (!database_changed)
                    {
                        this.Text += "*";
                        database_changed = true;
                    }
                }

                comboDatabaseTable_SelectedIndexChanged(this, EventArgs.Empty);  // refresh table view
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private void saveFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                if (save_table)
                {
                    string file = saveFileDialog.FileName;
                    string table = comboDatabaseTable.Text;
                    DB.ExportTable(file, table);
                }
                else
                {
                    string file = saveFileDialog.FileName;
                    DB.ExportDatabase(file);
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                MessageBox.Show("Failed to export Table! \n" + ex.ToString(), "Error!");
            }
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                if (open_table)
                {
                    string file = openFileDialog.FileName;
                    string table_name = "";

                    if (DB.CheckImportTable(file, out table_name))      // check if valid one table exist!
                    {
                        if (DB.ImportTable(file, table_name))
                        {
                            string[] tables;
                            DB.GetTableNames(out tables);
                            comboDatabaseTable.Items.Clear();

                            foreach (string str in tables)
                                comboDatabaseTable.Items.Add(str);
                        }
                    }

                    comboDatabaseTable_SelectedIndexChanged(this, EventArgs.Empty);  // refresh table view

                    if (!database_changed)
                    {
                        this.Text += "*";
                        database_changed = true;
                    }
                }
                else
                {
                    string file = openFileDialog.FileName;
                    DB.ImportDatabase(file);
                    string[] tables;
                    DB.GetTableNames(out tables);
                    comboDatabaseTable.Items.Clear();
                    foreach (string str in tables)
                        comboDatabaseTable.Items.Add(str);
                    comboDatabaseTable.SelectedIndex = 0;
                    comboDatabaseTable_SelectedIndexChanged(this, EventArgs.Empty);  // refresh table view
                    string[] s = DB.FileName.Split('\\');
                    this.Text = app_name + "  " + s[s.Length - 1];
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                MessageBox.Show("Failed to import Table! \n" + ex.ToString(), "Error!");
            }
        }

        #endregion
    }
}
