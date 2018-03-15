using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Snake {
    public partial class Canvas : Form {


        private readonly Func<int,int,int,Bitmap> _renderer;
        private readonly Action<KeyEventArgs> _handle;
        public Canvas(Func<int, int, int, Bitmap> renderer, Action<KeyEventArgs> handle): this() {
            _renderer = renderer;
            _handle = handle;
        }

        private Canvas() {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer
                      | ControlStyles.ResizeRedraw
                      | ControlStyles.Selectable
                      | ControlStyles.AllPaintingInWmPaint
                      | ControlStyles.UserPaint
                      | ControlStyles.SupportsTransparentBackColor,
                    true);
        }


        protected override void OnKeyDown(KeyEventArgs e){
            base.OnKeyDown(e);
            _handle(e);
        }

        private int _frames;
        protected override void OnPaint(PaintEventArgs e){
            //base.OnPaint(e);
            using (var buff = _renderer(ClientSize.Width, ClientSize.Height,_frames++)){
                e.Graphics.DrawImage(buff,0,0);
            }
            var delay = Environment.TickCount + 16;//1000 / 60 FPS;
            while (Environment.TickCount<delay){
                Thread.Sleep(1);
            }
            Invalidate();
        }

        protected override void OnPaintBackground(PaintEventArgs e){
            //base.OnPaintBackground(e);
        }
    }
}
