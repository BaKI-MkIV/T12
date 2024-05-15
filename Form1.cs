using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace T12
{
    public partial class Form1 : Form
    {
        private string _alph = "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        private List<string> _check;
        public HashTable _prototype;

        public Form1()
        {
            InitializeComponent();
            _check = Loader("..\\..\\ruslib.xlsx", 0);
            _prototype = new HashTable(_check, _alph, 3);
            
            _prototype.PrintTable();
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

        private string Crop(string focus)
        {
            
            int lastSpaceIndex = focus.LastIndexOf(' ');
            if (lastSpaceIndex == -1)
            {
                return focus;
            }
            
            return focus.Substring(lastSpaceIndex + 1);
        }

        private void HeshChecker(HashTable point)
        {
            label1.Text = "";
            string marker = textBox1.Text;
            if (string.IsNullOrEmpty(marker))
            {
                return;
            }

            List<string> result = point.Search(marker);
            
            foreach (var it in result)
            {
                LabelPush(it);
            }
        }

        private void LabelPush(string word)
        {
            label1.Text += word + Environment.NewLine;
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            HeshChecker(_prototype);
        }
    }
    
    public class HashTable
    {
        private Dictionary<string, List<string>> table;
        private string alphabet;
        private int part;

        public HashTable(List<string> words, string alphabet, int part)
        {
            this.alphabet = alphabet;
            this.part = part;
            table = new Dictionary<string, List<string>>();

            foreach (var word in words)
            {
                Add(word);
            }
        }

        public void Add(string word)
        {
            string hash = ConvertorX16(word);
            if (!table.ContainsKey(hash))
            {
                table[hash] = new List<string>();
            }

            table[hash].Add(word);
        }

        public List<string> Search(string word)
        {
            //string hash = ConvertorX16(word);
            if (table.ContainsKey(word))
            {
                return table[word];
            }

            return new List<string>();
        }

        private string ConvertorX16(string word)
        {
            string word16 = "";
            string alph16 = "123456789ABC";
            foreach (var it in word)
            {
                int number = alphabet.ToLower().IndexOf(char.ToLower(it));
                word16 += alph16[(number / part)];
            }
            return word16;
        }
        
        public void PrintTable()
        {
            foreach (var kvp in table)
            {
                
                if (kvp.Value.Count > 1)
                {   
                    Console.WriteLine($"Hash: {kvp.Key}");
                    foreach (var word in kvp.Value)
                    {
                        Console.WriteLine(word);
                    }
                }
            }
        }
    }
}