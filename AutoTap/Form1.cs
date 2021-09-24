using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace AutoTap
{
    public partial class Form1 : Form
    {
        #region " Fields "
        private WindowsHook.MouseHook MouseHook1;
        private WindowsHook.KeyboardHook KeyHook1;
        private Boolean doLoop = false;
        private Form2 DisplayForm = new Form2();
        private Thread tPlay;
        Image img;
        IntPtr hWnd;
        WindowsHook.UnsafeNativeMethods.RECT rect = new WindowsHook.UnsafeNativeMethods.RECT();
        #endregion

        #region " Constructor "
        public Form1()
        {
            InitializeComponent();

            cmbWindows.DataSource = new BindingSource(WindowList(), null);
            cmbWindows.DisplayMember = "Value";
            cmbWindows.ValueMember = "Key";
            MouseHook1 = new WindowsHook.MouseHook();
            KeyHook1 = new WindowsHook.KeyboardHook();

            MouseHook1.InstallHook();
            KeyHook1.InstallHook();
            KeyHook1.KeyDown += KeyHook1_KeyDown;

            MouseHook1.MouseDown += MouseHook1_MouseClickCancel;

            img = Image.FromFile(Application.StartupPath + "\\Cursor1.png");
        }

        Dictionary<Int32, string> WindowList()
        {
            Dictionary<Int32, string> results = new Dictionary<Int32, string>();

            Process[] processlist = Process.GetProcesses();
            results.Add(0, "<please select>");
            foreach (Process process in processlist)
            {
                if (!String.IsNullOrEmpty(process.MainWindowTitle))
                {
                    results.Add(process.Id, $"{process.MainWindowTitle}");
                }
            }
            return results;
        }
        #endregion

        #region " Event Handlers "

        #region " Add New Point "
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SetCursor();
                MouseHook1.MouseClick += MouseHook1_MouseClick;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                WindowsHook.ErrorLog.ExceptionToFile(ex, TraceEventType.Error);
            }
        }

        private void MouseHook1_MouseClick(object sender, MouseEventArgs e)
        {
            MouseHook1.MouseClick -= MouseHook1_MouseClick;
            lstPoints.Items.Add($"{lstPoints.Items.Count+1}: {e.X - rect.Left} , {e.Y - rect.Top} 1000");
            ResetCursor();
        }
        #endregion

        #region " Maintains DisplayForm "
        private void cmbWindows_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbWindows.SelectedIndex > 0)
            {
                Process p = Process.GetProcessById((int)cmbWindows.SelectedValue);
                hWnd = p.MainWindowHandle;
                WindowsHook.UnsafeNativeMethods.GetWindowRect(hWnd, ref rect);
                lblSelectedWindow.Text = $"{rect.Left}, {rect.Top}, {rect.Right - rect.Left}, {rect.Bottom - rect.Top}";

                udX.Maximum = rect.Right - rect.Left;
                udY.Maximum = rect.Bottom - rect.Top;

                DisplayForm.Top = rect.Top;
                DisplayForm.Left = rect.Left;
                DisplayForm.Size = new Size(rect.Right - rect.Left, rect.Bottom - rect.Top);

                if (!DisplayForm.Visible)
                {
                    timer1.Enabled = true;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            IntPtr curhandle = WindowsHook.UnsafeNativeMethods.GetForegroundWindow();
            WindowsHook.UnsafeNativeMethods.GetWindowRect(hWnd, ref rect);
            DisplayForm.Top = rect.Top;
            DisplayForm.Left = rect.Left;
            DisplayForm.Size = new Size(rect.Right - rect.Left, rect.Bottom - rect.Top);

            string disp = $"{rect.Left}, {rect.Top}, {rect.Right - rect.Left}, {rect.Bottom - rect.Top}";
            if (disp != lblSelectedWindow.Text) lblSelectedWindow.Text = disp;
            udX.Maximum = rect.Right - rect.Left;
            udY.Maximum = rect.Bottom - rect.Top;

            if (curhandle != hWnd && curhandle != this.Handle && curhandle != DisplayForm.Handle && !doLoop)
            {
                if (DisplayForm.Visible) DisplayForm.Hide();
            }
            else
            {
                if (!DisplayForm.Visible && this.WindowState!=FormWindowState.Minimized) DisplayForm.Show(this);
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (DisplayForm != null)
            {
                if (this.WindowState == FormWindowState.Minimized)
                    DisplayForm.Visible = false;
                else
                    DisplayForm.Visible = true;
            }
        }
        #endregion

        #region " Play Points "
        private void btnPlay_Click(object sender, EventArgs e)
        {
            doLoop = true;

            this.Text = "Auto Tap ** RUNNING **";

            tPlay = new Thread(PlayPoints);
            tPlay.Start();
        }

        private void MouseHook1_MouseClickCancel(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                doLoop = false;
                this.Text = "Auto Tap";
            }
        }

        private void KeyHook1_KeyDown(object sender, WindowsHook.KeyboardEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.PageDown)
            {
                doLoop = false;
                this.Text = "Auto Tap";
            }
        }
        #endregion

        #region " Edit Point "
        private void lstPoints_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstPoints.SelectedIndex >= 0)
            {
                string[] pnt = lstPoints.SelectedItem.ToString().Split(' ');
                udTime.Value = decimal.Parse(pnt[4]);
                udX.Value = decimal.Parse(pnt[1]);
                udY.Value = decimal.Parse(pnt[3]);
            }
        }

        private void udTime_ValueChanged(object sender, EventArgs e)
        {
            if (lstPoints.SelectedItem != null)
            {
                string[] pnt = lstPoints.SelectedItem.ToString().Split(' ');
                lstPoints.Items[lstPoints.SelectedIndex] = $"{lstPoints.SelectedIndex+1}: {pnt[1]} , {pnt[3]} {udTime.Value}";
            }
        }

        private void udTime_KeyUp(object sender, KeyEventArgs e)
        {
            udTime_ValueChanged(null, null);
        }

        private void udX_ValueChanged(object sender, EventArgs e)
        {
            if (lstPoints.SelectedItem != null)
            {
                string[] pnt = lstPoints.SelectedItem.ToString().Split(' ');
                lstPoints.Items[lstPoints.SelectedIndex] = $"{lstPoints.SelectedIndex+1}: {udX.Value} , {pnt[3]} {pnt[4]}";
            }
        }

        private void udX_KeyUp(object sender, KeyEventArgs e)
        {
            udX_ValueChanged(null, null);
        }

        private void udY_ValueChanged(object sender, EventArgs e)
        {
            if (lstPoints.SelectedItem != null)
            {
                string[] pnt = lstPoints.SelectedItem.ToString().Split(' ');
                lstPoints.Items[lstPoints.SelectedIndex] = $"{lstPoints.SelectedIndex+1}: {pnt[1]} , {udY.Value} {pnt[4]}";
            }
        }

        private void udY_KeyUp(object sender, KeyEventArgs e)
        {
            udY_ValueChanged(null, null);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            udX.Value = 0;
            udY.Value = 0;
            udTime.Value = 0;
            lstPoints.Items.Clear();
        }
        #endregion

        #endregion

        #region " Mouse Cursor "
        IntPtr oldCursor;

        public override void ResetCursor()
        {
            WindowsHook.UnsafeNativeMethods.SetSystemCursor(WindowsHook.UnsafeNativeMethods.CopyIcon(oldCursor), WindowsHook.UnsafeNativeMethods.OCR_NORMAL);
        }

        public void SetCursor()
        {
            oldCursor = WindowsHook.UnsafeNativeMethods.CopyIcon(Cursors.Default.CopyHandle());
            WindowsHook.UnsafeNativeMethods.SetSystemCursor(WindowsHook.UnsafeNativeMethods.CopyIcon(WindowsHook.UnsafeNativeMethods.LoadCursorFromFile(Application.StartupPath + "\\Cursor1.cur")), WindowsHook.UnsafeNativeMethods.OCR_NORMAL);
        }
        #endregion

        #region " Save Load "

        private void mnuSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.DefaultExt = "tap";
            saveFileDialog1.Filter = "TAP files (*.tap)|*.tap|All files (*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter writetext = new StreamWriter(saveFileDialog1.FileName))
                {
                    foreach (string i in lstPoints.Items)
                    {
                        writetext.WriteLine(i.Substring(i.IndexOf(": ")));
                    }
                }
            }
        }

        private void mnuLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.DefaultExt = "tap";
            openFileDialog1.Filter = "TAP files (*.tap)|*.tap|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                lstPoints.Items.Clear();
                using (StreamReader readtext = new StreamReader(openFileDialog1.FileName))
                {
                    string line;
                    while ((line = readtext.ReadLine()) != null)
                    {
                        lstPoints.Items.Add($"{lstPoints.Items.Count+1}: {line}");
                    }
                }
            }
        }
        #endregion

        #region " Methods "
        private void PlayPoints()
        {
            int i = 0;
            if (sIndex > 0) i = sIndex;
            SetCursor();
            while (doLoop)
            {
                while(i<lstPoints.Items.Count)
                {
                    string[] pnt =lstPoints.Items[i].ToString().Split(' ');
                    Point p = new Point(rect.Left + int.Parse(pnt[1]), rect.Top + int.Parse(pnt[3]));

                    sIndex = i;
                    WindowsHook.MouseHook.SynthesizeMouseMove(p, WindowsHook.MapOn.PrimaryMonitor, IntPtr.Zero);
                    WindowsHook.MouseHook.SynthesizeMouseDown(System.Windows.Forms.MouseButtons.Left, IntPtr.Zero);
                    WindowsHook.MouseHook.SynthesizeMouseUp(System.Windows.Forms.MouseButtons.Left, IntPtr.Zero);
                    System.Threading.Thread.Sleep(int.Parse(pnt[4]));

                    if (!doLoop) break;
                    i++;
                }
                i = 0;
            }
            ResetCursor();
        }

        private int sIndex {
            get
            {
                if (lstPoints.InvokeRequired)
                    return (int)this.Invoke(new Func<int>(() => this.sIndex));
                else
                    return lstPoints.SelectedIndex;
            }
            set
            {
                if (lstPoints.InvokeRequired)
                    this.Invoke(new Func<int>(() => this.sIndex=value));
                else
                    lstPoints.SelectedIndex=value;
            }
        }

        public void DrawDots(Graphics g)
        {
            System.Drawing.Font drawFont = new System.Drawing.Font("Calibri", 12,FontStyle.Bold);
            Pen borderPen = new Pen(Color.Blue,2);
            int c = 1;
            foreach (string i in lstPoints.Items)
            {
                string[] pnts = i.Split(' ');
                Point pnt = new Point(int.Parse(pnts[1]) - 16, int.Parse(pnts[3]) - 16);
                g.DrawImage(img, pnt);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;
                g.DrawString(c.ToString(), drawFont, Brushes.Blue, float.Parse(pnts[1]) + 8, float.Parse(pnts[3]) - 20);
                c++;
            }
            if (doLoop) borderPen.Color = Color.Red;
            g.DrawRectangle(borderPen, 0,0,g.VisibleClipBounds.Width,g.VisibleClipBounds.Height);

            drawFont.Dispose();
            borderPen.Dispose();
        }
        #endregion
    }
}