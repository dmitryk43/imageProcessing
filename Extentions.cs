using System;
using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using System.Drawing.Imaging;
//using System.Runtime.InteropServices;
//using Emgu.CV;
//using Emgu.CV.Structure;
//using Emgu.CV.Util;
//using Emgu.CV.CvEnum;


namespace ImageProcessing
{
    public static partial class Extentions //статический класс в котором реализованы почти все функции обработки изображения, в том числе в других файлах кода
    {

        #region MEDIAN
        public static byte Median(byte[] array)
        {
            int len = array.Length; //количество элементов массива для нахождения медианы
            int mid_idx = len / 2; //индекс ср. знач.
            var sorted = new byte[len];
            Array.Copy(array, sorted, len);
            Array.Sort(sorted);
            byte median;

            if ((len % 2) == 0)
                median = (byte)((sorted.ElementAt(mid_idx) + sorted.ElementAt(mid_idx - 1)) / 2);
            else
                median = sorted.ElementAt(mid_idx);
            return median;
        }
        
        /// <summary>
        /// Входные данные - исходное изображение в формате HSI. Выходные данные - отфильтрованное изображение HSI с нулевой S компонентой (за ненадобностью)
        /// </summary>
        /// <param name="image"></param>
        /// <param name="windowSize"></param>
        /// <returns></returns>
        public static HSIimage MedianFilter(this HSIimage image, in int windowSize)
        {
            if (windowSize == 0)
                return image;
            HSIimage result; //= new Image<Hsv, byte>(,) новое изображение
            int yMin, xMin, yMax, xMax; //переменные, задающие минимальные и максимальные значения итераторов цикла с поправкой на размер окна
                                        //byte[] setForFilter; //массив значений компоненты пикселей окна для фильтрации конкретной компоненты каждого отдельного пикселя

            yMin = xMin = windowSize;
            yMax = image.Height - windowSize - 1;
            xMax = image.Width - windowSize - 1;
            //setForFilter = new byte[9];
            byte[] setForFilter = new byte[(int)Math.Pow((1 + windowSize * 2), 2)]; //размер массива сета значений вычисляется как квадрат количества пикселей в строке/столбце
            result = new HSIimage(image.Width - windowSize * 2, image.Height - windowSize * 2);

            for (int y = yMin; y <= yMax; ++y) //начинаем отчет с начала и до конца массива, не учитывая крайние 1 или 2 row&collumn 
            {
                for (int x = xMin; x <= xMax; ++x) //в зависимости от степени фильтра
                {
                    result.Data[x - windowSize, y - windowSize].Saturation = image.Data[x, y].Saturation;
                    for (int p = 0; p < 2; p++) //перепрыгиваем S-компоненту
                    {
                        int setindex = 0;
                        for (int y2 = y - windowSize; y2 <= y + windowSize; y2++)
                        {
                            for (int x2 = x - windowSize; x2 <= x + windowSize; ++x2)
                            {
                                if (p == 0) setForFilter[setindex] = image.Data[x2, y2].Hue;
                                else setForFilter[setindex] = image.Data[x2, y2].Intensity;
                                setindex++;
                            }
                        }
                        if (p == 0)
                            result.Data[x - windowSize, y - windowSize].Hue = Extentions.Median(setForFilter);//pixel[y, x, p];
                        else
                            result.Data[x - windowSize, y - windowSize].Intensity = Extentions.Median(setForFilter);//pixel[y, x, p];

                    }
                }
            }
            return result;
        }
        #endregion

