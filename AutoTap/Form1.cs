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
            lstPoints.Items.Add($"{e.X - rect.Left} , {e.Y - rect.Top} 1000");
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

                DisplayForm.Size = new Size(rect.Right - rect.Left, rect.Bottom - rect.Top);
                DisplayForm.Top = rect.Top;
                DisplayForm.Left = rect.Left;

                if (!DisplayForm.Visible)
                {
                    timer1.Enabled = true;
                    DisplayForm.Show(this);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            IntPtr curhandle = WindowsHook.UnsafeNativeMethods.GetForegroundWindow();
            WindowsHook.UnsafeNativeMethods.GetWindowRect(hWnd, ref rect);
            DisplayForm.Size = new Size(rect.Right - rect.Left, rect.Bottom - rect.Top);
            DisplayForm.Top = rect.Top;
            DisplayForm.Left = rect.Left;

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
                if (!DisplayForm.Visible) DisplayForm.Show();
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
                udTime.Value = decimal.Parse(pnt[3]);
                udX.Value = decimal.Parse(pnt[0]);
                udY.Value = decimal.Parse(pnt[2]);
            }
        }

        private void udTime_ValueChanged(object sender, EventArgs e)
        {
            if (lstPoints.SelectedItem != null)
            {
                string[] pnt = lstPoints.SelectedItem.ToString().Split(' ');
                lstPoints.Items[lstPoints.SelectedIndex] = $"{pnt[0]} , {pnt[2]} {udTime.Value}";
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
                lstPoints.Items[lstPoints.SelectedIndex] = $"{udX.Value} , {pnt[2]} {pnt[3]}";
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
                lstPoints.Items[lstPoints.SelectedIndex] = $"{pnt[0]} , {udY.Value} {pnt[3]}";
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
                        writetext.WriteLine(i);
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
                        lstPoints.Items.Add(line);
                    }
                }
            }
        }
        #endregion

        #region " Methods "
        private void PlayPoints()
        {
            SetCursor();
            while (doLoop)
            {
                foreach (string i in lstPoints.Items)
                {
                    string[] pnt = i.Split(' ');
                    Point p = new Point(rect.Left + int.Parse(pnt[0]), rect.Top + int.Parse(pnt[2]));

                    WindowsHook.MouseHook.SynthesizeMouseMove(p, WindowsHook.MapOn.PrimaryMonitor, IntPtr.Zero);
                    WindowsHook.MouseHook.SynthesizeMouseDown(System.Windows.Forms.MouseButtons.Left, IntPtr.Zero);
                    WindowsHook.MouseHook.SynthesizeMouseUp(System.Windows.Forms.MouseButtons.Left, IntPtr.Zero);
                    System.Threading.Thread.Sleep(int.Parse(pnt[3]));

                    if (!doLoop) break;
                }
            }
            ResetCursor();
        }

        public void DrawDots(Graphics g)
        {
            foreach (string i in lstPoints.Items)
            {
                string[] pnts = i.Split(' ');
                Point pnt = new Point(int.Parse(pnts[0]) - 16, int.Parse(pnts[2]) - 16);
                g.DrawImage(img, pnt);
            }
        }
        #endregion

    }
}