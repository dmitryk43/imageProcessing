using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace ImageProcessing
{
    public static partial class Extentions
    {
        #region SKELETON

        public static List<HSISegment> GetSkeletons(List<HSISegment> segments, int width, int height) //лист сегментов, длина и ширина исходного
        {                                                                                           //изображения(для построения сегмента на отдельном поле пикселей)
            List<HSISegment> Skeletons = new List<HSISegment>();
            foreach (var segment in segments)
            {
                bool[,] temp = new bool[width, height]; //array of 0 and 1

                foreach (var pixel in segment.pixels)
                {
                    temp[pixel.X, pixel.Y] = true; //создаем на поле единицы на месте пикселей сегмента
                }

                uint numOfPixels;
                do
                {
                    numOfPixels = GetNumOfOnes(temp, segment); //получаем значение количества пикселей УЗНАЕМ КАЖДЫЙ РАЗ КОЛИЧЕСТВО 1 В МАССИВЕ и ИЗМЕНИЛОСЬ ЛИ оно
                    //ВЫЗЫВАЕМ ДВА РАЗА ПОДИТЕРАЦИЮ (ИЗМЕНИТЬ ЕЕ ДЛЯ РАБОТЫ сначала с г3, потом г3')
                    Subiteration(temp, true);
                    Subiteration(temp, false);
                }
                while (GetNumOfOnes(temp, segment) != numOfPixels); //вызываем функции подитераций до тех пор, пока в их результате происходит удаление
                                                                    //пикселей, т.е. изменяется количество пикселей сегмента
                Skeletons.Add(new HSISegment(temp)); //сохраняем битовое поле в сегмент (лист координат)
            }

            return Skeletons;//HSIimage(width, height);
        }

        public static uint GetNumOfOnes(in bool[,] temp, in HSISegment segment) //функция считает количество единиц на бинарной картинке, и вызывается
                                                                                //дважды - до и после подитераций
        { //и так происходит до тех пор, пока количество единиц до и после внутри цикла не станет одинаковым, т.е. мы не достигнем состояния, при котором
          // дальнейшее истончение сегмента невозможно
            uint result = 0;
            foreach (var pixel in segment.pixels)
            {
                int x = pixel.X, y = pixel.Y;
                if (temp[x, y] == true)
                    result++;
            }
            return result;
        }
        
        public static void Subiteration(bool[,] temp, bool isFirstSub) //old name FindSkeleton
        {
            int height = temp.GetLength(1);
            int width = temp.GetLength(0);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (temp[x, y] == false) //Если пиксель == 0, пропускаем его
                        continue;
                    bool[] neighbors = new bool[]
                    {
                            temp[x + 1, y + 1], //x0 = x8
                            temp[x + 1, y ], temp[x + 1, y - 1 ], temp[x, y - 1 ], temp[x - 1, y - 1 ], //x1 - x4
                            temp[x - 1, y ], temp[x - 1, y + 1 ], temp[x , y + 1 ], temp[x + 1, y + 1 ],  //x5 - x8
                            temp[x + 1, y ] //x9 = x1
                    };

                    if (isFirstSub)
                    {
                        if (G1(neighbors) && G2(neighbors) && G3(neighbors))
                            temp[x, y] = false;
                    }
                    else
                    {
                        if (G1(neighbors) && G2(neighbors) && G31(neighbors))
                            temp[x, y] = false;
                    }

                }
            }

        }

        public static bool G1(in bool[] neighbors)
        {
            byte Xh_p = 0;
            for (int i = 1; i <= 4; i++)
            {
                if (neighbors[2 * i - 1] == false && (neighbors[2 * i] == true || neighbors[2 * i + 1] == true))
                    Xh_p++;
            }
            if (Xh_p == 1)
            {
                return true;
            }

            return false;
        }

        public static bool G2(in bool[] neighbors)
        {
            byte n1_p = 0, n2_p = 0;
            for (int i = 1; i <= 4; i++)
            {
                if (neighbors[2 * i - 1] || neighbors[2 * i])
                    n1_p++;
                if (neighbors[2 * i + 1] || neighbors[2 * i])
                    n2_p++;
            }

            if (Math.Min(n1_p, n2_p) >= 2 && Math.Min(n1_p, n2_p) <= 3)
                return true;

            return false;
        }

        public static bool G3(in bool[] neighbors)
        {
            if (((neighbors[2] || neighbors[3] || !neighbors[8]) && (neighbors[1])) == false)
                return true;
            return false;
        }

        public static bool G31(in bool[] neighbors)
        {
            if (((neighbors[6] || neighbors[7] || !neighbors[4]) && (neighbors[5])) == false)
                return true;
            return false;
        }
        #endregion

        #region FIND SEGMENTS OF SKELETON

        public static List<PixelCoordinates> GetrNeighbors(in HSIimage hsiSkeletons, in PixelCoordinates pixel)
        {
            List<PixelCoordinates> neighbors = new List<PixelCoordinates> { };

            for (int y = pixel.Y - 1; y <= pixel.Y + 1; y++)
            {
                for (int x = pixel.X - 1; x <= pixel.X + 1; x++)
                {
                    if ((y == pixel.Y) && (x == pixel.X)) //for original pixel continue
                        continue;
                    if (hsiSkeletons.Data[x, y].Intensity > 0) //если пиксель на картинке не нулевой интенсивности, добавляем его в лист соседей
                        neighbors.Add(new PixelCoordinates(x, y));
                }
            }

            return neighbors;

        }

        public static List<HSISegment> GetSkeletSegments(List<HSISegment> skeletons, HSIimage hsiSkeletons)
        {
            
            List<HSISegment> skeletSegments = new List<HSISegment>(); //создали лист сегментов скелета
            skeletSegments.Add(new HSISegment()); //закинули первый сегмент
            foreach (var skelet in skeletons)
            {
                Stack<PixelCoordinates> skeletonPoints = new Stack<PixelCoordinates>(); //создаем стек точек  скелета
                skeletonPoints.Push(skelet.pixels[0]); //закидываем первую точку скелета в стек
                //bool isBegin = true;
                do
                {
                    //HSISegment segment = new HSISegment(); //сегмент скелета, который сохраняем в skeletSegments
                    var pixel = skeletonPoints.Pop();
                    //int x = pixel.X, y = pixel.Y;
                    List<PixelCoordinates> neighbors = GetrNeighbors(hsiSkeletons, pixel); //получаем соседей - пиксели скелета, и их кол-во

                    switch (neighbors.Count)
                    {
                        case 1: //ОДИН СОСЕД - КОНЦЕВАЯ ТОЧКА. ОТМЕЧАЕМ ПРОСМОТРЕННОЙ И ДОБАВЛЯЕМ СОСЕДА В СТЕК КОГДА КОНЦЕВАЯ ТОЧКА _ ЕДИНСТВЕННАЯ В СЕГМЕНТЕ (ТО ЕСТЬ ОНА ПЕРВАЯ)
                            
                            hsiSkeletons.Data[pixel.X, pixel.Y].Intensity >>= 1; //Сдвиг на 1 бит - метка просмотренности пикселя
                            skeletSegments.Last().pixels.Add(pixel); //сохраняем точку в последний сегмент скелета

                            if (skeletSegments.Last().pixels.Count == 1) //если кол-во пикселей в последнем сегменте скелета = 1, что соотв. началу всего
                            {   //цикла (САМОЙ ПЕРВОЙ ТОЧКЕ СКЕЛЕТА - УНИКАЛЬНАЯ СИТУАЦИЯ), мы просто вызываем следующую итерацию цикла, который будет
                                //работать ИСКЛЮЧИТЕЛЬНО СО ВТОРОЙ точкой сегмента, перед этим
                                skeletonPoints.Push(neighbors[0]); //сохраняем единственного соседа пикселя в стек
                                continue;
                            }  //
                            skeletSegments.Add(new HSISegment()); //в остальных случаях завершаем построение сегмента путем добавления нового сегмента в skeletSegments
                            break;

                        case 2: //2 соседа - ДЕФОЛТНАЯ точка сегмента
                            
                            hsiSkeletons.Data[pixel.X, pixel.Y].Intensity >>= 1; //Сдвиг на 1 бит - метка просмотренности дефолтного пикселя пикселя
                            skeletSegments.Last().pixels.Add(pixel);
                            foreach (var neighbor in neighbors) //для обоих пикселей-соседей
                            {
                                if (hsiSkeletons.Data[neighbor.X, neighbor.Y].Intensity == 255) //просматриваем кто из них с максимальной интенсивностью 
                                    skeletonPoints.Push(neighbor); //это непросмотренный пиксель, добавляем его в стек
                                else if (hsiSkeletons.Data[neighbor.X, neighbor.Y].Intensity == 63) //ЧАСТНЫЙ СЛУЧАЙ - СЕГМЕНТ ПРИХОДИТ В УЖЕ ПРОСМОТРЕННУЮ 
                                    skeletSegments.Add(new HSISegment()); //ТОЧКУ РАЗВЕТВЛЕНИЯ - создаем новый сегмент, т.е. текущий сегмент завершается
                            }
                            break;

                        case 3: case 4: case 5: case 6: case 7: //остальные случаи возможных соседей - точка РАЗВЕТВЛЕНИЯ

                            hsiSkeletons.Data[pixel.X, pixel.Y].Intensity >>= 2; //Сдвиг на 2 бита - метка просмотренности пикселя РАЗВЕТВЛЕНИЯ
                            skeletSegments.Last().pixels.Add(pixel); //закидываем точку в сегмент (она станет последней в нем)
                            skeletSegments.Add(new HSISegment()); //создаем новый сегмент
                            foreach (var neighbor in neighbors) //для всех пикселей-соседей
                            {
                                if (hsiSkeletons.Data[neighbor.X, neighbor.Y].Intensity == 255) //просматриваем кто из них с максимальной интенсивностью 
                                    skeletonPoints.Push(neighbor); // непросмотренный пиксель добавляем в стек
                            }

                            break;
                        
                        default:

                            break;
                    }

                } while (skeletonPoints.Count > 0);
            }

            int delArgument = hsiSkeletons.Height * hsiSkeletons.Width / 50000;

            for (int i = 0; i < skeletSegments.Count; ++i) // (var segm in skeletSegments)
            {
                if (skeletSegments[i].pixels.Count == 0) //(skeletSegments[i].pixels.Count == 0)
                    skeletSegments.RemoveAt(i);

            }
            return skeletSegments;
        }

        #endregion

        #region RESTORE SEGMENTS

        public static bool FoundContour(in int distance, in PixelCoordinates pixel, in HSIimage image)
        {
            //byte[] window = new byte[8];
            for (int y = (pixel.Y - distance); y <= pixel.Y + distance; y += distance) //для 8 пикселей окрестности точки
            {
                for (int x = (pixel.X - distance); x <= pixel.X + distance; x += distance) //ищем хотя бы один пиксель, обозначающий контур
                {
                    if (image.Data[x, y].Intensity > 0) //возвращаем тру, когда его находим
                        return true;
                }
            }

            return false; //иначе фолс
        }

        public static int FindDistance(in PixelCoordinates pixel, in HSIimage image)
        {
            int distance = 1; //расстояние от точки скелета до точки контура сегмента
            do //цикл поиска кратчайшего расстояния работает до тех пор, пока функция обнаружения контуров на контурном изображении не вернет тру
            {
                if (FoundContour(distance, pixel, image))
                    return distance;
                else distance++;
            } while (true);

            //return distance;
        }

        public static List<HSISegment> RestoreSegments(List<HSISegment> skeletSegments, HSIimage hsicontours, out List<int> widthsOfSegments) //на вход поступают сегменты скелетов и изображение контуров
        {
            List<HSISegment> restored = new List<HSISegment>(); //создаем лист сегментов, который будет содержать координаты всех
                                                                //пикселей восстановленных сегментов
            widthsOfSegments = new List<int>();
            foreach (var skeletSegment in skeletSegments)
            {
                if (skeletSegment.pixels.Count == 0)
                    continue;
                restored.Add(new HSISegment()); //добавляем новый сегмент
                uint sumOfDist = 0; //сумма кратчайших дистанций до границы сегмента от каждого пикселя скелета
                foreach (var pixel in skeletSegment.pixels) //для каждого пикселя скелета
                {
                    sumOfDist += (uint) FindDistance(pixel, hsicontours); //находим расстояние
                }

                int meanDistance = (int)(sumOfDist / skeletSegment.pixels.Count); //вычисляем среднее значение расстояний

                if (meanDistance <= 2)
                    meanDistance++;

                foreach (var pixel in skeletSegment.pixels)
                {
                    for (int y = (pixel.Y - meanDistance); y <= pixel.Y + meanDistance; y++) //для всех пикселей окрестности, радиусом которой является среднее
                    {                                                                       //значение расстояний до контура
                        for (int x = (pixel.X - meanDistance); x <= pixel.X + meanDistance; x++)
                        {
                            if (x < 0 || x >= hsicontours.Width || y < 0 || y >= hsicontours.Height)
                                continue;
                                restored.Last().pixels.Add(new PixelCoordinates(x, y)); //сохраняем пиксели в сегмент; так как восстановление сегмента подразумевает
                        } //сохранение формы объекта по скелету и отсутствие разрывов в изображении, используем все пиксели окрестности
                    }
                }
                widthsOfSegments.Add(meanDistance * 2); //добавляем в дист ширин сегментов 
            }

            return restored;
        }

        #endregion
    }
}