        #region COUNTOURS
        public static HSIimage GetContours(this HSIimage image, in bool isOnlyHue)
        {
            HSIimage output = new HSIimage(image.Width - 1, image.Height - 1);
            int h = image.Height;
            int w = image.Width;
            //int HueShift = 4; //сдвиг определен на 4 бита для тона, так как одного старшего бита недостаточно для сегментирования значений,
                              //определенных на "тоновой окружности" как значения от нуля градусов до 360. Минимальное значение разницы - для 12 тонов - 21 -
                              // укладывается в 5 бит, следовательно его старший - 5-ый бит должен входить в сравнение
            int pixelIntens, previousIntensY, previousIntensX; //, nextIntensX, nextIntensY;
            //byte[] Intensities = new byte[5];
            int pixelHue, previousHueY, previousHueX; //, nextHueY, nextHueX;
            //int[] Hues = new int[5];
            //int pixelSat, previousSatY, previousSatX;

            int resultI, resultH; //, resultS;

            for (int y = 1; y < h; y++)
            {
                for (int x = 1; x < w; x++)
                {
                    pixelHue = image.Data[x, y].Hue; //pixelHue >>= HueShift;
                    previousHueY = image.Data[x, y - 1].Hue; //previousHueY >>= HueShift;
                    previousHueX = image.Data[x - 1, y].Hue; //previousHueX >>= HueShift;
                    //nextHueY = image.Data[x, y + 1].Hue; nextHueY >>= valueShift;
                    //nextHueX = image.Data[x + 1, y].Hue; nextHueX >>= valueShift;
                    resultH = (pixelHue ^ previousHueY) | (pixelHue ^ previousHueX); // | (pixelHue ^ nextHueY) | (pixelHue ^ nextHueX);


                    //pixelSat = image.Data[x, y].Saturation; pixelSat >>= valueShift; //<<= 1; pixelIntens>>= 6;
                    //previousSatY = image.Data[x, y - 1].Saturation; previousSatY >>= valueShift; //<<= 1; previousIntensY >>= 6;
                    //previousSatX = image.Data[x - 1, y].Saturation; previousSatX >>= valueShift; //<<= 1; previousIntensX >>= 6;

                    if (!isOnlyHue) //если нужно определять контура по яркости
                    {
                        pixelIntens = image.Data[x, y].Intensity; //pixelIntens >>= HueShift + 1; //интенсивность сдвигаем на 5 бит, так как для нее минимальная разница
                                                                                                //значений - 32 - укладывается в 3 старших бита
                        previousIntensY = image.Data[x, y - 1].Intensity; //previousIntensY >>= HueShift + 1; //<<= 1; previousIntensY >>= 6;
                        previousIntensX = image.Data[x - 1, y].Intensity; //previousIntensX >>= HueShift + 1; //<<= 1; previousIntensX >>= 6;
                        //nextIntensY = image.Data[x, y + 1].Intensity; nextIntensY >>= 7; //<<= 1; nextIntensY >>= 6;
                        //nextIntensX = image.Data[x + 1, y].Intensity; nextIntensX >>= 7; //<<= 1; nextIntensX >>= 6;
                        resultI = (pixelIntens ^ previousIntensY) | (pixelIntens ^ previousIntensX); // | (pixelIntens ^ nextIntensY) | (pixelIntens ^ nextIntensX);
                    }
                    else resultI = 0;
                    //resultS = (pixelSat ^ previousSatY) | (pixelSat ^ previousSatY);
                    if ((resultH | resultI) > 0) //  | resultI | resultS
                        output.Data[x - 1, y - 1].Intensity = 255;

                }
            }

            for (int x0 = 0, xLast = output.Width - 1, y = 0; y < output.Height; ++y)//Края изображения обозначаем контурами, чтобы получить все области замкнутыми
            {
                output.Data[x0, y].Intensity = 255;
                output.Data[xLast, y].Intensity = 255;
            }
            for (int y0 = 0, yLast = output.Height - 1, x = 0; x < output.Width; ++x)//Края изображения обозначаем контурами, чтобы получить все области замкнутыми
            {
                output.Data[x, y0].Intensity = 255;
                output.Data[x, yLast].Intensity = 255;
            }
            return output;
        }
        #endregion

        #region RAREFACTION

