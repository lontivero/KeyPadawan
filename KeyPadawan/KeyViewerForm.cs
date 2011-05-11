namespace KeyPadawan
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;

    public class KeyViewerForm : Form
    {
        private ToolStripMenuItem menuItem;
        private IContainer container;
        private TextBox textBox;
        private TrackBar trackBar;
        private PictureBox pictureBox;
        private ContextMenuStrip menu;

        public KeyViewerForm()
        {
            this.InitializeComponents();
        }

        private void OnLoad(object menuItemA, EventArgs args)
        {
            App.ki.KeyIntercepted += new KeyboardInterceptor.KeyboardEventHandler(this.OnKeyPressed);
        }

        private void OnKeyPressed(KeyboardInterceptor.KeyboardEventArgs args)
        {
            this.textBox.Text = this.textBox.Text + args.Ascii;
            this.textBox.Select(this.textBox.Text.Length, 0);
            this.textBox.ScrollToCaret();
        }

        private void OnTrackBarScroll(object menuItemA, EventArgs args)
        {
            base.Opacity = ((double) this.trackBar.Value) / 100.0;
        }

        private void OnPictureBoxClick(object menuItemA, EventArgs args)
        {
            Process.Start("http://geeks.ms/blogs/lontivero");
        }

        private void OnClearLogMenuClick(object menuItemA, EventArgs args)
        {
            this.textBox.Clear();
        }

        private void InitializeComponents()
        {
            this.container = new Container();
            ComponentResourceManager manager = new ComponentResourceManager(typeof(KeyViewerForm));
            this.textBox = new TextBox();
            this.trackBar = new TrackBar();
            this.pictureBox = new PictureBox();
            this.menu = new ContextMenuStrip(this.container);
            this.menuItem = new ToolStripMenuItem();
            this.trackBar.BeginInit();
            ((ISupportInitialize) this.pictureBox).BeginInit();
            this.menu.SuspendLayout();
            base.SuspendLayout();
            this.textBox.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.textBox.BackColor = Color.White;
            this.textBox.BorderStyle = BorderStyle.None;
            this.textBox.CausesValidation = false;
            this.textBox.ContextMenuStrip = this.menu;
            this.textBox.Font = new Font("Arial", 22f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.textBox.ForeColor = Color.DimGray;
            this.textBox.Location = new Point(0, 0);
            this.textBox.Multiline = true;
            this.textBox.Name = "txtKeyViewer";
            this.textBox.ReadOnly = true;
            this.textBox.ScrollBars = ScrollBars.Vertical;
            this.textBox.ShortcutsEnabled = false;
            this.textBox.Size = new Size(0x1b1, 0x9b);
            this.textBox.TabIndex = 0;
            this.trackBar.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.trackBar.LargeChange = 1;
            this.trackBar.Location = new Point(0xe4, 0xa7);
            this.trackBar.Maximum = 100;
            this.trackBar.Minimum = 0x19;
            this.trackBar.Name = "trackBar";
            this.trackBar.Size = new Size(0xc1, 0x2a);
            this.trackBar.TabIndex = 1;
            this.trackBar.TickStyle = TickStyle.None;
            this.trackBar.Value = 0x4b;
            this.trackBar.Scroll += new EventHandler(this.OnTrackBarScroll);
            this.pictureBox.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.pictureBox.Cursor = Cursors.Hand;
            this.pictureBox.Image = (Image) manager.GetObject("pictureBoxPaeLogo.Image");
            this.pictureBox.Location = new Point(0, 0x9e);
            this.pictureBox.Name = "pictureBoxPaeLogo";
            this.pictureBox.Size = new Size(0x90, 0x1f);
            this.pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            this.pictureBox.TabIndex = 2;
            this.pictureBox.TabStop = false;
            this.pictureBox.Click += new EventHandler(this.OnPictureBoxClick);
            this.menu.Items.AddRange(new ToolStripItem[] { this.menuItem });
            this.menu.Name = "contextMenuStrip1";
            this.menu.Size = new Size(0x99, 0x30);
            this.menuItem.Name = "clearLogToolStripMenuItem";
            this.menuItem.Size = new Size(0x98, 0x16);
            this.menuItem.Text = "Clear log";
            this.menuItem.Click += new EventHandler(this.OnClearLogMenuClick);
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            base.ClientSize = new Size(0x1b1, 0xbd);
            base.Controls.Add(this.pictureBox);
            base.Controls.Add(this.trackBar);
            base.Controls.Add(this.textBox);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.Name = "KeyViewerForm";
            base.Opacity = 0.75;
            this.Text = "Key Padawan";
            base.TopMost = true;
            base.Load += new EventHandler(this.OnLoad);
            this.trackBar.EndInit();
            ((ISupportInitialize) this.pictureBox).EndInit();
            this.menu.ResumeLayout(false);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.container != null))
            {
                this.container.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

