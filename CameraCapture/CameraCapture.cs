using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;

namespace CameraCapture
{
    public partial class CameraCapture : Form
    {
        //Declare global variables
        private Capture capture;        //Takes images from camera as frames
        private bool captureInProgress;

        public CameraCapture()
        {
            InitializeComponent();
        }

        /**
         * Creates an EmguCV Image, captures a frame from camera and 
         * allocates it to the imageFrame, and displays it in ImageBox
         */
        private void processFrame(object sender, EventArgs arg)
        {
            Image<Bgr, Byte> imageFrame = capture.QueryFrame();
            Image<Gray, Byte> gray = imageFrame.Convert<Gray, Byte>().PyrDown().PyrUp();

            gray = getColorRegions(imageFrame, new Bgr(33, 65, 85), new Bgr(80, 119, 140));
            imageFrame = getCircles(gray);
            camImageBox.Image = imageFrame;
        }

        /**
         * Find regions with color values between lower and upper Bgr bounds
         */
        private Image<Gray, Byte> getColorRegions(Image<Bgr, Byte> img, Bgr lower, Bgr upper)
        {
            Image<Gray, Byte> blobs = img.InRange(lower, upper);

            return blobs;
        }

        /**
         * Find circles from a Bgr Image
         */
        private Image<Bgr, Byte> getCircles(Image<Bgr, Byte> img)
        {
            Image<Gray, Byte> gray = img.Convert<Gray, Byte>().PyrDown().PyrUp();
            Gray cannyThreshold = new Gray(180);
            Gray circleAccumulatorThreshold = new Gray(120);
            CircleF[] circles = gray.HoughCircles(cannyThreshold, circleAccumulatorThreshold, 2, 50, 5, 100)[0];

            foreach (CircleF circle in circles)
            {
                img.Draw(circle, new Bgr(Color.Red), 2);
            }

            return img;
        }

        /**
         * Find circles from a Gray image
         */
        private Image<Bgr, Byte> getCircles(Image<Gray, Byte> img)
        {
            Image<Bgr, Byte> color = img.Convert<Bgr, Byte>().PyrDown().PyrUp();
            Gray cannyThreshold = new Gray(180);
            Gray circleAccumulatorThreshold = new Gray(120);
            CircleF[] circles = img.HoughCircles(cannyThreshold, circleAccumulatorThreshold, 3, 25, 5, 100)[0];

            foreach (CircleF circle in circles)
            {
                color.Draw(circle, new Bgr(Color.Red), 2);
            }

            return color;
        }

        /**
         * Toggles captureInProgress and starts/stops capture
         */
        private void btnStart_Click(object sender, EventArgs e)
        {
            #region Instantiate new Capture if one doesn't already exist
            if (capture == null)
            {
                try
                {
                    capture = new Capture();
                }
                catch (NullReferenceException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            #endregion

            if (capture != null)
            {
                if (captureInProgress)      //Stop capture and reset btnStart text to "Start"
                {
                    btnStart.Text = "Start";
                    Application.Idle -= processFrame;
                }
                else                        //Start capture and set btnStart text to "Stop"
                {
                    btnStart.Text = "Stop";
                    Application.Idle += processFrame;
                }

                captureInProgress = !captureInProgress;     //Invert captureInProgress
            }
        }

        private void releaseData()
        {
            if (capture != null)
            {
                capture.Dispose();
            }
        }
    }
}