        public static void Preprocessing(this HSIimage image, byte HueCoef, byte IntensCoef) // прорежаем изображение кэф 42 - до 6 значений цветов, кэф 84 - до 3 цветов 
        {
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    if (image.Data[x, y].Hue % HueCoef > HueCoef / 2) //если остаток от деления на коэффициент больше половины коэффициента
                        image.Data[x, y].Hue = (byte)((image.Data[x, y].Hue + HueCoef) / HueCoef * HueCoef); //прибавляем его значение к значению тона, так как
                                                                                                    //такой тон нужно округлять в большую сторону
                    else //если меньше или равен половине коэффициента
                        image.Data[x, y].Hue = (byte)(image.Data[x, y].Hue / HueCoef * HueCoef); //делим и умножаем на коэффициент, в результате
                                                                                           //целочисленного деления получаем округление в меньшую сторону
                    if (image.Data[x, y].Hue >= byte.MaxValue / HueCoef * HueCoef) //если в рез-те округления тон получается около 360 градусов, 
                        image.Data[x, y].Hue = 0; //приравниваем его к нулю

                    
                    //int intensityCoef = (byte)(1.5 * HueCoef); //для интенсити коэф умножаем в полтора раза, получаем меньше градаций яркости
                    
                    if (image.Data[x, y].Intensity % IntensCoef > IntensCoef / 2) //если остаток от деления на коэффициент больше половины коэффициента
                        image.Data[x, y].Intensity = (byte)((image.Data[x, y].Intensity + IntensCoef) / IntensCoef * IntensCoef); //прибавляем его значение к значению тона, так как
                                                                                                    //такой тон нужно округлять в большую сторону
                    else //если меньше или равен половине коэффициента
                        image.Data[x, y].Intensity = (byte)(image.Data[x, y].Intensity / IntensCoef * IntensCoef);

                    //image.Data[x, y].Hue = (byte)(image.Data[x, y].Hue / coef * coef);
                    //image.Data[x, y].Saturation = (byte)(image.Data[x, y].Saturation / coef * coef);
                    //image.Data[x, y].Intensity = (byte)(image.Data[x, y].Intensity / intensityCoef * intensityCoef);
                }
            }
        }

        #endregion


        #region FINAL CLASSIFICATION

        public static List<HSISegment> GetFinalResult(List<HSISegment> restoredSegments, List<HSISegment> skeletSegments, List <int> widthsOfSegments) //, int width, int height
        {
            //HSIimage result = new HSIimage(width, height);
            List<HSISegment> result = new List<HSISegment>();
            

            for (int i = 0; i < restoredSegments.Count; i++)
            {
                if (classification(restoredSegments[i], skeletSegments[i], widthsOfSegments[i]))
                    result.Add(restoredSegments[i]);

            }

            return result;
        }

        public static bool classification(HSISegment segment, HSISegment skelet, int width) //ДОДЕЛАТЬ EI И ДОРАБОТАТЬ КЛАССИФИКАТОР, может добавить возможность настраиваемой классификации по цвету
        {
            double SI,  LI; //EI,

            SI = ((double)skelet.pixels.Count * 2 + width * 2) / Math.Sqrt(segment.pixels.Count);

            LI = (double)skelet.pixels.Count / width;

            if ((SI > 1.2) && (LI > 4)) //&& (SI < 2)
                return true;

            //EI = 


            return false;
        }

        #endregion

    }

}


/*public static int GetPerimeter(HSISegment segment, int width, int height)
        {
            int result;
            HSIimage image = 

            return result;
        }*/



/*
        #region SEGMENTS

        public static void Crop(ref HSIimage image, in int filterWindow)
        {
            HSIimage cropped = new HSIimage(image.Width - (filterWindow * 2 + 1), image.Height - (filterWindow * 2 + 1));
            int w = cropped.Width; //новое изображение обрезается на удвоенный размер диаметра окна пикселей плюс необработанные нулевые строку и столбец
            int h = cropped.Height; //из функции нахождения контуров
            for (int y = 0; y < h - 1; y++) //оставляем в обрезанном изобр. только пиксели, не входящие в края, не обработанные медианным фильтром 
            {
                for (int x = 0; x < w - 1; x++)
                {
                    cropped.Data[x, y] = image.Data[x + filterWindow + 1, y + filterWindow + 1];
                }
            }

            image = cropped; //присваиваем в имейдж обрезанную версию

            //NO NEED, already done
            *//*HSIimage croppedContours = new HSIimage(contours.Width - 1, contours.Height - 1); // 
            for (int y = 0; y < croppedContours.Height; y++)
            {
                for (int x = 0; x < croppedContours.Width; x++)
                {
                    croppedContours.Data[x, y] = contours.Data[x + 1, y + 1];
                }
            }*//*
        }

        public static byte[,] GetHSIintensities(HSIimage image)
        {
            byte[,] intens = new byte[image.Width, image.Height];
            int h = image.Height;
            int w = image.Width;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    intens[x, y] = image.Data[x, y].Intensity;
                }
            }
            return intens;
        }

        public static void GetSegments(this List<HSISegment> segments, byte[,] contours, int width, int height) //передавать не оригинальные контура, а их копию
        {
            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    if (contours[x, y] == 0) //если пиксель черный
                    {
                        HSISegment segment = new HSISegment(contours, x, y); //создаем сегмент, содержащий этот пиксель (и все остальные)
                        segments.Add(segment);
                    }

                }
            }
        }

        public static HSIimage DrawSegments(this List<HSISegment> segments, int width, int height)
        {
            HSIimage segmAll = new HSIimage(width, height);

            foreach (var segment in segments)
            {
                segment.DrawSegment(segmAll);
            }

            return segmAll;
        }

        #endregion*/





/*public static void ShadowRemove(this HSIimage image)
        {
            
        }*/



/*public static void GetSegments(this List<HSISegment> segments, in HSIimage image, HSIimage contours) //передавать не оригинальные контура, а их копию
{
   for (int y = 1; y < image.Height - 1; y++)
   {
       for (int x = 1; x < image.Width - 1; x++)
       {
           if (contours.Data[x, y].Intensity == 0) //если пиксель черный
           {
               HSISegment segment = new HSISegment(image, contours, x, y); //создаем сегмент, содержащий этот пиксель (и все остальные)
               segments.Add(segment);
           }

       }
   }
}*/

