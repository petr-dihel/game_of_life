using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pa_3
{
    class Program
    {
        class MyForm : Form
        {
            public bool[,] Square { get; set; }

            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = this.CreateGraphics();
                Pen selPen = new Pen(Color.Blue);
                SolidBrush brush = new SolidBrush(Color.Blue);
                base.OnPaint(e);
                
                if (Square != null)
                {
                   
                    int partX = (int)((this.Width - 50) / this.Square.GetLength(0));
                    int partY = (int)((this.Height - 50) / this.Square.GetLength(0));
            
                    for (int i = 0; i < this.Square.GetLength(0); i++)
                    {
                        for (int j = 0; j < this.Square.GetLength(0); j++)
                        {
                            if (this.Square[i, j])
                            {
                                g.FillRectangle(brush, partX * i, partY * j, partX, partY);
                            } else
                            {
                                g.DrawRectangle(selPen, partX * i, partY * j, partX, partY);
                            }
                        }
                    }

                }

                g.Dispose();
            }
        }


        static void initFalse(ref bool[,] square) {
            for (int i = 0; i < square.GetLength(0); i++)
            {
                for (int j = 0; j < square.GetLength(1); j++)
                {
                    square[i, j] = false;
                }
            }
        }

        static void show(ref bool[,] square)
        {
            Console.WriteLine("Square:");
            for (int i = 0; i < square.GetLength(0); i++)
            {
                for (int j = 0; j < square.GetLength(1); j++)
                {
                    char write = square[i, j] ? '#' : 'O';
                    Console.Write(write);
                    if (j < square.GetLength(1) - 1 ) {
                        Console.Write("-");
                    }
                }
                Console.WriteLine("");
            }
        }

        static void draw(ref bool[,] square, ref Form form)
        {
            Graphics g = form.CreateGraphics();
            Pen selPen = new Pen(Color.Blue);
            Console.WriteLine("Square:");
            int partX = (int) (form.Width / square.GetLength(0));
            int partY = (int) (form.Height / square.GetLength(0));

            for (int i = 0; i < square.GetLength(0); i++)
            {
                for (int j = 0; j < square.GetLength(1); j++)
                {
                    g.DrawRectangle(selPen, 10, 10, 50, 50);
                }
            }

            g.Dispose();
        }

        static void ProccessPixel(ref bool[,] square, ref bool[,] newSquare, int tmpX, int tmpY)
        {
            int x = tmpX;
            int y = tmpY;
            int sumOfLivingN = 0;

            for (int k = (y - 1); k <= (y + 1); k++)
            {
                for (int l = (x - 1); l <= (x + 1); l++)
                {
                    if (k == y && x == l)
                    {
                        continue;
                    }
                    if (
                        k >= 0 && l >= 0
                        && k < square.GetLength(1) && l < square.GetLength(1)
                        )
                    {
                        if (square[k, l])
                        {
                            sumOfLivingN += 1;
                        }

                    }
                }
            }

            if (square[y, x])
            {

                if (sumOfLivingN < 2)
                {
                    newSquare[y, x] = false;
                }
                if (sumOfLivingN > 3)
                {
                    newSquare[y, x] = false;
                }

            }
            else
            {
                if (sumOfLivingN == 3)
                {
                    newSquare[y, x] = true;
                }
            }
        }

        static void copyArray(ref bool[,] sq1, ref bool[,] sq2)
        {
            for (int i = 0; i < sq1.GetLength(0); i++)
            {
                for (int j = 0; j < sq1.GetLength(1); j++)
                {
                    sq2[i, j] = sq1[i, j];
                }
            }
        }

        static void Main(string[] args)
        {
            int maxCountOfThreads = 6;
            bool[, ] square = new bool[15, 15];
            bool[, ] newSquare = new bool[15, 15];
            List<Thread> threads = new List<Thread>();
            initFalse(ref square);
            // frog
            square[1, 3] = true;    
            square[2, 1] = true;    
            square[2, 4] = true;    
            square[3, 1] = true;    
            square[3, 4] = true;    
            square[4, 2] = true;

            square[10, 3] = true;
            square[10, 4] = true;
            square[10, 5] = true;

            square[1, 10] = true;
            square[2, 10] = true;
            square[1, 11] = true;
            square[2, 11] = true;

            square[3, 12] = true;
            square[4, 12] = true;
            square[3, 13] = true;
            square[4, 13] = true;
            show(ref square);
           
            MyForm myform = new MyForm();
            myform.Text = "Main Window";
            myform.Size = new Size(640, 400);
            myform.FormBorderStyle = FormBorderStyle.FixedDialog;
            myform.StartPosition = FormStartPosition.CenterScreen;
            myform.Show();

            while (true)
            {
                initFalse(ref newSquare);
                copyArray(ref square, ref newSquare);
                
                for (int y = 0; y < square.GetLength(0); y++)
                {
                    for (int x = 0; x < square.GetLength(1); x++)
                    {

                        int tmpX = x;
                        int tmpY = y;
                        Thread myNewThread = new Thread(() => ProccessPixel(ref square, ref newSquare, tmpX, tmpY));
                        myNewThread.Start();
                        threads.Add(myNewThread);
                        if (threads.Count >= maxCountOfThreads)
                        {
                            foreach (Thread thread in threads)
                            {
                                thread.Join();
                            }
                            threads.Clear();
                        }

                    }
                }
                myform.Square = newSquare;
                myform.Refresh();
                System.Threading.Thread.Sleep(1000);
                copyArray(ref newSquare, ref square);
            }
            
        }
    }
}
