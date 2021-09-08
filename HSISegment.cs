using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing
{
    public static partial class Extentions
    {
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
            /*HSIimage croppedContours = new HSIimage(contours.Width - 1, contours.Height - 1); // 
            for (int y = 0; y < croppedContours.Height; y++)
            {
                for (int x = 0; x < croppedContours.Width; x++)
                {
                    croppedContours.Data[x, y] = contours.Data[x + 1, y + 1];
                }
            }*/
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

        #endregion
    }


    public class HSISegment //Сегмент содержит в себе лист сегментов  
    {
        public List<PixelCoordinates> pixels; //лист координат пикселей, обозначающий собой сегмент изображения

        #region CTORS
        public HSISegment() { pixels = new List<PixelCoordinates>(); } //default ctor

        public HSISegment(byte[,] contours, int x, int y) //в конструктор сегмента мы попадаем гарантированно с пикселем черного цвета на координатах х, у
        {
            pixels = new List<PixelCoordinates>(); //выделяем память под наш лист пикселей
            Stack<PixelCoordinates> segmentStack  = new Stack<PixelCoordinates> { };//создаем стек пикселей, в который они будут попадать после каждой итерации цикла, пока не кончатся
            PixelCoordinates pixel = new PixelCoordinates(x, y); 
            segmentStack.Push(pixel);//сохраняем пиксель в сегментСтек
            do
            {
                pixel = segmentStack.Pop(); //извлекаем из стека последний добавленный пиксель
                pixels.Add(pixel); //добавляем его в лист
                x = pixel.X; y = pixel.Y;
                contours[x, y] = 255; //присваиваем ему значение белого
                
                int[] xx = new int[] { (x - 1), x, (x + 1), x}; //задаем 4 координаты для поиска соседей: (x - 1, y) (x, y - 1) (x + 1, y) (x, y + 1)
                int[] yy = new int[] { y, (y - 1), y, (y + 1)};
                for (int i = 0; i < 4; i++)
                {
                    if (contours[(xx[i]), (yy[i])] == 0) //если на контурном рисунке соседний пиксель черный, значит 
                        segmentStack.Push(new PixelCoordinates(xx[i], yy[i])); //добавляем его в стек
                }

            } while (segmentStack.Count > 0);//выполняем цикл до тех пор, пока не освободим Стек

        }

        public HSISegment(bool[,] binaryField)
        {
            int height = binaryField.GetLength(1);
            int width = binaryField.GetLength(0);
            pixels = new List<PixelCoordinates>();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (binaryField[x, y] == true)
                        pixels.Add(new PixelCoordinates(x, y));
                }
            }
        }

        #endregion

        public void DrawSegment(HSIimage image)
        {
            foreach (var pixel in pixels)
            {
                image.Data[pixel.X, pixel.Y].Intensity = 255;
            }
        }
    }

}










//contours[x, y] = 255; //присваиваем ему значение белого, чтобы не повторялся при других итерациях
//bool isCoincidenceFound; //Найдено ли совпадение (т.е. хотя бы один черный пиксель среди окружающих)

//isCoincidenceFound = false; //задаем булевой переменной фолс
/*for (int yy = pixel[1] - 1; yy <= pixel[1] + 1; yy++) //для всех 9 пикселей (уже добавленный и 8 окружающих)
{
    for (int xx = pixel[0] - 1; xx <= pixel[0] + 1; xx++)
    {
        if (contours[xx, yy] == 0) //если среди соседей пиксель черный
        {
            //pixel = new int[] { xx, yy };
            segmentStack.Push(new int[] { xx, yy }); //добавляем его в стек
            //contours[xx, yy] = 255; //присваиваем значение белого цвета
            //isCoincidenceFound = true; //даем добро на продолжение цикла
        }
    }
}*/


