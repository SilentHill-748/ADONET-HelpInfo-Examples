
namespace ADONET.SQLite
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.updateBut = new System.Windows.Forms.Button();
            this.showTableBut = new System.Windows.Forms.Button();
            this.createTableBut = new System.Windows.Forms.Button();
            this.studentsData = new System.Windows.Forms.DataGridView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.loadBut = new System.Windows.Forms.Button();
            this.saveBut = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.studentsData)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(838, 520);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.updateBut);
            this.tabPage1.Controls.Add(this.showTableBut);
            this.tabPage1.Controls.Add(this.createTableBut);
            this.tabPage1.Controls.Add(this.studentsData);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(830, 492);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // updateBut
            // 
            this.updateBut.Location = new System.Drawing.Point(170, 96);
            this.updateBut.Name = "updateBut";
            this.updateBut.Size = new System.Drawing.Size(75, 23);
            this.updateBut.TabIndex = 3;
            this.updateBut.Text = "Update";
            this.updateBut.UseVisualStyleBackColor = true;
            this.updateBut.Click += new System.EventHandler(this.Update_Click);
            // 
            // showTableBut
            // 
            this.showTableBut.Location = new System.Drawing.Point(89, 96);
            this.showTableBut.Name = "showTableBut";
            this.showTableBut.Size = new System.Drawing.Size(75, 23);
            this.showTableBut.TabIndex = 2;
            this.showTableBut.Text = "Show";
            this.showTableBut.UseVisualStyleBackColor = true;
            this.showTableBut.Click += new System.EventHandler(this.ShowStudents_Click);
            // 
            // createTableBut
            // 
            this.createTableBut.Location = new System.Drawing.Point(8, 96);
            this.createTableBut.Name = "createTableBut";
            this.createTableBut.Size = new System.Drawing.Size(75, 23);
            this.createTableBut.TabIndex = 1;
            this.createTableBut.Text = "Create";
            this.createTableBut.UseVisualStyleBackColor = true;
            this.createTableBut.Click += new System.EventHandler(this.CreateStudents_Click);
            // 
            // studentsData
            // 
            this.studentsData.AllowUserToDeleteRows = false;
            this.studentsData.AllowUserToResizeRows = false;
            this.studentsData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.studentsData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.studentsData.Location = new System.Drawing.Point(0, 125);
            this.studentsData.Name = "studentsData";
            this.studentsData.RowTemplate.Height = 25;
            this.studentsData.Size = new System.Drawing.Size(830, 367);
            this.studentsData.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.loadBut);
            this.tabPage2.Controls.Add(this.saveBut);
            this.tabPage2.Controls.Add(this.pictureBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(830, 492);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // loadBut
            // 
            this.loadBut.Location = new System.Drawing.Point(330, 299);
            this.loadBut.Name = "loadBut";
            this.loadBut.Size = new System.Drawing.Size(107, 23);
            this.loadBut.TabIndex = 2;
            this.loadBut.Text = "Load from Db";
            this.loadBut.UseVisualStyleBackColor = true;
            this.loadBut.Click += new System.EventHandler(this.LoadPicture_Click);
            // 
            // saveBut
            // 
            this.saveBut.Location = new System.Drawing.Point(230, 299);
            this.saveBut.Name = "saveBut";
            this.saveBut.Size = new System.Drawing.Size(94, 23);
            this.saveBut.TabIndex = 1;
            this.saveBut.Text = "Save To Db";
            this.saveBut.UseVisualStyleBackColor = true;
            this.saveBut.Click += new System.EventHandler(this.SavePicture_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox1.Location = new System.Drawing.Point(219, 6);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(378, 287);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(838, 520);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "SQLite прога";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.studentsData)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button createTableBut;
        private System.Windows.Forms.DataGridView studentsData;
        private System.Windows.Forms.Button showTableBut;
        private System.Windows.Forms.Button updateBut;
        private System.Windows.Forms.Button loadBut;
        private System.Windows.Forms.Button saveBut;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

