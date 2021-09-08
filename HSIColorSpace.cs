using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
using System.Drawing.Imaging;
//using System.Runtime.InteropServices;
//using Emgu.CV;
//using Emgu.CV.Structure;
//using Emgu.CV.Util;
//using Emgu.CV.CvEnum;


namespace ImageProcessing
{
    public class HSIimage
    {
        #region Properties
        public HSIPixel[,] Data { get; set; } //изображение в представлении HSI является двумерным массивом HSI пикселей с соотв. знач. width & height
        public int Width { get; set; }
        public int Height { get; set; }
        #endregion

        #region CTORS
        public HSIimage(int width, int height) //создание нового изображения по переданным к-тору длине и ширине
        {
            Width = width; Height = height;
            Data = new HSIPixel[Width, Height]; //сoздаем объект класса по осям y, x
            
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Data[x, y] = new HSIPixel(0, 0, 0);
                }
            }

        }

        public HSIimage(HSIimage image) //копирование одного изображения в другое
        {
            Width = image.Width; Height = image.Height;
            Data = new HSIPixel[Width, Height]; //сoздаем объект класса по осям y, x
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Data[x, y] = new HSIPixel(image.Data[x, y]);
                }
            }
        }

        public HSIimage(Bitmap image) //создание hsi from Bitmap - представления изображения в ARGB
        {
            Width = image.Width; Height = image.Height;
            Data = new HSIPixel[Width, Height]; //сoздаем объект класса по осям y, x
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Data[x, y] = new HSIPixel(image.GetPixel(x, y));
                }
            }
        }
        #endregion

        #region METHODS

        public Bitmap ToBitmapGrayScale()
        {
            Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    byte intens = Data[x, y].Intensity;
                    bitmap.SetPixel(x, y, Color.FromArgb(intens, intens, intens));
                }
            }
            return bitmap;
        }

        public Bitmap ToBitmapBlackAndWhite() //черно-белое изображение 
        {
            Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (Data[x, y].Intensity == 255) 
                        bitmap.SetPixel(x, y, Color.White);
                }
            }
            return bitmap;
        }

        public Bitmap ToBitmap() //ДОДЕЛАТЬ ФОРМУЛЫ
        {
            Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format24bppRgb); //Format32bppRgb
            byte r, g, b;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    //if (Data[x, y].Intensity > 0) bitmap.SetPixel(x, y, Color.White);
                    if (Data[x, y].Hue < byte.MaxValue / 3)
                    {
                        b = (byte)((byte.MaxValue - Data[x, y].Saturation) / 3);
                        r = (byte)((byte.MaxValue * (1 + ((double)Data[x, y].Saturation / byte.MaxValue) * Math.Cos(Data[x, y].Hue * Math.PI / 180) / Math.Cos((60 - Data[x, y].Hue) * Math.PI / 180))) / 3);
                        g = (byte)(byte.MaxValue - r - b);
                        bitmap.SetPixel(x, y, Color.FromArgb(r, g, b));
                    }
                    else if (Data[x, y].Hue >= byte.MaxValue / 3 && Data[x, y].Hue < byte.MaxValue * 2 / 3)
                    {
                        r = (byte)((byte.MaxValue - Data[x, y].Saturation) / 3);
                        g = (byte)((byte.MaxValue * (1 + ((double)Data[x, y].Saturation / byte.MaxValue) * Math.Cos(Data[x, y].Hue * Math.PI / 180) / Math.Cos((60 - Data[x, y].Hue) * Math.PI / 180))) / 3);
                        b = (byte)(byte.MaxValue - r - g);
                        bitmap.SetPixel(x, y, Color.FromArgb(r, g, b));
                    }
                    else if (Data[x, y].Hue < byte.MaxValue && Data[x, y].Hue >= byte.MaxValue * 2 / 3)
                    {
                        g = (byte)((byte.MaxValue - Data[x, y].Saturation) / 3);
                        b = (byte)((byte.MaxValue * (1 + ((double)Data[x, y].Saturation / byte.MaxValue) * Math.Cos(Data[x, y].Hue * Math.PI / 180) / Math.Cos((60 - Data[x, y].Hue) * Math.PI / 180))) / 3);
                        r = (byte)(byte.MaxValue - g - b);
                        bitmap.SetPixel(x, y, Color.FromArgb(r, g, b));
                    }

                    //bitmap.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
            return bitmap;

        }
        #endregion
    }

    public class HSIPixel
    {
        #region Properties
        public byte Hue { get; set; }
        public byte Saturation { get; set; }
        public byte Intensity { get; set; }
        #endregion

        #region CTORS
        public HSIPixel(byte hue, byte saturation, byte intensity) //ctor from direct values
        {
            Hue = hue; Saturation = saturation; Intensity = intensity;
        }

        public HSIPixel(HSIPixel pixel) //ctor for copy
        {
            Hue = pixel.Hue;
            Saturation = pixel.Saturation;
            Intensity = pixel.Intensity;
        }

        public HSIPixel(Color pixel)//ctor from Bitmap.Color
        {
            Hue =(byte)(pixel.GetHue() /360 * 255);
            Saturation = ((pixel.R + pixel.G + pixel.B) == 0)? (byte)0 : (byte)(255 - 3 * (Math.Min(pixel.R, Math.Min(pixel.G, pixel.B)) / (pixel.R + pixel.G + pixel.B) ));
            
            Intensity = (byte)((pixel.R + pixel.G + pixel.B) / 3);
        }
        #endregion
    }

    


    public class PixelCoordinates //добавить к пикселю значения его координат
    {
        public int X { get; set; }
        public int Y { get; set; }

        //public int x, y;

        public PixelCoordinates(int x, int y)
        {
            X = x; Y = y;
        }
    }

}



/*public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

    }*/

/*public class HSISegment //Сегмент содержит в себе лист строк                  //и св-во - высоту сегмента в строках
{
    private List<HSIPixel> segment;

    public List <HSIPixel> Segment 
    { 
        get { return segment; } //вместо сеттера используем функцию добавления строки в лист
    }


    public void AddPixel(HSIPixel pixel)
    {
        segment.Add(pixel);
    }
    *//*private int height; //static
    public int Height //static
    {
        get { return height; }
        set { height = value; }
    }*//*

    public HSISegment(HSIPixel pixel)
    {
        segment = new List<HSIPixel> { pixel };
        //Segment.Add(row);
    }
}*/

/*public class HSIRow //Строка содержит в себе пиксели и св-во - ширину строки в пикселях. Строкой считается только непрерывный фрагмент строки пикселей
                    //изображения, если сегмент в строке прерывается другим сегментом, строка заканчивается
{
    private List<HSIPixel> row;
    public List<HSIPixel> Row 
    { get { return row; }
    } //property - list of pixels
    *//*private int width;
    public int Width
    {
        get { return width; }
        set { width = value; }
    }*//*

    public HSIRow(HSIPixel pixel)
    {
        Row.Add(pixel);
    }

    public void AddPixel(HSIPixel pixel)
    {
        row.Add(pixel);
    }
}*/
