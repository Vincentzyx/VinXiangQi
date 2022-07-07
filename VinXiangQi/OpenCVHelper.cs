using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using OpenCvSharp;
using System.Diagnostics;

namespace VinXiangQi
{
    public class OpenCVHelper
    {
        public static List<System.Drawing.Rectangle> MatchTemplate(System.Drawing.Bitmap source, System.Drawing.Bitmap template, double threshold=0.8)
        {

            //var templateMat = OpenCvSharp.Extensions.BitmapConverter.ToMat(template);
            //var sourceMat = OpenCvSharp.Extensions.BitmapConverter.ToMat(source);
            //var result = sourceMat.MatchTemplate(templateMat, TemplateMatchModes.CCoeffNormed);
            //List<System.Drawing.Rectangle> MatchList = new List<System.Drawing.Rectangle>();
            //while (true)
            //{
            //    double minval, maxval;
            //    Point minloc, maxloc;
            //    Cv2.MinMaxLoc(result, out minval, out maxval, out minloc, out maxloc);

            //    if (maxval >= threshold)
            //    {
            //        System.Drawing.Rectangle r = new System.Drawing.Rectangle(maxloc.X, maxloc.Y,templateMat.Width, templateMat.Height);
            //        MatchList.Add(r);
            //        Rect outRect;
            //        break;
            //        Cv2.FloodFill(result, maxloc, new Scalar(0), out outRect, new Scalar(0.1), new Scalar(1.0));
            //    }
            //    else
            //        break;
            //}
            //sourceMat.Dispose();
            //templateMat.Dispose();
            //result.Dispose();
            //return MatchList;
            return null;
        }
    }
}
