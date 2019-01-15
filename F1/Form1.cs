using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

namespace Test00
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            g = panel1.CreateGraphics();
        }
        Graphics g;

        void readFromFile(String filename)
        {
            String line = "";
            try
            {
                StreamReader sr = new StreamReader(filename);

                line = sr.ReadLine();

                String[] a = line.Split(' ');
                n = Convert.ToInt32(a[0]);
                m = Convert.ToInt32(a[1]);

                line = sr.ReadLine();

                array = new int[n, m];

                int i = 0;
                while (line != null && i < m)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (line[j] == 'G' || line[j] == 'g')
                        {
                            array[j, i] = 5;
                            Ex = j;
                            Ey = i;
                        }
                        else if (line[j] == 'S' || line[j] == 's')
                        {
                            array[j, i] = 6;
                            Sx = j;
                            Sy = i;
                        }
                        else
                        {
                            if (line[j] == '1')
                                array[j, i] = 1;
                            else
                                array[j, i] = 0;
                        }

                    }
                    i++;
                    line = sr.ReadLine();
                }
                sr.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(line + "\n" + e.Message);
            }
        }

        void print()
        {
            float s = n > m ? n : m;

            float size = panel1.Height < panel1.Width ? panel1.Height / s : panel1.Width / s;

            SolidBrush Start = new SolidBrush(Color.DeepPink);
            SolidBrush Go = new SolidBrush(Color.DeepSkyBlue);
            SolidBrush Block = new SolidBrush(Color.DarkGray);
            SolidBrush Line = new SolidBrush(Color.Blue);

            SolidBrush Seen = new SolidBrush(Color.DarkSeaGreen);
            Pen Seen1 = Pens.DarkSeaGreen;

            Pen Empty = Pens.LightGray;

            g.Clear(Color.Black);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    float x = i * size;
                    float y = j * size;

                    if (array[i, j] == 1)
                    {
                        g.FillRectangle(Block, x, y, size, size);
                    }
                    else if (array[i, j] == 2)
                    {
                        g.FillPie(Line, x, y, size, size, 0, 360);
                    }
                    else if (array[i, j] == 3)
                    {
                        //g.FillRectangle(Seen, x, y, size, size);
                        g.DrawLine(Seen1, x, y, x + size, y + size);
                        g.DrawLine(Seen1, x + size, y, x, y + size);
                    }
                    else if (array[i, j] == 5) // Go
                    {
                        g.FillRectangle(Go, x, y, size, size);
                    }
                    else if (array[i, j] == 6) // Start
                    {
                        g.FillRectangle(Start, x, y, size, size);
                    }
                    g.DrawRectangle(Empty, x, y, size, size);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button2_Click(this, null);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            DirectoryInfo d = new DirectoryInfo(Application.StartupPath);
            FileInfo[] files = d.GetFiles("*.txt");

            foreach (FileInfo a in files)
            {
                listBox1.Items.Add(a.Name);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                readFromFile(listBox1.SelectedItem.ToString());
                func();
                print();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        // 0 = مکان های آزاد
        // 1 = مکان های بلاک
        // 2 = مکان های طی شده
        // 3 = مکان های دیده شده
        // 5 = مکان هدف
        // 6 = مکان شروع

        int[,] array;
        int n, m;
        int Sx, Sy; // start point
        int Ex, Ey; // end point

        // start = 6, end = 5, block = 1, free = 0

        class returntype
        {
            public int[,] a;
            public int n = -1;
        }

        // برای کپی از آرایه
        int[,] CopyOf(int[,] arr)
        {
            int[,] res = new int[n, m];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    res[i, j] = arr[i, j];
                }
            }

            return res;
        }

        returntype rec(int[,] arr, int i, int j)
        {
            if (arr[i, j] == 1)
            {
                returntype rt = new returntype();
                rt.a = arr;
                rt.n = -1;
                return rt;
            }

            // بررسی رسیدن به هدف توسط مقادیر نقطه هدف
            if ((i == Ex && j + 1 == Ey) || (i + 1 == Ex && j == Ey) || (i == Ex && j - 1 == Ey) || (i - 1 == Ex && j == Ey) ) 
            {
                returntype rt = new returntype();
                rt.a = arr;
                rt.n = 1;
                arr[i, j] = 2;
                return rt;
            }
            
            // بررسی رسیدن به هدف توسط مقدار درون آرایه
            if ((array[i,j + 1] == 5) || (array[i + 1, j] == 5) || (array[i, j - 1] == 5) || (array[i - 1, j] == 5))
            {
                returntype rt = new returntype();
                rt.a = arr;
                rt.n = 1;
                arr[i, j] = 2;
                return rt;
            }
           
            // برای ذخیره کردن طول هر جهت از مکان فعلی تا هدف
            int[] a = new int[4];
            a[0] = a[1] = a[2] = a[3] = -1;

            // جهت کپی کردن ماتریس برای هر یک از جهت ها
            int[,] cpr = new int[n, m], cpb = new int[n, m], cpl = new int[n, m], cpt = new int[n, m];

            if(j+1 < m) //  بازه آرایه
                if (arr[i,j+1] == 0) // به سمت راست
                {
                    cpr = CopyOf(arr);
                    cpr[i, j + 1] = 2;
                    returntype rt = new returntype();
                    rt = rec(cpr, i, j + 1);
                    a[0] = rt.n;
                    cpr = rt.a;
                }

            if(i+1<n) //  بازه آرایه
                if (arr[i+1, j] == 0) // سمت پایین
                {
                    cpb = CopyOf(arr);
                    cpb[i+ 1, j] = 2;
                    returntype rt = new returntype();
                    rt = rec(cpb, i + 1, j);
                    a[1] = rt.n;
                    cpb = rt.a;
                }

            if(j>0) //  بازه آرایه
                if (arr[i, j - 1] == 0) // سمت چپ
                {
                    cpl = CopyOf(arr);
                    cpl[i, j-1] = 2;
                    returntype rt = new returntype();
                    rt = rec(cpl, i, j - 1);
                    a[2] = rt.n;
                    cpl = rt.a;
                }

            if(i>0) //  بازه آرایه
                if (arr[i-1,j] == 0) // سمت بالا
                {
                    cpt = CopyOf(arr);
                    cpt[i-1, j] = 2;
                    returntype rt = new returntype();
                    rt = rec(cpt,i - 1, j);
                    a[3] = rt.n;
                    cpt = rt.a;
                }


            // کمینه ترین راه
            // اگر بن بست باشد مقدار
            // index
            // تغییری نمیکند
            int index = -1;
            int min = int.MaxValue;
            for (int f = 0; f < 4; f++)
            {
                if (a[f] != -1)
                    if (a[f] < min)
                    {
                        min = a[f];
                        index = f;
                    }
            }

            //  برگرداندن کمینه ترین راه
            returntype r = new returntype();
            switch (index)
            {
                case 0: // سمت راست

                    for (int q = 0; q < n; q++)
                    {
                        for (int w = 0; w < m; w++)
                        {
                            if (cpr[q, w] != 2)
                            {
                                if (cpl[q, w] == 2)
                                {
                                    cpr[q, w] = 3;
                                }
                                else if (cpb[q, w] == 2)
                                {
                                    cpr[q, w] = 3;
                                }
                                else if (cpt[q, w] == 2)
                                {
                                    cpr[q, w] = 3;
                                }
                            }
                        }
                    }

                    r.a = cpr;
                    r.n = a[0] + 1;
                    return r;
                case 1: // سمت پایین
                    for (int q = 0; q < n; q++)
                    {
                        for (int w = 0; w < m; w++)
                        {
                            if (cpb[q, w] != 2)
                            {
                                if (cpl[q, w] == 2)
                                {
                                    cpb[q, w] = 3;
                                }
                                else if (cpr[q, w] == 2)
                                {
                                    cpb[q, w] = 3;
                                }
                                else if (cpt[q, w] == 2)
                                {
                                    cpb[q, w] = 3;
                                }
                            }
                        }
                    }

                    r.a = cpb;
                    r.n = a[1] + 1;
                    return r;
                case 2: // سمت چپ
                    for (int q = 0; q < n; q++)
                    {
                        for (int w = 0; w < m; w++)
                        {
                            if (cpl[q, w] != 2)
                            {
                                if (cpb[q, w] == 2)
                                {
                                    cpl[q, w] = 3;
                                }
                                else if (cpr[q, w] == 2)
                                {
                                    cpl[q, w] = 3;
                                }
                                else if (cpt[q, w] == 2)
                                {
                                    cpl[q, w] = 3;
                                }
                            }
                        }
                    }
                    r.a = cpl;
                    r.n = a[2] + 1;
                    return r;
                case 3: // سمت بالا
                    for (int q = 0; q < n; q++)
                    {
                        for (int w = 0; w < m; w++)
                        {
                            if (cpt[q, w] != 2)
                            {
                                if (cpb[q, w] == 2)
                                {
                                    cpt[q, w] = 3;
                                }
                                else if (cpr[q, w] == 2)
                                {
                                    cpt[q, w] = 3;
                                }
                                else if (cpl[q, w] == 2)
                                {
                                    cpt[q, w] = 3;
                                }
                            }
                        }
                    }
                    r.a = cpt;
                    r.n = a[3] + 1;
                    return r;
                default: // راهی وجود ندارد
                    for (int q = 0; q < n; q++)
                    {
                        for (int w = 0; w < m; w++)
                        {
                            if (cpt[q, w] == 2)
                            {
                                arr[q, w] = 3;
                            }
                            if (cpb[q, w] == 2)
                            {
                                arr[q, w] = 3;
                            }
                            if (cpr[q, w] == 2)
                            {
                                arr[q, w] = 3;
                            }
                            if (cpl[q, w] == 2)
                            {
                                arr[q, w] = 3;
                            }
                        }
                    }
                    r.a = arr;
                    r.n = -1;
                    return r;
            }

        }

        void func()
        {
            listBox2.Items.Clear();
            listBox2.Items.Add("Start: " + Sx + "  " + Sy);
            listBox2.Items.Add("End: " + Ex + " " + Ey);
            if (Sx == Ex && Sy == Ey)
            {
                listBox2.Items.Add("Result: " + 0);
            }
            else
            {
                returntype rt = new returntype();
                rt = rec(array, Sx, Sy);
                rt.a[Sx, Sy] = 6;
                array = rt.a;
                listBox2.Items.Add("Result: " + rt.n);
            }
        }
    }
}
