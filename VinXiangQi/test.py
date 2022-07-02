import win32gui
import win32ui
import win32api
import time
from ctypes import windll
from PIL import Image
from win32con import WM_LBUTTONDOWN, MK_LBUTTON, WM_LBUTTONUP, WM_MOUSEMOVE
import matplotlib.pyplot as plt


def LeftClick(handle, pos):
    x, y = pos
    # print(pos, "Realsize", self.RealSize)
    lParam = win32api.MAKELONG(x, y)
    win32gui.PostMessage(handle, WM_MOUSEMOVE, MK_LBUTTON, lParam)
    win32gui.PostMessage(handle, WM_LBUTTONDOWN, MK_LBUTTON, lParam)
    time.sleep(0.5)
    win32gui.PostMessage(handle, WM_LBUTTONUP, MK_LBUTTON, lParam)
    win32gui.PostMessage(handle, WM_MOUSEMOVE, MK_LBUTTON, lParam)
    time.sleep(0.5)

def Screenshot(hwnd, region=None):  # -> (im, (left, top))
    try_count = 3
    success = False
    while try_count > 0 and not success:
        try:
            try_count -= 1
            left, top, right, bot = win32gui.GetWindowRect(hwnd)
            width = right - left
            height = bot - top
            # self.RealSize = (width, height)
            hwndDC = win32gui.GetWindowDC(hwnd)
            mfcDC = win32ui.CreateDCFromHandle(hwndDC)
            saveDC = mfcDC.CreateCompatibleDC()
            saveBitMap = win32ui.CreateBitmap()
            saveBitMap.CreateCompatibleBitmap(mfcDC, width, height)
            saveDC.SelectObject(saveBitMap)
            result = windll.user32.PrintWindow(hwnd, saveDC.GetSafeHdc(), 0)
            bmpinfo = saveBitMap.GetInfo()
            bmpstr = saveBitMap.GetBitmapBits(True)
            im = Image.frombuffer(
                "RGB",
                (bmpinfo['bmWidth'], bmpinfo['bmHeight']),
                bmpstr, 'raw', 'BGRX', 0, 1)
            win32gui.DeleteObject(saveBitMap.GetHandle())
            saveDC.DeleteDC()
            mfcDC.DeleteDC()
            win32gui.ReleaseDC(hwnd, hwndDC)
            if region is not None:
                im = im.crop((region[0], region[1], region[0] + region[2], region[1] + region[3]))
            if result:
                success = True
                return im, (left, top, width, height)
        except Exception as e:
            print("截图时出现错误:", repr(e))
    return None, (0, 0)

def ShowImg(image):
    plt.imshow(image)
    plt.show()

hwnd1 = win32gui.FindWindow(None, "天天象棋")
print("hwnd1", hwnd1)
hwnd2 = win32gui.FindWindowEx(hwnd1, 0, "Intermediate D3D Window", None)
print("hwnd2", hwnd2)
# hwnd2 = win32gui.FindWindowEx(hwnd2, 0, None, 'sub')
# print("hwnd2", hwnd2)
# hwnd3 = win32gui.FindWindow(None, "sub")
# print("hwnd3", hwnd3)
# time.sleep(2)
# h4 = win32gui.GetForegroundWindow();
# # get cursor position
# x, y = win32api.GetCursorPos()
# h5 = win32gui.WindowFromPoint((x, y));
# print("h4", h4)
# print("h5", h5)
# img, _ = Screenshot(0x00720ddc)
handle = hwnd1
# LeftClick(handle, (300, 300))
# LeftClick(handle, (, 1348))
# ShowImg(img)