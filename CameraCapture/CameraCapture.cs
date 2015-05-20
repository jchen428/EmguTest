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

//using DirectShowLib;

namespace CameraCapture
{
    public partial class CameraCapture : Form
    {
        //Declare global variables
        private Capture capture;        //Takes images from camera as frames
        private bool captureInProgress;
        private Image<Bgr, Byte> frame;
        
        public CameraCapture()
        {
            InitializeComponent();
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (fileNameTextBox.Text != String.Empty)
            {
                Image<Bgr, Byte> img =      //Resize the image to a more manageable size. Also reduces noise
                   new Image<Bgr, byte>(fileNameTextBox.Text)
                   .Resize(80, 60, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR, true);    //8x smaller
                rawImageBox.Image = img;

                Image<Bgr, Byte> cropped = cropToContour(img);
                char c = charRec(cropped);
                //processedImageBox.Image = cropped;
            }
        }

        private void loadImageButton_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK || result == DialogResult.Yes)
            {
                fileNameTextBox.Text = openFileDialog1.FileName;
            }
        }

        private void releaseData()
        {
            if (capture != null)
            {
                capture.Dispose();
            }
        }

        /**
         * Creates an Image, captures a frame from camera and displays it 
         * rawImageBox,and processes and displays the Image in processedImageBox
         */
        private void processFrame(object sender, EventArgs arg)
        {
            frame = capture.QueryFrame();
            rawImageBox.Image = frame;

            Image<Gray, Byte> gray = frame.Convert<Gray, Byte>().PyrDown().PyrUp();

            //gray = filterColor(frame, new Bgr(0, 116, 167), new Bgr(91, 222, 251));
            //frame = getCircles(gray);
            ////frame = getCorners(gray).Convert<Bgr, Byte>().PyrDown().PyrUp();

            processedImageBox.Image = cropToContour(frame);
        }

        /**
         * Find regions with color values between lower and upper Bgr bounds
         */
        private Image<Gray, Byte> filterColor(Image<Bgr, Byte> img, Bgr lower, Bgr upper)
        {
            Image<Gray, Byte> regions = img.InRange(lower, upper);

            return regions;
        }
        
        /**
         * Find circles from a Bgr Image
         */
        /*private Image<Bgr, Byte> getCircles(Image<Bgr, Byte> img)
        {
            Image<Gray, Byte> gray = img.Convert<Gray, Byte>().PyrDown().PyrUp();

            double cannyThreshold = 180.0;
            double circleAccumulatorThreshold = 50;

            CircleF[] circles = gray.HoughCircles(new Gray(cannyThreshold), new Gray(circleAccumulatorThreshold), 2.5, 50, 25, 150)[0];

            foreach (CircleF circle in circles)
            {
                frame.Draw(circle, new Bgr(Color.Red), 2);
            }

            return frame;
        }*/

        /**
         * Find circles from a Gray image
         */
        private Image<Bgr, Byte> getCircles(Image<Gray, Byte> gray)
        {
            Image<Bgr, Byte> bgrCopy = gray.Convert<Bgr, Byte>().PyrDown().PyrUp();
            double cannyThreshold = 180.0;
            double circleAccumulatorThreshold = 50;

            CircleF[] circles = gray.HoughCircles(new Gray(cannyThreshold), new Gray(circleAccumulatorThreshold), 2.5, 50, 25, 150)[0];

            foreach (CircleF circle in circles)
            {
                bgrCopy.Draw(circle, new Bgr(Color.Red), 2);
            }

            return bgrCopy;
        }

        /**
         * May be deprecated
         */
        private Image<Gray, Byte> getCorners(Image<Gray, Byte> gray)
        {
            Image<Gray, float> rawCorners = null;   //Raw corner strength image (must be 32-bit float)
            Image<Gray, Byte> dispCorners = null;   //Inverted threshold corner strengths (for display)

            //Create raw corner strength image and do Harris algorithm
            rawCorners = new Image<Gray, float>(gray.Size);
            CvInvoke.cvCornerHarris(gray, rawCorners, 3, 3, 0.01);

            //Create and return inverted threshold image
            dispCorners = new Image<Gray, Byte>(gray.Size);
            CvInvoke.cvThreshold(rawCorners, dispCorners, 0.0001, 255, Emgu.CV.CvEnum.THRESH.CV_THRESH_BINARY);

            return dispCorners;
        }

        /**
         * TUNE THE THRESHOLD VALUE
         */
        private Image<Bgr, Byte> cropToContour(Image<Bgr, Byte> color)
        {
            //Find and draw contours/bounding box
            int thresholdValue = 15;
            Image<Gray, Byte> gray = color.Convert<Gray, Byte>().PyrDown().PyrUp();
            gray = gray.ThresholdBinary(new Gray(thresholdValue), new Gray(255));
            Rectangle bound = new Rectangle();

            using (MemStorage storage = new MemStorage())
            {
                for (Contour<Point> contours = gray.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_TREE, storage);
                    contours != null; contours = contours.HNext)
                {
                    Contour<Point> curr = contours.ApproxPoly(contours.Perimeter * 0.015, storage);

                    //if (curr.BoundingRectangle.Width > 20)
                    //{
                        bound = curr.BoundingRectangle;
                        /*bound.X += (int) (bound.Width * 0.05);
                        bound.Y += (int) (bound.Height * 0.05);
                        bound.Width -= (int) (bound.Width * 0.1);
                        bound.Height -= (int)(bound.Height * 0.1);*/
                        //CvInvoke.cvDrawContours(color, contours, new MCvScalar(255), new MCvScalar(255), -1, 2, Emgu.CV.CvEnum.LINE_TYPE.EIGHT_CONNECTED, new Point(0, 0));
                        color.Draw(bound, new Bgr(0, 255, 0), 1);
                    //}
                }
            }

            color = color.Copy(bound);
            //Image<Gray, Byte> cropped = color.Convert<Gray, Byte>().PyrDown().PyrUp();

            return color;
        }

        private char charRec(Image<Bgr, Byte> img)
        {
            Image<Gray, Byte> three = new Image<Gray, Byte>("Samples/Ex3.png");
            //Console.WriteLine(Environment.CurrentDirectory);

            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    if (img[i, j].Red > 150)
                        img[i, j] = new Bgr(255, 255, 255);
                    else
                        img[i, j] = new Bgr(0, 0, 0);
                }
            }
            //img.Convert<Gray, Byte>().PyrDown().PyrUp();
            img = img.Resize(3, 5, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);

            processedImageBox.Image = img;

            return '0';
        }
    }
}
