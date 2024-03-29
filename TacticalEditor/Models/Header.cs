﻿using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace TacticalEditor.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Header
    {
        // <summary>
        // всего заголовок 68 символов
        // TacticalEditor UTF8toDecimal
        [Description("header")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 68)]
        public byte[] Head = new byte[]
        {
            84, 97, 99, 116, 105, 99, 97, 108, 69, 100, 105, 116, 111, 114, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        };

        public byte[] GetHead(string caption)
        {
            byte[] bytesCap = Encoding.UTF8.GetBytes(caption);
            byte[] head = new byte[68];
            bytesCap.CopyTo(head, 0);
            return head;
        }

        public byte[] SetHead(string caption, string subCaption)
        {
	        byte[] cap = Encoding.UTF8.GetBytes(caption);
	        byte[] subCap = Encoding.UTF8.GetBytes(subCaption);
            byte[] head = new byte[68];
            cap.CopyTo(head, 0);
            subCap.CopyTo(head, 31);
            return head;
        }
    }
}
