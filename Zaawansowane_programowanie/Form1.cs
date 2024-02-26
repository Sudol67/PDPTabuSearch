using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using static Zaawansowane_programowanie.Algorytm;

namespace Zaawansowane_programowanie
{
    public partial class Form1 : Form
    {
        List<int> mapa = new List<int>();
        List<int> multizbior = new List<int>();
        Random random = new Random();
        private Algorytm algorytm;
        public Form1()
        {
            InitializeComponent();
            algorytm = new Algorytm();
            algorytm.AlgorytmZakonczony += Algorytm_AlgorytmZakonczony;
            algorytm.NajlepszeRozwiazanieZmienione += Algorytm_NajlepszeRozwiazanieZmienione;
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        //------------------------------------ Generator Instancji ----------------------------------------------------------------------
        public void GeneratorMapy()
        {
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox5.Text))
            {
                MessageBox.Show("Długość instancji lub zakres nie mogą być puste", "Error", MessageBoxButtons.OK);
            }
            else
            {
                for (int i = 0; i < int.Parse(textBox1.Text); i++)
                {
                    int wygenerowanaLiczba = random.Next(1, int.Parse(textBox5.Text));
                    mapa.Add(wygenerowanaLiczba);
                }
                string result = string.Join(" ", mapa);
                richTextBox2.Text = result;
            }
        }

        public String GeneratorMultizbior(List<int> wejscie)
        {
            int suma = 0;
            foreach (var liczba in wejscie)
            {
                multizbior.Add(liczba);
                suma += liczba;
            }
            multizbior.Add(suma);

            multizbior.Add(multizbior[multizbior.Count - 1] - multizbior[0]);

            int suma_multizbior = 0;
            int wielkość_mapy = multizbior.Count;
            for (int i = 1; i < wielkość_mapy - 3; i++) 
            {
                suma_multizbior += multizbior[i];
                for (int j = i - 1; j >= 0; j--)
                {
                    suma_multizbior += multizbior[j];
                    multizbior.Add(suma_multizbior);
                    if ((j) == 0)
                    {
                        multizbior.Add(multizbior.Max() - suma_multizbior);
                    }
                }
                suma_multizbior = 0; 
            }
            multizbior.RemoveAt(multizbior.Count - 1); 
            multizbior.Sort();
            richTextBox3.Clear();
            richTextBox1.Clear();
            string result = string.Join(" ", multizbior);
            richTextBox3.Text = result;
            return result;
        }

