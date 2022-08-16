﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using MarkdownSharp;

namespace MarkdownWin
{
    public partial class MainForm : Form
    {
        private string _pendingPreviewHtml;
        private readonly Markdown _markdown;
        private readonly FileSystemWatcher _fileWatcher;

        public MainForm(string[] args)
        {
            InitializeComponent();

            browser.DocumentCompleted += BrowserDocumentCompleted;
            browser.PreviewKeyDown += BrowserPreviewKeyDown;

            browser.AllowWebBrowserDrop = false;
            browser.IsWebBrowserContextMenuEnabled = false;
            browser.WebBrowserShortcutsEnabled = false;
            browser.AllowNavigation = true;
            browser.ScriptErrorsSuppressed = true;

            _markdown = new Markdown();
            _fileWatcher = new FileSystemWatcher();
            _fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
            _fileWatcher.Changed += new FileSystemEventHandler(OnWatchedFileChanged);

            this.Disposed += new EventHandler(Watcher_Disposed);
            browser.AllowWebBrowserDrop = true;

            //Open file from parameter if given
            if (args.Length > 0)
            {
                
                try
                {
                    RefreshPreview(args[0]);
                } catch
                {
                    MessageBox.Show("Could not open requested markdown file", "failed to open", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (args.Length > 1)
                {
                    this.Text = args[1];
                }
            }
        }

        private void Watcher_Disposed(object sender, EventArgs e)
        {
            _fileWatcher.Dispose();
        }

        private void Watcher_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.Link;
            }
        }

        private void Watcher_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            WatchFile(files[0]);
        }

        private void OnWatchedFileChanged(object sender, FileSystemEventArgs e)
        {
            RefreshPreview(e.FullPath);
        }

        private void mnuOpenFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.Filter = "All files (*.*)|*.*";
            openFileDialog1.FileName = string.Empty;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                WatchFile(openFileDialog1.FileName);
            }
        }

        private void RefreshPreview(string fileName)
        {
            browser.Stop();

            string text;
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }

            _pendingPreviewHtml = _markdown.Transform(text);

            //browser.AllowNavigation = true;
            browser.Navigate("about:blank");
            //browser.AllowNavigation = false;
        }

        private void BrowserDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            const string htmlTemplate = "<html><head><style type=\"text/css\">{0}</style></head><body>{1}</body></html>";

            if (browser.Document != null)
            {
                string stylesheet;
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(this.GetType(), "markdown.css"))
                using (var reader = new StreamReader(stream))
                {
                    stylesheet = reader.ReadToEnd();
                }

                string html = string.Format(htmlTemplate, stylesheet, _pendingPreviewHtml);
                browser.Document.Write(html);

                Debug.WriteLine("Document Completed and written to.");
            }
        }

        private void BrowserPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (mnuFloatWindow.ShortcutKeys == e.KeyData)
                mnuFloatWindow_Click(mnuFloatWindow, EventArgs.Empty);

            else if (mnuOpenFile.ShortcutKeys == e.KeyData)
                mnuOpenFile_Click(mnuOpenFile, EventArgs.Empty);

            else if (mnuCopyHtml.ShortcutKeys == e.KeyData)
                mnuCopyHtml_Click(mnuCopyHtml, EventArgs.Empty);

            else if (mnuPrint.ShortcutKeys == e.KeyData)
                mnuPrint_Click(mnuPrint, EventArgs.Empty);

            else if (mnuExit.ShortcutKeys == e.KeyData)
                mnuExit_Click(mnuExit, EventArgs.Empty);
        }

        private void WatchFile(string fileName)
        {
            RefreshPreview(fileName);

            _fileWatcher.Path = Path.GetDirectoryName(fileName);
            _fileWatcher.Filter = Path.GetFileName(fileName);
            _fileWatcher.EnableRaisingEvents = true;
        }

        private void mnuPrint_Click(object sender, EventArgs e)
        {
            browser.ShowPrintDialog();
        }

        private void mnuPrintPreview_Click(object sender, EventArgs e)
        {
            browser.ShowPrintPreviewDialog();
        }

        private void mnuCopyHtml_Click(object sender, EventArgs e)
        {
            if (_pendingPreviewHtml != null)
                Clipboard.SetText(_pendingPreviewHtml.Trim());
        }

        private void mnuFloatWindow_Click(object sender, EventArgs e)
        {
            mnuFloatWindow.Checked = !mnuFloatWindow.Checked;
            TopMost = mnuFloatWindow.Checked;
        }

        private void mnuMarkdownGuide_Click(object sender, EventArgs e)
        {
            Process.Start("http://daringfireball.net/projects/markdown/syntax");
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void mnuAbout_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.github.com/jpoehls/MarkdownWin");
        }

        private void toolStripMenuItemExtended_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Obama360/MarkdownWinExtended");
        }

        //replace link opening behaviour to be able to open relative .md files
        private void browser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (!e.Url.ToString().Contains("res://") && !e.Url.ToString().Contains("about:blank"))
            {
                try
                {
                    RefreshPreview(e.Url.ToString().Replace("about:", ""));
                    e.Cancel = true;
                } catch
                {
                    e.Cancel = false;
                }
            }
        }
    }
}
