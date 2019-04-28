using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Clipboard_Forms
{
    public partial class Form1 : Form
    {
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        private const int WM_DRAWCLIPBOARD = 0x0308;
        private IntPtr _clipboardViewerNext;

        private const string DirectoryPath = "Clipboadr";
        private const string FileText = "Text.md";

        public Form1()
        {
            InitializeComponent();
            CreateDirectory();
            _clipboardViewerNext = SetClipboardViewer(Handle);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);    // Process the message 

            IDataObject iData = Clipboard.GetDataObject();      // Clipboard's data

            if (m.Msg == WM_DRAWCLIPBOARD)
            {
                if (Clipboard.ContainsText())
                    SaveText(Clipboard.GetText());

                if (Clipboard.ContainsImage())
                    SaveImage(Clipboard.GetImage());
            }
        }

        private void CreateDirectory()
        {
            if (!Directory.Exists(DirectoryPath))
                Directory.CreateDirectory(DirectoryPath);

            if (!File.Exists(DirectoryPath + Path.DirectorySeparatorChar + FileText))
            {
                try
                {
                    FileStream fs = File.Create(DirectoryPath + Path.DirectorySeparatorChar + FileText);
                    fs.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void SaveImage(Image image)
        {
            CreateDirectory();
            try
            {
                image.Save(DirectoryPath + Path.DirectorySeparatorChar + String.Format("{0:HH-mm-ss}.jpg", DateTime.Now));
                image.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SaveText(string text)
        {
            CreateDirectory();
            try
            {
                using (StreamWriter sw = File.AppendText(DirectoryPath + Path.DirectorySeparatorChar + FileText))
                {
                    sw.WriteLine("```\n" + text + "\n```\n\n");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            ChangeClipboardChain(Handle, _clipboardViewerNext);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(false);
        }
    }
}
