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

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = string.Empty;
            textBox2.Clear();
            dataGridView1.Rows.Clear();
            statusLabel.Text = "Ready to scan the input text.";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            CompileSource(textBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            dataGridView1.Rows.Clear();
            Errors.Error_List.Clear();
            statusLabel.Text = "Workspace cleared.";
        }

        private void CompileSource(string sourceCode)
        {
            dataGridView1.Rows.Clear();
            textBox2.Clear();
            Errors.Error_List.Clear();

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

            statusLabel.Text = $"Compiled {scanner.Tokens.Count} token(s) with {Errors.Error_List.Count} error(s).";
        }
    }
}
