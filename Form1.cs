//**********************************************************************************
//*Ашрафзянов Альберт 7-78-11 20.11.09                                             *
//*Методы и средства защиты компьютерной информаций                                *
//*Программа "Энигма", моделирует работу немецкой шифровальной машины "Энигма "    *
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;


namespace Enigma
{

    public partial class Form1 : Form
    {
        private Enigma ob = new Enigma(); //экземпляр класса Энигма
        public Form1()
        {
            InitializeComponent();
            //заполнение алфовитом UpDown
            for (int i = 0; i < ob.length_alfavit; i++)
            {
                domainUpDown1.Items.Add(ob.alfavit[i]);
                domainUpDown2.Items.Add(ob.alfavit[i]);
                domainUpDown3.Items.Add(ob.alfavit[i]);
                domainUpDown4.Items.Add(ob.alfavit[i]);
            }
            domainUpDown1.SelectedIndex = 0;
            domainUpDown2.SelectedIndex = 0;
            domainUpDown3.SelectedIndex = 0;
            domainUpDown4.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = ob.Enigma_(richTextBox1.Text); //обработка текста и вывод
            //поворот UpDown
            domainUpDown1.SelectedIndex = ob.ptr_alf[3];
            domainUpDown2.SelectedIndex = ob.ptr_alf[2];
            domainUpDown3.SelectedIndex = ob.ptr_alf[1];
            domainUpDown4.SelectedIndex = ob.ptr_alf[0];
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы действительно хотите сменить роторы?", "Энигма", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (result == DialogResult.Yes) //подтверждение
                ob.set_spaika(); //смена роторов
        }

        //поворот роторов 1
        private void domainUpDown1_SelectedItemChanged(object sender, EventArgs e)
        {
            ob.ptr_alf[3] = (byte)(domainUpDown1.SelectedIndex);
        }
        //2
        private void domainUpDown2_SelectedItemChanged(object sender, EventArgs e)
        {
            ob.ptr_alf[2] = (byte)(domainUpDown2.SelectedIndex);
        }
        //3
        private void domainUpDown3_SelectedItemChanged(object sender, EventArgs e)
        {
            ob.ptr_alf[1] = (byte)(domainUpDown3.SelectedIndex);
        }
        //4
        private void domainUpDown4_SelectedItemChanged(object sender, EventArgs e)
        {
            ob.ptr_alf[0] = (byte)(domainUpDown4.SelectedIndex);
        }

