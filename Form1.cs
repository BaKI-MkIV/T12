using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace T12
{
    public partial class Form1 : Form
    {
        private string _previous = "";
        
        private int _currentIndex = -10; // переменная для отслеживания текущего индекса символа
        private Timer _timer; // Таймер для отслеживания времени
        private bool _timerStarted = false; // Флаг для отслеживания состояния таймера
        private TextBox _outputTextBox; // Текстовое поле для вывода совпадений
        private List<string> _check = Loader("..\\..\\ruslib.xlsx", 0);

        public Form1()
        {
            InitializeComponent();

            // Инициализируем таймер
            _timer = new Timer();
            _timer.Interval = 1000; // Интервал таймера в миллисекундах (1 секунда)
            _timer.Tick += Timer_Tick; // Обработчик события таймера

            // Инициализация текстового поля вывода
            _outputTextBox = new TextBox();
            _outputTextBox.Multiline = true;
            _outputTextBox.ReadOnly = true;
            _outputTextBox.ScrollBars = ScrollBars.Vertical; // Включаем вертикальную прокрутку
            _outputTextBox.Dock = DockStyle.Right; // Докаем текстовое поле справа
            _outputTextBox.Width = 200; // Устанавливаем ширину текстового поля
            _outputTextBox.Font = new Font("Times New Roman", 14);
            Controls.Add(_outputTextBox);
            CreateKeyboard();
        }
        
        private void CreateKeyboard()
        {
            string alph = "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";

            List<string> alphBlocks = new List<string>();

            for (int i = 0; i < alph.Length; i += 3)
            {
                // Определяем длину подстроки (не превышающую 3 символа)
                int length = Math.Min(3, alph.Length - i);
                // Получаем подстроку из строки alph, начиная с позиции i и длиной length
                string block = alph.Substring(i, length);
                // Добавляем подстроку в список
                alphBlocks.Add(block);
            }

            int x = 20;
            int y = 20;
            int buttonWidth = 100;
            int buttonHeight = 60;

            for (int i = 0; i < alphBlocks.Count; i++)
            {
                SelfButton button = new SelfButton(alphBlocks[i]);
                button.Text = alphBlocks[i];

                button.Size = new Size(buttonWidth, buttonHeight);
                button.Location = new Point(x, y);
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
            space.Text = @"Space";
            space.Size = new Size(buttonWidth, buttonHeight);
            space.Location = new Point(x, y);
            space.Click += button_Click;
            Controls.Add(space);


        }

        private void button_Click(object sender, EventArgs e)
        {
            SelfButton button = (SelfButton)sender;
            
            if (_currentIndex == -10)
            {
                label1.Text += @" ";
            }

            if (_previous == "")
            {
                _previous = button.Text;
            }

            if (_currentIndex >= button.Text.Length || _currentIndex < 0)
            {
                _currentIndex = 0;
            }
            
            if (button.Text != _previous)
            {
                _timer.Stop(); // Остановка таймера
                _timerStarted = false;
                Timer_Tick(_timer, EventArgs.Empty);
            }

            if (_currentIndex == -10)
            {
                label1.Text += @" ";
                _currentIndex = 0;
            }

            
            // Обновляем текст метки
            UpdateLabel(button);

            // Сброс таймера до нуля и запуск, если не был запущен
            _timer.Stop();
            _timerStarted = false;
            _timer.Start();
            _timerStarted = true;

            _currentIndex++;
            _previous = button.Text;
        }
        

        private void Timer_Tick(object sender, EventArgs e)
        {
            Finder(_check, Crop(label1.Text));
            _currentIndex = -10;
            
            _timer.Stop();
            _timerStarted = false;
            _previous = "";
        }

        private void UpdateLabel(object sender)
        {
            SelfButton button = (SelfButton)sender; // Явное приведение объекта sender к типу SelfButton
            
            
            if (button.Text != @"Space")
            {
                string labelText = label1.Text; // Получаем текущий текст метки
                int ch = labelText.Length - 1;
                labelText = labelText.Remove(ch, 1)
                    .Insert(ch, button.CodeText[_currentIndex].ToString()); // Заменяем символ в указанной позиции
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
                MessageBox.Show($@"Ошибка при загрузке слов из файла: {ex.Message}");
            }

            return words;
        }

        private List<string> Variable(string target)
        {
            List<string> options =  new List<string>();
            target = target.Substring(0, target.Length - 1);

            foreach (var it in _previous)
            {
                options.Add(target + it);
            }
            
            return options;
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
        
        private void Finder(List<string> focus, string target)
        {
            _outputTextBox.Clear();
            if (target != "")
            {
                //target = target.Substring(0, target.Length - 1);
                Console.WriteLine(_previous);
                int count = 0;
                List<string> targetWords = Variable(target);
                
                foreach (var it in focus)
                {
                    if (targetWords.Any(word => it.StartsWith(word.ToLower())))
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
            _outputTextBox.AppendText(match + Environment.NewLine); // Добавляем совпадение и переходим на новую строку
        }

        private void rmv_Click(object sender, EventArgs e)
        {
            label1.Text = label1.Text.Substring(0, label1.Text.Length - 1);
            Finder(_check, Crop(label1.Text));
        }
    }
    
    public class SelfButton : Button
    {
        public string CodeText { get; set; }

        public SelfButton(string txt)
        {
            CodeText = txt;
        }
    }
}