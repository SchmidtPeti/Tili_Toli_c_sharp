using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tili_Toli
{
    public partial class frmTilitoli : Form
    {
        public frmTilitoli()
        {
            InitializeComponent();
        }

        Timer timer;
        static int max = 2, maxn = 2, magassag = 500, szelesseg = 500, lyukx, lyuky, handler_width = 200, time = 100;

        Button[,] gombok = new Button[max, maxn];
        Image[,] images = new Image[max, maxn];

        static Random vél = new Random();

        private void frmTilitoli_Load(object sender, EventArgs e)
        {
            timer = new Timer();
            timer.Interval = 1000;
            timer.Start();
            set_form();
            image_array();
            Panel game_Area = set_game_panel();
            Panel handler = set_handler();
            this.Controls.Add(handler);
            this.Controls.Add(game_Area);
            timer.Tick += delegate {
                time -= 1;
                Label time_label = handler.Controls.Find("score", true).FirstOrDefault() as Label;time_label.Text = time.ToString();
                if (time == 0)
                {
                    game_over();
                }
                if (win())
                {
                    MessageBox.Show("Nagy vagy gyíkarc");
                }
            };
            game_area_load(game_Area);

        }
        private void game_over()
        {
            MessageBox.Show("Game over!");
            time = 100;
            Kever();
        }
        private void pifi_ful_message()
        {

        }
        private void game_area_load(Panel panel)
        {
            int sorszam = 0;
            for (int i = 0; i < max; i++)
            {
                for (int j = 0; j < maxn; j++)
                {
                    sorszam++;
                    gombok[i, j] = new Button();
                    gombok[i, j].Name = sorszam.ToString();
                    gombok[i, j].Size = new Size(szelesseg / maxn, magassag / max);
                    gombok[i, j].Location = new Point(j * (szelesseg / max), i * (magassag / max + 1));
                    gombok[i, j].FlatStyle = FlatStyle.Popup;
                    gombok[i, j].BackColor = Color.Beige;
                    gombok[i, j].Visible = true;
                    gombok[i, j].Font = new Font(gombok[i, j].Font.Name, 10, FontStyle.Bold);
                    panel.Controls.Add(gombok[i, j]);
                    gombok[i, j].Click += new EventHandler(Kattint);
                    gombok[i, j].BackgroundImage = images[i, j];
                    gombok[i, j].Tag = sorszam;
                  //  gombok[i, j].Text = gombok[i, j].Tag.ToString();

                }
            }
            Kever();
        }
        private void set_form()
        {
            this.MinimumSize = new Size(szelesseg + handler_width, magassag);
            this.MaximizeBox = false;
            this.ClientSize = new Size(szelesseg + handler_width, magassag);
        }
        private Panel set_game_panel()
        {
            Panel panel1 = new Panel();
            panel1.Size = new Size(szelesseg, magassag);
            panel1.Location = new Point(0, 0);
            panel1.BackColor = Color.Black;
            panel1.Visible = true;
            return panel1;
        }
        private void image_array()
        {
            int sorszam = 0;
            for (int i = 0; i < max; i++)
            {
                for (int j = 0; j < maxn; j++)
                {
                    sorszam++;
                    images[i,j] = cropImage(ResizeImage(Image.FromFile("imag1.jpg"), szelesseg, magassag), j * (szelesseg / max), i * (szelesseg / max), szelesseg / max, szelesseg / max);
                    images[i, j].Tag = sorszam.ToString(); 
                }
            }
        }
        private Panel set_handler()
        {
            Panel handler = new Panel();
            handler.Size = new Size(handler_width, magassag);
            handler.Location = new Point(szelesseg, 0);
            Button button = new Button();
            button.Text = "Keveres";
            button.Size = new Size(handler_width, 100);
            button.FlatStyle = FlatStyle.Standard;
            button.Click += Handler_Click;
            Label label = new Label();
            label.Text = time.ToString();
            label.Size = new Size(handler_width, 100);
            label.Location = new Point(0, button.Height);
            label.Name = "score";
            handler.Controls.Add(button);
            handler.Controls.Add(label);
            return handler;
        }
        private Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        private void Handler_Click(object sender, EventArgs e)
        {
            Kever();
        }

        private void Kever()
        {
            int szám, k;
            List < int > számok = new List<int>();

            for (int i = 0; i < max; i++)
            {
                for (int j = 0; j < max; j++)
                {
                    do
                    {
                        szám = vél.Next(max * max);
                        k = számok.IndexOf(szám);
                    } while (k != -1);
                    számok.Add(szám);
                    //gombok[i, j].Tag = Convert.ToString(szám);
                    //gombok[i, j].Text = gombok[i, j].Tag.ToString();
                    index index = number_to_index(szám);
                    gombok[i, j].BackgroundImage = images[index.x, index.y];
                    if (szám == 0)
                    {
                        gombok[i, j].Visible = false;
                        lyukx = i;
                        lyuky = j;
                    }
                    else
                    {
                        gombok[i, j].Visible = true;
                    }

                }
            }

        }

        private bool lyukszomszed(Button gomb)
        {
            Button lyuk;
            lyuk = gombok[lyukx, lyuky];
            int hasonlitas = int.Parse(lyuk.Name);
            return (int.Parse(gomb.Name) == hasonlitas+1)|| (int.Parse(gomb.Name) == hasonlitas - 1) || (int.Parse(gomb.Name)==hasonlitas+max)|| (int.Parse(gomb.Name) == hasonlitas - max);
        }

        private void Check(object sender , EventArgs e)
        {
            for (int i = 0; i < max; i++)
            {
                for (int j = 0; j < maxn; j++)
                {
                    if ((i + 1 + j * maxn).ToString() == gombok[i, j].Name) continue;
                }
            }

        }
        private Image cropImage(Image img,int top,int left, int width, int height)
        {
            Rectangle cropArea = new Rectangle(new Point(top, left), new Size(width, height));
            Bitmap bmpImage = new Bitmap(img);
            return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        }
        private void Kattint(object sender, EventArgs e)
        {
           Button gomb = (sender as Button);
            if (lyukszomszed(gomb))
            {
                int szam = int.Parse(gomb.Name);
                index index = number_to_index(szam);
                int x = index.x;
                int y = index.y;
                Image old_img = gombok[lyukx, lyuky].BackgroundImage;
                gombok[lyukx, lyuky].Visible = true;
                gombok[lyukx, lyuky].BackgroundImage = gombok[x, y].BackgroundImage;
                gombok[x, y].Visible = false;
                gombok[x, y].BackgroundImage = old_img;
                //gombok[x, y].Text = gombok[x, y].BackgroundImage.Tag.ToString();
                lyukx = x;
                lyuky = y;
            }
        }
        private index number_to_index(int number)
        {
            int x = 0;
            while (number - max > 0)
            {
                number -= max;
                x++;
            }
            int y = 0;
            if (number != 0)
            {
                y = (number - 1);
            }
            else
            {
                y = number;
            }
            index index = new index();
            index.x = x;
            index.y = y;
            return index;
        }
        private bool win()
        {
            bool one_fault = false;
            for (int i = 0; i < max; i++)
            {
                for (int j = 0; j < maxn-1; j++)
                {
                    //gombok[i, j].Text = sorszam.ToString();
                    gombok[i, j].Text = ": " + gombok[i, j].BackgroundImage.Tag.ToString();
                    if ((int.Parse(gombok[i, j + 1].BackgroundImage.Tag.ToString())) == (int.Parse(gombok[i, j].BackgroundImage.Tag.ToString())+1))
                    {
                    }
                    else
                    {
                        if (!one_fault)
                        {
                            one_fault = true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            return false;
        }
    }
    public class index
    {
        public int x { get; set; }
        public int y { get; set; }
    }
}