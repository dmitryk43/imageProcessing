using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace ImageProcessing
{
    public static partial class Extentions
    {
        #region BINARIZATION

        public static HSIimage Binarization(in HSIimage image)  //OtsuThreshold()
        {
            HSIimage result = new HSIimage(image.Width, image.Height);
            int h = image.Height;
            int w = image.Width;
            byte treshold = OtsuTreshhold(image);
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    result.Data[x, y].Hue = image.Data[x, y].Hue;
                    result.Data[x, y].Saturation = image.Data[x, y].Saturation;

                    if (image.Data[x, y].Intensity < treshold)
                        result.Data[x, y].Intensity = 0;
                    else
                        result.Data[x, y].Intensity = 255;
                }
            }

            return result;
        }

        public static byte OtsuTreshhold(in HSIimage image)
        {
            uint[] histogram = GetHistogram(image);

            int levels = byte.MaxValue + 1; //number of gray-scale levels - 256
            double numOfPixels = (double)(image.Height * image.Width);

            /*double[] probabilities = new double[levels]; //probabilities of every gray level

            for (int i = 0; i < levels; i++)
            {
                probabilities[i] = (double)histogram[i] / numOfPixels; // вероятность конкретной градации серого во всем изображении от 0 до 1
            } //иными словами, частота нахождения конкретной градации серого
*/

            uint all_intensity_sum = 0;
            for (int i = 0; i < levels; i++)
            {
                all_intensity_sum += (uint)(histogram[i] * i); //256 значений интенсивности помноженных на количества пикселей в сумме дают общую сумму интенсивности
            }

            int best_thresh = 0;
            double best_sigma = 0.0;

            double zeroClassPixelCount = 0;
            uint zeroClassIntensSum = 0;

            for (int threshold = 1; threshold < levels - 1; threshold++) //for every gray level добавить трешхолд меньше левелс минус 1
            {
                zeroClassPixelCount += histogram[threshold];
                double firstClassPixelCount = numOfPixels - zeroClassPixelCount;
                zeroClassIntensSum += (uint)threshold * histogram[threshold]; //(byte)
                double zeroClassOmega = (double)(zeroClassPixelCount / numOfPixels); //W_0 - probability (вероятность) нулевого класса
                double firstClassOmega = 1 - zeroClassOmega; //W_1
                double zeroClassMean = (zeroClassPixelCount == 0) ? 0 : zeroClassIntensSum / zeroClassPixelCount;
                double firstClassMean = (firstClassPixelCount == 0) ? 0 : (all_intensity_sum - zeroClassIntensSum) / firstClassPixelCount;
                double meanDelta = firstClassMean - zeroClassMean;

                double sigma = firstClassOmega * zeroClassOmega * meanDelta * meanDelta;

                if (sigma > best_sigma)
                {
                    best_sigma = sigma;
                    best_thresh = threshold;
                }
            }
            return (byte)best_thresh;
        }

        public static uint[] GetHistogram(in HSIimage image) //get histogram
        {
            int w = image.Width, h = image.Height;
            int size = byte.MaxValue + 1; //histogram size - 256 values
            uint[] hist = new uint[size]; //initialize it

            for (int y = 0; y < h; y++) //for each pixel 
            {
                for (int x = 0; x < w; x++)
                {
                    hist[image.Data[x, y].Intensity]++; //increment a counter of found intensity
                }
            }
            return hist;
        }

        public static void RemoveBackground(this List<HSISegment> segments, HSIimage hsibinary)
        {
            int lowSizeArgument = hsibinary.Width * hsibinary.Height / 100; //аргумент, по которому будут удаляться маленькие (занимающие менее 2% площади) сегменты
            //foreach (var segment in segments) //для каждого сегмента
            for (int s = 0; s < segments.Count; ++s)
            {
                if (segments[s].pixels.Count <= lowSizeArgument)
                {
                    segments.RemoveAt(s); //удаляем сегмент если количество его пикселей меньше 5
                    continue; //переходим к след. сегменту
                }
                byte[] allIntensities = new byte[segments[s].pixels.Count]; //создаем массив значений интенсивностей бинарного изображения
                int i = 0;
                foreach (var pixel in segments[s].pixels) //для каждого пикселя (координат х,у пикселя)
                {
                    allIntensities[i] = hsibinary.Data[pixel.X, pixel.Y].Intensity; //сохраняем интенсивность в массив, чтобы далее найти медиану
                                                                                      //их значений, по значению которой принимается решение об
                                                                                      //удалении/оставлении сегмента - медиана 255 - foreground, else delete
                    ++i;
                }
                if (Extentions.Median(allIntensities) == 0)
                {
                    segments.RemoveAt(s); //удаляем сегмент если медиана значений его пикселей равна нулю
                    //continue;
                }

            }
        }
        #endregion
    }
}
