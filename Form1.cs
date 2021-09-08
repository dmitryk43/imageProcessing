using System;
using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
//using Emgu.CV;
//using Emgu.CV.Structure;
//using Emgu.CV.CvEnum;
using System.Diagnostics;

namespace ImageProcessing
{
    public partial class main : Form
    {
        public main()
        {
            InitializeComponent();
            ofd = new OpenFileDialog();
            sfd = new SaveFileDialog();
            ofd.Filter = filter;
            sfd.Filter = filter;
        }

        OpenFileDialog ofd;
        SaveFileDialog sfd;
        //string image_path;
        string filter = "Image | *.jpg; *.png; *.tiff; *.bmp";
        //public static int width, height;
        Bitmap image;
        //Image<Hsv, byte> HSVImage, HSVprocessed;
        int filterWindow = 1; //размер окна фильтра в пикселях, по умолчанию 1, может задаваться пользователем в меню  
        static byte threeColors = 84, sixColors = 42, TwelveColors = 21;
        byte rareCoef = threeColors; //коэфф. сжатия тона, определяющий, сколько цветов оставлять на исходном изображении, приблизительно соотв. 8-бит  
                                     //представлениям значений угла в 120 (84), 60 (42) и 30 (21) градусов
        static byte lowRange = 126, middleRange = 63, highRange = 32;
        byte exposure = lowRange;
        bool isOnlyHue = true;
        HSIimage hsiimage, //оригинальное изображение в формате HSI
            hsiprocessed, //прореженное изображение (до 3-х, 6-и или 12-и цветов)
            hsifiltered, //прореженное изображение после медианной фильтрации
            hsicontours, //контурное изображение сегментов
            hsibinary, //бинарное изображение (по методу ОТСУ)
            hsiSkeletons,
            hsiSkeletSegments,
            hsiRestoredSegments,
            hsiFinalResult;
        //hsisegments;


        #region MAIN FUNCTION
        private void detectRoadsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (hsiimage == null)
                    throw new Exception("Изображение не было выбрано!");

                var sw = Stopwatch.StartNew(); 

                hsiprocessed = new HSIimage(hsiimage); //создаем копию изображения
                hsiprocessed.Preprocessing(rareCoef, exposure); //подготавливаем копию к дальнейшей обработке, прорежая цвета и яркость

                hsifiltered = hsiprocessed.MedianFilter(filterWindow); //MEDIAN FILTER

                hsicontours = hsifiltered.GetContours(isOnlyHue); //после фильтра и алгоритма обнаружения контуров необходимо избавиться на контурном изображении

                Extentions.Crop(ref hsiimage, filterWindow); //обрезаем оригинальное изображение по краям на размер окна медианного фильтра,
                                                             //а так же первый ряд и строку контуров
                byte[,] intensities = Extentions.GetHSIintensities(hsicontours); //массив значений яркости контуров, к которому будет обращаться функция поиска
                                                                                 //сегментов
                List<HSISegment> segments = new List<HSISegment>();
                segments.GetSegments(intensities, hsiimage.Width, hsiimage.Height);

                hsibinary = Extentions.Binarization(hsiimage);
                
                segments.RemoveBackground(hsibinary); //передаем в функцию бинаризации hsiimage, работаем с интенсити, после нахождения порога и бинаризации
                                                      //изображения пропускаем сегмент через медиану, если медиана сегмента не нулевая
                                                      //(то есть большинство пикселей сегмента белые), записываем сегмент в кандидаты в дороги

                //hsisegments = segments.DrawSegments(hsiimage.Width, hsiimage.Height);//рисуем все сегменты в новом изображении


                List<HSISegment> skeletons = Extentions.GetSkeletons(segments, hsibinary.Width, hsibinary.Height); //скелеты всех сегментов
                hsiSkeletons = skeletons.DrawSegments(hsibinary.Width, hsibinary.Height);

                List<HSISegment> skeletSegments = Extentions.GetSkeletSegments(skeletons, new HSIimage(hsiSkeletons)); //скелеты, разобранные на "косточки"


                hsiSkeletSegments = skeletSegments.DrawSegments(hsibinary.Width, hsibinary.Height);

                //ProcessedPictureBox.Image = skeletons.DrawSegments(hsibinary.Width, hsibinary.Height).ToBitmapGrayScale();
                ProcessedPictureBox.Image = hsiSkeletSegments.ToBitmapGrayScale();
                List<int> widthsOfSegments;
                List<HSISegment> restoredSegments = Extentions.RestoreSegments(skeletSegments, new HSIimage(hsicontours), out widthsOfSegments); //восстановленные сегменты

                //restoredSegments.RemoveBackground(hsibinary); //попробовать

                hsiRestoredSegments = restoredSegments.DrawSegments(hsibinary.Width, hsibinary.Height);
                ProcessedPictureBox.Image = hsiRestoredSegments.ToBitmapGrayScale();

