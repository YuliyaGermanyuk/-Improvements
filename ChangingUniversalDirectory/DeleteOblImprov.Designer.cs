namespace DeleteOblImprov
{
    partial class DeleteOblImprov
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
            this.CheckType = new System.Windows.Forms.Button();
            this.TypeName = new System.Windows.Forms.TextBox();
            this.DeleteImprov = new System.Windows.Forms.Button();
            this.StatusStrip = new System.Windows.Forms.StatusStrip();
            this.StatusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.DocName = new System.Windows.Forms.TextBox();
            this.CheckDocument = new System.Windows.Forms.Button();
            this.BindImprov = new System.Windows.Forms.Button();
            this.Next = new System.Windows.Forms.Button();
            this.AddImprov = new System.Windows.Forms.Button();
            this.StatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // CheckType
            // 
            this.CheckType.Location = new System.Drawing.Point(291, 23);
            this.CheckType.Name = "CheckType";
            this.CheckType.Size = new System.Drawing.Size(182, 23);
            this.CheckType.TabIndex = 0;
            this.CheckType.Text = "выбрать тип прибора";
            this.CheckType.UseVisualStyleBackColor = true;
            this.CheckType.Click += new System.EventHandler(this.CheckType_Click);
            // 
            // TypeName
            // 
            this.TypeName.Location = new System.Drawing.Point(92, 25);
            this.TypeName.Name = "TypeName";
            this.TypeName.Size = new System.Drawing.Size(184, 20);
            this.TypeName.TabIndex = 1;
            // 
            // DeleteImprov
            // 
            this.DeleteImprov.Location = new System.Drawing.Point(12, 94);
            this.DeleteImprov.Name = "DeleteImprov";
            this.DeleteImprov.Size = new System.Drawing.Size(461, 23);
            this.DeleteImprov.TabIndex = 7;
            this.DeleteImprov.Text = "Начать удаление доработок";
            this.DeleteImprov.UseVisualStyleBackColor = true;
            this.DeleteImprov.Click += new System.EventHandler(this.DeleteImprov_Click);
            // 
            // StatusStrip
            // 
            this.StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusText});
            this.StatusStrip.Location = new System.Drawing.Point(0, 211);
            this.StatusStrip.Name = "StatusStrip";
            this.StatusStrip.Size = new System.Drawing.Size(485, 22);
            this.StatusStrip.TabIndex = 8;
            this.StatusStrip.Text = "statusStrip1";
            // 
            // StatusText
            // 
            this.StatusText.Name = "StatusText";
            this.StatusText.Size = new System.Drawing.Size(16, 17);
            this.StatusText.Text = "...";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Тип прибора:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Документ:";
            // 
            // DocName
            // 
            this.DocName.Location = new System.Drawing.Point(92, 54);
            this.DocName.Name = "DocName";
            this.DocName.Size = new System.Drawing.Size(184, 20);
            this.DocName.TabIndex = 11;
            // 
            // CheckDocument
            // 
            this.CheckDocument.Location = new System.Drawing.Point(291, 52);
            this.CheckDocument.Name = "CheckDocument";
            this.CheckDocument.Size = new System.Drawing.Size(182, 23);
            this.CheckDocument.TabIndex = 10;
            this.CheckDocument.Text = "выбрать документ-основание";
            this.CheckDocument.UseVisualStyleBackColor = true;
            this.CheckDocument.Click += new System.EventHandler(this.CheckDocument_Click);
            // 
            // BindImprov
            // 
            this.BindImprov.Location = new System.Drawing.Point(12, 149);
            this.BindImprov.Name = "BindImprov";
            this.BindImprov.Size = new System.Drawing.Size(461, 23);
            this.BindImprov.TabIndex = 13;
            this.BindImprov.Text = "Начать обновление доработок";
            this.BindImprov.UseVisualStyleBackColor = true;
            this.BindImprov.Click += new System.EventHandler(this.BindImprov_Click);
            // 
            // Next
            // 
            this.Next.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.Next.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Next.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.Next.Location = new System.Drawing.Point(435, 176);
            this.Next.Margin = new System.Windows.Forms.Padding(0);
            this.Next.Name = "Next";
            this.Next.Size = new System.Drawing.Size(38, 26);
            this.Next.TabIndex = 14;
            this.Next.Text = "->";
            this.Next.UseVisualStyleBackColor = false;
            this.Next.Click += new System.EventHandler(this.Next_Click);
            // 
            // AddImprov
            // 
            this.AddImprov.Location = new System.Drawing.Point(12, 121);
            this.AddImprov.Name = "AddImprov";
            this.AddImprov.Size = new System.Drawing.Size(461, 23);
            this.AddImprov.TabIndex = 15;
            this.AddImprov.Text = "Начать добавление доработок";
            this.AddImprov.UseVisualStyleBackColor = true;
            this.AddImprov.Click += new System.EventHandler(this.AddImprov_Click);
            // 
            // DeleteOblImprov
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(485, 233);
            this.Controls.Add(this.AddImprov);
            this.Controls.Add(this.Next);
            this.Controls.Add(this.BindImprov);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.DocName);
            this.Controls.Add(this.CheckDocument);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.StatusStrip);
            this.Controls.Add(this.DeleteImprov);
            this.Controls.Add(this.TypeName);
            this.Controls.Add(this.CheckType);
            this.Name = "DeleteOblImprov";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Редактирование необходимых доработок";
            this.StatusStrip.ResumeLayout(false);
            this.StatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button CheckType;
        private System.Windows.Forms.TextBox TypeName;
        private System.Windows.Forms.Button DeleteImprov;
        private System.Windows.Forms.StatusStrip StatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel StatusText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox DocName;
        private System.Windows.Forms.Button CheckDocument;
        private System.Windows.Forms.Button BindImprov;
        private System.Windows.Forms.Button Next;
        private System.Windows.Forms.Button AddImprov;
    }
}