/*public static void GetSegments(this List<HSISegment> segments, HSIimage image, HSIimage contours)
{
    //static int segmCounter = 0;
    for (int y = 1; y < image.Height; y++) //начинаем с 1 так как контура детектированы только с 1 ряда и столбца 
    {

        for (int x = 1; x < image.Width; x++) //проходим по строке до тех пор, пока
        {

            if (contours.Data[x, y].Intensity == 0) //если пиксель контуров черный
            {
                HSIPixel pixel = new HSIPixel(image.Data[x, y]); //создаем пиксель 
                HSIRow row = new HSIRow(pixel);
                HSISegment segment = new HSISegment(row);
                segments.Add(segment);//записываем пиксель в строку, сохраняем ее в сегмент, и добавляем сегмент в лист
                //new HSISegment(new HSIRow(new HSIPixel(image.Data[x, y])))
                contours.Data[x, y].Intensity = 255; //присваиваем пикселю в контурном изображении белый цвет, чтобы исключить его из дальнейших изысканий

                int xx = x + 1; //вводим новый счетчик для отчета по строке
                while(contours.Data[xx, y].Intensity == 0) //пока мы идем по линии черных пикселей
                {
                    pixel = new HSIPixel(image.Data[xx, y]); 
                    row.AddPixel(pixel); //добавляем их в строку
                    contours.Data[xx, y].Intensity = 255;
                    xx++;
                }

                //segments[segments.Count - 1].AddRow();

            }

        }
    }
}*/



/*public static Image<Gray, byte> GetContoursOld(Emgu.CV.Image<Hsv, byte> image)
{
    Image<Gray, byte> output = new Image<Gray, byte>(image.Width, image.Height);
    int valueShift = 6; //сдвигаем для сравнения 2-х старших битов на 6, для сравнения трех - на 5
    //int bitMask = 3; //для нахождения значения двух битов после сдвига
    byte pixelValue, previousY, previousX, nextX, nextY; //исследуемый пиксель, предыдущий по осям Х и У

    byte pixelHue, previousHueY, previousHueX, nextHueX, nextHueY;

    int resultL, resultH;
    for (int y = 0; y < image.Height; y++)
    {
        for (int x = 0; x < image.Width; x++)
        {
            //Собственно алгоритм, учитывающий верхний край, остальные по аналогии
            */

/*if (x != 0) //если пиксель не из верхнего края
            {
                previousX = image.Data[y, x - 1, 2]; previousX >>= valueShift; //присваиваем значение пикселя выше переменной привьюсХ
            }  //previousX &= 3;
            else previousX = pixelValue; //иначе присваиваем ей значение исследуемого пикселя, чтобы получить нуль при XOR
            */