/*temp[x + 1, y + 1], //x0 = x8
temp[x + 1, y], temp[x + 1, y - 1], temp[x, y - 1], temp[x - 1, y - 1], //x1 - x4
temp[x - 1, y], temp[x - 1, y + 1], temp[x, y + 1], temp[x + 1, y + 1]
image.Data[pixel.X + distance, pixel.Y + distance].Intensity, image.Data[pixel.X + distance, pixel.Y + distance].Intensity,
image.Data[pixel.X + distance, pixel.Y + distance].Intensity, image.Data[pixel.X + distance, pixel.Y + distance].Intensity,
image.Data[pixel.X + distance, pixel.Y + distance].Intensity, image.Data[pixel.X + distance, pixel.Y + distance].Intensity,
image.Data[pixel.X + distance, pixel.Y + distance].Intensity, image.Data[pixel.X + distance, pixel.Y + distance].Intensity,

 */



















/*public static byte[] GetrNeighborsOld(HSIimage hsiSkeletons, int x, int y)
        {
            byte[] neighbors = new byte[]
            {
                hsiSkeletons.Data[x + 1, y + 1 ].Intensity, //x0 = x8
                hsiSkeletons.Data[x + 1, y ].Intensity, hsiSkeletons.Data[x + 1, y - 1 ].Intensity,
                hsiSkeletons.Data[x, y - 1 ].Intensity, hsiSkeletons.Data[x - 1, y - 1 ].Intensity, //x1 - x4
                hsiSkeletons.Data[x - 1, y ].Intensity, hsiSkeletons.Data[x - 1, y + 1 ].Intensity,
                hsiSkeletons.Data[x , y + 1 ].Intensity, hsiSkeletons.Data[x + 1, y + 1 ].Intensity  //x5 - x8  
            };
            return neighbors;

        }
*/

