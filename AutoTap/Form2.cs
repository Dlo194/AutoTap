using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoTap
{
    public partial class Form2 : Form
    {
        Thread PrePaintThread;

        public Form2()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Make mouse clicks pass through form
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x80000 | 0x20;
                return cp;
            }
        }

        public void PrePaint()
        {
            while (true)
            {
                DoRefresh();
                System.Threading.Thread.Sleep(50);
            }
        }

        private void DoRefresh()
        {
            if(this.InvokeRequired)
                this.Invoke(new MethodInvoker(() => { DoRefresh(); }));
            else
                this.Refresh();
        }

        private void Form2_Paint(object sender, PaintEventArgs e)
        {
            ((Form1)this.Owner).DrawDots(e.Graphics);
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            PrePaintThread = new Thread(new ThreadStart(this.PrePaint));
            PrePaintThread.Start();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            PrePaintThread.Abort();
        }
    }
}
