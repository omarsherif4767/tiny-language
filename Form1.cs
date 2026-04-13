using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tiny
{
    public partial class Form1 : Form
    {
        private readonly Scanner scanner = new Scanner();
        private int nextTabNumber = 1;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox2.Clear();
            dataGridView1.Rows.Clear();
            statusLabel.Text = "Ready to scan the input text.";
            AddNewTab();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            CompileActiveTab();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var activeEditor = GetActiveEditor();
            if (activeEditor != null)
            {
                activeEditor.Clear();
            }

            textBox2.Clear();
            dataGridView1.Rows.Clear();
            Errors.Error_List.Clear();
            statusLabel.Text = $"Cleared {GetActiveTabTitle()}.";
        }

        private void buttonAddTab_Click(object sender, EventArgs e)
        {
            AddNewTab();
        }

        private void tabControlSources_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tabPage = tabControlSources.TabPages[e.Index];
            var tabBounds = tabControlSources.GetTabRect(e.Index);
            var background = e.Index == tabControlSources.SelectedIndex ? Color.White : Color.FromArgb(232, 237, 243);
            var border = Color.FromArgb(208, 216, 224);

            using (var backgroundBrush = new SolidBrush(background))
            using (var borderPen = new Pen(border))
            using (var textBrush = new SolidBrush(Color.FromArgb(33, 37, 41)))
            using (var closeBrush = new SolidBrush(Color.FromArgb(120, 128, 138)))
            using (var closeFont = new Font("Segoe UI", 8F, FontStyle.Bold))
            {
                e.Graphics.FillRectangle(backgroundBrush, tabBounds);
                e.Graphics.DrawRectangle(borderPen, tabBounds);

                var textRect = new Rectangle(tabBounds.X + 12, tabBounds.Y + 8, tabBounds.Width - 30, tabBounds.Height - 14);
                TextRenderer.DrawText(e.Graphics, tabPage.Text, tabPage.Font ?? Font, textRect, Color.FromArgb(33, 37, 41), TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

                var closeRect = GetCloseButtonBounds(tabBounds);
                TextRenderer.DrawText(e.Graphics, "x", closeFont, closeRect, Color.FromArgb(120, 128, 138), TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }

        private void tabControlSources_MouseDown(object sender, MouseEventArgs e)
        {
            for (var index = 0; index < tabControlSources.TabCount; index++)
            {
                var tabBounds = tabControlSources.GetTabRect(index);
                var closeBounds = GetCloseButtonBounds(tabBounds);

                if (closeBounds.Contains(e.Location))
                {
                    CloseTab(tabControlSources.TabPages[index]);
                    return;
                }
            }

            for (var index = 0; index < tabControlSources.TabCount; index++)
            {
                if (tabControlSources.GetTabRect(index).Contains(e.Location))
                {
                    tabControlSources.SelectedIndex = index;
                    break;
                }
            }
        }

        private void CompileActiveTab()
        {
            var activeEditor = GetActiveEditor();
            if (activeEditor == null)
            {
                statusLabel.Text = "No source tab is available.";
                return;
            }

            CompileSource(activeEditor.Text, GetActiveTabTitle());
        }

        private void CompileSource(string sourceCode, string tabName)
        {
            dataGridView1.Rows.Clear();
            textBox2.Clear();
            Errors.Error_List.Clear();
            scanner.Tokens.Clear();

            scanner.StartScanning(sourceCode);

            foreach (var token in scanner.Tokens)
            {
                dataGridView1.Rows.Add(token.lex, token.token_type.ToString());
            }

            foreach (var error in Errors.Error_List)
            {
                textBox2.AppendText(error + "\r\n");
            }

            if (Errors.Error_List.Count == 0)
            {
                textBox2.Text = "No Errors";
            }

            statusLabel.Text = $"{tabName}: {scanner.Tokens.Count} token(s), {Errors.Error_List.Count} error(s).";
        }

        private void AddNewTab()
        {
            var tabPage = new TabPage($"Source {nextTabNumber++}");
            tabPage.BackColor = Color.White;

            var editor = new TextBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 249, 251),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 10F, FontStyle.Regular, GraphicsUnit.Point, 0),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                AcceptsTab = true,
                WordWrap = false
            };

            editor.TextChanged += textBox1_TextChanged;
            tabPage.Controls.Add(editor);
            tabControlSources.TabPages.Add(tabPage);
            tabControlSources.SelectedTab = tabPage;
            editor.Focus();
            statusLabel.Text = $"Opened {tabPage.Text}.";
        }

        private void CloseTab(TabPage tabPage)
        {
            if (tabControlSources.TabPages.Count <= 1)
            {
                statusLabel.Text = "At least one source tab must remain open.";
                return;
            }

            var isSelected = tabControlSources.SelectedTab == tabPage;
            tabControlSources.TabPages.Remove(tabPage);
            tabPage.Dispose();

            if (isSelected && tabControlSources.TabCount > 0)
            {
                tabControlSources.SelectedIndex = 0;
            }

            statusLabel.Text = "Tab closed.";
        }

        private TextBox GetActiveEditor()
        {
            if (tabControlSources.SelectedTab == null)
            {
                return null;
            }

            return tabControlSources.SelectedTab.Controls.OfType<TextBox>().FirstOrDefault();
        }

        private string GetActiveTabTitle()
        {
            return tabControlSources.SelectedTab?.Text ?? "Active source";
        }

        private Rectangle GetCloseButtonBounds(Rectangle tabBounds)
        {
            return new Rectangle(tabBounds.Right - 22, tabBounds.Top + 10, 12, 12);
        }
    }
}
