using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.Windows.Controls.Media;
using Xceed.Wpf.Toolkit;

namespace finalcover_app
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private Point startPoint;
        private Rectangle rect;
        protected bool isDragging = false;
        private Point clickPosition;
        private TranslateTransform originTT;
        private ColorPicker colorPicker;
        private Color color;
        protected bool isColorPickerSelected = false;
        protected bool ColorPickerTrigger = false;
        private bool mSingleClick;

        private void BtnLoadFromFile_Click(object sender, RoutedEventArgs e)
        {
            ImageBrush ib = new ImageBrush();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                ib.ImageSource = new BitmapImage(new Uri(openFileDialog.FileName, UriKind.Relative));
                canvas.Background = ib;
            }
        }

        private void BtnSaveImage(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)canvas.RenderSize.Width,
            (int)canvas.RenderSize.Height, 96d, 96d, System.Windows.Media.PixelFormats.Default);
            rtb.Render(canvas);

            var crop = new CroppedBitmap(rtb, new Int32Rect(50, 50, 250, 250));

            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(crop));

            using (var fs = System.IO.File.OpenWrite("canvas.png"))
            {
                pngEncoder.Save(fs);
            }
        }
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!isDragging && !isColorPickerSelected)
            {
                startPoint = e.GetPosition(canvas);

                rect = new Rectangle
                {
                    Stroke = Brushes.LightBlue,
                    Fill = Brushes.LightBlue,
                    StrokeThickness = 2
                };

                //binding = new MouseBinding();
                //binding.Gesture = "LeftClick"};
                //binding.Command = RectClick;

                Canvas.SetLeft(rect, startPoint.X);
                Canvas.SetTop(rect, startPoint.Y);
                //rect.InputBindings.Add();
                rect.MouseLeftButtonDown += Rect_MouseLeftButtonDown;
                rect.MouseLeftButtonUp += Rect_MouseLeftButtonUp;
                rect.MouseMove += Rect_MouseMove;
                rect.MouseRightButtonDown += Rect_MouseRightButtonDown;
          
                canvas.Children.Add(rect);

            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
                return;

            var pos = e.GetPosition(canvas);

            var x = Math.Min(pos.X, startPoint.X);
            var y = Math.Min(pos.Y, startPoint.Y);

            var w = Math.Max(pos.X, startPoint.X) - x;
            var h = Math.Max(pos.Y, startPoint.Y) - y;

            rect.Width = w;
            rect.Height = h;

            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            return;
        }

        private void Rect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                // Handle double-click
                //Shape s = sender as Shape;
                canvas.Children.Remove(rect);
                isDragging = false;
            }
            else if (e.ClickCount == 1)
            {
                var draggableControl = sender as Shape;
                originTT = draggableControl.RenderTransform as TranslateTransform ?? new TranslateTransform();
                isDragging = true;
                clickPosition = e.GetPosition(this);
                draggableControl.CaptureMouse();
            }
        }

        private void Rect_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!ColorPickerTrigger)
            {
                colorPicker = new ColorPicker();
                startPoint = e.GetPosition(canvas);
                Canvas.SetLeft(colorPicker, startPoint.X);
                Canvas.SetTop(colorPicker, startPoint.Y);
                colorPicker.SelectedColorChanged += SelectedColorChanged;
                canvas.Children.Add(colorPicker);
                isColorPickerSelected = true;
                ColorPickerTrigger = true;

                //selectedColor = (Color)colorPicker.SelectedColor;

                //comboBox = new ComboBox();
                //comboBox.Name = "comboBox1";
                //comboBox.ItemsSource = "{Binding Source={StaticResource colorPropertiesOdp}}";
                //comboBox.DisplayMemberPath = "Name";
                //comboBox.SelectedValuePath = "Name";

                //Canvas.SetLeft(comboBox, startPoint.X);
                //Canvas.SetTop(comboBox, startPoint.Y);

            }
        }

        private void SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (colorPicker.SelectedColor.HasValue)
            {
                color = colorPicker.SelectedColor.Value;
                rect.Stroke = new SolidColorBrush(color);
                rect.Fill = new SolidColorBrush(color);
                canvas.Children.Remove(colorPicker);
                isColorPickerSelected = false;
                ColorPickerTrigger = false;
            }

        }

        private void Rect_DoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
        private void Rect_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            var draggable = sender as Shape;
            draggable.ReleaseMouseCapture();
        }

        private void Rect_MouseMove(object sender, MouseEventArgs e)
        {
            var draggableControl = sender as Shape;
            if (isDragging && draggableControl != null)
            {
                Point currentPosition = e.GetPosition(this);
                var transform = draggableControl.RenderTransform as TranslateTransform ?? new TranslateTransform();
                transform.X = originTT.X + (currentPosition.X - clickPosition.X);
                transform.Y = originTT.Y + (currentPosition.Y - clickPosition.Y);
                draggableControl.RenderTransform = new TranslateTransform(transform.X, transform.Y);
            }

        }

    }
}