        //сброс UpDown
        private void button3_Click(object sender, EventArgs e)
        {
            domainUpDown1.SelectedIndex = 0;
            domainUpDown2.SelectedIndex = 0;
            domainUpDown3.SelectedIndex = 0;
            domainUpDown4.SelectedIndex = 0;
        }


        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
                ob.write_rotor(saveFileDialog1.FileName);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
                ob.read_rotor(openFileDialog1.FileName);
        }
    }



    //--------------------------------------------------
    //класс Энигма (моделирование работы машины "Энигма")
    class Enigma
    {
        private Random gen = new Random(unchecked((int)DateTime.Now.Ticks)); //случайные числа;
        public string alfavit = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя "; //алфавит
        public byte length_alfavit = 34;
        private byte count_rotor = 4;
        private byte[,] tabl_rotor = { //спаики (по умолчанию)
                                     {3,4,15,8,13,19,0,6,23,18,10,1,17,22,12,9,11,7,2,21,20,16,5,14,24,25,26,27,28,29,30,31,32,33},
                                     {5,3,22,6,23,8,2,20,9,21,17,10,7,0,13,18,12,11,15,1,16,4,19,14,24,25,26,27,28,29,30,31,32,33},
                                     {1,13,15,11,23,12,19,0,2,21,7,3,14,20,22,9,17,4,8,18,6,10,16,5,24,25,26,27,28,29,30,31,32,33},
                                     {18,12,7,1,17,20,5,21,22,13,3,4,9,23,19,6,14,2,16,8,0,10,15,11,24,25,26,27,28,29,30,31,32,33}
                                     };

        public byte[] ptr_alf = {0,0,0,0}; //указатели на начало алфавита

        private byte[] reflector = {33,32,31,30,29,28,27,26,25,24,23,22,21,20,19,18,17,16,15,14,13,12,11,10,9,8,7,6,5,4,3,2,1,0}; //рефлектор

        private byte find_tablAlfavit(byte[,] mas, byte numberRotor, byte numberBukva) //пойск буквы на роторе
        {
            byte i = 0;
            while ( i < length_alfavit)
            {
                if (mas[numberRotor, i] == numberBukva)
                    return (byte)(i+1);
                i++;
            }
            return 0;
        }

        private byte find_charAlfavit(char bukva) //Возвращает номер индекса сивола в алфавите, если не найдено то 0, возвращаеться значение больше на 1
        {
            byte i = 0;
            while (i < length_alfavit)
            {
                if (alfavit[i] == bukva)
                    return (byte)(i+1); //порядковый номер символа в строке
                i++;
            }
            return 0; //не найден
        }

        private bool find_digit(byte index_i, byte index_j, int digit) //пойск сгенрированного числа в массива (чтобы не повторялись)
        {
            for (int j = 0; j < index_j; j++)
            {
                if (tabl_rotor[index_i, j] == digit)
                    return true; //найден 
            }
            return false;  //не найден
        }

        public void set_spaika()
        {
            int gen_digit; //сгенерированное число
            for (byte i = 0; i < count_rotor; i++) //роторы
            {
                for (byte j = 0; j < length_alfavit; j++) //спайки
                {
                    gen_digit = gen.Next(0, length_alfavit); //генерируем число от 0 до 21
                    while (find_digit(i, j, gen_digit))
                    {
                        gen_digit = gen.Next(0, length_alfavit); //пока повторяются   
                    }
                    tabl_rotor[i, j] = (byte)gen_digit;
                }
            } 
        }

        private void rotor_povorot(byte rot1, byte rot2, byte rot3)
        {
            ptr_alf[0] = (byte)((ptr_alf[0] + 1) % length_alfavit); //поворот первого ротора
            if (ptr_alf[0] == 0)
            {
                ptr_alf[1] = (byte)((ptr_alf[1] + 1) % length_alfavit); //поворот второго ротора на одну позицию
                if (ptr_alf[1] == 0)
                {
                    ptr_alf[2] = (byte)((ptr_alf[2] + 1) % length_alfavit); //поворот третьего ротора на одну позицию
                    if (ptr_alf[2] == 0)
                        ptr_alf[3] = (byte)((ptr_alf[3] + 1) % length_alfavit); //поворот четвертого ротора на одну позицию   
                }
            }
        }

        //запись значение спаек (роторов)
        public void write_rotor(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);

            for (byte i = 0; i < count_rotor; i++)
            {
                for (byte j = 0; j < length_alfavit; j++)
                {
                    bw.Write(tabl_rotor[i, j]);
                }
            }
            bw.Close();
            fs.Close();
        }

        //чтение значения спаек (роторов)
        public void read_rotor(string fileName)
        {
            try
            {
                FileStream fs = new FileStream(fileName, FileMode.Open);
                BinaryReader br = new BinaryReader(fs);

                for (byte i = 0; i < count_rotor; i++)
                {
                    for (byte j = 0; j < length_alfavit; j++)
                    {
                        tabl_rotor[i, j] = br.ReadByte();
                    }
                }
                br.Close();
                fs.Close();
            }
            catch (System.IO.FileNotFoundException)
            {
            }
        }

        private char Enigma_simbol(char Openchar)
        {
            byte number_char; //расположение буквы на роторе
            byte number; //соотвествующая замена на текущем роторе
            number = find_charAlfavit(Openchar); //определяем порядковый номер буквы в алфавите
            if (number != 0)
            {
                number--; //
                for (byte i = 0; i < count_rotor; i++) //прямой ход сигнала
                {
                    number_char = (byte)((ptr_alf[i] + number) % length_alfavit);  //Пойск буквы на роторе
                    number = tabl_rotor[i, number_char]; //Замена
                }

                number = reflector[number]; //рефлектор (отражение сигнала)

                for (byte i = (byte)(count_rotor - 1); i != 255; i--) //обратный ход сигнала
                {
                    number_char = (byte)(find_tablAlfavit(tabl_rotor, i, number) - 1); //Пойск буквы на роторе
                    number = (byte)((length_alfavit - ptr_alf[i] + number_char) % length_alfavit); //Замена
                }
                return alfavit[number];
            }
            else
                return Openchar; //если символа нет в алфавите не шифруем
        }



        //-Полное (де)шифрование сообщения
        public string Enigma_(string OpenMessage)
        {
            int len_str = OpenMessage.Length;
            string CloseMessage = "";
            for (int i = 0; i < len_str; i++)
            {
                CloseMessage = CloseMessage + Enigma_simbol(OpenMessage[i]);
                rotor_povorot(1, 2, 3);
            }
            return CloseMessage;
        }

    } //***конец класс Enigma
    
    
}
