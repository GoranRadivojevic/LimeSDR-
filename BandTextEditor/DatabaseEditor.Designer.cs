namespace DatabaseEditor
{
    partial class DatabaseEditorClass
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnFirst = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnLast = new System.Windows.Forms.Button();
            this.comboDatabaseTable = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.btnImportTable = new System.Windows.Forms.Button();
            this.btnExportTable = new System.Windows.Forms.Button();
            this.btnTableRemove = new System.Windows.Forms.Button();
            this.btnExportDatabase = new System.Windows.Forms.Button();
            this.btnOpenDatabase = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.dataGridView = new DatabaseEditor.DBDataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(125, 477);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(33, 23);
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Text = "+";
            this.toolTip.SetToolTip(this.btnAdd, "Add new row");
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(164, 477);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(33, 23);
            this.btnRemove.TabIndex = 2;
            this.btnRemove.Text = "-";
            this.toolTip.SetToolTip(this.btnRemove, "Remove selected row");
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(331, 477);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.toolTip.SetToolTip(this.btnSave, "Save database to file");
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(412, 477);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 4;
            this.btnExit.Text = "Exit";
            this.toolTip.SetToolTip(this.btnExit, "Exit without saving");
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(86, 477);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(33, 23);
            this.btnDown.TabIndex = 5;
            this.btnDown.Text = "<";
            this.toolTip.SetToolTip(this.btnDown, "Previous row");
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnFirst
            // 
            this.btnFirst.Location = new System.Drawing.Point(47, 477);
            this.btnFirst.Name = "btnFirst";
            this.btnFirst.Size = new System.Drawing.Size(33, 23);
            this.btnFirst.TabIndex = 6;
            this.btnFirst.Text = "<<";
            this.toolTip.SetToolTip(this.btnFirst, "Go to first row");
            this.btnFirst.UseVisualStyleBackColor = true;
            this.btnFirst.Click += new System.EventHandler(this.btnFirst_Click);
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(203, 477);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(33, 23);
            this.btnUp.TabIndex = 8;
            this.btnUp.Text = ">";
            this.toolTip.SetToolTip(this.btnUp, "Next row");
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnLast
            // 
            this.btnLast.Location = new System.Drawing.Point(242, 477);
            this.btnLast.Name = "btnLast";
            this.btnLast.Size = new System.Drawing.Size(33, 23);
            this.btnLast.TabIndex = 7;
            this.btnLast.Text = ">>";
            this.toolTip.SetToolTip(this.btnLast, "Go to last row");
            this.btnLast.UseVisualStyleBackColor = true;
            this.btnLast.Click += new System.EventHandler(this.btnLast_Click);
            // 
            // comboDatabaseTable
            // 
            this.comboDatabaseTable.FormattingEnabled = true;
            this.comboDatabaseTable.Location = new System.Drawing.Point(230, 12);
            this.comboDatabaseTable.Name = "comboDatabaseTable";
            this.comboDatabaseTable.Size = new System.Drawing.Size(144, 21);
            this.comboDatabaseTable.TabIndex = 9;
            this.toolTip.SetToolTip(this.comboDatabaseTable, "Select table");
            this.comboDatabaseTable.SelectedIndexChanged += new System.EventHandler(this.comboDatabaseTable_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(161, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Table name";
            // 
            // btnImportTable
            // 
            this.btnImportTable.Location = new System.Drawing.Point(132, 43);
            this.btnImportTable.Name = "btnImportTable";
            this.btnImportTable.Size = new System.Drawing.Size(75, 23);
            this.btnImportTable.TabIndex = 11;
            this.btnImportTable.Text = "Import table";
            this.toolTip.SetToolTip(this.btnImportTable, "Import table from database");
            this.btnImportTable.UseVisualStyleBackColor = true;
            this.btnImportTable.Click += new System.EventHandler(this.btnImportTable_Click);
            // 
            // btnExportTable
            // 
            this.btnExportTable.Location = new System.Drawing.Point(213, 43);
            this.btnExportTable.Name = "btnExportTable";
            this.btnExportTable.Size = new System.Drawing.Size(75, 23);
            this.btnExportTable.TabIndex = 12;
            this.btnExportTable.Text = "Export table";
            this.toolTip.SetToolTip(this.btnExportTable, "Export table from database");
            this.btnExportTable.UseVisualStyleBackColor = true;
            this.btnExportTable.Click += new System.EventHandler(this.btnExportTable_Click);
            // 
            // btnTableRemove
            // 
            this.btnTableRemove.Location = new System.Drawing.Point(40, 43);
            this.btnTableRemove.Name = "btnTableRemove";
            this.btnTableRemove.Size = new System.Drawing.Size(86, 23);
            this.btnTableRemove.TabIndex = 13;
            this.btnTableRemove.Text = "Remove table";
            this.toolTip.SetToolTip(this.btnTableRemove, "Remove table from database");
            this.btnTableRemove.UseVisualStyleBackColor = true;
            this.btnTableRemove.Click += new System.EventHandler(this.btnTableRemove_Click);
            // 
            // btnExportDatabase
            // 
            this.btnExportDatabase.Location = new System.Drawing.Point(396, 43);
            this.btnExportDatabase.Name = "btnExportDatabase";
            this.btnExportDatabase.Size = new System.Drawing.Size(98, 23);
            this.btnExportDatabase.TabIndex = 15;
            this.btnExportDatabase.Text = "Export database";
            this.toolTip.SetToolTip(this.btnExportDatabase, "Export database");
            this.btnExportDatabase.UseVisualStyleBackColor = true;
            this.btnExportDatabase.Click += new System.EventHandler(this.btnExportDatabase_Click);
            // 
            // btnOpenDatabase
            // 
            this.btnOpenDatabase.Location = new System.Drawing.Point(294, 43);
            this.btnOpenDatabase.Name = "btnOpenDatabase";
            this.btnOpenDatabase.Size = new System.Drawing.Size(96, 23);
            this.btnOpenDatabase.TabIndex = 14;
            this.btnOpenDatabase.Text = "Open database";
            this.toolTip.SetToolTip(this.btnOpenDatabase, "Open database");
            this.btnOpenDatabase.UseVisualStyleBackColor = true;
            this.btnOpenDatabase.Click += new System.EventHandler(this.btnOpenDatabase_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog1";
            this.openFileDialog.Filter = "XML file|*.xml| All files|*.*";
            this.openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog_FileOk);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "XML file|*.xml| All files|*.*";
            this.saveFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog_FileOk);
            // 
            // dataGridView
            // 
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Location = new System.Drawing.Point(12, 72);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.Size = new System.Drawing.Size(510, 386);
            this.dataGridView.TabIndex = 16;
            // 
            // DatabaseEditorClass
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 512);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.btnExportDatabase);
            this.Controls.Add(this.btnOpenDatabase);
            this.Controls.Add(this.btnTableRemove);
            this.Controls.Add(this.btnExportTable);
            this.Controls.Add(this.btnImportTable);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboDatabaseTable);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btnLast);
            this.Controls.Add(this.btnFirst);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(550, 550);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(550, 550);
            this.Name = "DatabaseEditorClass";
            this.Text = "LimeSDR# Database Editor v2.0 by yt7pwr";
            this.Load += new System.EventHandler(this.LoadForm);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnFirst;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnLast;
        private System.Windows.Forms.ComboBox comboDatabaseTable;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button btnImportTable;
        private System.Windows.Forms.Button btnExportTable;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Button btnTableRemove;
        private System.Windows.Forms.Button btnExportDatabase;
        private System.Windows.Forms.Button btnOpenDatabase;
        private DBDataGridView dataGridView;
    }

    public class DBDataGridView : System.Windows.Forms.DataGridView
    {
        public DBDataGridView() { DoubleBuffered = true; }    // for render speed
    }
}