                List<HSISegment> final = Extentions.GetFinalResult(restoredSegments, skeletSegments, widthsOfSegments);

                hsiFinalResult = final.DrawSegments(hsibinary.Width, hsibinary.Height);
                ProcessedPictureBox.Image = hsiFinalResult.ToBitmapGrayScale();

                sw.Stop();
                Text = "На обработку потрачено " + sw.Elapsed.Minutes.ToString() + " минут " + sw.Elapsed.Seconds.ToString() + "секунд";


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //throw;
            }
        }

        private void сравнитьСЭталономToolStripMenuItem_Click(object sender, EventArgs e) //ДОДЕЛАТЬ ОПЕН ФАЙЛ ДАЙЛОГ И МЕТРИКИ
        {
            //MessageBox.Show("Неправильно классифицированных пикселей 7.7 %; \n F-мера 0.94 \n", "Результаты!", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        #endregion

        #region DISPAY
        private void originalHSIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (hsiimage == null)
                    throw new Exception("Изображение не было выбрано!");
                ProcessedPictureBox.Image = hsiimage.ToBitmap();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //throw;
            }
        }

        private void rarefiedHSIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (hsiimage == null)
                    throw new Exception("Изображение не было выбрано!");
                ProcessedPictureBox.Image = hsiprocessed.ToBitmap();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //throw;
            }
        }

        private void filteredHSIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (hsiimage == null)
                    throw new Exception("Изображение не было выбрано!");
                ProcessedPictureBox.Image = hsifiltered.ToBitmap();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //throw;
            }
        }
        
        private void contoursHSIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (hsiimage == null)
                    throw new Exception("Изображение не было выбрано!");
                ProcessedPictureBox.Image = hsicontours.ToBitmapBlackAndWhite();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //throw;
            }
        }

        private void binarizedHSIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (hsiimage == null)
                    throw new Exception("Изображение не было выбрано!");
                ProcessedPictureBox.Image = hsibinary.ToBitmapGrayScale();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //throw;
            }
        }

        private void скелетноеИзображениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (hsiimage == null)
                    throw new Exception("Изображение не было выбрано!");
                ProcessedPictureBox.Image = hsiSkeletSegments.ToBitmapGrayScale();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //throw;
            }
        }

        private void результатToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (hsiimage == null)
                    throw new Exception("Изображение не было выбрано!");
                ProcessedPictureBox.Image = hsiFinalResult.ToBitmapGrayScale();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //throw;
            }
        }

        #endregion

        #region CHOOSE QUALITY

        //получаем коэф. для прорежения тона (выбирается в зависимости от разрешения картинки )
        private void rareCoefComboBox_Click(object sender, EventArgs e) 
        {
            if (rareCoefComboBox.Items.Count == 0)
            {
                string[] items = { "низкое", "среднее", "высокое" };
                rareCoefComboBox.Items.AddRange(items);
            }
        }
        private void rareCoefComboBox_DropDownClosed(object sender, EventArgs e)
        {
            switch (rareCoefComboBox.SelectedItem)
            {
                case "низкое":
                    rareCoef = threeColors;
                    break;
                case "среднее":
                    rareCoef = sixColors;
                    break;
                case "высокое":
                    rareCoef = TwelveColors;
                    break;
                default:
                    rareCoef = threeColors;
                    break;
            }
        }

        //коэф. для прорежения яркости (выбирается в зависимости от количества света, запечатленного на фотографии - чем светлее самые яркие участки (и темнее 
        //темные), тем шире динамический диапазон яркости, и соотв. больше коэфф.) А вообще зависит от конкретной картинки..
        private void exposureComboBox_Click(object sender, EventArgs e)
        {
            if (exposureComboBox.Items.Count == 0)
            {
                string[] items = { "узкий", "средний", "широкий" };
                exposureComboBox.Items.AddRange(items);
            }
        }
        private void exposureComboBox_DropDownClosed(object sender, EventArgs e)
        {
            switch (exposureComboBox.SelectedItem)
            {
                case "узкий":
                    exposure = lowRange; 
                    break;
                case "средний":
                    exposure = middleRange;
                    break;
                case "широкий":
                    rareCoef = highRange;
                    break;
                default:
                    exposure = lowRange;
                    break;
            }
        }

        //выбираем метод сегментирования - только по тону или по тону и яркости
        private void contourComboBox_Click(object sender, EventArgs e)
        {
            if (contourComboBox.Items.Count == 0)
            {
                string[] items = { "По тону (по умолчанию)", "По тону и интенсивности" };
                contourComboBox.Items.AddRange(items);
            }
        }
        private void contourComboBox_DropDownClosed(object sender, EventArgs e)
        {
            switch (contourComboBox.SelectedItem)
            {
                case "По тону (по умолчанию)":
                    isOnlyHue = true;
                    break;
                case "По тону и интенсивности":
                    isOnlyHue = false;
                    break;
                default:
                    isOnlyHue = true;
                    break;
            }
        }
        #endregion

        #region FILTER

        private void filterWindowComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (int.TryParse((string)filterWindowComboBox.SelectedItem, out filterWindow)) //если получилось распарсить значение комбобокса в инт, возвращаемся
                return;
            filterWindow = 0; //иначе присваиваем нуль
        }

        private void filterWindowComboBox_Click(object sender, EventArgs e) //при нажатии на комбобокс инициализируем список из 12 вариантов для окна фильтра
        {
            if (filterWindowComboBox.Items.Count == 0)
            {
                string[] items = {"0 (фильтр отключен)", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" };
                filterWindowComboBox.Items.AddRange(items);
            }
            
        }

        /*private void medianFilteringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            hsiprocessed.MedianFilter(filterWindow); //MEDIAN FILTER
            processed = hsiprocessed.ToBitmap();
            ProcessedPictureBox.Image = processed;
        }*/
        #endregion

        #region SAVEBUTTON
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sfd.FileName = "result";
            if (sfd.ShowDialog() == DialogResult.OK)
            {

            }
        }
        #endregion

        #region OPENBUTTON
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    image = new Bitmap(ofd.FileName);
                    OriginalPictureBox.Image = image;
                    /*if (image.Height < 100 || image.Width < 100) //исключение малого размера изображения ПОКА ОТКЛЮЧИЛИ
                        throw new Exception("Image resolution too low!");*/

                    //ПОКА УБЕРЕМ 
                    //OriginalPictureBox.Image = target;
                    //image = target.Clone(new Rectangle(0, 0, target.Width, target.Height), System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                    //OriginalPictureBox.Image = image;

                    //image_path = ofd.FileName;
                    //emguImage = new Image<Hsv, byte>(ofd.FileName);
                    //HSVImage = new Image<Hsv, byte>(ofd.FileName);
                    hsiimage = new HSIimage(image); //создаем имейдж в формате HSI
                    //hsiprocessed = new HSIimage(hsiimage); //и его копию
                    //hsiprocessed.Rarefaction(rareCoef); //подготавливаем копию к дальнейшей обработке, прорежая цвета и яркость

                }
                else
                    throw new Exception("Image not chosen!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //throw;
            }


        }
        #endregion

       


    }
}