/*foreach (var pixel in skelet.pixels) //цикл поиска концевых точек
        {
            int x = pixel[0], y = pixel[1];
            byte[] neighbors =
            {
                hsiSkeletons.Data[x + 1, y ].Intensity, hsiSkeletons.Data[x + 1, y - 1 ].Intensity, 
                hsiSkeletons.Data[x, y - 1 ].Intensity, hsiSkeletons.Data[x - 1, y - 1 ].Intensity, //x1 - x4
                    hsiSkeletons.Data[x - 1, y ].Intensity, hsiSkeletons.Data[x - 1, y + 1 ].Intensity, 
                hsiSkeletons.Data[x , y + 1 ].Intensity, hsiSkeletons.Data[x + 1, y + 1 ].Intensity,  //x5 - x8
            };
            byte skeletNeighborsCount = 0;
            foreach (var neighbor in neighbors)
            {
                if (neighbor > 0)
                    skeletNeighborsCount++;
            }
            if (skeletNeighborsCount == 1)
                endPoints.Add(pixel);

        }*/




//List<int[]> segment = new List<int[]>(); //создаем лист для сохранения всех точек отрезка, позднее сохраняем его в HSISegment
//который сохраняем в skeletSegments

//HSISegment segment = new HSISegment(); //сегмент скелета, который сохраняем в skeletSegments

