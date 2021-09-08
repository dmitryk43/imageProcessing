
namespace ImageProcessing
{
    partial class main
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.detectRoadsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.сравнитьСЭталономToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.parametersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.medianFilterWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filterWindowComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.detectionMethodToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contourComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.imageQualityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rareCoefComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.exposureAltitudeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exposureComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.originalHSIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rarefiedHSIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filteredHSIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contoursHSIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.binarizedHSIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.скелетноеИзображениеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.OriginalPictureBox = new System.Windows.Forms.PictureBox();
            this.ProcessedPictureBox = new System.Windows.Forms.PictureBox();
            this.результатToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OriginalPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ProcessedPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.imageToolStripMenuItem,
            this.parametersToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.fileToolStripMenuItem.Text = "Файл";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.openToolStripMenuItem.Text = "Открыть";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.saveToolStripMenuItem.Text = "Сохранить";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // imageToolStripMenuItem
            // 
            this.imageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.detectRoadsToolStripMenuItem,
            this.сравнитьСЭталономToolStripMenuItem});
            this.imageToolStripMenuItem.Name = "imageToolStripMenuItem";
            this.imageToolStripMenuItem.Size = new System.Drawing.Size(95, 20);
            this.imageToolStripMenuItem.Text = "Изображение";
            // 
            // detectRoadsToolStripMenuItem
            // 
            this.detectRoadsToolStripMenuItem.Name = "detectRoadsToolStripMenuItem";
            this.detectRoadsToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.detectRoadsToolStripMenuItem.Text = "Найти дороги";
            this.detectRoadsToolStripMenuItem.Click += new System.EventHandler(this.detectRoadsToolStripMenuItem_Click);
            // 
            // сравнитьСЭталономToolStripMenuItem
            // 
            this.сравнитьСЭталономToolStripMenuItem.Name = "сравнитьСЭталономToolStripMenuItem";
            this.сравнитьСЭталономToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.сравнитьСЭталономToolStripMenuItem.Text = "Сравнить с эталоном";
            this.сравнитьСЭталономToolStripMenuItem.Click += new System.EventHandler(this.сравнитьСЭталономToolStripMenuItem_Click);
            // 
            // parametersToolStripMenuItem
            // 
            this.parametersToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.medianFilterWindowToolStripMenuItem,
            this.detectionMethodToolStripMenuItem,
            this.imageQualityToolStripMenuItem,
            this.exposureAltitudeToolStripMenuItem});
            this.parametersToolStripMenuItem.Name = "parametersToolStripMenuItem";
            this.parametersToolStripMenuItem.Size = new System.Drawing.Size(83, 20);
            this.parametersToolStripMenuItem.Text = "Параметры";
            // 
            // medianFilterWindowToolStripMenuItem
            // 
            this.medianFilterWindowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filterWindowComboBox});
            this.medianFilterWindowToolStripMenuItem.Name = "medianFilterWindowToolStripMenuItem";
            this.medianFilterWindowToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.medianFilterWindowToolStripMenuItem.Text = "Радиус окна фильтра";
            // 
            // filterWindowComboBox
            // 
            this.filterWindowComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.filterWindowComboBox.Name = "filterWindowComboBox";
            this.filterWindowComboBox.Size = new System.Drawing.Size(121, 23);
            this.filterWindowComboBox.DropDownClosed += new System.EventHandler(this.filterWindowComboBox_DropDownClosed);
            this.filterWindowComboBox.Click += new System.EventHandler(this.filterWindowComboBox_Click);
            // 
            // detectionMethodToolStripMenuItem
            // 
            this.detectionMethodToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contourComboBox});
            this.detectionMethodToolStripMenuItem.Name = "detectionMethodToolStripMenuItem";
            this.detectionMethodToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.detectionMethodToolStripMenuItem.Text = "Метод сегментации";
            // 
            // contourComboBox
            // 
            this.contourComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.contourComboBox.Name = "contourComboBox";
            this.contourComboBox.Size = new System.Drawing.Size(121, 23);
            this.contourComboBox.DropDownClosed += new System.EventHandler(this.contourComboBox_DropDownClosed);
            this.contourComboBox.Click += new System.EventHandler(this.contourComboBox_Click);
            // 
            // imageQualityToolStripMenuItem
            // 
            this.imageQualityToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rareCoefComboBox});
            this.imageQualityToolStripMenuItem.Name = "imageQualityToolStripMenuItem";
            this.imageQualityToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.imageQualityToolStripMenuItem.Text = "Разрешение изображения";
            // 
            // rareCoefComboBox
            // 
            this.rareCoefComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.rareCoefComboBox.Name = "rareCoefComboBox";
            this.rareCoefComboBox.Size = new System.Drawing.Size(121, 23);
            this.rareCoefComboBox.DropDownClosed += new System.EventHandler(this.rareCoefComboBox_DropDownClosed);
            this.rareCoefComboBox.Click += new System.EventHandler(this.rareCoefComboBox_Click);
            // 
            // exposureAltitudeToolStripMenuItem
            // 
            this.exposureAltitudeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exposureComboBox});
            this.exposureAltitudeToolStripMenuItem.Name = "exposureAltitudeToolStripMenuItem";
            this.exposureAltitudeToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.exposureAltitudeToolStripMenuItem.Text = "Динамический диапазон";
            // 
            // exposureComboBox
            // 
            this.exposureComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.exposureComboBox.Name = "exposureComboBox";
            this.exposureComboBox.Size = new System.Drawing.Size(121, 23);
            this.exposureComboBox.DropDownClosed += new System.EventHandler(this.exposureComboBox_DropDownClosed);
            this.exposureComboBox.Click += new System.EventHandler(this.exposureComboBox_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.originalHSIToolStripMenuItem,
            this.rarefiedHSIToolStripMenuItem,
            this.filteredHSIToolStripMenuItem,
            this.contoursHSIToolStripMenuItem,
            this.binarizedHSIToolStripMenuItem,
            this.скелетноеИзображениеToolStripMenuItem,
            this.результатToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(93, 20);
            this.viewToolStripMenuItem.Text = "Просмотреть";
            // 
            // originalHSIToolStripMenuItem
            // 
            this.originalHSIToolStripMenuItem.Name = "originalHSIToolStripMenuItem";
            this.originalHSIToolStripMenuItem.Size = new System.Drawing.Size(326, 22);
            this.originalHSIToolStripMenuItem.Text = "Оригинальное изображение HSI";
            this.originalHSIToolStripMenuItem.Click += new System.EventHandler(this.originalHSIToolStripMenuItem_Click);
            // 
            // rarefiedHSIToolStripMenuItem
            // 
            this.rarefiedHSIToolStripMenuItem.Name = "rarefiedHSIToolStripMenuItem";
            this.rarefiedHSIToolStripMenuItem.Size = new System.Drawing.Size(326, 22);
            this.rarefiedHSIToolStripMenuItem.Text = "Подготовленное к сегментации изображение";
            this.rarefiedHSIToolStripMenuItem.Click += new System.EventHandler(this.rarefiedHSIToolStripMenuItem_Click);
            // 
            // filteredHSIToolStripMenuItem
            // 
            this.filteredHSIToolStripMenuItem.Name = "filteredHSIToolStripMenuItem";
            this.filteredHSIToolStripMenuItem.Size = new System.Drawing.Size(326, 22);
            this.filteredHSIToolStripMenuItem.Text = "Изображение после медианной фильтрации";
            this.filteredHSIToolStripMenuItem.Click += new System.EventHandler(this.filteredHSIToolStripMenuItem_Click);
            // 
            // contoursHSIToolStripMenuItem
            // 
            this.contoursHSIToolStripMenuItem.Name = "contoursHSIToolStripMenuItem";
            this.contoursHSIToolStripMenuItem.Size = new System.Drawing.Size(326, 22);
            this.contoursHSIToolStripMenuItem.Text = "Контурное изображение";
            this.contoursHSIToolStripMenuItem.Click += new System.EventHandler(this.contoursHSIToolStripMenuItem_Click);
            // 
            // binarizedHSIToolStripMenuItem
            // 
            this.binarizedHSIToolStripMenuItem.Name = "binarizedHSIToolStripMenuItem";
            this.binarizedHSIToolStripMenuItem.Size = new System.Drawing.Size(326, 22);
            this.binarizedHSIToolStripMenuItem.Text = "Бинарное изображение";
            this.binarizedHSIToolStripMenuItem.Click += new System.EventHandler(this.binarizedHSIToolStripMenuItem_Click);
            // 
            // скелетноеИзображениеToolStripMenuItem
            // 
            this.скелетноеИзображениеToolStripMenuItem.Name = "скелетноеИзображениеToolStripMenuItem";
            this.скелетноеИзображениеToolStripMenuItem.Size = new System.Drawing.Size(326, 22);
            this.скелетноеИзображениеToolStripMenuItem.Text = "Скелетное изображение";
            this.скелетноеИзображениеToolStripMenuItem.Click += new System.EventHandler(this.скелетноеИзображениеToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.OriginalPictureBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.ProcessedPictureBox);
            this.splitContainer1.Size = new System.Drawing.Size(800, 426);
            this.splitContainer1.SplitterDistance = 400;
            this.splitContainer1.TabIndex = 2;
            // 
            // OriginalPictureBox
            // 
            this.OriginalPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OriginalPictureBox.Location = new System.Drawing.Point(0, 0);
            this.OriginalPictureBox.Name = "OriginalPictureBox";
            this.OriginalPictureBox.Size = new System.Drawing.Size(396, 422);
            this.OriginalPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.OriginalPictureBox.TabIndex = 0;
            this.OriginalPictureBox.TabStop = false;
            // 
            // ProcessedPictureBox
            // 
            this.ProcessedPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProcessedPictureBox.Location = new System.Drawing.Point(0, 0);
            this.ProcessedPictureBox.Name = "ProcessedPictureBox";
            this.ProcessedPictureBox.Size = new System.Drawing.Size(392, 422);
            this.ProcessedPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ProcessedPictureBox.TabIndex = 0;
            this.ProcessedPictureBox.TabStop = false;
            // 
            // результатToolStripMenuItem
            // 
            this.результатToolStripMenuItem.Name = "результатToolStripMenuItem";
            this.результатToolStripMenuItem.Size = new System.Drawing.Size(326, 22);
            this.результатToolStripMenuItem.Text = "Результат";
            this.результатToolStripMenuItem.Click += new System.EventHandler(this.результатToolStripMenuItem_Click);
            // 
            // main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "main";
            this.Text = "main";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.OriginalPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ProcessedPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.PictureBox OriginalPictureBox;
        private System.Windows.Forms.PictureBox ProcessedPictureBox;
        private System.Windows.Forms.ToolStripMenuItem imageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem parametersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem detectRoadsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem medianFilterWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox filterWindowComboBox;
        private System.Windows.Forms.ToolStripMenuItem imageQualityToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox rareCoefComboBox;
        private System.Windows.Forms.ToolStripMenuItem detectionMethodToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox contourComboBox;
        private System.Windows.Forms.ToolStripMenuItem exposureAltitudeToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox exposureComboBox;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem originalHSIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rarefiedHSIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filteredHSIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem contoursHSIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem binarizedHSIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem скелетноеИзображениеToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem сравнитьСЭталономToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem результатToolStripMenuItem;
    }
}

