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
using System.IO;

namespace Automat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    #region Window
    public partial class MainWindow : Window
    {
        String editedValue;
        AutomatClass automat;
        MovingParameters move;
      //  VisualWindow vis;
        public MainWindow()
        {
            InitializeComponent();
            //canvas1.Background = new SolidColorBrush(Colors.Blue);
            for (int i = 2; i < 31; i++)
            {
                stateCounter.Items.Add(i.ToString());
            }
            List<Pair> data1 = new List<Pair>()
            {
                new Pair(){first = 3, second = 5},
                new Pair(){first = 2, second = 5},
                new Pair(){first = 2, second = 5},
                new Pair(){first = 1, second = 1},
                new Pair(){first = 2, second = 2},
            };
            dataGrid1.ItemsSource = data1;
            List<Pair> data2 = new List<Pair>()
            {
                new Pair(){first = 1, second = 1},
                new Pair(){first = 0, second = 0},
                new Pair(){first = 0, second = 0},
                new Pair(){first = 1, second = 0},
                new Pair(){first = 0, second = 1},
            };
            dataGrid2.ItemsSource = data2;
            automat = new AutomatClass();
            updateAutomat();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            stateCounter.SelectedIndex = 3;
        }

        private void dataGrid1_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void stateCounter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var a = Int16.Parse(e.AddedItems[0].ToString());
            var data1 = dataGrid1.ItemsSource as List<Pair>;
            var data2 = dataGrid2.ItemsSource as List<Pair>;
            while (dataGrid1.Items.Count > a)
            {
                data1.RemoveAt(dataGrid1.Items.Count - 1);
                data2.RemoveAt(dataGrid2.Items.Count - 1);
            }
            while (dataGrid1.Items.Count < a)
            {
                data1.Add(new Pair() { first = 0, second = 0 });
                data2.Add(new Pair() { first = 0, second = 0 });
            }
            updateData(dataGrid1);
            updateData(dataGrid2);
            dataGrid1.Height = dataGrid1.RowHeight * dataGrid1.Items.Count;
            dataGrid2.Height = dataGrid2.RowHeight * dataGrid1.Items.Count;
        }
        private void dataGrid1_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            switch (e.Column.DisplayIndex)
            {
                case 0:
                    editedValue = ((Pair)e.Row.Item).first.ToString();
                    break;
                case 1:
                    editedValue = ((Pair)e.Row.Item).second.ToString();
                    break;
            }
        }
        private void dataGrid1_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.DisplayIndex == 0)
            {
                int data = ((Pair)(e.Row.Item)).first;
                //bool success = int.TryParse(((Pair)(e.Row.Item)).first, out data);
                if (/*!success ||*/ data < 1 || data > int.Parse(stateCounter.Text))
                    ((Pair)(e.Row.Item)).first = int.Parse(editedValue);
            }
            else
            {
                int data = ((Pair)(e.Row.Item)).second;
           //     bool success = int.TryParse(((Pair)(e.Row.Item)).second, out data);
                if (/*!success ||*/ data < 1 || data > int.Parse(stateCounter.Text))
                    ((Pair)(e.Row.Item)).second = int.Parse(editedValue);
            }
            updateData(dataGrid1);
            updateAutomat();
        }

        private void dataGrid2_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            switch (e.Column.DisplayIndex)
            {
                case 0:
                    editedValue = ((Pair)e.Row.Item).first.ToString();
                    break;
                case 1:
                    editedValue = ((Pair)e.Row.Item).second.ToString();
                    break;
            }
        }

        private void dataGrid2_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {/*
            if (!"01".Contains(((Pair)(e.Row.Item)).first))
                ((Pair)(e.Row.Item)).first = editedValue;
            if (!"01".Contains(((Pair)(e.Row.Item)).second))
                ((Pair)(e.Row.Item)).second = editedValue;*/
            updateData(dataGrid2);
            updateAutomat();
        }

        void updateData(DataGrid dataGrid)
        {
            var src = dataGrid.ItemsSource;
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = src;
        }

        private void rnd_Button_Click(object sender, RoutedEventArgs e)
        {
            int size = int.Parse(stateCounter.Text);
            Random rnd = new Random();
            var data1 = new List<Pair>();
            var data2 = new List<Pair>();
            for (int i = 0; i < size; i++)
            {
                data1.Add(new Pair() { first = (rnd.Next(size) + 1), second = (rnd.Next(size) + 1) });
                data2.Add(new Pair() { first = rnd.Next(2), second = rnd.Next(2) });
            }
            dataGrid1.ItemsSource = data1;
            dataGrid2.ItemsSource = data2;
            automat.init(dataGrid1.ItemsSource as List<Pair>, dataGrid2.ItemsSource as List<Pair>);
            paintGraph();
        }
        private void Min_Button_Click(object sender, RoutedEventArgs e)
        {
            var n = automat.minimize();
            if (n.outputs.Count - 2 == stateCounter.SelectedIndex)
            {
                MessageBox.Show("Автомат минимален");
            }
            else
            {
                stateCounter.SelectedIndex = n.outputs.Count - 2;
                dataGrid1.ItemsSource = n.transitions;
                dataGrid2.ItemsSource = n.outputs;
                automat = n;
            }
        }
        private void Mem_Button_Click(object sender, RoutedEventArgs e)
        {
            int memSize = automat.memorySize(MemoryType.Both);
            if (memSize == 0)
                MessageBox.Show("Память отсутствует");
            else
            {
                int inputMemorySize = automat.memorySize(MemoryType.Input);
                int outputMemorySize = automat.memorySize(MemoryType.Output);
                Console.WriteLine(String.Format("Общая: {0}, Входная: {1}, Выходная: {2}", memSize, inputMemorySize, outputMemorySize));
                if (inputMemorySize == 0 && outputMemorySize == 0)
                    inputMemorySize = outputMemorySize = memSize;
               // MessageBox.Show(String.Format("Общая память: {0}\nВходная память: {1}\nВыходная память: {2}", memSize.ToString(), inputMemorySize == -1?"отсутствует":inputMemorySize.ToString(), outputMemorySize == -1?"отсутствует":outputMemorySize.ToString()), "Размер памяти");
                var vis = new VisualWindow(new List<Shape>(){new RegisterPic() { X1 = 50, Y1 = 90, Stroke = Brushes.Black, inputMemory = inputMemorySize, outputMemory = outputMemorySize }});
                vis.Show();
            }
        }
        private void graph_Button_Click(object sender, RoutedEventArgs e)
        {
            updateAutomat();
            paintGraph();
        }
        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var el = sender as Ellipse;
            move.coord = e.GetPosition(el);
            move.isMoving = true;
            move.instance = sender as Ellipse;
            //lx1.Content = e.GetPosition(canvas1).ToString();
            //lx2.Content = move.coord.ToString();
        }

        private void Ellipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            move.isMoving = false;
            move.instance = null;
        }

        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && move.isMoving && move.instance == sender as Ellipse)
            {
                var el = sender as Ellipse;
                Canvas.SetLeft(el, e.GetPosition(canvas1).X - move.coord.X);
                Canvas.SetTop(el, e.GetPosition(canvas1).Y - move.coord.Y);
                //               el.Margin = new Thickness(e.GetPosition(canvas1).X - move.coord.X, e.GetPosition(canvas1).Y - move.coord.Y,0,0);
                var text = (el.Tag as Vertice).text;
                Canvas.SetLeft(text, Canvas.GetLeft(el) + (el.Width / 2) - text.DesiredSize.Width / 2);
                Canvas.SetTop(text, Canvas.GetTop(el) + (el.Height / 2) - text.DesiredSize.Height / 2);
                //         text.Margin = new Thickness(el.Margin.Left + 14, el.Margin.Top + 13, 0, 0);

                //lx1.Content = Canvas.GetLeft(el);
                //lx2.Content = Canvas.GetTop(el);
                //(el.Tag as Vertice).update_edges();
                (el.Tag as Vertice).edges.Where(ed => !ed.isLoop).ToList().ForEach(ed => drawEdge(ed));
                (el.Tag as Vertice).edges.Where(ed => ed.isLoop).ToList().ForEach(ed => drawEdge(ed));
            }
        }
        void drawEdge(Edge edge)
        {
            if (!edge.isLoop)
            {
                var x1 = Canvas.GetLeft(edge.start.shape)/*edge.start.shape.Margin.Left*/ + (edge.start.shape.Width / 2);
                var y1 = Canvas.GetTop(edge.start.shape)/*edge.start.shape.Margin.Top*/ + (edge.start.shape.Height / 2);
                var x2 = Canvas.GetLeft(edge.end.shape)/*edge.end.shape.Margin.Left*/ + (edge.end.shape.Width / 2);
                var y2 = Canvas.GetTop(edge.end.shape)/*edge.end.shape.Margin.Top*/ + (edge.end.shape.Height / 2);
                if (edge.line == null)
                {
                    edge.line = new ArrowLine() { Stroke = Brushes.Black, offset = edge.end.shape.Width / 2.0 };
                    canvas1.Children.Add(edge.line);
                    Canvas.SetZIndex(edge.line, 1);
                    edge.text = new Label() { Content = edge.labelText, FontSize = 14 };
                    canvas1.Children.Add(edge.text);
                    Canvas.SetZIndex(edge.text, 1);
                }
                var line = edge.line as ArrowLine;
                double Xp = y2 - y1;
                double Yp = x1 - x2;

                line.X1 = x1;
                line.Y1 = y1;
                line.X2 = x2;
                line.Y2 = y2;

                if (edge.twoSided)           //Если есть обратное ребро, надо раздвинуть их
                {
                    x1 = x1 - (Xp / line.Length) * 5;
                    y1 = y1 - (Yp / line.Length) * 5;
                    x2 = x2 - (Xp / line.Length) * 5;
                    y2 = y2 - (Yp / line.Length) * 5;

                    line.X1 = x1;
                    line.Y1 = y1;
                    line.X2 = x2;
                    line.Y2 = y2;

                    Xp = y2 - y1;
                    Yp = x1 - x2;
                }

                // координаты перпендикуляров, удалённой от точки X4;Y4 на 10px в разные стороны
                double X5 = (x2 + x1) / 2 - (Xp / line.Length) * 10;
                double Y5 = (y2 + y1) / 2 - (Yp / line.Length) * 10;
                /*
                Canvas.SetLeft(edge.text, x1 + (x2 - x1 - edge.text.ActualWidth) / 2);
                Canvas.SetTop(edge.text, (y1 + (y2 - y1 - edge.text.ActualHeight) / 2));*/
                edge.text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Canvas.SetLeft(edge.text, X5 - edge.text.DesiredSize.Width / 2);
                Canvas.SetTop(edge.text, Y5 - edge.text.DesiredSize.Height / 2);
                edge.start.edges.Where(e => e.isLoop).ToList().ForEach(e => drawEdge(e));
                edge.end.edges.Where(e => e.isLoop).ToList().ForEach(e => drawEdge(e));
                //edge.text.Margin = new Thickness(x1 + (x2 - x1) / 2, (y1 + (y2 - y1) / 2)-20, 0, 0);
            }
            else
            {
                var edges = edge.start.edges.Where(e => !e.isLoop).ToList();
                //var angles = edges.Select(e => getAngle(e, edge.start));
                double angle = getAngle(edges, edge.start); //Math.Atan(angles.Select(a => Math.Sin(a)).Sum()/angles.Select(a => Math.Cos(a)).Sum())*180/Math.PI + 180;
                //lx1.Content = String.Join(" ", angles);
                //lx2.Content = angle;
                //    Random rnd = new Random();
                //double angle = angles; //rnd.Next(360);
                var radius = edge.start.shape.Width / 2.0;
                var center = new Point(Canvas.GetLeft(edge.start.shape) + (1 - Math.Cos(angle * Math.PI / 180)) * radius, Canvas.GetTop(edge.start.shape) + (1 - Math.Sin(angle * Math.PI / 180)) * radius);
                if (edge.line == null)
                {
                    edge.line = new ArrowCircle() { Stroke = Brushes.Black, Height = radius * 2, Width = radius * 2 };
                    canvas1.Children.Add(edge.line);
                    Canvas.SetZIndex(edge.line, 1);
                    edge.text = new Label() { Content = edge.labelText };
                    canvas1.Children.Add(edge.text);
                    Canvas.SetZIndex(edge.text, 1);
                }
                var loop = edge.line as ArrowCircle;
                loop.angle = angle + 110;   //Magic constant for best appearance
                Canvas.SetLeft(loop, center.X - radius + 5);
                Canvas.SetTop(loop, center.Y - radius + 5);
                edge.text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            //    Canvas.SetLeft(edge.text, center.X - edge.text.DesiredSize.Width / 2);
            //    Canvas.SetTop(edge.text, center.Y - radius - 20);
                Canvas.SetLeft(edge.text, Canvas.GetLeft(edge.start.shape) - edge.start.shape.Width / 2 - 5 + (1 - Math.Cos(angle * Math.PI / 180)) * (2 * radius + 5) - edge.text.DesiredSize.Width / 2);
                Canvas.SetTop(edge.text, Canvas.GetTop(edge.start.shape) - edge.start.shape.Height / 2 - 5 + (1 - Math.Sin(angle * Math.PI / 180)) * (2 * radius + 5) - edge.text.DesiredSize.Height / 2);
            }
        }
        double getAngle(List<Edge> edges, Vertice start)
        {
            double angle = 0;
            double x = 0;
            double y = 0;
            var start_center = new Point(Canvas.GetLeft(start.shape) + start.shape.Width / 2.0, Canvas.GetTop(start.shape) + start.shape.Height / 2.0);
            foreach (var edge in edges)
            {
                var end = start == edge.start ? edge.end : edge.start;      //Смотрим угол относительно вершины start, направление ребра не важно
                var end_center = new Point(Canvas.GetLeft(end.shape) + end.shape.Width / 2.0, Canvas.GetTop(end.shape) + end.shape.Height / 2.0);
                var len = (Math.Sqrt(Math.Pow(end_center.X - start_center.X, 2) + Math.Pow(end_center.Y - start_center.Y, 2))) * edges.Count(e => e.start == edge.start && e.end == edge.end || e.start == edge.end && e.end == edge.start);
                if (len != 0)
                {
                    x = x + (end_center.X - start_center.X) / len;
                    y = y + (end_center.Y - start_center.Y) / len;
                }
            }
            if (x == 0)
            {
                if (y > 0)
                    angle = 90;
                else
                    angle = -90;
            }
            else
                angle = Math.Atan(y / x) * 180 / Math.PI;
            if (x < 0)
                angle = angle - 180;
            return angle;
        }
        void paintGraph()
        {
            /*List<UIElement> todel = new List<UIElement>();
            foreach (UIElement child in canvas1.Children)
            {
                if (child is Ellipse || child is Line)
                {
                    todel.Add(child);
                }
            }
            todel.ForEach(t => canvas1.Children.Remove(t));*/
            canvas1.Children.Clear();
            Random rnd = new Random();
            List<Point> cells = new List<Point>();
            int rows = 0;
            int cols = 0;
            if (automat.graph.vertices.Count < 9)
            {
                rows = cols = 3;
            }
            else if (automat.graph.vertices.Count <= 25)
            {
                rows = cols = 5;
            }
            else
            {
                if (canvas1.DesiredSize.Width > canvas1.DesiredSize.Height)
                {
                    rows = 5;
                    cols = 6;
                }
                else
                {
                    rows = 6;
                    cols = 5;
                }
            }
            var x_step = canvas1.DesiredSize.Width / (2 * cols);
            var y_step = canvas1.DesiredSize.Height / (2 * rows);
            for (int i = 0; i < cols; i++)
                for (int j = 0; j < rows; j++)
                    cells.Add(new Point(x_step * (2 * i + 1), y_step * (2 * j + 1)));

            automat.graph.vertices.ForEach(v =>
                {
                    var el = new Ellipse() { Height = 50, Width = 50, /*Margin = new Thickness(rnd.Next((int)canvas1.Width - 50), rnd.Next((int)canvas1.Height - 50), 0, 0),*/ Stroke = Brushes.Black, Fill = Brushes.Cyan, Tag = v };
                    int cellIndex = rnd.Next(cells.Count);
                    //     Canvas.SetLeft(el, rnd.Next((int)canvas1.ActualWidth - 100));
                    //       Canvas.SetTop(el, rnd.Next((int)canvas1.ActualHeight - 100));
                    Canvas.SetLeft(el, cells[cellIndex].X - el.Width / 2);
                    Canvas.SetTop(el, cells[cellIndex].Y - el.Height / 2);
                    cells.RemoveAt(cellIndex);
                    el.MouseLeftButtonDown += Ellipse_MouseLeftButtonDown;
                    el.MouseLeftButtonUp += Ellipse_MouseLeftButtonUp;
                    el.MouseMove += Ellipse_MouseMove;
                    v.shape = el;
                    v.text = new Label() { Content = v.number, FontSize = 14, /*Margin = new Thickness(el.Margin.Left + 14, el.Margin.Top + 13, 0, 0),*/ IsHitTestVisible = false };
                    v.text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Canvas.SetLeft(v.text, Canvas.GetLeft(el) + el.Width/2 - v.text.DesiredSize.Width/2);
                    Canvas.SetTop(v.text, Canvas.GetTop(el) + el.Height/2 - v.text.DesiredSize.Height/2);
                    v.text.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                    v.text.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    //v.text.MouseLeftButtonDown += Ellipse_MouseLeftButtonDown(el, new MouseButtonEventArgs();
                    Canvas.SetZIndex(el, 2);
                    Canvas.SetZIndex(v.text, 2);
                    canvas1.Children.Add(el);
                    canvas1.Children.Add(v.text);
                });
            automat.graph.edges.ForEach(e => drawEdge(e));
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            canvas1.Height = mainWindow.Height - ((Border)canvas1.Parent).Margin.Top;
            canvas1.Width = mainWindow.Width - ((Border)canvas1.Parent).Margin.Left;
            //lx1.Content = canvas1.Width;
            //lx2.Content = canvas1.Height;
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            //var margin = canvas1.Margin;
            //  canvas1.Margin = default(Thickness);
            var dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "graph";
            dlg.DefaultExt = ".png"; // Default file extension
            dlg.Filter = "Images (.png)|*.png"; // Filter files by extension
            if (dlg.ShowDialog().Value)
            {
                Rect rect = new Rect(canvas1.DesiredSize);
                RenderTargetBitmap rtb = new RenderTargetBitmap((int)rect.Right,
                  (int)rect.Bottom, 96d, 96d, System.Windows.Media.PixelFormats.Default);

                rtb.Render(canvas1);
                //endcode as PNG
                BitmapEncoder pngEncoder = new PngBitmapEncoder();
                pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

                //save to memory stream
                System.IO.MemoryStream ms = new System.IO.MemoryStream();

                pngEncoder.Save(ms);
                ms.Close();
                System.IO.File.WriteAllBytes(dlg.FileName, ms.ToArray());
                String automatData = "";
                for (int i = 0; i < automat.outputs.Count; i++)
                {
                    automatData += String.Format("{0}\t{1}\t{2}\t{3}\r\n", automat.transitions[i].first, automat.transitions[i].second, automat.outputs[i].first, automat.outputs[i].second);
                }
                var sw = new System.IO.StreamWriter(dlg.FileName.Replace(".png", ".txt"));
                sw.Write(automatData);
                sw.Close();
                MessageBox.Show("Успешно сохранено");
            }
        }
        private void Load_Button_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "graph";
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Text files (.txt)|*.txt"; // Filter files by extension
            if (dlg.ShowDialog().Value)
            {
                var sr = new StreamReader(dlg.FileName);
                var dataLines = sr.ReadToEnd().Split(new string[]{"\n"}, StringSplitOptions.RemoveEmptyEntries);
                sr.Close();
                if (dataLines.Length > 2)
                {
                    stateCounter.SelectedIndex = dataLines.Length - 2;
                    List<Pair> data1 = new List<Pair>();
                    List<Pair> data2 = new List<Pair>();
                    for (int i = 0; i < dataLines.Length; i++)
                    {
                        var values = dataLines[i].Split('\t').Select(d => int.Parse(d)).ToArray();
                        data1.Add(new Pair() { first = values[0], second = values[1] });
                        data2.Add(new Pair() { first = values[2], second = values[3] });
                    }
                    dataGrid1.ItemsSource = data1;
                    dataGrid2.ItemsSource = data2;
                }
                updateAutomat();
                MessageBox.Show("Успешно загружено");
                paintGraph();
            }
        }
        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void canvas1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && move.isMoving)
            {
                Ellipse_MouseMove(move.instance, e);
            }
        }

        private void canvas1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            move.isMoving = false;
            move.instance = null;
        }

        private void canvas1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Pressed");
        }
        void updateAutomat()
        {
            automat.init(dataGrid1.ItemsSource as List<Pair>, dataGrid2.ItemsSource as List<Pair>);
        }

        private void Bans_Button_Click(object sender, RoutedEventArgs e)
        {
            updateAutomat();
            var bans = new BanGraph();
            bans.init(automat);
            if (bans.bans.Count == 0)
                MessageBox.Show("Запреты отсутствуют", "Запреты");
            else
            {
                string bansData = String.Join("\r\n", bans.bans.Select(b => String.Join("", b.data)));
                MessageBox.Show(bansData, "Запреты");
                var dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = "restrictions";
                dlg.DefaultExt = ".txt"; // Default file extension
                dlg.Filter = "Text files (.txt)|*.txt"; // Filter files by extension
                if (dlg.ShowDialog().Value)
                {
                    var sw = new System.IO.StreamWriter(dlg.FileName);
                    sw.Write(bansData);
                    sw.Close();
                    MessageBox.Show("Успешно сохранено");
                }
            }
        }

        private void mainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.M)
            {
                Mem_Button_Click(sender, null);
            }
            else if (e.Key == Key.R)
            {
                rnd_Button_Click(sender, null);
            }
        }
    }
    
    public class DataGridNumericColumn : DataGridTextColumn
    {
        protected override object PrepareCellForEdit(System.Windows.FrameworkElement editingElement, System.Windows.RoutedEventArgs editingEventArgs)
        {
            TextBox edit = editingElement as TextBox;
            edit.PreviewTextInput += OnPreviewTextInput;

            return base.PrepareCellForEdit(editingElement, editingEventArgs);
        }

        void OnPreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            try
            {
                Convert.ToInt32(e.Text);
            }
            catch
            {
                // Show some kind of error message if you want

                // Set handled to true
                e.Handled = true;
            }
        }
    }

    #endregion

    public class AutomatClass
    {
        public List<Pair> outputs;
        public List<Pair> transitions;
        public Graph graph;
        public AutomatClass()
        {
            graph = new Graph();
        }
        public bool init(List<Pair> transitions, List<Pair> outputs)
        {
            if (transitions.SelectMany(t => new int[] { t.first, t.second }).Any(t => t < 1 || t > transitions.Count))
            {
                return false;
            }
            this.outputs = outputs;
            this.transitions = transitions;
            graph.init(transitions, outputs);
            return true;
        }
        public AutomatClass minimize()
        {
            AutomatClass minimized = new AutomatClass();
            List<List<State>> partitions = new List<List<State>>();
            for (int i = 0; i < outputs.Count; i++)
            {
                List<State> group = partitions.FirstOrDefault(p => p.Any(s => s.outputs == outputs[i]));
                var state = new State() { number = i + 1, outputs = outputs[i], transitions = transitions[i] };
                if (group != null)
                    group.Add(state);
                else
                    partitions.Add(new List<State>() { state });
            }
            int partitionCount = 0;
            while (partitionCount != partitions.Count)
            {
                partitionCount = partitions.Count;
                var newPartitions = new List<List<State>>();
                foreach (var state in partitions.SelectMany(p => p))
                {
                    var group = newPartitions.FirstOrDefault(p => p.Any(pp => partitions.Any(part => part.Select(st => st.number).Contains(pp.transitions.first) && part.Select(st => st.number).Contains(state.transitions.first)) && partitions.Any(part => part.Select(st => st.number).Contains(pp.transitions.second) && part.Select(st => st.number).Contains(state.transitions.second))));
                    if (group != null)
                        group.Add(state);
                    else
                        newPartitions.Add(new List<State> { state });
                }
                partitions = newPartitions;
            }
            var newStates = partitions.Select(p=>p.First()).ToList();
            newStates.ForEach(ns => { ns.transitions.first = partitions.IndexOf(partitions.First(p => p.Any(pp => pp.number == ns.transitions.first))) + 1; ns.transitions.second = partitions.IndexOf(partitions.First(p => p.Any(pp => pp.number == ns.transitions.second))) + 1; });
            minimized.init(newStates.Select(s => s.transitions).ToList(), newStates.Select(s => s.outputs).ToList());
            return minimized;
        }
        public int memorySize(MemoryType mem)
        {
            int matrixSize = ((outputs.Count) * (outputs.Count - 1)) / 2;
            bool[,] matrix = new bool[matrixSize, matrixSize];
            Dictionary<Pair, int> rows = new Dictionary<Pair, int>();
            int k = 0;
            for (int i = 0; i < outputs.Count - 1; i++)
                for (int j = i + 1; j < outputs.Count; j++)
                {
                    rows.Add(new Pair() { first = i, second = j }, k);
                    k++;
                }
            foreach (var row in rows)
            {
                if (mem == MemoryType.Input)
                {
                    //По 0
                    int fromi = transitions[row.Key.first].first - 1;
                    int fromj = transitions[row.Key.second].first - 1;
                    if (fromi != fromj)
                        matrix[row.Value, rows[new Pair() { first = Math.Min(fromi, fromj), second = Math.Max(fromi, fromj) }]] = true;
                    //По 1
                    fromi = transitions[row.Key.first].second - 1;
                    fromj = transitions[row.Key.second].second - 1;
                    if (fromi != fromj)
                        matrix[row.Value, rows[new Pair() { first = Math.Min(fromi, fromj), second = Math.Max(fromi, fromj) }]] = true;
                }
                else if (mem == MemoryType.Output)
                {
                    int i = row.Key.first;
                    int j = row.Key.second;
                    int fromi;
                    int fromj;
                    if (outputs[i].first == outputs[j].first)
                    {
                        fromi = transitions[i].first - 1;
                        fromj = transitions[j].first - 1;
                        if (fromi != fromj)
                            matrix[row.Value, rows[new Pair() { first = Math.Min(fromi, fromj), second = Math.Max(fromi, fromj) }]] = true;
                    }
                    if (outputs[i].first == outputs[j].second)
                    {
                        fromi = transitions[i].first - 1;
                        fromj = transitions[j].second - 1;
                        if (fromi != fromj)
                            matrix[row.Value, rows[new Pair() { first = Math.Min(fromi, fromj), second = Math.Max(fromi, fromj) }]] = true;
                    }
                    if (outputs[i].second == outputs[j].first)
                    {
                        fromi = transitions[i].second - 1;
                        fromj = transitions[j].first - 1;
                        if (fromi != fromj)
                            matrix[row.Value, rows[new Pair() { first = Math.Min(fromi, fromj), second = Math.Max(fromi, fromj) }]] = true;
                    }
                    if (outputs[i].second == outputs[j].second)
                    {
                        fromi = transitions[i].second - 1;
                        fromj = transitions[j].second - 1;
                        if (fromi != fromj)
                            matrix[row.Value, rows[new Pair() { first = Math.Min(fromi, fromj), second = Math.Max(fromi, fromj) }]] = true;
                    }
                }
                else
                {
                    //По входу 0
                    int fromi = transitions[row.Key.first].first - 1;
                    int fromj = transitions[row.Key.second].first - 1;
                    if (outputs[row.Key.first].first == outputs[row.Key.second].first && fromi != fromj)
                        matrix[row.Value, rows[new Pair() { first = Math.Min(fromi, fromj), second = Math.Max(fromi, fromj) }]] = true;
                    //По входу 1
                    fromi = transitions[row.Key.first].second - 1;
                    fromj = transitions[row.Key.second].second - 1;
                    if (outputs[row.Key.first].second == outputs[row.Key.second].second &&fromi != fromj)
                        matrix[row.Value, rows[new Pair() { first = Math.Min(fromi, fromj), second = Math.Max(fromi, fromj) }]] = true;
                }
            }
            List<int> toRemove = new List<int>();
            k = 0;
            do
            {
                toRemove.Clear();
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    bool allfalse = true;
                    for (int j = 0; j < matrix.GetLength(1); j++)
                    {
                        if (matrix[i, j])
                        {
                            allfalse = false;
                            break;
                        }
                    }
                    if (allfalse)
                        toRemove.Add(i);
                }
                matrix = reduceMatrix(matrix, toRemove);
                k++;
            }
            while (toRemove.Count > 0 && matrix.GetLength(0) > 0);
            if (matrix.GetLength(0) == 0)
                return k;
            return 0;
        }

        bool[,] reduceMatrix(bool[,] matrix, List<int> toRemove)
        {
            bool[,] result = new bool[matrix.GetLength(0) - toRemove.Count, matrix.GetLength(1) - toRemove.Count];
            for (int i = 0, k1 = 0; i < matrix.GetLength(0); i++)
            {
                if (toRemove.Contains(i))
                    continue;
                for (int j = 0, k2 = 0; j < matrix.GetLength(1); j++)
                {
                    if (toRemove.Contains(j))
                        continue;
                    result[k1, k2] = matrix[i, j];
                    k2++;
                }
                k1++;
            }
            return result;
        }
    }

    public enum MemoryType
    {
        Input, Output, Both
    }

    #region Stuff
    public struct MovingParameters
    {
        private bool _isMoving;
        public bool isMoving
        {
            get { return _isMoving; }
            set { _isMoving = value; Console.WriteLine(value); }
        }
        public Point coord;
        public Ellipse instance;
    }
    public class Pair
    {
        public int first { get; set; }
        public int second { get; set; }
        public override bool Equals(object obj)
        {
            if (obj is Pair)
                return this == (Pair)obj;
            else
                return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return first.GetHashCode() ^ second.GetHashCode();
        }
        public static bool operator==(Pair p1, Pair p2)
        {
            return ((p1.first == p2.first) && (p1.second == p2.second));
        }
        public static bool operator!=(Pair p1, Pair p2)
        {
            return !(p1 == p2);
        }
        public override String ToString()
        {
            return String.Format("{0}; {1}", first.ToString(), second.ToString());
        }
    }
    public class State
    {
        public int number;
        public Pair outputs;
        public Pair transitions;
    }
    #endregion

    #region Graph
    public class Vertice
    {
        public int number;
        public Ellipse shape;
        public Label text;
        public List<Edge> edges;
        public Vertice(int num)
        {
            number = num;
            edges = new List<Edge>();
        }
        public void addEdge(Edge edge)
        {
            edges.Add(edge);
        }
    }
    public class Edge
    {
        public Vertice start;
        public Vertice end;
        public String[] inputs;
        public String[] outputs;
        public Shape line;
        public Label text;
        public bool twoSided;
        public String labelText
        {
            get
            {
                String t = "";
                for (int i=0; i<inputs.Length; i++)
                {
                    t += inputs[i] + "/" + outputs[i] + ((i == inputs.Length - 1)?"":";");
                }
                return t;
            }
        }
        public bool isLoop
        {
            get
            {
                return start == end;
            }
        }
        public Edge(Vertice v1, Vertice v2)
        {
            start = v1;
            end = v2;
            v1.addEdge(this);
            v2.addEdge(this);
        }

    }
    public class Graph
    {
        public List<Vertice> vertices;
        public List<Edge> edges
        {
            get
            {
                return vertices.SelectMany(x => x.edges).Distinct().ToList();
            }
        }
        public void init(List<Pair> transitions, List<Pair> outputs)
        {
            clear();
            for (int i = 0; i < transitions.Count; i++)
                vertices.Add(new Vertice(i + 1));

            String input;
            String output;
            Vertice start;
            Vertice end;
            for (int i = 0; i < transitions.Count; i++)
            {
                start = vertices[i];
                for (int k = 0; k < 2; k++)
                {
                    input = k.ToString();
                    if (k == 0)
                    {
                        output = outputs[i].first.ToString();
                        end = vertices[transitions[i].first - 1];
                    }
                    else
                    {
                        output = outputs[i].second.ToString();
                        end = vertices[transitions[i].second - 1];
                    }

                    var dupl = edges.FirstOrDefault(e => e.start == start && e.end == end);
                    Edge cur;
                    if (dupl == null)
                    {
                        cur = new Edge(start, end) { inputs = new String[] { input }, outputs = new String[] { output } };
                    }
                    else
                    {
                        cur = dupl;
                        bool copiesFound = false;
                        for (int j = 0; j < dupl.inputs.Length; j++)
                            if (dupl.inputs[j] == input && dupl.outputs[j] == output)
                            {
                                copiesFound = true;
                                break;
                            }
                        if (!copiesFound)
                        {
                            Array.Resize<String>(ref dupl.inputs, dupl.inputs.Length + 1);
                            dupl.inputs[dupl.inputs.Length - 1] = input;
                            Array.Resize<String>(ref dupl.outputs, dupl.outputs.Length + 1);
                            dupl.outputs[dupl.outputs.Length - 1] = output;
                        }
                    }
                    var rev = edges.FirstOrDefault(e => e.start == end && e.end == start);
                    if (rev != null)
                    {
                        rev.twoSided = true;
                        cur.twoSided = true;
                    }
                }
            }
        }
        void clear()
        {
            if (vertices == null)
                vertices = new List<Vertice>();
            else
                vertices.Clear();
        }
    }
    public class ArrowCircle : Shape
    {
        public double angle
        {
            get { return (double)this.GetValue(angleProperty); }
            set { this.SetValue(angleProperty, value); }
        }

        public double arrowTiltAngle = 25;

        public static readonly DependencyProperty angleProperty = System.Windows.DependencyProperty.Register(
            "angle",
            typeof(double),
            typeof(ArrowCircle),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));
        protected override Geometry DefiningGeometry
        {
            get
            {
                double radius = Width / 2.0 - 5;

                //Конец стрелки
                double X2 = radius * (1 - Math.Cos(angle * Math.PI / 180));
                double Y2 = radius * (1 - Math.Sin(angle * Math.PI / 180));

                //Секущая
                double X1 = radius * (1 - Math.Cos((angle - arrowTiltAngle) * Math.PI / 180));
                double Y1 = radius * (1 - Math.Sin((angle - arrowTiltAngle) * Math.PI / 180));

                //Длина секущей
                double d = Math.Sqrt(Math.Pow(X2 - X1, 2) + Math.Pow(Y2 - Y1, 2));

                // координаты конца стрелки
                double X3 = X2;
                double Y3 = Y2;

                // координаты вектора
                double X = X2 - X1;
                double Y = Y2 - Y1;

                // координаты точки, удалённой от конца стрелки к началу перпендикуляра на 10px
                double X4 = X3 - (X / d) * 10;
                double Y4 = Y3 - (Y / d) * 10;

                // из уравнения прямой { (x - x1)/(x1 - x2) = (y - y1)/(y1 - y2) } получаем вектор перпендикуляра
                // (x - x1)/(x1 - x2) = (y - y1)/(y1 - y2) =>
                // (x - x1)*(y1 - y2) = (y - y1)*(x1 - x2) =>
                // (x - x1)*(y1 - y2) - (y - y1)*(x1 - x2) = 0 =>
                // полученные множители x и y => координаты вектора перпендикуляра
                double Xp = Y2 - Y1;
                double Yp = X1 - X2;

                // координаты перпендикуляров, удалённой от точки X4;Y4 на 5px в разные стороны
                double X5 = X4 + (Xp / d) * 5;
                double Y5 = Y4 + (Yp / d) * 5;
                double X6 = X4 - (Xp / d) * 5;
                double Y6 = Y4 - (Yp / d) * 5;

                GeometryGroup geometryGroup = new GeometryGroup();

                EllipseGeometry ellipseGeometry = new EllipseGeometry(new Point(radius, radius), radius, radius);
                //LineGeometry lineGeometry = new LineGeometry(new Point(this.X1, this.Y1), new Point(this.X2, this.Y2));
                LineGeometry arrowPart1Geometry = new LineGeometry(new Point(X3, Y3), new Point(X5, Y5));
                LineGeometry arrowPart2Geometry = new LineGeometry(new Point(X3, Y3), new Point(X6, Y6));

                geometryGroup.Children.Add(ellipseGeometry);
                geometryGroup.Children.Add(arrowPart1Geometry);
                geometryGroup.Children.Add(arrowPart2Geometry);
                //geometryGroup.Children.Add(new LineGeometry(new Point(X1, Y1), new Point(X3, Y3)));
           //     geometryGroup.Children.Add(new LineGeometry(new Point(radius, radius), new Point()));

                return geometryGroup;
            }
        }
    }

    public class ArrowLine : Shape
    {
        public double X1
        {
            get { return (double)this.GetValue(X1Property); }
            set { this.SetValue(X1Property, value); }
        }

        public static readonly DependencyProperty X1Property = System.Windows.DependencyProperty.Register(
            "X1",
            typeof(double),
            typeof(ArrowLine),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public double Y1
        {
            get { return (double)this.GetValue(Y1Property); }
            set { this.SetValue(Y1Property, value); }
        }

        public static readonly DependencyProperty Y1Property = System.Windows.DependencyProperty.Register(
            "Y1",
            typeof(double),
            typeof(ArrowLine),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public double X2
        {
            get { return (double)this.GetValue(X2Property); }
            set { this.SetValue(X2Property, value); }
        }

        public static readonly DependencyProperty X2Property = System.Windows.DependencyProperty.Register(
            "X2",
            typeof(double),
            typeof(ArrowLine),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public double Y2
        {
            get { return (double)this.GetValue(Y2Property); }
            set { this.SetValue(Y2Property, value); }
        }

        public static readonly DependencyProperty Y2Property = System.Windows.DependencyProperty.Register(
            "Y2",
            typeof(double),
            typeof(ArrowLine),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public double offset
        {
            get;
            set;
        }

        public double Length
        {
            get
            {
                return Math.Sqrt(Math.Pow(this.X2 - this.X1, 2) + Math.Pow(this.Y2 - this.Y1, 2));
            }
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                // длина отрезка
                double d = Length;
                double ratio = (d - offset) / d;

                // координаты конца стрелки
                double X3 = X1 + (X2 - X1) * ratio; //(this.X1 + this.X2) / 2;
                double Y3 = Y1 + (Y2 - Y1) * ratio; //(this.Y1 + this.Y2) / 2;

                // координаты вектора
                double X = this.X2 - this.X1;
                double Y = this.Y2 - this.Y1;

                // координаты точки, удалённой от конца стрелки к началу отрезка на 10px
                double X4 = X3 - (X / d) * 10;
                double Y4 = Y3 - (Y / d) * 10;

                // из уравнения прямой { (x - x1)/(x1 - x2) = (y - y1)/(y1 - y2) } получаем вектор перпендикуляра
                // (x - x1)/(x1 - x2) = (y - y1)/(y1 - y2) =>
                // (x - x1)*(y1 - y2) = (y - y1)*(x1 - x2) =>
                // (x - x1)*(y1 - y2) - (y - y1)*(x1 - x2) = 0 =>
                // полученные множители x и y => координаты вектора перпендикуляра
                double Xp = this.Y2 - this.Y1;
                double Yp = this.X1 - this.X2;

                // координаты перпендикуляров, удалённой от точки X4;Y4 на 5px в разные стороны
                double X5 = X4 + (Xp / d) * 5;
                double Y5 = Y4 + (Yp / d) * 5;
                double X6 = X4 - (Xp / d) * 5;
                double Y6 = Y4 - (Yp / d) * 5;

                //       // координаты перпендикуляров, удалённой от точки X4;Y4 на 5px в разные стороны
                //       double X52 = (X2 + X1) / 2 - (Xp / Length) * 10;
                //       double Y52 = (Y2 + Y1) / 2 - (Yp / Length) * 10;

                GeometryGroup geometryGroup = new GeometryGroup();

                LineGeometry lineGeometry = new LineGeometry(new Point(this.X1, this.Y1), new Point(this.X2, this.Y2));
                LineGeometry arrowPart1Geometry = new LineGeometry(new Point(X3, Y3), new Point(X5, Y5));
                LineGeometry arrowPart2Geometry = new LineGeometry(new Point(X3, Y3), new Point(X6, Y6));

                geometryGroup.Children.Add(lineGeometry);
                geometryGroup.Children.Add(arrowPart1Geometry);
                geometryGroup.Children.Add(arrowPart2Geometry);
                //        geometryGroup.Children.Add(new EllipseGeometry(new Point((X1 + X2) / 2, (Y1 + Y2) / 2), 10, 10));
                //        geometryGroup.Children.Add(new LineGeometry(new Point((X1 + X2) / 2, (Y1 + Y2) / 2), new Point(X52, Y52)));

                return geometryGroup;
            }
        }
    }

    public class RegisterPic : Shape
    {
        public double X1
        {
            get { return (double)this.GetValue(X1Property); }
            set { this.SetValue(X1Property, value); }
        }

        public static readonly DependencyProperty X1Property = System.Windows.DependencyProperty.Register(
            "X1",
            typeof(double),
            typeof(RegisterPic),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public double Y1
        {
            get { return (double)this.GetValue(Y1Property); }
            set { this.SetValue(Y1Property, value); }
        }

        public static readonly DependencyProperty Y1Property = System.Windows.DependencyProperty.Register(
            "Y1",
            typeof(double),
            typeof(RegisterPic),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));


        public int inputMemory;
        public int outputMemory;
        void drawArrow(GeometryGroup group, Point start, Point end)
        {
            Point arrowwing1;
            Point arrowwing2;
            if (start.X > end.X)
            {
                arrowwing1 = new Point(end.X + 5, end.Y - 5);
                arrowwing2 = new Point(end.X + 5, end.Y + 5);
            }
            else if (start.X < end.X)
            {
                arrowwing1 = new Point(end.X - 5, end.Y - 5);
                arrowwing2 = new Point(end.X - 5, end.Y + 5);
            }
            else
            {
                if (start.Y > end.Y)
                {
                    arrowwing1 = new Point(end.X - 5, end.Y + 5);
                    arrowwing2 = new Point(end.X + 5, end.Y + 5);
                }
                else
                {
                    arrowwing1 = new Point(end.X - 5, end.Y - 5);
                    arrowwing2 = new Point(end.X + 5, end.Y - 5);
                }
            }
            group.Children.Add(new LineGeometry(start, end));
            group.Children.Add(new LineGeometry(end, arrowwing1));
            group.Children.Add(new LineGeometry(end, arrowwing2));
        }
        protected override Geometry DefiningGeometry
        {
            get
            {
                int height = 50;
                int squareside = 20;
                int offset = 10;

                Double Length = squareside * (inputMemory + outputMemory + (((outputMemory != 0) && (inputMemory != 0)) ? 2 : 1));
                //Width = Length;

                Point nose = new Point(X1 + Length / 2.0, Y1 + height);
                Point peak = new Point(nose.X, nose.Y + 20);

                GeometryGroup geometryGroup = new GeometryGroup();

                //Общая геометрия
                LineGeometry lineGeometry = new LineGeometry(new Point(this.X1, this.Y1), new Point(this.X1 + Length, this.Y1));
                LineGeometry line2 = new LineGeometry(new Point(X1, Y1), new Point(X1 + Length / 2.0, Y1 + height));
                LineGeometry line3 = new LineGeometry(new Point(X1 + Length, Y1), nose);
                LineGeometry line4 = new LineGeometry(nose, peak);
                double max_x = peak.X + Length / 2.0 + 2 * squareside;

                double min_x = X1 - squareside;
                drawArrow(geometryGroup, peak, new Point(peak.X - (Length / 2.0 + 2 * squareside), peak.Y));
                geometryGroup.Children.Add(lineGeometry);
                geometryGroup.Children.Add(line2);
                geometryGroup.Children.Add(line3);
                geometryGroup.Children.Add(line4);

                if (inputMemory == 0)
                {
                    if (outputMemory != 0)
                    {
                        for (int i = 0; i < outputMemory; i++)
                        {
                            geometryGroup.Children.Add(new RectangleGeometry(new Rect(X1 + i * squareside, Y1 - offset - squareside, squareside, squareside)));
                        }

                        LineGeometry line6 = new LineGeometry(new Point(peak.X - Length / 2.0 - squareside, peak.Y), new Point(peak.X - Length / 2.0 - squareside, Y1 - offset - (squareside * 1.5)));
                        LineGeometry line7 = new LineGeometry(new Point(peak.X - Length / 2.0 - squareside, Y1 - offset - (squareside * 1.5)), new Point(X1 + squareside * (outputMemory + 0.5), Y1 - offset - (squareside * 1.5)));
                        LineGeometry line8 = new LineGeometry(new Point(X1 + squareside * (outputMemory + 0.5), Y1 - offset - (squareside * 1.5)), new Point(X1 + squareside * (outputMemory + 0.5), Y1 - offset - squareside / 2.0 - 2));
                        drawArrow(geometryGroup, new Point(X1 + squareside * (outputMemory + 0.5), Y1 - offset - squareside / 2.0 - 2), new Point(X1 + squareside * outputMemory, Y1 - offset - squareside / 2.0 - 2));
                        geometryGroup.Children.Add(new LineGeometry(new Point(X1 + Length + squareside, Y1 - offset - squareside / 2.0), new Point(X1 + Length - squareside / 2.0, Y1 - offset - squareside / 2.0)));
                        geometryGroup.Children.Add(line6);
                        geometryGroup.Children.Add(line7);
                        geometryGroup.Children.Add(line8);
                    }
                }
                else if (outputMemory == 0)
                {
                    if (inputMemory != 0)
                    {
                        for (int i = 0; i < inputMemory; i++)
                        {
                            geometryGroup.Children.Add(new RectangleGeometry(new Rect(X1 + i * squareside, Y1 - offset - squareside, squareside, squareside)));
                        }
                        drawArrow(geometryGroup, new Point(X1 + Length + squareside, Y1 - offset - squareside / 2.0), new Point(X1 + Length - squareside, Y1 - offset - squareside / 2.0));
                    }
                }
                else
                {
                    for (int i = 0; i < outputMemory; i++)
                    {
                        geometryGroup.Children.Add(new RectangleGeometry(new Rect(X1 + i * squareside, Y1 - offset - squareside, squareside, squareside)));
                    }
                    for (int i = 0; i < inputMemory; i++)
                    {
                        geometryGroup.Children.Add(new RectangleGeometry(new Rect(X1 + (outputMemory + i + 1) * squareside, Y1 - offset - squareside, squareside, squareside)));
                    }
                    LineGeometry line6 = new LineGeometry(new Point(peak.X - Length / 2.0 - squareside, peak.Y), new Point(peak.X - Length / 2.0 - squareside, Y1 - offset - (squareside * 1.5)));
                    LineGeometry line7 = new LineGeometry(new Point(peak.X - Length / 2.0 - squareside, Y1 - offset - (squareside * 1.5)), new Point(X1 + squareside * (outputMemory + 0.5), Y1 - offset - (squareside * 1.5)));
                    LineGeometry line8 = new LineGeometry(new Point(X1 + squareside * (outputMemory + 0.5), Y1 - offset - (squareside * 1.5)), new Point(X1 + squareside * (outputMemory + 0.5), Y1 - offset - squareside / 2.0));
                    drawArrow(geometryGroup, new Point(peak.X + Length / 2.0 + squareside, Y1 - offset - squareside / 2.0), new Point(peak.X + Length / 2.0 - squareside, Y1 - offset - squareside / 2.0));
                    geometryGroup.Children.Add(line6);
                    geometryGroup.Children.Add(line7);
                    geometryGroup.Children.Add(line8);
                    drawArrow(geometryGroup, new Point(X1 + squareside * (outputMemory + 0.5), Y1 - offset - squareside / 2.0), new Point(X1 + squareside * outputMemory, Y1 - offset - squareside / 2.0));
                }
                drawArrow(geometryGroup, new Point(X1 + Length - squareside / 2.0, Y1 - offset - squareside / 2.0), new Point(X1 + Length - squareside / 2.0, Y1));
                Width = max_x + min_x;
                return geometryGroup;
            }
        }
    }
#endregion

    #region Ban
    public class BanGraph
    {
        public BanGraphElement root;
        public List<BanGraphElement> allVertices;
        public List<BanGraphElement> bans;
        public void init(AutomatClass automat)
        {
            root = new BanGraphElement() { data = new List<int>(), states = Enumerable.Range(1, automat.transitions.Count).ToList() };
            allVertices = new List<BanGraphElement>();
            bans = new List<BanGraphElement>();
            Queue<BanGraphElement> thisLayer;
            Queue<BanGraphElement> nextLayer = new Queue<BanGraphElement>();
            allVertices.Add(root);
            nextLayer.Enqueue(root);
            while (nextLayer.Count > 0)
            {
                thisLayer = nextLayer;
                var cur = thisLayer.Dequeue();
                List<int> _0 = new List<int>();
                List<int> _1 = new List<int>();
                foreach (var state in cur.states.Select(s => s - 1))
                {
                    if (automat.outputs[state].first == 0)
                    {
                        _0.Add(automat.transitions[state].first);
                    }
                    else
                    {
                        _1.Add(automat.transitions[state].first);
                    }
                    if (automat.outputs[state].second == 0)
                    {
                        _0.Add(automat.transitions[state].second);
                    }
                    else
                    {
                        _1.Add(automat.transitions[state].second);
                    }

                }

                var __0 = new BanGraphElement() { states = new List<int>(_0.Distinct()), data = new List<int>(cur.data) };
                var __1 = new BanGraphElement() { states = new List<int>(_1.Distinct()), data = new List<int>(cur.data) };

                __0.data.Add(0);
                __1.data.Add(1);

                if (__0.states.Count == 0)
                    bans.Add(__0);
                else
                {
                    var to0 = allVertices.FirstOrDefault(v => v == __0);
                    if (!object.ReferenceEquals(to0, null))
                        cur._0 = to0;
                    else
                    {
                        allVertices.Add(__0);
                        cur._0 = __0;
                        nextLayer.Enqueue(__0);
                    }
                }
                if (__1.states.Count == 0)
                    bans.Add(__1);
                else
                {
                    var to1 = allVertices.FirstOrDefault(v => v == __1);
                    if (!object.ReferenceEquals(to1, null))
                        cur._1 = to1;
                    else
                    {
                        allVertices.Add(__1);
                        cur._1 = __1;
                        nextLayer.Enqueue(__1);
                    }
                } 
            }
        }
    }

    public class BanGraphElement
    {
        public List<int> data;
        public List<int> states;
        public BanGraphElement _0;
        public BanGraphElement _1;
        public static bool operator == (BanGraphElement b1, BanGraphElement b2)
        {
            return Enumerable.SequenceEqual(b1.states.OrderBy(t => t), b2.states.OrderBy(t => t));
        }
        public static bool operator != (BanGraphElement b1, BanGraphElement b2)
        {
            return !(b1 == b2);
        }
        public override String ToString()
        {
            return String.Join("", states);
        }
    }

#endregion

}
