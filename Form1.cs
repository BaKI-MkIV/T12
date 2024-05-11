using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace T12
{
    public partial class Form1 : Form
    {
        private string previus = "";
        private int keyCount = 12;
        private int currentIndex = 0; // переменная для отслеживания текущего индекса символа
        private int ch = 0;
        private Timer timer; // Таймер для отслеживания времени
        private bool timerStarted = false; // Флаг для отслеживания состояния таймера
        private TextBox outputTextBox; // Текстовое поле для вывода совпадений
        private List<string> check = Loader("..\\..\\ruslib.xlsx", 0);

        public Form1()
        {
            InitializeComponent();

            // Инициализируем таймер
            timer = new Timer();
            timer.Interval = 1000; // Интервал таймера в миллисекундах (1 секунда)
            timer.Tick += Timer_Tick; // Обработчик события таймера

            // Инициализация текстового поля вывода
            outputTextBox = new TextBox();
            outputTextBox.Multiline = true;
            outputTextBox.ReadOnly = true;
            outputTextBox.ScrollBars = ScrollBars.Vertical; // Включаем вертикальную прокрутку
            outputTextBox.Dock = DockStyle.Right; // Докаем текстовое поле справа
            outputTextBox.Width = 200; // Устанавливаем ширину текстового поля
            outputTextBox.Font = new Font("Times New Roman", 14);
            Controls.Add(outputTextBox);
            CreateKeyboard();
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
            int buttonWidth = 100;
            int buttonHeight = 60;

            for (int i = 0; i < alph_blocks.Count; i++)
            {
                SelfButton button = new SelfButton(alph_blocks[i]);
                button.Text = alph_blocks[i];

                button.Size = new System.Drawing.Size(buttonWidth, buttonHeight);
                button.Location = new System.Drawing.Point(x, y);
                button.Click += button_Click;
                Controls.Add(button);

                if ((i + 1) % 3 == 0)
                {
                    x = 20;
                    y += buttonHeight + 15;
                }
                else
                {
                    x += buttonWidth + 15;
                }
            }

            SelfButton space = new SelfButton(" ");
            space.Text = "Space";
            space.Size = new System.Drawing.Size(buttonWidth, buttonHeight);
            space.Location = new System.Drawing.Point(x, y);
            space.Click += button_Click;
            Controls.Add(space);


        }

        private void button_Click(object sender, EventArgs e)
        {
            SelfButton button = (SelfButton)sender;

            if (previus == "")
            {
                previus = button.Text;
            }

            if (button.Text != previus)
            {
                timer.Stop(); // Остановка таймера
                timerStarted = false;
                Timer_Tick(timer, EventArgs.Empty);
            }


            // Проверяем, чтобы индекс не выходил за пределы длины текста кнопки
            if (currentIndex >= button.Text.Length)
            {
                currentIndex = 0;
            }

            // Обновляем текст метки
            UpdateLabel(button);

            // Сброс таймера до нуля и запуск, если не был запущен
            timer.Stop();
            timerStarted = false;
            timer.Start();
            timerStarted = true;

            currentIndex++;
            previus = button.Text;
        }
        

        private void Timer_Tick(object sender, EventArgs e)
        {
            Pip(check, Crop(label1.Text));
            currentIndex = 0;
            label1.Text += " ";
            ch++;
            
            timer.Stop();
            timerStarted = false;
            previus = "";
        }

        private void UpdateLabel(object sender)
        {
            SelfButton button = (SelfButton)sender; // Явное приведение объекта sender к типу SelfButton

            if (button.Text != "Space")
            {
                string labelText = label1.Text; // Получаем текущий текст метки
                labelText = labelText.Remove(ch, 1)
                    .Insert(ch, button.codeText[currentIndex].ToString()); // Заменяем символ в указанной позиции
                label1.Text = labelText;
            }
        }
        
        private static List<string> Loader(string filepath, int columnIndex)
        {
            List<string> words = new List<string>();

            try
            {
                using (FileStream file = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                {
                    IWorkbook workbook = new XSSFWorkbook(file);
                    ISheet sheet = workbook.GetSheetAt(0);
                    
                    for (int row = 1; row <= sheet.LastRowNum; row++)
                    {
                        IRow currentRow = sheet.GetRow(row);
                        if (currentRow != null)
                        {
                            string word = currentRow.GetCell(columnIndex).StringCellValue.ToLower(); 
                            words.Add(word);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке слов из файла: {ex.Message}");
            }

            return words;
        }


        private string Crop(string focus)
        {
            
            int lastSpaceIndex = focus.LastIndexOf(' ');
            if (lastSpaceIndex == -1)
            {
                return focus;
            }
            
            return focus.Substring(lastSpaceIndex + 1);
        }
        
        private void Pip(List<string> focus, string target)
        {
            outputTextBox.Clear();
            if (target != "")
            {
                int count = 0;
                foreach (var it in focus)
                {
                    if (it.StartsWith(target.ToLower()))
                    {
                        AddMatch(it);
                        count++;
                        if (count >= 20) // Если найдено уже 20 совпадений, прекращаем поиск
                            break;
                    }
                }
            }
        }
        
        private void AddMatch(string match)
        {
            // Добавляем новое совпадение в текстовое поле
            outputTextBox.AppendText(match + Environment.NewLine); // Добавляем совпадение и переходим на новую строку
        }

        private void rmv_Click(object sender, EventArgs e)
        {
            if (label1.Text.Length != 1)
            {
                label1.Text = label1.Text.Substring(0, label1.Text.Length - 1);
                
                ch--;
            }
            else
            {
                label1.Text = " ";
            }
        }
    }
    
    public class SelfButton : Button
    {
        public string codeText { get; set; }

        public SelfButton(string txt) : base()
        {
            codeText = txt;
        }
    }
}