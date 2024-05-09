using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace T12
{
    public partial class Form1 : Form
    {
        private TextBox textBox1; 
        private int keyCount = 12;

        public Form1()
        {
            InitializeComponent();
            CreateKeyboard(); 
            CreateTextBox(); 
        }

        private void CreateTextBox()
        {
            textBox1 = new TextBox();
            textBox1.Multiline = true;
            textBox1.Dock = DockStyle.Bottom;
            Controls.Add(textBox1);
        }

        private void CreateKeyboard()
        {
            string alph = "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";

            List<string> alph_blocks = new List<string>();
            
            for (int i = 0; i < alph.Length; i += 3)
            {
                // Определяем длину подстроки (не превышающую 3 символа)
                int length = Math.Min(3, alph.Length - i);
                // Получаем подстроку из строки alph, начиная с позиции i и длиной length
                string block = alph.Substring(i, length);
                // Добавляем подстроку в список
                alph_blocks.Add(block);
            }

            int x = 20;
            int y = 20; 
            int buttonWidth = 90; 
            int buttonHeight = 50; 

            for (int i = 0; i < alph_blocks.Count; i++)
            {
                SelfButton button = new SelfButton(alph_blocks[i]);
                button.Text = alph_blocks[i];
                button.Size = new System.Drawing.Size(buttonWidth, buttonHeight);
                button.Location = new System.Drawing.Point(x, y);
                button.Click += Button_Click;
                Controls.Add(button);
                
                if ((i + 1) % 3 == 0)
                {
                    x = 20;
                    y += buttonHeight + 10;
                }
                else
                {
                    x += buttonWidth + 10;
                }
            }
            
            SelfButton space = new SelfButton(" ");
            space.Text = "Space";
            space.Size = new System.Drawing.Size(buttonWidth, buttonHeight);
            space.Location = new System.Drawing.Point(x, y);
            space.Click += Button_Click; 
            Controls.Add(space);
            
            
        }

        private void Button_Click(object sender, EventArgs e)
        {
            SelfButton button = (SelfButton)sender; 
            textBox1.Text += button.codeText; 
        }
    }
    
    public class SelfButton : Button
    {
        public string codeText { get; set; }

        public SelfButton() : base()
        {
            codeText = "Default Text";
        }

        public SelfButton(string txt) : base()
        {
            codeText = txt;
        }
    }
}