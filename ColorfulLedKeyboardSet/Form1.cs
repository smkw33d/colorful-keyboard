using System;
using System.Diagnostics;
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

            if (!(_green == MaxColorValue && _red == 0 && _blue == MaxColorValue))
            {
                if (_green < MaxColorValue)
                {
                    _green = Math.Min(MaxColorValue, _green + step);
                }
                else if (_red > 0)
                {
                    _red = Math.Max(0, _red - step);
                }
                else if (_blue < MaxColorValue)
                {
                    _blue = Math.Min(MaxColorValue, _blue + step);
                }
            }
            else
            {
                if (_green > 0)
                {
                    _green = Math.Max(0, _green - step);
                }
                else if (_red < MaxColorValue)
                {
                    _red = Math.Min(MaxColorValue, _red + step);
                }
                else if (_blue > 0)
                {
                    _blue = Math.Max(0, _blue - step);
                }
            }

            return Color.FromArgb(_red, _green, _blue);
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

            MessageBox.Show("RGB循环发生错误:\r\n" + ex.Message, "发生错误");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!File.Exists(Application.StartupPath + "\\InsydeDCHU.dll"))
            {
                MessageBox.Show("发生错误:InsydeDCHU.dll缺失\r\n，请检查程序运行文件夹下是否有InsydeDCHU.dll", "发生错误");
                Environment.Exit(0);
            }

            MessageBox.Show("此程序为墨水制作\r\n利用逆向手段获取API编写而成\r\n有任何硬件问题开发者不承担任何责任！", "免责声明");
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

        private void information_B_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("你好，我是墨水\r\n这个程序是写给七彩虹笔记本（有可能神舟也可以）用户的\r\n" +
                 "由于我发现这款电脑虽能设置键盘灯光\r\n但无法RGB循环以及自定义RGB\r\n故诞生了此程序\r\n" +
                 "使用循环RGB模式可以拖动滑动条调整循环速度\r\n更多信息前往此项目Github仓库查看!\r\n" +
                 "是否自动打开此程序Github仓库？", "一些信息", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Process.Start("https://github.com/moshuiD/Colorful-Keyborad-Led-Color-Setting");
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            await StopRgbLoopAsync();
        }

        private void GetSource_L_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/moshuiD/Colorful-Keyborad-Led-Color-Setting");
        }

        private void speedBar_ValueChanged(object sender, EventArgs e)
        {
            _speedLevel = speedBar.Value;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopRgbLoop();
            base.OnFormClosing(e);
        }
    }
}
