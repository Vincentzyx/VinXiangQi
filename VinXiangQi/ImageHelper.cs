using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace VinXiangQi
{
    class ImageHelper
    {
        [DllImport("msvcrt.dll")]
        private static extern int memcmp(IntPtr b1, IntPtr b2, IntPtr count);

        public static bool CompareMemCmp(Bitmap b1, Bitmap b2)
        {
            if ((b1 == null) != (b2 == null)) return false;
            if (b1.Size != b2.Size) return false;

            var bd1 = b1.LockBits(new Rectangle(new Point(0, 0), b1.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var bd2 = b2.LockBits(new Rectangle(new Point(0, 0), b2.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try
            {
                IntPtr bd1scan0 = bd1.Scan0;
                IntPtr bd2scan0 = bd2.Scan0;
                int stride = bd1.Stride;
                int len = stride * b1.Height;
                return memcmp(bd1scan0, bd2scan0, (IntPtr)len) == 0;
            }
            finally
            {
                b1.UnlockBits(bd1);
                b2.UnlockBits(bd2);
            }
        }

        public static byte[] ShaHash(Image image)
        {
            var bytes = new byte[1];
            bytes = (byte[])(new ImageConverter()).ConvertTo(image, bytes.GetType());

            return (new SHA256Managed()).ComputeHash(bytes);
        }

        public static bool AreEqual(Image imageA, Image imageB)
        {
            if (imageA.Width != imageB.Width) return false;
            if (imageA.Height != imageB.Height) return false;

            var hashA = ShaHash(imageA);
            var hashB = ShaHash(imageB);

            return !hashA
                .Where((nextByte, index) => nextByte != hashB[index])
                .Any();
        }

        public static Image CropImage(Image img, Rectangle cropArea)
        {
            try
            {
                Bitmap bmpImage = new Bitmap(img);
                return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return new Bitmap(10, 10);
            }

        }

        public static bool DetectImage(Bitmap sample, Bitmap model)
        {

            int sWidth = sample.Width - model.Width, sHeight = sample.Height - model.Height;
            for (int y = 0; y < sHeight; y++)
            {
                for (int x = 0; x < sWidth; x++)
                {
                    if (CompareImage(sample, model, x, y, 30)) return true;
                }
            }
            return false;
        }

        public static Point FindImage(Bitmap sample, Bitmap model, int threshold = 30)
        {

            int sWidth = sample.Width - model.Width, sHeight = sample.Height - model.Height;
            for (int y = 0; y < sHeight; y++)
            {
                for (int x = 0; x < sWidth; x++)
                {
                    if (CompareImageFaster(sample, model, x, y, threshold)) return new Point(x, y);
                }
            }
            return new Point(-1, -1);
        }

        unsafe public static Point FindImageFromLeft(Bitmap sample, Bitmap model, int threshold = 30, int startFindX = 0, int startFindY = 0, int deltaX = int.MaxValue, int deltaY = int.MaxValue)
        {
            bool jumpOut = false;
            int endX = sample.Width - model.Width, endY = sample.Height - model.Height;
            endX = endX - startFindX <= deltaX ? endX : startFindX + deltaX;
            endY = endY - startFindY <= deltaY ? endY : startFindY + deltaY;
            int mWidth = model.Width;
            int mHeight = model.Height;
            int sWidth = sample.Width;
            int sHeight = sample.Height;
            BitmapData modelBMD = model.LockBits(new Rectangle(0, 0, model.Width, model.Height), ImageLockMode.ReadWrite, model.PixelFormat);
            BitmapData sampleBMD = sample.LockBits(new Rectangle(0, 0, sample.Width, sample.Height), ImageLockMode.ReadWrite, sample.PixelFormat);
            byte* sampleScan0 = (byte*)sampleBMD.Scan0;
            byte* modelScan0 = (byte*)modelBMD.Scan0;
            byte c1r, c1g, c1b, c2a, c2r, c2g, c2b;

            for (int startX = startFindX; startX < endX; startX++)
            {
                for (int startY = startFindY; startY < endY; startY++)
                {
                    for (int y = 0; y < mHeight; y++)
                    {
                        jumpOut = false;
                        if (y + startY >= sHeight) break;
                        for (int x = 0; x < mWidth; x++)
                        {
                            c2a = modelScan0[y * modelBMD.Stride + x * 4 + 3];
                            if (c2a != 0)
                            {
                                c1b = sampleScan0[(y + startY) * sampleBMD.Stride + (x + startX) * 4];
                                c1g = sampleScan0[(y + startY) * sampleBMD.Stride + (x + startX) * 4 + 1];
                                c1r = sampleScan0[(y + startY) * sampleBMD.Stride + (x + startX) * 4 + 2];
                                // c1a = sampleScan0[(y + startY) * sampleBMD.Stride + (x + startX) * 4 + 3];
                                c2b = modelScan0[y * modelBMD.Stride + x * 4];
                                c2g = modelScan0[y * modelBMD.Stride + x * 4 + 1];
                                c2r = modelScan0[y * modelBMD.Stride + x * 4 + 2];
                                if (Math.Abs(c1r - c2r) + Math.Abs(c1g - c2g) + Math.Abs(c1b - c2b) > threshold) { jumpOut = true; break; }
                            }
                        }
                        if (jumpOut) break;
                    }
                    if (jumpOut) continue;
                    sample.UnlockBits(sampleBMD);
                    model.UnlockBits(modelBMD);
                    return new Point(startX, startY);
                }
            }
            sample.UnlockBits(sampleBMD);
            model.UnlockBits(modelBMD);
            return new Point(-1, -1);
        }

        unsafe public static Point FindImageFromTop(Bitmap sample, Bitmap model, int threshold = 50, int startFindX = 0, int startFindY = 0, int deltaX = int.MaxValue, int deltaY = int.MaxValue)
        {
            bool jumpOut = false;
            int endX = sample.Width - model.Width, endY = sample.Height - model.Height;
            endX = endX - startFindX <= deltaX ? endX : startFindX + deltaX;
            endY = endY - startFindY <= deltaY ? endY : startFindY + deltaY;
            int mWidth = model.Width;
            int mHeight = model.Height;
            int sWidth = sample.Width;
            int sHeight = sample.Height;
            BitmapData modelBMD = model.LockBits(new Rectangle(0, 0, model.Width, model.Height), ImageLockMode.ReadWrite, model.PixelFormat);
            BitmapData sampleBMD = sample.LockBits(new Rectangle(0, 0, sample.Width, sample.Height), ImageLockMode.ReadWrite, sample.PixelFormat);
            byte* sampleScan0 = (byte*)sampleBMD.Scan0;
            byte* modelScan0 = (byte*)modelBMD.Scan0;
            byte c1r, c1g, c1b, c2a, c2r, c2g, c2b;

            for (int startY = startFindY; startY < endY; startY++)
            {
                for (int startX = startFindX; startX < endX; startX++)
                {
                    for (int y = 0; y < mHeight; y++)
                    {
                        jumpOut = false;
                        if (y + startY >= sHeight) break;
                        for (int x = 0; x < mWidth; x++)
                        {
                            c2a = modelScan0[y * modelBMD.Stride + x * 4 + 3];
                            if (c2a != 0)
                            {
                                c1b = sampleScan0[(y + startY) * sampleBMD.Stride + (x + startX) * 4];
                                c1g = sampleScan0[(y + startY) * sampleBMD.Stride + (x + startX) * 4 + 1];
                                c1r = sampleScan0[(y + startY) * sampleBMD.Stride + (x + startX) * 4 + 2];
                                // c1a = sampleScan0[(y + startY) * sampleBMD.Stride + (x + startX) * 4 + 3];
                                c2b = modelScan0[y * modelBMD.Stride + x * 4];
                                c2g = modelScan0[y * modelBMD.Stride + x * 4 + 1];
                                c2r = modelScan0[y * modelBMD.Stride + x * 4 + 2];
                                if (Math.Abs(c1r - c2r) + Math.Abs(c1g - c2g) + Math.Abs(c1b - c2b) > threshold) { jumpOut = true; break; }
                            }
                        }
                        if (jumpOut) break;
                    }
                    if (jumpOut) continue;
                    sample.UnlockBits(sampleBMD);
                    model.UnlockBits(modelBMD);
                    return new Point(startX, startY);
                }
            }
            sample.UnlockBits(sampleBMD);
            model.UnlockBits(modelBMD);
            return new Point(-1, -1);
        }

        unsafe public static Point FindImageFromRight(Bitmap sample, Bitmap model, int threshold = 30, int startFindX = 0, int startFindY = 0, int deltaX = int.MaxValue, int deltaY = int.MaxValue)
        {
            bool jumpOut = false;
            int endX = sample.Width - model.Width, endY = sample.Height - model.Height;
            endX = endX - startFindX <= deltaX ? endX : startFindX + deltaX;
            endY = endY - startFindY <= deltaY ? endY : startFindY + deltaY;
            int mWidth = model.Width;
            int mHeight = model.Height;
            int sWidth = sample.Width;
            int sHeight = sample.Height;
            BitmapData modelBMD = model.LockBits(new Rectangle(0, 0, model.Width, model.Height), ImageLockMode.ReadWrite, model.PixelFormat);
            BitmapData sampleBMD = sample.LockBits(new Rectangle(0, 0, sample.Width, sample.Height), ImageLockMode.ReadWrite, sample.PixelFormat);
            byte* sampleScan0 = (byte*)sampleBMD.Scan0;
            byte* modelScan0 = (byte*)modelBMD.Scan0;
            byte c1r, c1g, c1b, c2a, c2r, c2g, c2b;

            for (int startX = endX; startX > startFindX; startX--)
            {
                for (int startY = startFindY; startY < endY; startY++)
                {
                    for (int y = 0; y < mHeight; y++)
                    {
                        jumpOut = false;
                        if (y + startY >= sHeight) break;
                        for (int x = 0; x < mWidth; x++)
                        {
                            c2a = modelScan0[y * modelBMD.Stride + x * 4 + 3];
                            if (c2a != 0)
                            {
                                c1b = sampleScan0[(y + startY) * sampleBMD.Stride + (x + startX) * 4];
                                c1g = sampleScan0[(y + startY) * sampleBMD.Stride + (x + startX) * 4 + 1];
                                c1r = sampleScan0[(y + startY) * sampleBMD.Stride + (x + startX) * 4 + 2];
                                // c1a = sampleScan0[(y + startY) * sampleBMD.Stride + (x + startX) * 4 + 3];
                                c2b = modelScan0[y * modelBMD.Stride + x * 4];
                                c2g = modelScan0[y * modelBMD.Stride + x * 4 + 1];
                                c2r = modelScan0[y * modelBMD.Stride + x * 4 + 2];
                                if (Math.Abs(c1r - c2r) + Math.Abs(c1g - c2g) + Math.Abs(c1b - c2b) > threshold) { jumpOut = true; break; }
                            }
                        }
                        if (jumpOut) break;
                    }
                    if (jumpOut) continue;
                    sample.UnlockBits(sampleBMD);
                    model.UnlockBits(modelBMD);
                    return new Point(startX, startY);
                }
            }
            sample.UnlockBits(sampleBMD);
            model.UnlockBits(modelBMD);
            return new Point(-1, -1);
        }

        unsafe public static Point FindImageFromBottomRight(Bitmap sample, Bitmap model, int threshold = 30, int startFindX = 0, int startFindY = 0, int deltaX = int.MaxValue, int deltaY = int.MaxValue)
        {
            bool jumpOut = false;
            int endX = sample.Width - model.Width, endY = sample.Height - model.Height;
            endX = endX - startFindX <= deltaX ? endX : startFindX + deltaX;
            endY = endY - startFindY <= deltaY ? endY : startFindY + deltaY;
            int mWidth = model.Width;
            int mHeight = model.Height;
            int sWidth = sample.Width;
            int sHeight = sample.Height;
            BitmapData modelBMD = model.LockBits(new Rectangle(0, 0, model.Width, model.Height), ImageLockMode.ReadWrite, model.PixelFormat);
            BitmapData sampleBMD = sample.LockBits(new Rectangle(0, 0, sample.Width, sample.Height), ImageLockMode.ReadWrite, sample.PixelFormat);
            byte* sampleScan0 = (byte*)sampleBMD.Scan0;
            byte* modelScan0 = (byte*)modelBMD.Scan0;
            byte c1r, c1g, c1b, c2a, c2r, c2g, c2b;

            for (int startX = endX; startX > startFindX; startX--)
            {
                for (int startY = endY; startY > startFindY; startY--)
                {
                    for (int y = 0; y < mHeight; y++)
                    {
                        jumpOut = false;
                        if (y + startY >= sHeight) break;
                        for (int x = 0; x < mWidth; x++)
                        {
                            c2a = modelScan0[y * modelBMD.Stride + x * 4 + 3];
                            if (c2a != 0)
                            {
                                c1b = sampleScan0[(y + startY) * sampleBMD.Stride + (x + startX) * 4];
                                c1g = sampleScan0[(y + startY) * sampleBMD.Stride + (x + startX) * 4 + 1];
                                c1r = sampleScan0[(y + startY) * sampleBMD.Stride + (x + startX) * 4 + 2];
                                // c1a = sampleScan0[(y + startY) * sampleBMD.Stride + (x + startX) * 4 + 3];
                                c2b = modelScan0[y * modelBMD.Stride + x * 4];
                                c2g = modelScan0[y * modelBMD.Stride + x * 4 + 1];
                                c2r = modelScan0[y * modelBMD.Stride + x * 4 + 2];
                                if (Math.Abs(c1r - c2r) + Math.Abs(c1g - c2g) + Math.Abs(c1b - c2b) > threshold) { jumpOut = true; break; }
                            }
                        }
                        if (jumpOut) break;
                    }
                    if (jumpOut) continue;
                    sample.UnlockBits(sampleBMD);
                    model.UnlockBits(modelBMD);
                    return new Point(startX, startY);
                }
            }
            sample.UnlockBits(sampleBMD);
            model.UnlockBits(modelBMD);
            return new Point(-1, -1);
        }

        unsafe public static bool CompareImageFaster(Bitmap sample, Bitmap model, int startX, int startY, int threshold = 20)
        {
            
            int mWidth = model.Width;
            int mHeight = model.Height;
            int sWidth = sample.Width;
            int sHeight = sample.Height;
            
            BitmapData modelBMD = model.LockBits(new Rectangle(0, 0, model.Width, model.Height), ImageLockMode.ReadWrite, model.PixelFormat);
            BitmapData sampleBMD = sample.LockBits(new Rectangle(0, 0, sample.Width, sample.Height), ImageLockMode.ReadWrite, sample.PixelFormat);
            byte* sampleScan0 = (byte*)sampleBMD.Scan0;
            byte* modelScan0 = (byte*)modelBMD.Scan0;
            byte c1r, c1g, c1b, c2a, c2r, c2g, c2b;
            for (int y = 0; y < mHeight; y++)
            {
                if (y + startY >= sHeight) break;
                for (int x = 0; x < mWidth; x++)
                {
                    c2a = modelScan0[y * modelBMD.Stride + x * 4 + 3];
                    if (c2a != 0)
                    {
                        c1b = sampleScan0[(y + startY) * sampleBMD.Stride + (x + startX) * 4];
                        c1g = sampleScan0[(y + startY) * sampleBMD.Stride + (x + startX) * 4 + 1];
                        c1r = sampleScan0[(y + startY) * sampleBMD.Stride + (x + startX) * 4 + 2];
                        // c1a = sampleScan0[(y + startY) * sampleBMD.Stride + (x + startX) * 4 + 3];
                        c2b = modelScan0[y * modelBMD.Stride + x * 4];
                        c2g = modelScan0[y * modelBMD.Stride + x * 4 + 1];
                        c2r = modelScan0[y * modelBMD.Stride + x * 4 + 2];
                        if (Math.Abs(c1r - c2r) + Math.Abs(c1g - c2g) + Math.Abs(c1b - c2b) > threshold) { sample.UnlockBits(sampleBMD); model.UnlockBits(modelBMD); return false; }
                    }
                }
            }
            sample.UnlockBits(sampleBMD);
            model.UnlockBits(modelBMD);
            return true;
        }

        public static bool CompareImage(Bitmap sample, Bitmap model, int startX, int startY, int threshold = 20)
        {
            int mWidth = model.Width;
            int mHeight = model.Height;
            int sWidth = sample.Width;
            int sHeight = sample.Height;
            Color c1, c2;
            for (int y = 0; y < mHeight; y++)
            {
                if (y + startY >= sHeight) break;
                for (int x = 0; x < mWidth; x++)
                {
                    if (x + startX >= sWidth) break;
                    c2 = model.GetPixel(x, y);
                    if (c2.A != 0)
                    {
                        c1 = sample.GetPixel(startX + x, startY + y);
                        if (GetColorDiff(c1, c2) > threshold) return false;
                    }
                }
            }
            return true;
        }

        unsafe public static byte[,,] Image2ByteArray(Bitmap img)
        {
            byte[,,] imgData = new byte[img.Width, img.Height, 4];
            int mWidth = img.Width;
            int mHeight = img.Height;
            BitmapData imgBMD = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadWrite, img.PixelFormat);
            byte* imgScan0 = (byte*)imgBMD.Scan0;

            if (img.PixelFormat == PixelFormat.Format32bppArgb)
            {
                for (int y = 0; y < mHeight; y++)
                {
                    for (int x = 0; x < mWidth; x++)
                    {
                        imgData[x, y, 0] = imgScan0[(y) * imgBMD.Stride + (x) * 4 + 3];
                        imgData[x, y, 1] = imgScan0[(y) * imgBMD.Stride + (x) * 4 + 2];
                        imgData[x, y, 2] = imgScan0[(y) * imgBMD.Stride + (x) * 4 + 1];
                        imgData[x, y, 3] = imgScan0[(y) * imgBMD.Stride + (x) * 4];
                    }
                }
            }
            else if (img.PixelFormat == PixelFormat.Format24bppRgb)
            {
                for (int y = 0; y < mHeight; y++)
                {
                    for (int x = 0; x < mWidth; x++)
                    {
                        imgData[x, y, 0] = 255;
                        imgData[x, y, 1] = imgScan0[(y) * imgBMD.Stride + (x) * 3 + 2];
                        imgData[x, y, 2] = imgScan0[(y) * imgBMD.Stride + (x) * 3 + 1];
                        imgData[x, y, 3] = imgScan0[(y) * imgBMD.Stride + (x) * 3];
                    }
                }
            }
            else
            {
                for (int y = 0; y < mHeight; y++)
                {
                    for (int x = 0; x < mWidth; x++)
                    {
                        imgData[x, y, 0] = 0;
                        imgData[x, y, 1] = 0;
                        imgData[x, y, 2] = 0;
                        imgData[x, y, 3] = 0;
                    }
                }
            }
            img.UnlockBits(imgBMD);
            return imgData;
        }

        public static int GetRGBAvg(Color c)
        {
            return (c.R + c.G + c.B) / 3;
        }

        public static int GetColorDiff(Color c1, Color c2)
        {
            return Math.Abs(c1.R - c2.R) + Math.Abs(c1.G - c2.G) + Math.Abs(c1.B - c2.B);
        }

        public static Bitmap CutImage(Bitmap bm, Rectangle rect)
        {
            Bitmap temp = new Bitmap(rect.Width, rect.Height);
            Graphics gdi = Graphics.FromImage(temp);
            gdi.DrawImage(bm, new Rectangle(0, 0, rect.Width, rect.Height), rect, GraphicsUnit.Pixel);
            return temp;
        }

        public static Bitmap GetWhiteTextFromImage(Bitmap bm, int threshold = 120)
        {
            Bitmap rbm = new Bitmap(bm.Width, bm.Height);
            for (int y = 0; y < bm.Height; y++)
            {
                for (int x = 0; x < bm.Width; x++)
                {
                    Color c = bm.GetPixel(x, y);
                    if (c.R * 0.299 + c.G * 0.587 + c.B * 0.114 >= threshold)
                    {
                        rbm.SetPixel(x, y, Color.Black);
                    }
                }
            }
            return rbm;
        }

    }
}
