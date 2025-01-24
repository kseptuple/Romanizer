namespace Kana
{
    partial class FormMain
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
            webView2Result = new Microsoft.Web.WebView2.WinForms.WebView2();
            buttonSubmit = new Button();
            groupBoxRomajiType = new GroupBox();
            radioButtonNewHepburn = new RadioButton();
            radioButtonOldHepburn = new RadioButton();
            radioButtonKunrei = new RadioButton();
            radioButtonNihon = new RadioButton();
            checkBoxKana = new CheckBox();
            checkBoxNoShortForm = new CheckBox();
            buttonDefault = new Button();
            tableLayoutPanel1 = new TableLayoutPanel();
            panel1 = new Panel();
            textBoxInput = new TextBox();
            ((System.ComponentModel.ISupportInitialize)webView2Result).BeginInit();
            groupBoxRomajiType.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // webView2Result
            // 
            webView2Result.AllowExternalDrop = false;
            webView2Result.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            webView2Result.CreationProperties = null;
            webView2Result.DefaultBackgroundColor = Color.White;
            webView2Result.Location = new Point(3, 313);
            webView2Result.Name = "webView2Result";
            webView2Result.Size = new Size(651, 305);
            webView2Result.TabIndex = 3;
            webView2Result.ZoomFactor = 1D;
            // 
            // buttonSubmit
            // 
            buttonSubmit.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonSubmit.Enabled = false;
            buttonSubmit.Location = new Point(3, 272);
            buttonSubmit.Name = "buttonSubmit";
            buttonSubmit.Size = new Size(94, 29);
            buttonSubmit.TabIndex = 5;
            buttonSubmit.Text = "转换";
            buttonSubmit.UseVisualStyleBackColor = true;
            buttonSubmit.Click += buttonSubmit_Click;
            // 
            // groupBoxRomajiType
            // 
            groupBoxRomajiType.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            groupBoxRomajiType.Controls.Add(radioButtonNewHepburn);
            groupBoxRomajiType.Controls.Add(radioButtonOldHepburn);
            groupBoxRomajiType.Controls.Add(radioButtonKunrei);
            groupBoxRomajiType.Controls.Add(radioButtonNihon);
            groupBoxRomajiType.Location = new Point(675, 12);
            groupBoxRomajiType.Name = "groupBoxRomajiType";
            groupBoxRomajiType.Size = new Size(211, 156);
            groupBoxRomajiType.TabIndex = 6;
            groupBoxRomajiType.TabStop = false;
            groupBoxRomajiType.Text = "罗马字类型";
            // 
            // radioButtonNewHepburn
            // 
            radioButtonNewHepburn.AutoSize = true;
            radioButtonNewHepburn.Checked = true;
            radioButtonNewHepburn.Location = new Point(6, 116);
            radioButtonNewHepburn.Name = "radioButtonNewHepburn";
            radioButtonNewHepburn.Size = new Size(105, 24);
            radioButtonNewHepburn.TabIndex = 3;
            radioButtonNewHepburn.TabStop = true;
            radioButtonNewHepburn.Text = "修正平文式";
            radioButtonNewHepburn.UseVisualStyleBackColor = true;
            // 
            // radioButtonOldHepburn
            // 
            radioButtonOldHepburn.AutoSize = true;
            radioButtonOldHepburn.Location = new Point(6, 86);
            radioButtonOldHepburn.Name = "radioButtonOldHepburn";
            radioButtonOldHepburn.Size = new Size(90, 24);
            radioButtonOldHepburn.TabIndex = 2;
            radioButtonOldHepburn.Text = "旧平文式";
            radioButtonOldHepburn.UseVisualStyleBackColor = true;
            // 
            // radioButtonKunrei
            // 
            radioButtonKunrei.AutoSize = true;
            radioButtonKunrei.Location = new Point(6, 56);
            radioButtonKunrei.Name = "radioButtonKunrei";
            radioButtonKunrei.Size = new Size(75, 24);
            radioButtonKunrei.TabIndex = 1;
            radioButtonKunrei.Text = "训令式";
            radioButtonKunrei.UseVisualStyleBackColor = true;
            // 
            // radioButtonNihon
            // 
            radioButtonNihon.AutoSize = true;
            radioButtonNihon.Location = new Point(6, 26);
            radioButtonNihon.Name = "radioButtonNihon";
            radioButtonNihon.Size = new Size(75, 24);
            radioButtonNihon.TabIndex = 0;
            radioButtonNihon.Text = "日本式";
            radioButtonNihon.UseVisualStyleBackColor = true;
            // 
            // checkBoxKana
            // 
            checkBoxKana.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            checkBoxKana.AutoSize = true;
            checkBoxKana.Location = new Point(675, 204);
            checkBoxKana.Name = "checkBoxKana";
            checkBoxKana.Size = new Size(181, 24);
            checkBoxKana.TabIndex = 7;
            checkBoxKana.Text = "显示平假名而非罗马字";
            checkBoxKana.UseVisualStyleBackColor = true;
            checkBoxKana.CheckedChanged += checkBoxKana_CheckedChanged;
            // 
            // checkBoxNoShortForm
            // 
            checkBoxNoShortForm.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            checkBoxNoShortForm.AutoSize = true;
            checkBoxNoShortForm.Checked = true;
            checkBoxNoShortForm.CheckState = CheckState.Checked;
            checkBoxNoShortForm.Location = new Point(675, 174);
            checkBoxNoShortForm.Name = "checkBoxNoShortForm";
            checkBoxNoShortForm.Size = new Size(211, 24);
            checkBoxNoShortForm.TabIndex = 8;
            checkBoxNoShortForm.Text = "不要缩写ウ行和オ行的长音";
            checkBoxNoShortForm.UseVisualStyleBackColor = true;
            // 
            // buttonDefault
            // 
            buttonDefault.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonDefault.Location = new Point(792, 234);
            buttonDefault.Name = "buttonDefault";
            buttonDefault.Size = new Size(94, 29);
            buttonDefault.TabIndex = 9;
            buttonDefault.Text = "默认设置";
            buttonDefault.UseVisualStyleBackColor = true;
            buttonDefault.Click += buttonDefault_Click;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(panel1, 0, 0);
            tableLayoutPanel1.Controls.Add(webView2Result, 0, 1);
            tableLayoutPanel1.Location = new Point(12, 12);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(657, 621);
            tableLayoutPanel1.TabIndex = 10;
            // 
            // panel1
            // 
            panel1.Controls.Add(textBoxInput);
            panel1.Controls.Add(buttonSubmit);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(3, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(651, 304);
            panel1.TabIndex = 11;
            // 
            // textBoxInput
            // 
            textBoxInput.AcceptsReturn = true;
            textBoxInput.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBoxInput.Location = new Point(3, 3);
            textBoxInput.Multiline = true;
            textBoxInput.Name = "textBoxInput";
            textBoxInput.ScrollBars = ScrollBars.Vertical;
            textBoxInput.Size = new Size(645, 263);
            textBoxInput.TabIndex = 4;
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(898, 635);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(buttonDefault);
            Controls.Add(checkBoxNoShortForm);
            Controls.Add(checkBoxKana);
            Controls.Add(groupBoxRomajiType);
            Name = "FormMain";
            Text = "日语罗马字转换工具";
            Load += FormMain_Load;
            ((System.ComponentModel.ISupportInitialize)webView2Result).EndInit();
            groupBoxRomajiType.ResumeLayout(false);
            groupBoxRomajiType.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Microsoft.Web.WebView2.WinForms.WebView2 webView2Result;
        private Button buttonSubmit;
        private GroupBox groupBoxRomajiType;
        private RadioButton radioButtonNewHepburn;
        private RadioButton radioButtonOldHepburn;
        private RadioButton radioButtonKunrei;
        private RadioButton radioButtonNihon;
        private CheckBox checkBoxKana;
        private CheckBox checkBoxNoShortForm;
        private Button buttonDefault;
        private TableLayoutPanel tableLayoutPanel1;
        private Panel panel1;
        private TextBox textBoxInput;
    }
}