/*
               private void prepareToolStripMenuItem_Click(object sender, EventArgs e)
               {

                   //hsiprocessed = new HSIimage(hsiimage);
                   //processed = hsiprocessed.ToBitmap();

                   //OriginalPictureBox.Image = image;

                   //hsiprocessed.Rarefaction(42);
                   //hsiprocessed.MedianFilter(9); //MEDIAN FILTER
                   //hsiprocessed.Rarefaction(42);

                   processed = hsiprocessed.ToBitmap();
                   ProcessedPictureBox.Image = processed;

                   hsicontours = hsiprocessed.GetContours();
                   processed = hsicontours.ToBitmapBlackAndWhite();
                   //ProcessedPictureBox.Image = processed;

                   //Image<Gray, byte> emguGray = HSVprocessed.Convert<Gray, Byte>();
                   //processed = emguGray.ToBitmap();
                   //ProcessedPictureBox.Image = processed;
                   //Emgu.CV.Util.VectorOfVectorOfPoint contours = new Emgu.CV.Util.VectorOfVectorOfPoint(); //contours
                   //Mat hier = new Mat();
                   //CvInvoke.FindContours(emguGray, contours, hier, RetrType.Tree, ChainApproxMethod.ChainApproxNone);
                   //Point[] contoursArr = contours[0].ToArray();
                   //CvInvoke.DrawContours(HSVprocessed, contours, -1, new MCvScalar(0, 0, 0), 20);
                   //Image<Gray, byte> contours = Extentions.GetContours(HSVprocessed);

                   //processed = contours.ToBitmap();
                   //byte b = processed.GetPixel(0, 0).R;


               }*/






//Image<Gray, byte> emguGray = HSVprocessed.Convert<Gray, Byte>();
//processed = emguGray.ToBitmap();
//ProcessedPictureBox.Image = processed;
//Emgu.CV.Util.VectorOfVectorOfPoint contours = new Emgu.CV.Util.VectorOfVectorOfPoint(); //contours
//Mat hier = new Mat();
//CvInvoke.FindContours(emguGray, contours, hier, RetrType.External, ChainApproxMethod.ChainApproxNone);
//Point[] contoursArr = contours[0].ToArray();
//CvInvoke.DrawContours(HSVprocessed, contours, -1, new MCvScalar(0, 0, 0), 20);
