using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saber_Interacive_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var ListSerialize = new ListRandom();

            for (int i = 0; i < 1_000_000; i++)
            {
                ListSerialize.Add($"Значение {i}");
            }

            ListSerialize.Serialize(new FileStream("List.json", FileMode.Create));

            Console.WriteLine("Файл готов, для продолжения нажмите любую кнопку\n");

            Console.ReadKey();

            var ListDeSerialize = new ListRandom();

            ListDeSerialize.Deserialize(new FileStream("List.json", FileMode.Open));

            Console.WriteLine("Файл считан, для продолжения нажмите любую кнопку\n");

            Console.WriteLine($"Случайный элемент из списка с начала списка: {ListDeSerialize.Head.Random.Data}\n");

            Console.WriteLine($"Случайный элемент из списка с конца списка: {ListDeSerialize.Tail.Random.Data}\n");

            Console.ReadKey();
        }
    }

    class ListNode
    {
        /// <summary>
        /// Предыдущий элемент списка
        /// </summary>
        public ListNode Previous;
        /// <summary>
        /// Следующий элемент списка
        /// </summary>
        public ListNode Next;
        private ListNode random;
        /// <summary>
        /// Выдача случайного элемента списка
        /// </summary>
        public ListNode Random
        {
            // Жаль, что вам придется увидеть строки с 61 по 86 :)
            get
            {
                Random r = new Random();

                int count = 1;

                random = this;

                bool isHead = false;

                if (random.Previous == null)
                    isHead = true;

                while (random != null)
                {
                    random = isHead ? random.Next : random.Previous;

                    count++;
                }

                random = this;

                for (int i = 0; i < r.Next(count); i++)
                {
                    random = isHead ? random.Next : random.Previous;
                }

                return random;
            }
        }

        /// <summary>
        /// Данные
        /// </summary>
        public string Data;
    }

    class ListRandom
    {
        /// <summary>
        /// Первый элмент списка
        /// </summary>
        public ListNode Head;
        /// <summary>
        /// Последний элемент списка
        /// </summary>
        public ListNode Tail;
        /// <summary>
        /// Текущее количество элементов в списке
        /// </summary>
        public int Count;

        /// <summary>
        /// Добавление нового элемента
        /// </summary>
        /// <param name="data"></param>
        public void Add(string data)
        {
            ListNode node = new ListNode();

            node.Data = data;

            if (Head == null)
            {
                Head = node;
            }
            else
            {
                Tail.Next = node;
                node.Previous = Tail;
            }

            Tail = node;
            Count++;
        }

        /// <summary>
        /// Сериализация списка
        /// </summary>
        /// <param name="s"></param>
        public void Serialize(Stream s)
        {
            StringBuilder strBuilder = new StringBuilder();

            string stringPart;

            ListNode node = this.Head;

            strBuilder.AppendLine("{");

            for (int i = 0; i < this.Count; i++)
            {
                if (node == null)
                    break;
                // Формирование строки для сериализации
                stringPart = i + 1 == this.Count ? $@"""{nameof(node.Data)}"": ""{node.Data}""" : $@"""{nameof(node.Data)}"": ""{node.Data}"", ";

                strBuilder.AppendLine(stringPart);

                node = node.Next;
            }

            strBuilder.AppendLine("}");

            // Запись потока на диск
            using (FileStream fs = s as FileStream)
            {
                byte[] ar = Encoding.Default.GetBytes(strBuilder.ToString());

                fs.Write(ar, 0, ar.Length);
            }
        }

        /// <summary>
        /// Десериализация списка
        /// </summary>
        /// <param name="s"></param>
        public void Deserialize(Stream s)
        {
            string stringData; // Строка данных

            // Получение данных из потока
            using (FileStream fs = s as FileStream)
            {
                byte[] ar = new byte[fs.Length];

                fs.Read(ar, 0, ar.Length);

                stringData = Encoding.Default.GetString(ar);
            }

            // Формирование ужного формата строки
            stringData = stringData.Replace(Environment.NewLine, "").Replace("{", "").Replace("}", "").Replace(@"""", "");

            // Разделение на подстроки
            string[] tempAr = stringData.Split(',');

            for (var i = 0; i < tempAr.Length; i++)
            {
                // Удаление пробела
                tempAr[i] = tempAr[i].Trim(new char[] { ' ' });
                // Отсечение ненужных данных
                tempAr[i] = tempAr[i].Remove(0, tempAr[i].IndexOf(":") + 1);
            }

            foreach (var t in tempAr)
            {
                Add(t);
            }
        }
    }
}
