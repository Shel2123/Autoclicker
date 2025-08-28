#nullable disable
using System.Drawing;
using System.Windows.Forms;

namespace Autoclicker.UI
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private DataGridView dataGridViewSteps;
        private FlowLayoutPanel panelTop;
        private Button btnAdd;
        private Button btnRemove;
        private Button btnUp;
        private Button btnDown;
        private Button btnSave;
        private Button btnLoad;
        private CheckBox chkLoop;
        private CheckBox chkDryRun;
        private Button btnStart;
        private Button btnStop;
        private TextBox txtLog;

        protected override void Dispose(bool disposing) { if (disposing && (components != null)) components.Dispose(); base.Dispose(disposing); }

        private void InitializeComponent()
        {
            dataGridViewSteps = new DataGridView();
            panelTop = new FlowLayoutPanel();
            btnAdd = new Button();
            btnRemove = new Button();
            btnUp = new Button();
            btnDown = new Button();
            chkLoop = new CheckBox();
            chkDryRun = new CheckBox();
            btnSave = new Button();
            btnLoad = new Button();
            btnStart = new Button();
            btnStop = new Button();
            txtLog = new TextBox();
            ((System.ComponentModel.ISupportInitialize)dataGridViewSteps).BeginInit();
            panelTop.SuspendLayout();
            SuspendLayout();
            // dataGridViewSteps
            dataGridViewSteps.AllowUserToAddRows = false;
            dataGridViewSteps.AllowUserToDeleteRows = false;
            dataGridViewSteps.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewSteps.Dock = DockStyle.Top;
            dataGridViewSteps.Location = new Point(0, 0);
            dataGridViewSteps.Name = "dataGridViewSteps";
            dataGridViewSteps.Size = new Size(953, 320);
            dataGridViewSteps.TabIndex = 0;
            // panelTop
            panelTop.Controls.Add(btnAdd);
            panelTop.Controls.Add(btnRemove);
            panelTop.Controls.Add(btnUp);
            panelTop.Controls.Add(btnDown);
            panelTop.Controls.Add(chkLoop);
            panelTop.Controls.Add(chkDryRun);
            panelTop.Controls.Add(btnSave);
            panelTop.Controls.Add(btnLoad);
            panelTop.Controls.Add(btnStart);
            panelTop.Controls.Add(btnStop);
            panelTop.Dock = DockStyle.Top;
            panelTop.Location = new Point(0, 320);
            panelTop.Name = "panelTop";
            panelTop.Padding = new Padding(6);
            panelTop.Size = new Size(953, 40);
            panelTop.TabIndex = 3;
            panelTop.WrapContents = false;
            // btnAdd
            btnAdd.Location = new Point(9, 9);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(75, 23);
            btnAdd.TabIndex = 0;
            btnAdd.Text = "Add";
            // btnRemove
            btnRemove.Location = new Point(90, 9);
            btnRemove.Name = "btnRemove";
            btnRemove.Size = new Size(75, 23);
            btnRemove.TabIndex = 1;
            btnRemove.Text = "Remove";
            // btnUp
            btnUp.Location = new Point(171, 9);
            btnUp.Name = "btnUp";
            btnUp.Size = new Size(75, 23);
            btnUp.TabIndex = 2;
            btnUp.Text = "Up";
            // btnDown
            btnDown.Location = new Point(252, 9);
            btnDown.Name = "btnDown";
            btnDown.Size = new Size(75, 23);
            btnDown.TabIndex = 3;
            btnDown.Text = "Down";
            // chkLoop
            chkLoop.Location = new Point(333, 9);
            chkLoop.Name = "chkLoop";
            chkLoop.Size = new Size(120, 24);
            chkLoop.TabIndex = 4;
            chkLoop.Text = "Loop sequence";
            // chkDryRun
            chkDryRun.Location = new Point(459, 9);
            chkDryRun.Name = "chkDryRun";
            chkDryRun.Size = new Size(160, 24);
            chkDryRun.TabIndex = 5;
            chkDryRun.Text = "Dry run (no key press)";
            // btnSave
            btnSave.Location = new Point(625, 9);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(75, 23);
            btnSave.TabIndex = 6;
            btnSave.Text = "Save";
            // btnLoad
            btnLoad.Location = new Point(706, 9);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(75, 23);
            btnLoad.TabIndex = 7;
            btnLoad.Text = "Load";
            // btnStart
            btnStart.Location = new Point(787, 9);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(75, 23);
            btnStart.TabIndex = 8;
            btnStart.Text = "Start (F8)";
            // btnStop
            btnStop.Enabled = false;
            btnStop.Location = new Point(868, 9);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(75, 23);
            btnStop.TabIndex = 9;
            btnStop.Text = "Stop (F9)";
            // txtLog
            txtLog.Dock = DockStyle.Fill;
            txtLog.Location = new Point(0, 360);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(953, 240);
            txtLog.TabIndex = 2;
            // MainForm
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(953, 600);
            Controls.Add(txtLog);
            Controls.Add(panelTop);
            Controls.Add(dataGridViewSteps);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Autoclicker – Keyboard";
            ((System.ComponentModel.ISupportInitialize)dataGridViewSteps).EndInit();
            panelTop.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
