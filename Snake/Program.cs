using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Snake {
    static class Program {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(){
            const int scale = 40;
            const int speed = 8; // 60 frames = 1 sec.
            var init = new Func<List<Point>>(() => new List<Point> {
                new Point(4, 0),
                new Point(3, 0),
                new Point(2, 0),
                new Point(1, 0),
                new Point(0, 0),
            });
            var rnd = new Random(Guid.NewGuid().ToByteArray().Aggregate(0, (current, b) => current + b));
            var genFood = new Func<Point>(() => new Point(rnd.Next(scale - 1), rnd.Next(scale - 1)));
            var snake = init();
            var food = genFood();
            var playing = 0;
            var direction = "Right";
            var handle = new Action<KeyEventArgs>(ev =>{
                var keyCh = ev.KeyCode.ToString();
                if (!new[] { "Up", "Down", "Left", "Right", "Space" }.Contains(keyCh)) return;
                if ("Up" == keyCh && "Down" == direction) return;
                if ("Down" == keyCh && "Up" == direction) return;
                if ("Left" == keyCh && "Right" == direction) return;
                if ("Right" == keyCh && "Left" == direction) return;
                if ("Space" == keyCh && playing != 1){
                    playing = playing == 2 ? 0 : 1;
                    snake = init();
                    direction = "Right";
                    food = genFood();
                    return;
                }
                direction = keyCh;
            });
            var lastFrames = 0;
            var fps = 0;
            var ticks = 0;
            var renderer = new Func<int, int, int, Bitmap>((width, height,frames) => {
                if (Environment.TickCount > ticks) {
                    fps = frames - lastFrames;
                    lastFrames = frames;
                    ticks = 1000 + Environment.TickCount;
                }
                var buff = new Bitmap(width,height);
                using(var g=Graphics.FromImage(buff)){
                    if (0 == frames%speed){
                        if (1==playing&&direction !="Space") {
                            var head = snake.FirstOrDefault();
                            var next = new Point(head.X,head.Y);
                            next = new Dictionary<string, Func<Point, Point>> {
                                {
                                    "Up",p => {
                                        p.Y -= 1;
                                        return p;
                                    }
                                },{
                                    "Down",p => {
                                        p.Y += 1;
                                        return p;
                                    }
                                },{
                                    "Left",p => {
                                        p.X -= 1;
                                        return p;
                                    }
                                },{
                                    "Right",p => {
                                        p.X += 1;
                                        return p;
                                    }
                                }
                            }[direction](next);

                            if (next.X < 0 || next.Y < 0 ||
                                next.X > scale - 1 || next.Y > scale - 1||
                                snake.Contains(next)
                                )
                            {
                                playing = 2;
                            }
                            if(playing==1){
                                snake.Insert(0,next);
                                if (next.X == food.X &&
                                    next.Y == food.Y) {
                                    food = genFood();
                                }else{
                                    snake.RemoveAt(snake.Count-1);
                                }
                            }
                        }//if(playing)
                    }
                    const int padding = 15;
                    var grid = (Math.Min(width,height)-padding)/scale;
                    using(var pen=new Pen(Color.DimGray)){
                        using (var brush = new SolidBrush(Color.White)){
                            switch (playing){
                                case 0:
                                    using (var brush1 = new SolidBrush(Color.Yellow)) {
                                        using (var font1 = new Font(FontFamily.GenericMonospace, 30)) {
                                            const string text = "Snake";
                                            var size = g.MeasureString(text, font1);
                                            g.DrawString(text, font1, brush1, (width - size.Width) / 2, (height - size.Height) / 2);
                                        }
                                    }
                                    using (var brush1 = new SolidBrush(Color.White)) {
                                        using (var font1 = new Font(FontFamily.GenericMonospace, 20)) {
                                            const string text = "press [space] to start";
                                            var size = g.MeasureString(text, font1);
                                            g.DrawString(text, font1, brush1, (width - size.Width) / 2, (height - size.Height) / 2+50);
                                        }
                                    }
                                    break;
                                case 1:
                                    for (var j = 0; j < scale; j++) {
                                        for (var i = 0; i < scale; i++){
                                            g.DrawRectangle(pen, padding + i*grid, padding + j*grid, grid, grid);
                                            if (i == food.X && j == food.Y){
                                                using (var brush2 = new SolidBrush(Color.Red)){
                                                    g.FillRectangle(brush2, padding + i * grid + 2, padding + j * grid + 2, grid - 4, grid - 4);
                                                }
                                            }
                                            if (snake.Contains(new Point(i, j))) {
                                                g.FillRectangle(brush, padding+i * grid + 2, padding+j * grid + 2, grid - 4, grid - 4);
                                            }
                                        }
                                    }
                                    using(var font2 = new Font(FontFamily.GenericMonospace, 10)){
                                        g.DrawString(string.Format("fps:{0:d3}",fps),font2, brush,0,0);
                                    }
                                    break;
                                case 2:
                                    using (var brush3 = new SolidBrush(Color.Yellow)) {
                                        using (var font3 = new Font(FontFamily.GenericMonospace, 30)) {
                                            const string text = "Game Over";
                                            var size = g.MeasureString(text, font3);
                                            g.DrawString(text, font3, brush3, (width - size.Width) / 2, (height - size.Height) / 2);
                                        }
                                    }
                                    break;
                            }//switch(playing)
                        }//using(var brush
                    }//using(var pen
                }//using(var g
                return buff;
            });//renderer
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Canvas(renderer,handle));
        }
    }
}