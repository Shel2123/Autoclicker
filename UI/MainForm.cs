using System.ComponentModel;
using Autoclicker.Core.Abstractions;
using Autoclicker.Core.Models;

namespace Autoclicker.UI
{
    public partial class MainForm : Form
    {
        private readonly BindingList<StepRow> _rows = new();
        private readonly ISequenceBuilder _builder;
        private readonly ISequenceRunner _runner;
        private readonly ISequencePersistence _store;
        private readonly IHotkeyService _hotkeys;

        public MainForm(ISequenceBuilder builder, ISequenceRunner runner, ISequencePersistence store, IHotkeyServiceFactory hotkeyFactory)
        {
            _builder = builder; _runner = runner; _store = store;
            InitializeComponent();

            _runner.Logged += msg => BeginInvoke(new Action(() => Log(msg)));

            // Grid
            dataGridViewSteps.AutoGenerateColumns = false;
            dataGridViewSteps.AllowUserToAddRows = false;
            dataGridViewSteps.Columns.Clear();

            var keyCol = new DataGridViewTextBoxColumn
            {
                Name = "colKey",
                HeaderText = "Key (W, Space, D, Enter, F1...)",
                DataPropertyName = "Key",
                Width = 320
            };
            var delayCol = new DataGridViewTextBoxColumn
            {
                Name = "colDelay",
                HeaderText = "Delay (sec)",
                DataPropertyName = "DelaySeconds",
                Width = 140,
                DefaultCellStyle = { Format = "0.###" }
            };
            var holdCol = new DataGridViewTextBoxColumn
            {
                Name = "colHold",
                HeaderText = "Input length (sec)",
                DataPropertyName = "HoldSeconds",
                Width = 160,
                DefaultCellStyle = { Format = "0.###" }
            };

            dataGridViewSteps.Columns.AddRange(new DataGridViewColumn[] { keyCol, delayCol, holdCol });
            dataGridViewSteps.DataSource = _rows;

            // Buttons
            btnAdd.Click += (_, __) => _rows.Add(new StepRow { Key = "W", DelaySeconds = 5.0, HoldSeconds = 0.12 });
            btnRemove.Click += (_, __) => RemoveSelected();
            btnUp.Click += (_, __) => MoveSelected(-1);
            btnDown.Click += (_, __) => MoveSelected(1);
            btnStart.Click += async (_, __) => await StartAsync();
            btnStop.Click += (_, __) => _runner.Stop();
            btnSave.Click += (_, __) => SaveToFile();
            btnLoad.Click += (_, __) => LoadFromFile();

            // Hotkeys
            _hotkeys = hotkeyFactory.Create(this);
            try { _hotkeys.Register(Keys.F8, () => _ = StartAsync()); _hotkeys.Register(Keys.F9, () => _runner.Stop()); }
            catch (InvalidOperationException ex) { Log($"[WARN] Failed to register global hotkeys F8/F9: {ex.Message}"); Log("Tip: choose another hotkey in code or free the system binding"); }

            // Demo rows
            _rows.Add(new StepRow { Key = "W", DelaySeconds = 5.0, HoldSeconds = 0.12 });
            _rows.Add(new StepRow { Key = "Space", DelaySeconds = 5.0, HoldSeconds = 0.12 });
            _rows.Add(new StepRow { Key = "D", DelaySeconds = 5.0, HoldSeconds = 0.12 });

            Log("Ready. Edit steps and press Start (F8)");
            Log("Note: target window must have focus. If target app is elevated, run this app as Admin too");
        }

        private void RemoveSelected()
        {
            if (dataGridViewSteps.CurrentRow?.DataBoundItem is StepRow row)
                _rows.Remove(row);
        }

        private void MoveSelected(int delta)
        {
            if (dataGridViewSteps.CurrentRow == null) return;
            int idx = dataGridViewSteps.CurrentRow.Index;
            int newIdx = idx + delta; if (newIdx < 0 || newIdx >= _rows.Count) return;
            var item = _rows[idx]; _rows.RemoveAt(idx); _rows.Insert(newIdx, item);
            dataGridViewSteps.ClearSelection(); dataGridViewSteps.Rows[newIdx].Selected = true; dataGridViewSteps.CurrentCell = dataGridViewSteps.Rows[newIdx].Cells[0];
        }

        private async Task StartAsync()
        {
            if (_runner.IsRunning) return;
            if (!_builder.TryBuild(_rows, out var steps, out var error)) { MessageBox.Show(error, "Validation error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            SwitchRunningState(true);
            Log("While running, the grid and file buttons are disabled to avoid mid-run edits");
            try { await _runner.RunAsync(steps, chkLoop.Checked, chkDryRun.Checked, Keys.F9); }
            finally { SwitchRunningState(false); }
        }

        private void SaveToFile()
        {
            using var sfd = new SaveFileDialog { Filter = "Autoclicker JSON|*.json", FileName = "sequence.json" };
            if (sfd.ShowDialog(this) == DialogResult.OK) { _store.Save(sfd.FileName, chkLoop.Checked, _rows); Log($"Saved: {sfd.FileName}"); }
        }

        private void LoadFromFile()
        {
            using var ofd = new OpenFileDialog { Filter = "Autoclicker JSON|*.json" };
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    var (loop, rows) = _store.Load(ofd.FileName);
                    _rows.Clear(); foreach (var r in rows) _rows.Add(r);
                    chkLoop.Checked = loop; Log($"Loaded: {ofd.FileName}");
                }
                catch { MessageBox.Show("Invalid file format", "Load", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            }
        }

        private void SwitchRunningState(bool isRunning)
        { btnStart.Enabled = !isRunning; btnStop.Enabled = isRunning; btnAdd.Enabled = btnRemove.Enabled = btnUp.Enabled = btnDown.Enabled = btnSave.Enabled = btnLoad.Enabled = !isRunning; dataGridViewSteps.ReadOnly = isRunning; }

        private void Log(string message) => txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");

        protected override void OnFormClosed(FormClosedEventArgs e)
        { base.OnFormClosed(e); _hotkeys.Dispose(); }
    }
}