//skeletonPoints.Push(skelet.pixels[0]); //закидываем первую точку скелета в стек
//hsiSkeletons.Data[skelet.pixels[0][0], skelet.pixels[0][1]].Intensity = 127; //отмечаем ее как просмотренную и отмеченную для добавления
//в сегмент скелета

/*byte[] neighbors = new byte[]
{
    hsiSkeletons.Data[x + 1, y ].Intensity, hsiSkeletons.Data[x + 1, y - 1 ].Intensity,
    hsiSkeletons.Data[x, y - 1 ].Intensity, hsiSkeletons.Data[x - 1, y - 1 ].Intensity, //x1 - x4
        hsiSkeletons.Data[x - 1, y ].Intensity, hsiSkeletons.Data[x - 1, y + 1 ].Intensity,
    hsiSkeletons.Data[x , y + 1 ].Intensity, hsiSkeletons.Data[x + 1, y + 1 ].Intensity,  //x5 - x8
};*/





/*
        public static uint GetNumOfOnes(bool[,] temp) //функция считает количество единиц на бинарной картинке, и вызывается дважды - до и после подитераций
        { //и так происходит до тех пор, пока количество единиц до и после внутри цикла не станет одинаковым, т.е. мы не достигнем состояния, при котором
          // дальнейшее истончение сегмента невозможно
            int height = temp.GetLength(1);
            int width = temp.GetLength(0);
            uint result = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (temp[x, y] == true)
                        result++;
                }
            }
            return result;
        }*/