        public void GeneratorInstancji()
        {
            //Delecja
            for (int i = 0; i < int.Parse(textBox4.Text); i++)
            {
                if (multizbior.Count > 0)
                {
                    int indexToDelete = random.Next(multizbior.Count);
                    multizbior.RemoveAt(indexToDelete);
                }
            }

            //Insercja
            if (multizbior.Count > 0)
            {
                int maxValue = multizbior.Max();
                for (int i = 0; i < int.Parse(textBox3.Text); i++)
                {
                    int valueToInsert = random.Next(1, maxValue + 1);
                    multizbior.Add(valueToInsert);
                }
            }

            //Substytucja
            for (int i = 0; i < int.Parse(textBox2.Text); i++)
            {
                if (multizbior.Count > 0)
                {
                    int indexToReplace = random.Next(multizbior.Count);
                    int maxValue = multizbior.Max();
                    int newValue = random.Next(maxValue + 1);
                    multizbior[indexToReplace] = newValue;
                }
            }

            richTextBox1.Clear();
            multizbior.Sort();
            string resultInstancja = string.Join(" ", multizbior);
            richTextBox1.Text = resultInstancja;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Generator instancji
            richTextBox1.Text = string.Empty;
            richTextBox2.Text = string.Empty;
            richTextBox3.Text = string.Empty;
            mapa.Clear();
            multizbior.Clear();

            int dlugosc = 0;
            try
            {
                dlugosc = int.Parse(textBox1.Text);
            }
            catch { }
            if (string.IsNullOrEmpty(textBox2.Text))
            {
                textBox2.Text = "0";
            }
            if (string.IsNullOrEmpty(textBox3.Text))
            {
                textBox3.Text = "0";
            }
            if (string.IsNullOrEmpty(textBox4.Text))
            {
                textBox4.Text = "0";
            }
            if(string.IsNullOrEmpty(richTextBox2.Text))
            {
                if(string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox5.Text))
                {
                    MessageBox.Show("Długość instancji lub zakres nie mogą być puste", "Error", MessageBoxButtons.OK);
                }
            }
            if (int.Parse(textBox2.Text) > dlugosc || int.Parse(textBox3.Text) > dlugosc || int.Parse(textBox4.Text) > dlugosc)
            {
                MessageBox.Show("Ilość błędów nie może być większa niż wielkość instancji", "Error", MessageBoxButtons.OK);
            }
            else
            {
                if (string.IsNullOrEmpty(richTextBox2.Text) && string.IsNullOrEmpty(richTextBox3.Text) && string.IsNullOrEmpty(richTextBox1.Text))
                {
                    GeneratorMapy();
                    GeneratorMultizbior(mapa);
                    GeneratorInstancji();
                }
                else if (string.IsNullOrEmpty(richTextBox2.Text) == false && string.IsNullOrEmpty(richTextBox3.Text) && string.IsNullOrEmpty(richTextBox1.Text))
                {
                    string input = richTextBox2.Text;
                    mapa.AddRange(input.Split(' ').Select(s => int.Parse(s.Trim())));
                    GeneratorMultizbior(mapa);
                    GeneratorInstancji();
                }
                else if (string.IsNullOrEmpty(richTextBox3.Text) == false && string.IsNullOrEmpty(richTextBox1.Text))
                {
                    GeneratorInstancji();
                }
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(Char.IsNumber(e.KeyChar) || e.KeyChar == 8);
        }
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(Char.IsNumber(e.KeyChar) || e.KeyChar == 8);
        }
        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(Char.IsNumber(e.KeyChar) || e.KeyChar == 8);
        }
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(Char.IsNumber(e.KeyChar) || e.KeyChar == 8);
        }
        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(Char.IsNumber(e.KeyChar) || e.KeyChar == 8);
        }
        private void richTextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(Char.IsNumber(e.KeyChar) || e.KeyChar == ' ');
        }
        private void richTextBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(Char.IsNumber(e.KeyChar) || e.KeyChar == ' ');
        }
        private void richTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(Char.IsNumber(e.KeyChar) || e.KeyChar == ' ');
        }
        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(Char.IsNumber(e.KeyChar) || e.KeyChar == 8);
        }

        private void textBox10_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(Char.IsNumber(e.KeyChar) || e.KeyChar == 8);
        }

        private void textBox8_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(Char.IsNumber(e.KeyChar) || e.KeyChar == 8);
        }

        private void textBox9_KeyPress(object sender, KeyPressEventArgs e)
        {
        }
        private void richTextBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(Char.IsNumber(e.KeyChar) || e.KeyChar == ' ');
        }

        private void richTextBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(Char.IsNumber(e.KeyChar) || e.KeyChar == ' ');
        }

        private void richTextBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(Char.IsNumber(e.KeyChar) || e.KeyChar == ' ');
        }

        private void richTextBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(Char.IsNumber(e.KeyChar) || e.KeyChar == ' ');
        }

        private void richTextBox8_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(Char.IsNumber(e.KeyChar) || e.KeyChar == ' ');
        }

        private void richTextBox10_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(Char.IsNumber(e.KeyChar) || e.KeyChar == ' ');
        }

        private void richTextBox9_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(Char.IsNumber(e.KeyChar) || e.KeyChar == ' ');
        }

        private void richTextBox11_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(Char.IsNumber(e.KeyChar) || e.KeyChar == 8 || e.KeyChar == 44 || e.KeyChar == 37);
        }

        private void richTextBox12_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(Char.IsNumber(e.KeyChar) || e.KeyChar == ' ');
        }


        private void button5_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Program obsługuje pliki w formacie .txt. W pierwszej linijce powinna znależć się mapa, w drugiej multizbiór a w trzeciej instancja wejściowa. Pozostawienie którejkolwiek linijki pustej spowoduje brak wpisania do pól danej zmiennej");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Zapisz do pliku
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.DefaultExt = "txt";
                saveFileDialog.Filter = "Pliki tekstowe (*.txt)|*.txt|Wszystkie pliki (*.*)|*.*";
                saveFileDialog.FileName = "Wyjscie";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;
                    using (StreamWriter sw = new StreamWriter(filePath))
                    {
                        sw.WriteLine(richTextBox2.Text);

                        sw.WriteLine(richTextBox3.Text);

                        sw.WriteLine(richTextBox1.Text);
                    }

                    MessageBox.Show("Zapisano treść do pliku: " + filePath);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Odczytaj z pliku
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
            textBox3.Text = string.Empty;
            textBox4.Text = string.Empty;
            textBox5.Text = string.Empty;
            richTextBox1.Text = string.Empty;
            richTextBox2.Text = string.Empty;
            richTextBox3.Text = string.Empty;
            mapa.Clear();
            multizbior.Clear();
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.DefaultExt = "txt";
                openFileDialog.Filter = "Pliki tekstowe (*.txt)|*.txt|Wszystkie pliki (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    if (File.Exists(filePath))
                    {
                        string[] lines = File.ReadAllLines(filePath);

                        if (lines.Length > 0 && !string.IsNullOrWhiteSpace(lines[0]))
                            richTextBox2.Text = lines[0];
                        if (lines.Length > 1 && !string.IsNullOrWhiteSpace(lines[1]))
                            richTextBox3.Text = lines[1];
                        if (lines.Length > 2 && !string.IsNullOrWhiteSpace(lines[2]))
                            richTextBox1.Text = lines[2];
                    }
                    else
                    {
                        MessageBox.Show("Plik nie istnieje.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
            textBox3.Text = string.Empty;
            textBox4.Text = string.Empty;
            textBox5.Text = string.Empty;
            richTextBox1.Text = string.Empty;
            richTextBox2.Text = string.Empty;
            richTextBox3.Text = string.Empty;
            mapa.Clear();
            multizbior.Clear();
        }

        //--------------------------------------------- Wyniki ------------------------------------------------------------------------
        private void richTextBox6_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //----------------------Oryginalne------------------------------------------------------
                List<int> poczatkowa = richTextBox2.Text.Split(' ').Select(int.Parse).ToList();
                int suma = 0;
                List<int> punktyCiec = new List<int>();
                punktyCiec.Add(suma);
                foreach (int odleglosc in poczatkowa)
                {
                    suma += odleglosc;
                    punktyCiec.Add(suma); 
                }
                richTextBox5.Text = string.Join(" ", punktyCiec);
                richTextBox7.Text = richTextBox1.Text;

                //-----------------Nowe----------------------------------------------------------
                List<int> koncowa = new List<int>();
                koncowa.Clear();
                koncowa = richTextBox6.Text.Split(' ').Select(int.Parse).ToList();
                List<int> multiNowe = new List<int>();
                multiNowe.Clear();
                for (int i = 0; i < koncowa.Count; i++)
                {
                    for (int j = i + 1; j < koncowa.Count; j++)
                    {
                        int odleglosc = koncowa[j] - koncowa[i];
                        multiNowe.Add(odleglosc);
                    }
                }
                multiNowe.Sort();
                richTextBox8.Text = string.Join(" ", multiNowe);

                //---------------Porównanie-------------------------------------------------------
                int dlugoscIdealna = punktyCiec.Count;
                int dlugoscUzyskana = koncowa.Count;
                int roznica = Math.Abs(dlugoscIdealna - dlugoscUzyskana);
                double pokrycie = (double)dlugoscUzyskana / dlugoscIdealna * 100;
                string wynik = $"Rozw. początkowe: {dlugoscIdealna}, Uzyskane: {dlugoscUzyskana}, Różnica: {roznica} elementów\n";
                wynik += $"Pokrycie: {pokrycie.ToString("F2")}%";
                richTextBox9.Text = wynik;

                int oryg = multizbior.Count;
                int wynikowy = multiNowe.Count;
                int roznicaMulti = Math.Abs(oryg - wynikowy);
                double pokrycieMulti = (double)wynikowy / oryg * 100;
                richTextBox11.Text = $"{pokrycieMulti.ToString("F2")}%";

                int z = 0, y = 0;

                while (z < multizbior.Count && y < multiNowe.Count)
                {
                    if (multizbior[z] < multiNowe[y])
                    {
                        richTextBox10.AppendText(multizbior[z] + " ");
                        z++;
                    }
                    else if (multizbior[z] > multiNowe[y])
                    {
                        y++;
                    }
                    else
                    {
                        z++;
                        y++;
                    }
                }

                while (z < multizbior.Count)
                {
                    richTextBox10.AppendText(multizbior[z] + " ");
                    z++;
                }
            }
            catch 
            { 

            }
        }
        //--------------------------------------------- Metaheurystyka ----------------------------------------------------------------
        private void Algorytm_NajlepszeRozwiazanieZmienione(List<int> najlepszeRozwiazanie)
        {
            this.Invoke((MethodInvoker)delegate
            {
                richTextBox4.Text = String.Join(" ", najlepszeRozwiazanie);
            });
        }
        private void Algorytm_AlgorytmZakonczony(AlgorytmWynik wynik)
        {
            List<int> najlepszyWynik = new List<int>();
            this.Invoke((MethodInvoker)delegate
            {
                string zawartoscListy = "";
                zawartoscListy = string.Join(" ", wynik.Rozwiazanie);
                string czasWykonania = "";
                czasWykonania = wynik.CzasWykonania.ToString(@"hh\:mm\:ss\.fff");

                richTextBox6.Text = zawartoscListy;
                richTextBox6_TextChanged(richTextBox6, EventArgs.Empty);
                richTextBox12.Text = czasWykonania;
                wynik.Rozwiazanie.Clear();
                wynik.CzasWykonania = TimeSpan.Zero;
            });
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //Start algorytm
            richTextBox6.Text = string.Empty;
            richTextBox4.Text = string.Empty;
            richTextBox5.Text = string.Empty;
            richTextBox7.Text = string.Empty;
            richTextBox8.Text = string.Empty;
            richTextBox9.Text = string.Empty;
            richTextBox10.Text = string.Empty;
            richTextBox11.Text = string.Empty;
            richTextBox12.Text = string.Empty;

            var parameters = Tuple.Create(richTextBox1.Text, int.Parse(textBox6.Text), int.Parse(textBox10.Text), int.Parse(textBox8.Text), int.Parse(textBox9.Text));
            algorytm.StartTask(parameters);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            algorytm.CancelTask();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
            textBox10.Text = string.Empty;
            textBox9.Text = string.Empty;
            textBox8.Text = string.Empty;
            textBox6.Text = string.Empty;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
            textBox1.Text = string.Empty;
            textBox5.Text = string.Empty;
            textBox2.Text = string.Empty;
            textBox3.Text = string.Empty;
            textBox4.Text = string.Empty;
        }
    }
}
