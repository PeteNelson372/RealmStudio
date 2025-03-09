/**************************************************************************************************************************
* Copyright 2024, Peter R. Nelson
*
* This file is part of the RealmStudio application. The RealmStudio application is intended
* for creating fantasy maps for gaming and world building.
*
* RealmStudio is free software: you can redistribute it and/or modify it under the terms
* of the GNU General Public License as published by the Free Software Foundation,
* either version 3 of the License, or (at your option) any later version.
*
* This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
* without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
* See the GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License along with this program.
* The text of the GNU General Public License (GPL) is found in the LICENSE.txt file.
* If the LICENSE.txt file is not present or the text of the GNU GPL is not present in the LICENSE.txt file,
* see https://www.gnu.org/licenses/.
*
* For questions about the RealmStudio application or about licensing, please email
* support@brookmonte.com
*
***************************************************************************************************************************/
using System.Runtime.InteropServices;

namespace RealmStudio
{
    // custom cursor code adapted from https://csharpindeep.wordpress.com/2013/10/25/c-tutorial-how-to-use-custom-cursors-intermediate/

    public struct IconInfo
    {
        public bool FIcon { get; set; }
        public int XHotspot { get; set; }
        public int YHotspot { get; set; }
        public IntPtr HbmMask { get; set; }
        public IntPtr HbmColor { get; set; }
    }

    internal class CustomCursor
    {
#pragma warning disable SYSLIB1054
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

        [DllImport("user32.dll")]
        public static extern IntPtr CreateIconIndirect(ref IconInfo icon);
#pragma warning restore SYSLIB1054

        public static Cursor CreateCursor(Bitmap bmp, int xHotSpot, int yHotSpot)
        {
            IntPtr ptr = bmp.GetHicon();
            IconInfo tmp = new();
            GetIconInfo(ptr, ref tmp);
            tmp.XHotspot = xHotSpot;
            tmp.YHotspot = yHotSpot;
            tmp.FIcon = false;
            ptr = CreateIconIndirect(ref tmp);
            return new Cursor(ptr);
        }
    }
}