/*
            pixelValue = image.Data[y, x, 2];
            pixelValue <<= 1;
            pixelValue >>= valueShift; //pixelValue &= 1;

            previousY = Neighbour(image, false, -1, false, x, y - 1, pixelValue);
            previousX = Neighbour(image, false, -1, true, x - 1, y, pixelValue);
            nextY = Neighbour(image, false, image.Height, false, x, y + 1, pixelValue);
            nextX = Neighbour(image, false, image.Width, true, x + 1, y, pixelValue);


            pixelHue = (byte)((double)image.Data[y, x, 0] * 255 / 180);
            pixelHue >>= 7; //сдвигаем на 7 битов

            previousHueY = Neighbour(image, true, -1, false, x, y - 1,  pixelValue);
            previousHueX = Neighbour(image, true, -1, true, x - 1, y, pixelValue);
            nextHueY = Neighbour(image, true, image.Height, false, x, y + 1, pixelValue);
            nextHueX = Neighbour(image, true, image.Width, true, x + 1, y, pixelValue);

            //previousY = image.Data[x, y - 1, 2];    previousY >>= valueShift; //previousY &= 3;
            //nextX = image.Data[x + 1, y, 2];        previousY >>= valueShift;
            //nextY = image.Data[x, y + 1, 2];        previousY >>= valueShift;

            resultL = (pixelValue ^ previousX) | (pixelValue ^ previousY) | (pixelValue ^ nextX) | (pixelValue ^ nextY);
            if (resultL == 1)
            {
                output.Data[y, x, 0] = 255;
            }
            else
            {

            }



        }
    }
    return output;

}


/// <summary>
/// 
/// </summary>
/// <param name="image">Исходное изображение</param>
/// <param name="isHue">true if Hue, false if lightness</param>
/// <param name="edgeFactor">Значение, которое обозначает край изображения, запрещенный для обработки(0, image.Height, etc.)</param>
/// <param name="isX">Переменная нужна для указания функции, параметр по какой из осей нуждается в проверке на выход за пределы массива </param>
/// <param name="x">координата по оси х сравниваемого с изначальным пикселя(равна х оригинального пикселя или больше/меньше на 1)</param>
/// <param name="y">координата по оси у</param>
/// <param name="valueShift">спараметр для побитового сдвига значения пикселя </param>
/// <param name="pixelValue"></param>
/// <returns></returns>
public static byte NeighbourOld(in Emgu.CV.Image<Hsv, byte> image, bool isHue, int edgeFactor, bool isX, int x, int y, int pixelValue) //int valueShift,
{
    int result, axisValForOutOfRangeCheck, componenta, valueShift;

    if (isX) axisValForOutOfRangeCheck = x;
    else axisValForOutOfRangeCheck = y;

    if (isHue) 
    { 
        componenta = 0;
        valueShift = 7;
    }
    else
    {
        componenta = 2;
        valueShift = 6;
    }

    if (axisValForOutOfRangeCheck != edgeFactor) //если пиксель не из верхнего края
    {
        result = image.Data[y, x, componenta]; //присваиваем значение пикселя выше переменной привьюсХ
        if (!isHue) //если считаем яркость
        { 
            result <<= 1; 
            //ПЕРЕВЕСТИ в И из В

        }
        else
        {
            result = (byte)((double)result * 255 / 180);
        }

        result >>= valueShift; 
    }  
    else result = pixelValue; //иначе присваиваем ей значение исследуемого пикселя, чтобы получить нуль при XOR
    return (byte)result;
}
*/






/*if (HighWindow)
{
    //setForFilter = new byte[25];

    for (int y2 = y - 2; y2 <= y + 2; y2++)
    {
        for (int x2 = x - 2; x2 <= x + 2; ++x2)
        {
            setForFilter[setindex] = image.Data[y2, x2, p];
            setindex++;
        }
    }
}
else
{
    //setForFilter = new byte[9];
    for (int y2 = y - 1; y2 <= y + 1; y2++)
    {
        for (int x2 = x - 1; x2 <= x + 1; ++x2)
        {
            setForFilter[setindex] = image.Data[y2, x2, p];
            setindex++;
        }
    }
    *//*setForFilter = {    image.Data[y-1, x-1, p], image.Data[y-1, x, p], image.Data[y-1, x+1, p],
                        image.Data[y, x-1, p],   image.Data[y, x, p],   image.Data[y, x+1, p],
                        image.Data[y+1, x-1, p], image.Data[y-1, x, p], image.Data[y-1, x+1, p] };*//*
}*/

/*public static HSVPixel[][] MedianFilter()
        {

        }*/

/*public class HSVPixel
    {
        public double Hue { get; set; }
        public double Saturation { get; set; }
        public double Value { get; set; }
        //public int BlabComponent { get; set; }

        public HSVPixel(double hue, double satur, double val) //int b
        {
            Hue = hue;
            Saturation = satur;
            Value = val;
            //BlabComponent = b;
        }


    }*/

/*#region GET HSV IMAGE
        public static HSVPixel[][] GetHSVImage(Bitmap image) //получаем HSV-изображение, как двумерный массив
        {
            HSVPixel[][] NewHSVImage = new HSVPixel[image.Height][];

            for (int y = 0; y < image.Height; y++)
            {
                NewHSVImage[y] = new HSVPixel[image.Width];
                for (int x = 0; x < image.Width; x++)
                {
                    //pixels.Add(new Pixel(in bitmap, x, y));
                    Color temp = image.GetPixel(x, y); //извлекаем пиксель во временную переменную
                    NewHSVImage[y][x] = new HSVPixel(temp.GetHue(), temp.GetSaturation(), temp.GetBrightness()); //Сохраняем ХСВ компоненты в ХСВИМЕЙДЖ
                     //= hsvtemp;
                }
            }

            //Image<Hsv, byte> emguImage = new Image<Hsv, byte>() 

            return NewHSVImage;
        }
        #endregion
*/

/*public static void getHSVB(Color pixel)
        {
            //Color pixel = Color.
            
            Hsv hsv = new Hsv(pixel.GetHue(), pixel.GetSaturation(), pixel.GetBrightness());
            //Lab lab = new Lab(pixel.)
        }*/

/*public static Bitmap ShadowProcessing()
        {
            
        }*/