/*private void AddPixel(byte[,] contours, int x, int y)
{
    if (contours[x, y] == 0)
    {
        int[] pixel = new int[] { x, y };//segment.Add(image.Data[xx, yy]); //добавляем его в сегмент
        //segment.Add(pixel); //добавляем координаты пикселя в сегмент
        contours[x, y] = 255; //делаем его белым
                              //ВМЕСТО 9 ПИКСЕЛЕЙ ПРОХОДИМ ПО 4 ТОЛЬКО (слева, сверху, справа, снизу)
        x--; // x - 1, y
        if (contours[x, y] == 0)
            AddPixel(contours, x, y);
        x++; y++; //x, y + 1
        if (contours[x, y] == 0)
            AddPixel(contours, x, y);
        x++; y--; // x + 1, y
        if (contours[x, y] == 0)
            AddPixel(contours, x, y);
        x--; y--; //x, y - 1
        if (contours[x, y] == 0)
            AddPixel(contours, x, y);
    }
}*/
//CREATE INTENSITIES AS ARRAY, work with them!
//УБРАТЬ КАЖДОЕ ОБРАЩЕНИЕ К СВОЙСТВУ, обращаться к элементу массива
/*public HSISegment(byte[,] contours, int x, int y) //переписываем конструктор //HSIimage image, 
{
    segment = new List<int[]> { };
    //int xx = x, yy = y;
    AddPixel();

    return;

    void AddPixel()
    {
        //int xMin = x - 1, yMin = y - 1, xMax = x + 1, yMax = y + 1; //ВМЕСТО СТРУКТУР СОЗДАДИМ МАССИВ В УПРАВЛЯЕМОЙ КУЧЕ
        //int[] environment = new int[4] { (y - 1), (y + 1), (x - 1), (x + 1) }; //yMin, yMax, xMin, xMax
        if (contours[x, y] == 0)//если пиксель контуров черный
        {
            int[] pixel = new int[] { x, y };//segment.Add(image.Data[xx, yy]); //добавляем его в сегмент
            segment.Add(pixel); //добавляем координаты пикселя в сегмент
               contours[x, y] = 255; //делаем его белым
            //ВМЕСТО 9 ПИКСЕЛЕЙ ПРОХОДИМ ПО 4 ТОЛЬКО (слева, сверху, справа, снизу)
            x--; // x - 1, y
            if (contours[x, y] == 0)
                AddPixel();
            x++; y++; //x, y + 1
            if (contours[x, y] == 0)
                AddPixel();
            x++; y--; // x + 1, y
            if (contours[x, y] == 0)
                AddPixel();
            x--; y--; //x, y - 1
            if (contours[x, y] == 0)
                AddPixel();
            *//*for (y = environment[0]; y <= environment[1]; y++) //для всех пикселей в округе
            {
                for (x = environment[2]; x <= environment[3]; x++)
                {
                    if (contours[x, y] == 0) //если пиксель черный
                    {
                        AddPixel();//рекурсивно вызываем функцией саму себя для последних установленных значений xx, yy
                    }
                }
            }*//*


        }
        //else return; //if white - return
    }
}*/



/*
public class HSISegment //Сегмент содержит в себе лист строк                  //и св-во - высоту сегмента в строках
{
    public List<int[]> segment; //НАХУЙ ВСЕ, ПРОСТО СОЗДАТЬ МАССИВ КООРДИНАТ, СОДЕРЖАЩИЙ ЗНАЧЕНИЯ Х И У
                                //CREATE INTENSITIES AS ARRAY, work with them!
                                //УБРАТЬ НАХУЙ КАЖДОЕ ОБРАЩЕНИЕ К СВОЙСТВУ, обращаться к элементу массива
    public HSISegment(HSIimage contours, int x, int y) //переписываем конструктор //HSIimage image, 
    {
        segment = new List<int[]> { };
        //int xx = x, yy = y;
        AddPixel();

        return;

        void AddPixel()
        {
            //int xMin = x - 1, yMin = y - 1, xMax = x + 1, yMax = y + 1; //ВМЕСТО СТРУКТУР СОЗДАДИМ МАССИВ В УПРАВЛЯЕМОЙ КУЧЕ
            int[] environment = new int[4] { (y - 1), (y + 1), (x - 1), (x + 1) }; //yMin, yMax, xMin, xMax
            if (contours.Data[x, y].Intensity == 0)//если пиксель контуров черный
            {
                int[] pixel = new int[] { x, y };//segment.Add(image.Data[xx, yy]); //добавляем его в сегмент
                segment.Add(pixel); //добавляем координаты пикселя в сегмент
                contours.Data[x, y].Intensity = 255; //делаем его белым

                for (y = environment[0]; y <= environment[1]; y++) //для всех пикселей в округе
                {
                    for (x = environment[2]; x <= environment[3]; x++)
                    {
                        if (contours.Data[x, y].Intensity == 0) //если пиксель черный
                        {
                            AddPixel();//рекурсивно вызываем функцией саму себя для последних установленных значений xx, yy
                        }
                    }
                }
            }
            //else return; //if white - return
        }
    }

}

}


*/


