using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorfulLedKeyboardSet
{
    public partial class Form1 : Form
    {
        [DllImport("InsydeDCHU.dll")]
        public static extern int SetDCHU_Data(int command, byte[] buffer, int length);

        private const int FrameDelayMs = 40;
        private const int MaxColorValue = 255;

        private readonly int[] _keyboardZones = { 1, 2, 3 };
        private CancellationTokenSource _loopCts;
        private Task _loopTask;
        private volatile int _speedLevel;

        private int _red = MaxColorValue;
        private int _green;
        private int _blue;
        private int _rgbPhase;

        public Form1()
        {
            InitializeComponent();
            _speedLevel = speedBar.Value;
        }

        private void SetKeyboardZoneColor(int keyboardZone, Color color)
        {
            int num = 0;
            switch (keyboardZone)
            {
                case 1:
                    num = 240;
                    break;
                case 2:
                    num = 241;
                    break;
                case 3:
                    num = 242;
                    break;
                case 7:
                    num = 246;
                    break;
                case 8:
                    num = 243;
                    break;
            }

            uint num2 = (uint)((int)color.B << 16 | (int)color.R << 8 | (int)color.G);
            if (color.R == 0 && color.G == 255 && color.B == 127)
            {
                num2 = (uint)(4587520 | (int)color.R << 8 | (int)color.G);
            }

            byte[] bytes = BitConverter.GetBytes((long)((long)num << 24) + (long)((ulong)num2));
            SetDCHU_Data(103, bytes, 4);
        }

        private void ApplyColorToAllZones(Color color)
        {
            foreach (int keyboardZone in _keyboardZones)
            {
                SetKeyboardZoneColor(keyboardZone, color);
            }
        }

        private void UpdateColorPreview(Color color)
        {
            if (IsDisposed || !IsHandleCreated)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(new Action<Color>(UpdateColorPreview), color);
                return;
            }

            ColorTestLabel.ForeColor = color;
        }

        private async Task RunRgbLoopAsync(CancellationToken token)
        {
            while (true)
            {
                token.ThrowIfCancellationRequested();

                Color color = GetNextRgbColor();
                ApplyColorToAllZones(color);
                UpdateColorPreview(color);

                await Task.Delay(FrameDelayMs, token).ConfigureAwait(false);
            }
        }

        private Color GetNextRgbColor()
        {
            int step = Math.Max(1, _speedLevel);

            bool phaseComplete;
            switch (_rgbPhase)
            {
                case 0:
                    phaseComplete = MoveChannelToward(ref _green, MaxColorValue, step);
                    break;
                case 1:
                    phaseComplete = MoveChannelToward(ref _red, 0, step);
                    break;
                case 2:
                    phaseComplete = MoveChannelToward(ref _blue, MaxColorValue, step);
                    break;
                case 3:
                    phaseComplete = MoveChannelToward(ref _green, 0, step);
                    break;
                case 4:
                    phaseComplete = MoveChannelToward(ref _red, MaxColorValue, step);
                    break;
                default:
                    phaseComplete = MoveChannelToward(ref _blue, 0, step);
                    break;
            }

            if (phaseComplete)
            {
                _rgbPhase = (_rgbPhase + 1) % 6;
            }

            return Color.FromArgb(_red, _green, _blue);
        }

        private static bool MoveChannelToward(ref int channel, int target, int step)
        {
            if (channel < target)
            {
                channel = Math.Min(target, channel + step);
            }
            else if (channel > target)
            {
                channel = Math.Max(target, channel - step);
            }

            return channel == target;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StartRgbLoop();
        }

        private void StartRgbLoop()
        {
            if (_loopTask != null && !_loopTask.IsCompleted)
            {
                return;
            }

            _loopCts = new CancellationTokenSource();
            CancellationToken token = _loopCts.Token;
            _loopTask = Task.Run(() => RunRgbLoopAsync(token), token);

            SetLoopButtons(true);
            ObserveRgbLoopCompletionAsync(_loopTask, _loopCts);
        }

        private async void ObserveRgbLoopCompletionAsync(Task loopTask, CancellationTokenSource loopCts)
        {
            try
            {
                await loopTask;
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                ShowLoopError(ex);
            }
            finally
            {
                loopCts.Dispose();

                if (ReferenceEquals(loopTask, _loopTask))
                {
                    _loopTask = null;
                    _loopCts = null;
                    SetLoopButtons(false);
                }
            }
        }

        private void StopRgbLoop()
        {
            CancellationTokenSource loopCts = _loopCts;
            if (loopCts != null && !loopCts.IsCancellationRequested)
            {
                loopCts.Cancel();
            }

            SetLoopButtons(false);
        }

        private async Task StopRgbLoopAsync()
        {
            Task loopTask = _loopTask;
            StopRgbLoop();

            if (loopTask == null)
            {
                return;
            }

            try
            {
                await loopTask;
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception)
            {
            }
        }

        private void SetLoopButtons(bool isRunning)
        {
            if (IsDisposed || !IsHandleCreated)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(new Action<bool>(SetLoopButtons), isRunning);
                return;
            }

            button1.Enabled = !isRunning;
            button2.Enabled = isRunning;
        }

        private void ShowLoopError(Exception ex)
        {
            if (IsDisposed || !IsHandleCreated)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(new Action<Exception>(ShowLoopError), ex);
                return;
            }

            string message = ex is BadImageFormatException
                ? "RGB循环发生错误:\r\nInsydeDCHU.dll 与程序位数不匹配。请使用 x64 版本程序，并确认运行目录中的 InsydeDCHU.dll 也是 64 位版本。\r\n\r\n原始错误: " + ex.Message
                : "RGB循环发生错误:\r\n" + ex.Message;

            MessageBox.Show(message, "发生错误");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            notifyIcon1.Icon = Icon;
            if (notifyIcon1.Icon == null)
            {
                notifyIcon1.Icon = SystemIcons.Application;
            }
            notifyIcon1.Visible = true;

            if (!File.Exists(Application.StartupPath + "\\InsydeDCHU.dll"))
            {
                MessageBox.Show("发生错误:InsydeDCHU.dll缺失\r\n，请检查程序运行文件夹下是否有InsydeDCHU.dll", "发生错误");
                Environment.Exit(0);
            }

            button2.Enabled = false;
        }

        private async void CustomRGB_B_Click(object sender, EventArgs e)
        {
            await StopRgbLoopAsync();
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                ApplyColorToAllZones(colorDialog1.Color);
                UpdateColorPreview(colorDialog1.Color);
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            await StopRgbLoopAsync();
        }

        private void speedBar_ValueChanged(object sender, EventArgs e)
        {
            _speedLevel = speedBar.Value;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                HideToTray();
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            RestoreFromTray();
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RestoreFromTray();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void HideToTray()
        {
            Hide();
            ShowInTaskbar = false;
            notifyIcon1.Visible = true;
        }

        private void RestoreFromTray()
        {
            ShowInTaskbar = true;
            Show();
            WindowState = FormWindowState.Normal;
            Activate();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            notifyIcon1.Visible = false;
            StopRgbLoop();
            base.OnFormClosing(e);
        }
    }
}
