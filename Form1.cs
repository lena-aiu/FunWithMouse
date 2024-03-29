﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FunWithMouse
{
    public partial class Form1 : Form
    {
        PictureBox pictureBox1;
        Label label1;
        Point spotClicked;



        public Form1()
        {
            //InitializeComponent();
            this.Size = new Size(640, 480);

            pictureBox1 = new PictureBox();
            pictureBox1.Image = (Image)new Bitmap(@"../../images/fun.jpg");
            pictureBox1.SizeMode = PictureBoxSizeMode.Normal;
            pictureBox1.Dock = DockStyle.Fill;
            this.Controls.Add(pictureBox1);



            label1 = new Label();
            label1.BackColor = Color.Wheat;
            label1.Dock = DockStyle.Bottom;
            label1.Text =
            "With left buttont click you might to draw rectangular. " +
            "Right button click change the brightness chosen part of rectangular " +
            "Press SHIFT and move the mouse, you'll draw circles.";
            label1.TextAlign = ContentAlignment.MiddleCenter;

            this.Controls.Add(label1);

            this.pictureBox1.MouseDown += new MouseEventHandler(MouseButtonIsDown);
            this.pictureBox1.MouseUp += new MouseEventHandler(MouseButtonIsUp);
            this.pictureBox1.MouseMove += new MouseEventHandler(TheMouseMoved);

        }

        public void TheMouseMoved(object sender, MouseEventArgs e)
         {
                // Если на клавиатуре нажата клавиша SHIFT
                if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                    {
                // Подготовка области рисования на изображении
                System.Drawing.Graphics g = this.pictureBox1.CreateGraphics();

                // Используем желтое перо
                System.Drawing.Pen yellowPen = new System.Drawing.Pen(Color.Yellow, 3);

                // Рисуем окружность (эллипс, вписанный в квадрат)
                // Верхний левый угол квадрата имеет координаты X и Y текущего положения мыши
                g.DrawEllipse(yellowPen, e.X, e.Y, 40, 40);

                // Очистка
                g.Dispose();
            }
         }


            public void MouseButtonIsDown(object sender, MouseEventArgs e)
            {
                // Запоминаем точку, в которой произошло нажатие кнопки мыши. Когда
                // кнопка будет отпущена, нам понадобятся ее координаты

                spotClicked.X = e.X;// горизонтальная координата
                spotClicked.Y = e.Y;// вертикальная координата
            }


            public void MouseButtonIsUp(object sender, MouseEventArgs e)
            {
                /* Пользователь отпустил кнопку мыши! */

                // Создаем прямоугольник (пока он еще не виден), ограничивающий
                // область изображения, с которой пользователь будет работать

                Rectangle r = new Rectangle();

                // Левый верхний угол соответствует точке, в которой была нажата кнопка мыши
                // Мы сохранили ее координаты с помощью описанного выше метода

                r.X = spotClicked.X;
                r.Y = spotClicked.Y;

                // Ширина и высота прямоугольника вычисляется
                // путем вычитания начальных координат мыши (в точке нажатия)
                // из текущих координат (в точке отжатия кнопки)

                r.Width = e.X - spotClicked.X;
                r.Height = e.Y - spotClicked.Y;

                if (e.Button == MouseButtons.Left)
                {
                    /* Если была нажата и отпущена левая кнопка мыши
                    рисуем видимый контур прямоугольника */

                    // Подготовка области рисования на изображении

                    Graphics g = this.pictureBox1.CreateGraphics();

                    // Рисуем красный контур прямоугольника

                    Pen redPen = new Pen(Color.Red, 2);
                    g.DrawRectangle(redPen, r);
                }
                else
                {
                    // Если была нажата другая кнопка, вызываем более сложный
                    // метод, подсвечивающий область изображения

                    ChangeLightness(r);
                }
            }

        public void ChangeLightness(Rectangle rect)
        {
            int newRed, newGreen, newBlue;
            Color pixel;

            // Копируем изображение, загруженное в PictureBox

            System.Drawing.Bitmap picture = new Bitmap(this.pictureBox1.Image);

            // Поскольку операция увеличения яркости может занять много времени,
            // необходимо предупредить об этом пользователя, если выбран большой участок

            if ((rect.Width > 150) || (rect.Height > 150))
            {

                DialogResult result = MessageBox.Show(
                    "Chosen zone is big! " +
                " Changeing the brighness might take a lot of time!",
                "Warning", MessageBoxButtons.OKCancel);

                // При нажатии кнопки Cancel (Отмена) выходим из метода
                // и возвращаемся к месту его вызова
                if (result == DialogResult.Cancel) return;
            }

            /* Перебираем последовательно все пиксели данного участка
            и удваиваем значение яркости компонент RGB пикселей */

            // Перебор по горизонтали слева направо ...

            for (int x = rect.X; x < rect.X + rect.Width; x++)
            {
                // и по вертикали сверху вниз ...

                for (int y = rect.Y; y < (rect.Y + rect.Height); y++)
                {
                    // Считываем текущий пиксель

                    pixel = picture.GetPixel(x, y);

                    // Увеличиваем яркость цветовых компонент пикселя

                    newRed = (int)Math.Round(pixel.R * 2.0, 0);
                    if (newRed > 255) newRed = 255;
                    newGreen = (int)Math.Round(pixel.G * 2.0, 0);
                    if (newGreen > 255) newGreen = 255;
                    newBlue = (int)Math.Round(pixel.B * 2.0, 0);
                    if (newBlue > 255) newBlue = 255;

                    // Присваиваем пикселю новые цветовые значения

                    picture.SetPixel
                    (x, y, Color.FromArgb(
                    (byte)newRed, (byte)newGreen, (byte)newBlue));
                }
            }

            // Помещаем измененную копию изображения в PictureBox
            // чтобы изменения отобразились на экране

            this.pictureBox1.Image = picture;



        }

        static void Main()
        {
            // Создаем экземпляр класса формы

            Application.Run(new Form1());

        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FunWithTheMouse
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "FunWithTheMouse";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