/*
  public class HSISegment //Сегмент содержит в себе лист строк                  //и св-во - высоту сегмента в строках
    {
        public List<HSIPixelCoordinates> segment; //НАХУЙ ВСЕ, ПРОСТО СОЗДАТЬ МАССИВ КООРДИНАТ, СОДЕРЖАЩИЙ ЗНАЧЕНИЯ Х И У
        

        public HSISegment(HSIimage contours, int x, int y) //переписываем конструктор //HSIimage image, 
        {
            segment = new List<HSIPixelCoordinates>();
            //int xx = x, yy = y;
            AddPixel();

            return;

            void AddPixel()
            {
                int xMin = x - 1, yMin = y - 1, xMax = x + 1, yMax = y + 1;
                if (contours.Data[x, y].Intensity == 0)//если пиксель контуров черный
                {
                    //segment.Add(image.Data[xx, yy]); //добавляем его в сегмент
                    segment.Add(new HSIPixelCoordinates(x, y)); //добавляем координаты пикселя в сегмент
                       contours.Data[x, y].Intensity = 255; //делаем его белым

                    for (y = yMin; y <= yMax; y++) //для всех пикселей в округе
                    {
                        for (x = xMin; x <= xMax; x++)
                        {
                            if (contours.Data[x, y].Intensity == 0) //если пиксель черный
                            {
                                AddPixel();//рекурсивно вызываем функцией саму себя для последних установленных значений xx, yy
                            }
                        }
                    }
                }
                //else return; //if white - return
            }
        }

    }
 */






/*class MyClass
{
    public void GetSegments(List<HSISegment> segments, in HSIimage image, HSIimage contours) //передавать не оригинальные контура, а их копию
    {
        for (int y = 1; y < image.Height - 1; y++)
        {
            for (int x = 1; x < image.Width - 1; x++)
            {
                if (contours.Data[x, y].Intensity == 0) //если пиксель черный
                {
                    HSISegment segment = new HSISegment(in image, ref contours, x, y); //создаем сегмент, содержащий этот пиксель (и все остальные)
                    segments.Add(segment);
                }

            }
        }
    }
}*/



/*public List<HSIPixel> Segment
       {
           get { return segment; } //
       }*/


/*public void Huinya(in HSIimage image, ref HSIimage contours, int x, int y) //основной конструктор, вызывается внутри функции гетСегмент
                                                                          //каждый раз при нахождении нового  черного пикселя
{
    segment = new List<HSIPixel>();
    AddPixel(in image, ref contours, x, y);
    //HSIPixel pixel = new HSIPixel(image.Data[x, y]);
    //segment.Add(pixel);
}*/


/*for (int yy = y - 1; yy <= y + 1; yy++)
    {
        for (int xx = x - 1; xx <= x + 1; xx++)
        {
            if (contours.Data[xx, yy].Intensity == 0) //if pixel black
            {
                segment.Add(image.Data[xx, yy]); //добавляем его в сегмент
                contours.Data[xx, yy].Intensity = 255; //делаем белым
                //goto begining of cycle
            }
        }
    }*/



/*public void AddPixel(in HSIimage image, ref HSIimage contours, int x, int y) //вызываем, если х и у больше нуля и меньше длины/ширины - 1
{
    //if (contours.Data[x, y].Intensity == 0)//если пиксель черный
    //{
        segment.Add(image.Data[x, y]); //добавляем его в сегмент
        contours.Data[x, y].Intensity = 255; //делаем белым
    //}
    //else return;

    for (int yy = y - 1; yy <= y + 1; yy++) //для всех пикселей в окружности
    {
        for (int xx = x - 1; xx <= x + 1; xx++)
        {
            if (contours.Data[xx, yy].Intensity == 0) //(!(xx == x && yy == y) && ()) 
            {

                //if ( xx!=0 | xx != (image.Width - 1) | yy != 0 | yy != (image.Height - 1)) //если пиксель не входит в края
                    AddPixel(in image, ref contours, xx, yy);//вызываем рекурсивно функцию снова
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