using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp.Extensions;

namespace screen.capture
{
    public class ScreenCapture : IDisposable
    {
        bool _isStarted = false;
        bool _isDisposed = false;
        Thread _mainThread;
        OpenCvSharp.VideoWriter _writer;


        public ScreenCapture()
        {
            _mainThread = new Thread(new ThreadStart(MainThread));
            _mainThread.Priority = ThreadPriority.Normal;
            _mainThread.Start();
        }


        public bool IsStarted()
        {
            return _isStarted;
        }

        public void Dispose()
        {
            _isDisposed = true;

            if (_mainThread != null && _mainThread.IsAlive)
                _mainThread.Join();
        }

        public void Start()
        {
            _isStarted = true;
        }

        public void Stop()
        {
            _isStarted = false;
        }

        private void MainThread()
        {
            System.Drawing.Point resolution = new System.Drawing.Point(
                    Screen.PrimaryScreen.Bounds.Width,
                    Screen.PrimaryScreen.Bounds.Height
                );


            DateTime lastFrameStamp = DateTime.Now;
            float fps_timespan = 1000 / Constants.FPS;


            while(!_isDisposed)
            {
                if (_isStarted)
                {
                    if ((DateTime.Now - lastFrameStamp).TotalMilliseconds >= fps_timespan)
                    {
                        lastFrameStamp = DateTime.Now;

                        Bitmap bmp = new Bitmap(resolution.X, resolution.Y, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        Graphics g = Graphics.FromImage(bmp);
                        g.CopyFromScreen(0, 0, 0, 0, bmp.Size);


                        if (_writer == null)
                        {
                            if (!System.IO.Directory.Exists(Constants.SAVE_FOLDER))
                                System.IO.Directory.CreateDirectory(Constants.SAVE_FOLDER);

                            var fileName = System.IO.Path.Combine(
                                Constants.SAVE_FOLDER,
                                $"{DateTime.Now.ToString("M.d.yyyy_h.mm.ss")}.avi");

                            _writer = new OpenCvSharp.VideoWriter(
                                fileName,
                                OpenCvSharp.VideoWriter.FourCC('M', 'P', '4', 'V'),
                                30,
                                new OpenCvSharp.Size(resolution.X, resolution.Y)
                                );
                        }


                        _writer.Write(bmp.ToMat());
                    }
                }
                else
                {
                    CloseWriter();
                }

                Thread.Sleep(5);
            }

            CloseWriter();
        }



        private void CloseWriter()
        {
            if (_writer != null)
            {
                _writer.Release();
                _writer.Dispose();
                _writer = null;
            }
        }
    }
}
