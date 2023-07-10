using Advanced_Combat_Tracker;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ACT_TriggerTree
{
    public class Macros
    {
        public static List<char> invalidMacroChars = new List<char> { '<', '>', '\'', '\"', ';' };
        public static List<string> invalidMacroStrings = new List<string> { @"\#" };
        public static bool AlternateEncoding;

        public static Bitmap GetActionBitmap()
        {
            //use https://littlevgl.com/image-to-c-array to convert a Visual Studio Image Library png
            // to a Raw color format C array
            // so ACT can load it as part of a .cs file
            byte[] png_map = {
                0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a, 0x00, 0x00, 0x00, 0x0d, 0x49, 0x48, 0x44, 0x52,
                0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x10, 0x08, 0x06, 0x00, 0x00, 0x00, 0x1f, 0xf3, 0xff,
                0x61, 0x00, 0x00, 0x00, 0x09, 0x70, 0x48, 0x59, 0x73, 0x00, 0x00, 0x0e, 0xc4, 0x00, 0x00, 0x0e,
                0xc4, 0x01, 0x95, 0x2b, 0x0e, 0x1b, 0x00, 0x00, 0x01, 0x0d, 0x49, 0x44, 0x41, 0x54, 0x78, 0x5e,
                0xed, 0x1d, 0x03, 0x60, 0x23, 0x40, 0x6c, 0xb6, 0x6d, 0xdb, 0xb6, 0x6d, 0xdb, 0xb6, 0x6d, 0x9b,
                0x6f, 0xdb, 0xb6, 0x6d, 0xdb, 0xb6, 0x6d, 0xa3, 0xb6, 0xdd, 0x21, 0x39, 0xdb, 0x06, 0xeb, 0xff,
                0xff, 0xff, 0x59, 0x28, 0x01, 0x36, 0x16, 0x0a, 0x81, 0x03, 0xc4, 0x7c, 0xff, 0xfe, 0x1d, 0x3d,
                0x18, 0x1e, 0x7c, 0x7c, 0x7c, 0x7b, 0xd1, 0x0d, 0x03, 0x43, 0x0b, 0x33, 0x8b, 0x19, 0x82, 0x80,
                0x80, 0x00, 0x30, 0x85, 0xc2, 0x9e, 0x6f, 0xdf, 0xbe, 0xcd, 0x01, 0x52, 0x71, 0x64, 0xc3, 0x68,
                0x66, 0xf1, 0x47, 0x61, 0xea, 0xce, 0x9b, 0xe9, 0x7f, 0xff, 0xfd, 0xbf, 0x09, 0x74, 0x24, 0x15,
                0x48, 0x59, 0x49, 0x4e, 0x83, 0xca, 0x25, 0x67, 0x59, 0x1c, 0x9a, 0x76, 0x88, 0x9f, 0xb9, 0xfb,
                0x76, 0x1e, 0x50, 0x7a, 0x08, 0xe8, 0x88, 0x3e, 0xf6, 0x34, 0xc0, 0x03, 0x17, 0x1f, 0xbe, 0x67,
                0x71, 0x6e, 0xdd, 0xc9, 0x92, 0xe1, 0xa6, 0xe9, 0xd0, 0x16, 0x65, 0x7c, 0x01, 0xa8, 0x34, 0x8b,
                0xe4, 0x5c, 0x00, 0xe5, 0xf2, 0xec, 0xbd, 0xb7, 0x59, 0x8c, 0x2a, 0xb7, 0x70, 0xac, 0x38, 0xf6,
                0x20, 0x8f, 0xe2, 0x6c, 0x24, 0xd9, 0x01, 0x56, 0x60, 0xf2, 0x65, 0xba, 0x6b, 0xb2, 0x5c, 0xea,
                0x0f, 0xf8, 0x13, 0x63, 0xa7, 0x32, 0x8d, 0x24, 0x07, 0x8c, 0x95, 0x45, 0x59, 0x0e, 0x36, 0x7b,
                0xb3, 0x4c, 0x4c, 0xb1, 0x3c, 0x22, 0xcc, 0xc7, 0x65, 0xc2, 0xcb, 0xcb, 0x9b, 0x4f, 0xb4, 0x03,
                0xfd, 0x09, 0xe6, 0x2c, 0x47, 0xda, 0x7c, 0xde, 0x5a, 0xa8, 0x8b, 0xa7, 0x01, 0xa5, 0x4e, 0xc0,
                0x82, 0x75, 0x95, 0xa4, 0x5c, 0xc8, 0xf7, 0xd6, 0x9e, 0x0b, 0xe4, 0xaa, 0x81, 0x16, 0xdf, 0xe2,
                0x2f, 0xca, 0x50, 0xd8, 0xb2, 0x65, 0x0b, 0x4a, 0x51, 0x06, 0xd2, 0xbd, 0x40, 0xcb, 0x68, 0xc6,
                0x31, 0xcd, 0x32, 0xbe, 0x36, 0x02, 0x00, 0x61, 0x34, 0x5a, 0xcd, 0x0b, 0xd0, 0xa5, 0xfd, 0x00,
                0x00, 0x00, 0x00, 0x49, 0x45, 0x4e, 0x44, 0xae, 0x42, 0x60, 0x82,
            };

            Bitmap bmp;
            using (var ms = new MemoryStream(png_map))
            {
                bmp = new Bitmap(ms);
            }
            return bmp;
        }

        public static Bitmap GetActionNotBitmap()
        {
            byte[] png_map = {
                0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a, 0x00, 0x00, 0x00, 0x0d, 0x49, 0x48, 0x44, 0x52,
                0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x10, 0x08, 0x06, 0x00, 0x00, 0x00, 0x1f, 0xf3, 0xff,
                0x61, 0x00, 0x00, 0x00, 0x01, 0x73, 0x52, 0x47, 0x42, 0x00, 0xae, 0xce, 0x1c, 0xe9, 0x00, 0x00,
                0x00, 0x04, 0x67, 0x41, 0x4d, 0x41, 0x00, 0x00, 0xb1, 0x8f, 0x0b, 0xfc, 0x61, 0x05, 0x00, 0x00,
                0x00, 0x09, 0x70, 0x48, 0x59, 0x73, 0x00, 0x00, 0x0e, 0xc4, 0x00, 0x00, 0x0e, 0xc4, 0x01, 0x95,
                0x2b, 0x0e, 0x1b, 0x00, 0x00, 0x02, 0x52, 0x49, 0x44, 0x41, 0x54, 0x38, 0x4f, 0x7d, 0x53, 0x5f,
                0x48, 0x53, 0x51, 0x18, 0xff, 0xee, 0xe5, 0x9a, 0xdb, 0x9d, 0x63, 0xc1, 0x84, 0x31, 0x12, 0x8c,
                0x6e, 0xa1, 0x08, 0x2d, 0x67, 0x0a, 0x32, 0x64, 0x3d, 0xf8, 0x90, 0x45, 0x2f, 0x82, 0x0f, 0xbd,
                0x18, 0xce, 0xfe, 0x40, 0xeb, 0xa9, 0x87, 0xde, 0x42, 0x23, 0x90, 0x04, 0x85, 0x30, 0x58, 0x63,
                0x20, 0xe2, 0x43, 0x90, 0xd0, 0x83, 0x04, 0x3d, 0xd8, 0x43, 0x50, 0x36, 0xca, 0x87, 0x45, 0x39,
                0x4c, 0x1b, 0xb1, 0xe1, 0x90, 0x7c, 0x18, 0x4d, 0x0b, 0xe2, 0x3a, 0xd9, 0xee, 0x3d, 0x7e, 0xdf,
                0x39, 0x67, 0xda, 0x1a, 0xf4, 0x83, 0xdf, 0x3d, 0xf7, 0x7c, 0xdf, 0xef, 0xf7, 0xdd, 0x73, 0xb8,
                0xdf, 0xa7, 0x30, 0xc6, 0xa0, 0x0e, 0x8a, 0xe2, 0xc0, 0x67, 0x2f, 0xb2, 0x13, 0x79, 0x1c, 0xf9,
                0x0b, 0xf9, 0x05, 0xb9, 0x02, 0x8c, 0x95, 0x70, 0x3d, 0x84, 0x2a, 0x57, 0x01, 0x32, 0x2a, 0xca,
                0x7d, 0x7c, 0xdb, 0x42, 0x3e, 0x42, 0xb6, 0x52, 0x58, 0xae, 0xb4, 0xdf, 0xc2, 0xfc, 0x03, 0xf9,
                0x01, 0x01, 0x3a, 0x81, 0x69, 0x9a, 0xcc, 0xcc, 0xe5, 0x98, 0xd5, 0xd5, 0xc5, 0x2a, 0x83, 0x83,
                0x6c, 0x3f, 0x1e, 0xbf, 0x26, 0xd3, 0x35, 0xc0, 0xb3, 0x5e, 0xb6, 0x1d, 0x0e, 0x66, 0x05, 0x02,
                0x5c, 0x4f, 0x3e, 0x51, 0xa0, 0x50, 0x60, 0x39, 0x97, 0x8b, 0xc5, 0x0d, 0x83, 0x07, 0x25, 0x67,
                0x91, 0x5e, 0xe9, 0x25, 0x73, 0x33, 0xf2, 0xfb, 0x7e, 0x22, 0xc1, 0x75, 0xa4, 0x27, 0x1f, 0xbf,
                0x82, 0x36, 0x31, 0x01, 0x19, 0xb7, 0x1b, 0x5e, 0xb4, 0x56, 0x4f, 0x0c, 0x10, 0x7b, 0x9d, 0xb9,
                0x6e, 0xd9, 0x2c, 0x83, 0x45, 0x22, 0x56, 0x30, 0xd8, 0x80, 0xa1, 0xe7, 0xc8, 0x25, 0x6b, 0x78,
                0x98, 0xeb, 0x48, 0x4f, 0x3e, 0x15, 0xef, 0xd3, 0xa4, 0xcd, 0xcf, 0xc3, 0xac, 0x61, 0x70, 0x63,
                0x15, 0xf7, 0x9e, 0x7d, 0x82, 0xbe, 0xb1, 0x25, 0x6f, 0x2a, 0x5b, 0x9c, 0xb3, 0xdb, 0xda, 0xf2,
                0xcc, 0xed, 0x76, 0x61, 0xf8, 0xae, 0xc8, 0x02, 0xd7, 0x93, 0x8f, 0x4e, 0x30, 0x60, 0x87, 0x42,
                0xb0, 0xdb, 0xd8, 0x28, 0x32, 0x7f, 0x61, 0x35, 0xbf, 0x0b, 0x73, 0x91, 0x31, 0xd8, 0x79, 0xf3,
                0xde, 0x5f, 0x48, 0x7d, 0xee, 0xd9, 0x33, 0xcd, 0x19, 0x99, 0xe2, 0x7a, 0xf2, 0x51, 0x81, 0x76,
                0x3b, 0x10, 0x10, 0xd1, 0x7f, 0xd0, 0x5b, 0xcc, 0xc2, 0xe3, 0xd5, 0x05, 0xe8, 0x3f, 0x77, 0x0b,
                0xce, 0x4e, 0xae, 0x68, 0x0b, 0x1f, 0x36, 0xa3, 0x32, 0xc5, 0x41, 0x3e, 0x2a, 0xa0, 0x89, 0x6d,
                0x2d, 0x5a, 0xcc, 0x1d, 0x58, 0xfc, 0x18, 0x83, 0xc8, 0xf9, 0x51, 0x58, 0xf3, 0x9c, 0x90, 0xd1,
                0x7a, 0x50, 0x81, 0x35, 0x75, 0x7d, 0x5d, 0xec, 0xaa, 0x28, 0x95, 0xb8, 0x39, 0x71, 0xea, 0x02,
                0xbc, 0x6c, 0x09, 0xc2, 0xcd, 0xfe, 0x33, 0x90, 0x9e, 0xba, 0x52, 0xb9, 0x1a, 0x3a, 0xf9, 0x54,
                0x2a, 0x38, 0xc8, 0x47, 0x05, 0xde, 0xaa, 0xcb, 0xcb, 0xe0, 0x2a, 0x97, 0x45, 0x14, 0xd1, 0x10,
                0x8d, 0xc2, 0xa6, 0xab, 0x19, 0x16, 0x2f, 0x8d, 0xc0, 0xbb, 0xf1, 0x8b, 0x30, 0x33, 0xd2, 0x93,
                0xf4, 0xe8, 0xc7, 0x3a, 0x9d, 0x4e, 0xe7, 0x1d, 0x29, 0xe1, 0x7a, 0xf2, 0xa9, 0xd8, 0x08, 0x3f,
                0xe9, 0xd7, 0xdc, 0xc8, 0x66, 0x79, 0x42, 0x9b, 0x9e, 0x06, 0x35, 0x9d, 0x86, 0x1f, 0x53, 0x4f,
                0x20, 0xf9, 0x70, 0xa0, 0xd8, 0x6d, 0x78, 0x47, 0x31, 0x1c, 0xd6, 0x75, 0xfd, 0x2b, 0x17, 0x48,
                0x90, 0x9e, 0x7c, 0xa2, 0x91, 0xb6, 0xb7, 0x59, 0x5e, 0xd7, 0xd9, 0x2b, 0xbf, 0x9f, 0xd9, 0x3e,
                0x1f, 0xdb, 0xdb, 0xd8, 0xa8, 0x6b, 0x24, 0x42, 0xb5, 0x6b, 0xa9, 0x91, 0x48, 0x4f, 0xbe, 0xa3,
                0x56, 0x46, 0x93, 0xad, 0x69, 0xac, 0x12, 0x0e, 0xf3, 0x56, 0xc6, 0x98, 0xb4, 0x1d, 0x01, 0x3b,
                0xf1, 0x74, 0x79, 0x68, 0x88, 0x59, 0x1d, 0x1d, 0x87, 0xad, 0x5c, 0x3b, 0x8d, 0x8a, 0xe2, 0xc3,
                0xe7, 0x6d, 0x24, 0xdd, 0xf5, 0x1b, 0x32, 0x85, 0xfc, 0x8d, 0xf4, 0x20, 0xbb, 0x91, 0xed, 0xc8,
                0x18, 0x72, 0x12, 0xbf, 0xcc, 0xa7, 0xf2, 0x7f, 0xe3, 0xdc, 0x87, 0xa4, 0x71, 0x6e, 0x42, 0xfe,
                0x41, 0xd2, 0x38, 0x27, 0xab, 0x46, 0x01, 0x80, 0x03, 0xc4, 0x88, 0x26, 0xec, 0x90, 0x8f, 0x67,
                0x89, 0x00, 0x00, 0x00, 0x00, 0x49, 0x45, 0x4e, 0x44, 0xae, 0x42, 0x60, 0x82,
            };

            Bitmap bmp;
            using (var ms = new MemoryStream(png_map))
            {
                bmp = new Bitmap(ms);
            }
            return bmp;
        }

        public static bool IsInvalidMacro(List<string> strings)
        {
            if (AlternateEncoding)
                return false;

            foreach (char invalid in invalidMacroChars)
            {
                foreach (string s in strings)
                {
                    if (s.IndexOf(invalid) >= 0)
                        return true;
                }
            }

            foreach (string invalid in invalidMacroStrings)
            {
                foreach (string s in strings)
                {
                    if (s.Contains(invalid))
                        return true;
                }
            }

            return false; //all the passed strings are valid in a macro
        }

        public static bool IsInvalidMacroTrigger(CustomTrigger trigger)
        {
            List<string> strings = new List<string>();
            strings.Add(trigger.ShortRegexString);
            strings.Add(trigger.Category);
            strings.Add(trigger.SoundData);
            strings.Add(trigger.TimerName);
            return IsInvalidMacro(strings);

        }

        public static bool IsInvalidMacroTimer(TimerData timer)
        {
            List<string> strings = new List<string>();
            strings.Add(timer.Category);
            strings.Add(timer.Name);
            strings.Add(timer.StartSoundData);
            strings.Add(timer.Tooltip);
            strings.Add(timer.WarningSoundData);
            return IsInvalidMacro(strings);
        }

        public static bool IsInvalidMacro(string text)
        {
            List<string> strings = new List<string>();
            strings.Add(text);
            return IsInvalidMacro(strings);
        }

        public static string EncodeXml_ish(string text, bool encodeHash, bool encodePos, bool encodeSlashes)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            int len = text.Length;
            for (int i = 0; i < len; i++)
            {
                switch (text[i])
                {
                    case '<':
                        sb.Append("&lt;");
                        break;
                    case '>':
                        sb.Append("&gt;");
                        break;
                    case '"':
                        sb.Append("&quot;");
                        break;
                    case '&':
                        sb.Append("&amp;");
                        break;
                    case '\'':
                        if (encodePos)
                            sb.Append("&apos;");
                        else
                            sb.Append(text[i]);
                        break;
                    case '\\':
                        if (encodeSlashes)
                        {
                            if (i < len - 1)
                            {
                                //only encode double backslashes
                                if (text[i + 1] == '\\')
                                {
                                    sb.Append("&#92;&#92;");
                                    i++;
                                }
                                else
                                    sb.Append(text[i]);
                            }
                            else
                                sb.Append(text[i]);
                        }
                        else
                            sb.Append(text[i]);
                        break;
                    case '#':
                        if (encodeHash)
                            sb.Append("&#35;");
                        else //leave it alone when double encoding
                            sb.Append(text[i]);
                        break;
                    default:
                        sb.Append(text[i]);
                        break;
                }
            }
            return sb.ToString();
        }

        // Encode with a scheme like HTML, but avoid characters that confuse or break
        // ACT XML handing, EQII chat pasting, and EQII macros.
        // Use ! to start and : to end a special charcter encode instead of & and ;
        public static string EncodeCustom(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            int len = text.Length;
            for (int i = 0; i < len; i++)
            {
                switch (text[i])
                {
                    case '<':
                        sb.Append("!lt:");
                        break;
                    case '>':
                        sb.Append("!gt:");
                        break;
                    case '"':
                        sb.Append("!quot:");
                        break;
                    case '&':
                        sb.Append("!amp:");
                        break;
                    case '\'':
                        sb.Append("!apos:");
                        break;
                    case '\\':
                        sb.Append("!#92:");
                        break;
                    case ':':
                        sb.Append("!#58:");
                        break;
                    case '!':
                        sb.Append("!#33:");
                        break;
                    case ';':
                        sb.Append("!#59:");
                        break;
                    default:
                        sb.Append(text[i]);
                        break;
                }
            }
            return sb.ToString();
        }

        public static string DecodeShare(string text)
        {
            // convert incoming string to encoded HTML, then decode the HTML
            return System.Net.WebUtility.HtmlDecode(text.Replace(':', ';').Replace('!', '&'));
        }


        public static string SpellTimerToXML(TimerData timer)
        {
            string result = string.Empty;
            if (timer != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("<Spell N=\"{0}\"", EncodeXml_ish(timer.Name, false, false, false)));
                sb.Append(string.Format(" T=\"{0}\"", timer.TimerValue));
                sb.Append(string.Format(" OM=\"{0}\"", timer.OnlyMasterTicks ? "T" : "F"));
                sb.Append(string.Format(" R=\"{0}\"", timer.RestrictToMe ? "T" : "F"));
                sb.Append(string.Format(" A=\"{0}\"", timer.AbsoluteTiming ? "T" : "F"));
                sb.Append(string.Format(" WV=\"{0}\"", timer.WarningValue));
                sb.Append(string.Format(" RD=\"{0}\"", timer.RadialDisplay ? "T" : "F"));
                sb.Append(string.Format(" M=\"{0}\"", timer.Modable ? "T" : "F"));
                sb.Append(string.Format(" Tt=\"{0}\"", EncodeXml_ish(timer.Tooltip, false, false, false)));
                sb.Append(string.Format(" FC=\"{0}\"", timer.FillColor.ToArgb()));
                sb.Append(string.Format(" RV=\"{0}\"", timer.RemoveValue));
                sb.Append(string.Format(" C=\"{0}\"", EncodeXml_ish(timer.Category, false, false, false)));
                sb.Append(string.Format(" RC=\"{0}\"", timer.RestrictToCategory ? "T" : "F"));
                sb.Append(string.Format(" SS=\"{0}\"", timer.StartSoundData));
                sb.Append(string.Format(" WS=\"{0}\"", timer.WarningSoundData));
                sb.Append(" />");

                result = sb.ToString();
            }

            return result;
        }


        public static string TriggerToXML(CustomTrigger trigger)
        {
            string result = string.Empty;
            if (trigger != null)
            {
                //match the character replacement scheme used by the Custom Triggers tab
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("<Trigger R=\"{0}\"", EncodeXml_ish(trigger.ShortRegexString, true, false, true)));
                sb.Append(string.Format(" SD=\"{0}\"", EncodeXml_ish(trigger.SoundData, false, true, false)));
                sb.Append(string.Format(" ST=\"{0}\"", trigger.SoundType.ToString()));
                sb.Append(string.Format(" CR=\"{0}\"", trigger.RestrictToCategoryZone ? "T" : "F"));
                sb.Append(string.Format(" C=\"{0}\"", EncodeXml_ish(trigger.Category, false, true, false)));
                sb.Append(string.Format(" T=\"{0}\"", trigger.Timer ? "T" : "F"));
                sb.Append(string.Format(" TN=\"{0}\"", EncodeXml_ish(trigger.TimerName, false, true, false)));
                sb.Append(string.Format(" Ta=\"{0}\"", trigger.Tabbed ? "T" : "F"));
                sb.Append(" />");

                result = sb.ToString();
            }
            return result;
        }

        public static string TriggerToMacro(CustomTrigger trigger)
        {
            string result = string.Empty;
            if (trigger != null)
            {
                //use single quotes because double quotes don't work
                StringBuilder sb = new StringBuilder();
                if(!AlternateEncoding)
                {
                    sb.Append(string.Format("<Trigger R='{0}'", trigger.ShortRegexString.Replace("\\\\", "\\\\\\\\")));
                    sb.Append(string.Format(" SD='{0}'", trigger.SoundData));
                    sb.Append(string.Format(" ST='{0}'", trigger.SoundType.ToString()));
                    sb.Append(string.Format(" CR='{0}'", trigger.RestrictToCategoryZone ? "T" : "F"));
                    sb.Append(string.Format(" C='{0}'", trigger.Category));
                    sb.Append(string.Format(" T='{0}'", trigger.Timer ? "T" : "F"));
                    sb.Append(string.Format(" TN='{0}'", trigger.TimerName));
                    sb.Append(string.Format(" Ta='{0}'", trigger.Tabbed ? "T" : "F"));
                    sb.Append(" />");
                }
                else
                {
                    sb.Append(string.Format("<TrigTree R='{0}'", EncodeCustom(trigger.ShortRegexString)));
                    sb.Append(string.Format(" SD='{0}'", EncodeCustom(trigger.SoundData)));
                    sb.Append(string.Format(" ST='{0}'", trigger.SoundType.ToString()));
                    sb.Append(string.Format(" CR='{0}'", trigger.RestrictToCategoryZone ? "T" : "F"));
                    sb.Append(string.Format(" C='{0}'", EncodeCustom(trigger.Category)));
                    sb.Append(string.Format(" T='{0}'", trigger.Timer ? "T" : "F"));
                    sb.Append(string.Format(" TN='{0}'", EncodeCustom(trigger.TimerName)));
                    sb.Append(string.Format(" Ta='{0}'", trigger.Tabbed ? "T" : "F"));
                    sb.Append(" />");
                }
                result = sb.ToString();
            }
            return result;
        }

        public static CustomTrigger TriggerFromMacro(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;
            else
            {
                var xmlFields = ExtractXmlFields(text);
                if(xmlFields == null)
                    return null;
                if (xmlFields.Count < 2)
                    return null;

                string re;
                string category;
                xmlFields.TryGetValue("R", out re);
                xmlFields.TryGetValue("C", out category);
                if (!string.IsNullOrEmpty(re) && !string.IsNullOrEmpty(category))
                {
                    CustomTrigger trigger = new CustomTrigger(DecodeShare(re), DecodeShare(category));
                    foreach (var field in xmlFields)
                    {
                        switch (field.Key)
                        {
                            case "SD":
                                trigger.SoundData = DecodeShare(field.Value);
                                break;
                            case "ST":
                                trigger.SoundType = Int32.Parse(field.Value);
                                break;
                            case "CR":
                                trigger.RestrictToCategoryZone = field.Value == "T" ? true : false;
                                break;
                            case "T":
                                trigger.Timer = field.Value == "T" ? true : false;
                                break;
                            case "TN":
                                trigger.TimerName = DecodeShare(field.Value);
                                break;
                            case "Ta":
                                trigger.Tabbed = field.Value == "T" ? true : false;
                                break;
                        }
                    }
                    return trigger;
                }
                else
                    return null;
            }
        }

        public static Dictionary<string, string> ExtractXmlFields(string xmlString)
        {
            var fieldPattern = @"(\w+)='([^']*)'";
            var fieldMatches = Regex.Matches(xmlString, fieldPattern);

            var xmlFields = new Dictionary<string, string>();

            foreach (Match match in fieldMatches)
            {
                string fieldName = match.Groups[1].Value;
                string fieldValue = match.Groups[2].Value;
                xmlFields[fieldName] = fieldValue;
            }

            return xmlFields;
        }

        public static string SpellTimerToMacro(TimerData timer)
        {
            string result = string.Empty;
            if (timer != null)
            {
                StringBuilder sb = new StringBuilder();
                if(!AlternateEncoding)
                {
                    sb.Append(string.Format("<Spell N='{0}'", timer.Name));
                    sb.Append(string.Format(" T='{0}'", timer.TimerValue));
                    sb.Append(string.Format(" OM='{0}'", timer.OnlyMasterTicks ? "T" : "F"));
                    sb.Append(string.Format(" R='{0}'", timer.RestrictToMe ? "T" : "F"));
                    sb.Append(string.Format(" A='{0}'", timer.AbsoluteTiming ? "T" : "F"));
                    sb.Append(string.Format(" WV='{0}'", timer.WarningValue));
                    sb.Append(string.Format(" RD='{0}'", timer.RadialDisplay ? "T" : "F"));
                    sb.Append(string.Format(" M='{0}'", timer.Modable ? "T" : "F"));
                    sb.Append(string.Format(" Tt='{0}'", timer.Tooltip));
                    sb.Append(string.Format(" FC='{0}'", timer.FillColor.ToArgb()));
                    sb.Append(string.Format(" RV='{0}'", timer.RemoveValue));
                    sb.Append(string.Format(" C='{0}'", timer.Category));
                    sb.Append(string.Format(" RC='{0}'", timer.RestrictToCategory ? "T" : "F"));
                    sb.Append(string.Format(" SS='{0}'", timer.StartSoundData));
                    sb.Append(string.Format(" WS='{0}'", timer.WarningSoundData));
                    sb.Append(" />");
                }
                else
                {
                    sb.Append(string.Format("<SpellTT N='{0}'", EncodeCustom(timer.Name)));
                    sb.Append(string.Format(" T='{0}'", timer.TimerValue));
                    sb.Append(string.Format(" OM='{0}'", timer.OnlyMasterTicks ? "T" : "F"));
                    sb.Append(string.Format(" R='{0}'", timer.RestrictToMe ? "T" : "F"));
                    sb.Append(string.Format(" A='{0}'", timer.AbsoluteTiming ? "T" : "F"));
                    sb.Append(string.Format(" WV='{0}'", timer.WarningValue));
                    sb.Append(string.Format(" RD='{0}'", timer.RadialDisplay ? "T" : "F"));
                    sb.Append(string.Format(" M='{0}'", timer.Modable ? "T" : "F"));
                    sb.Append(string.Format(" Tt='{0}'", EncodeCustom(timer.Tooltip)));
                    sb.Append(string.Format(" FC='{0}'", timer.FillColor.ToArgb()));
                    sb.Append(string.Format(" RV='{0}'", timer.RemoveValue));
                    sb.Append(string.Format(" C='{0}'", EncodeCustom(timer.Category)));
                    sb.Append(string.Format(" RC='{0}'", timer.RestrictToCategory ? "T" : "F"));
                    sb.Append(string.Format(" SS='{0}'", EncodeCustom(timer.StartSoundData)));
                    sb.Append(string.Format(" WS='{0}'", EncodeCustom(timer.WarningSoundData)));
                    sb.Append(" />");
                }

                result = sb.ToString();
            }
            return result;
        }

        public static TimerData SpellTimerFromMacro(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;
            else
            {
                var xmlFields = ExtractXmlFields(text);
                if (xmlFields == null)
                    return null;
                if (xmlFields.Count < 2)
                    return null;

                string name;
                string category;
                xmlFields.TryGetValue("N", out name);
                xmlFields.TryGetValue("C", out category);
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(category))
                {
                    TimerData timer = new TimerData(DecodeShare(name), DecodeShare(category));
                    foreach (var field in xmlFields)
                    {
                        switch (field.Key)
                        {
                            case "T":
                                timer.TimerValue = Int32.Parse(field.Value);
                                break;
                            case "OM":
                                timer.OnlyMasterTicks = field.Value == "T" ? true : false;
                                break;
                            case "R":
                                timer.RestrictToMe = field.Value == "T" ? true : false;
                                break;
                            case "A":
                                timer.AbsoluteTiming = field.Value == "T" ? true : false;
                                break;
                            case "WV":
                                timer.WarningValue = Int32.Parse(field.Value);
                                break;
                            case "RD":
                                timer.RadialDisplay = field.Value == "T" ? true : false;
                                break;
                            case "M":
                                timer.Modable = field.Value == "T" ? true : false;
                                break;
                            case "Tt":
                                timer.Tooltip= DecodeShare(field.Value);
                                break;
                            case "FC":
                                timer.FillColor = Color.FromArgb(Int32.Parse(field.Value));
                                break;
                            case "RV":
                                timer.RemoveValue = Int32.Parse(field.Value);
                                if(timer.RemoveValue > 0)
                                    timer.RemoveValue = -timer.RemoveValue; //positive # confuses ACT
                                break;
                            case "RC":
                                timer.RestrictToCategory = field.Value == "T" ? true : false;
                                break;
                            case "SS":
                                timer.StartSoundData = DecodeShare(field.Value);
                                break;
                            case "WS":
                                timer.WarningSoundData = DecodeShare(field.Value);
                                break;
                        }
                    }
                    return timer;
                }
                else
                    return null;
            }

        }

        public static int WriteCategoryMacroFile(string sayCmd, List<CustomTrigger> triggers, List<TimerData> categoryTimers, bool notifyTray = true)
        {
            int fileCount = 0;
            {
                int validTrigs = 0;
                int validTimers = 0;
                int invalid = 0;
                if(triggers.Count > 0)
                {
                    try
                    {
                        string category = triggers[0].Category;
                        StringBuilder sb = new StringBuilder();
                        //start with timers for the category
                        foreach (TimerData timer in categoryTimers)
                        {
                            if (!IsInvalidMacroTimer(timer))
                            {
                                sb.Append(sayCmd);
                                sb.Append(SpellTimerToMacro(timer));
                                sb.Append(Environment.NewLine);
                                validTimers++;
                                if (validTimers >= 16)
                                {
                                    MacroToFile(fileCount, category, sb.ToString(), invalid, validTimers, validTrigs, notifyTray);
                                    fileCount++;
                                    sb.Clear();
                                    invalid = 0;
                                    validTimers = 0;
                                }
                            }
                            else
                            {
                                invalid++;
                            }
                        }
                        //then category triggers
                        foreach (CustomTrigger trigger in triggers)
                        {
                            if (trigger.Active)
                            {
                                if (IsInvalidMacroTrigger(trigger))
                                {
                                    invalid++;
                                }
                                else
                                {
                                    sb.Append(sayCmd);
                                    sb.Append(TriggerToMacro(trigger));
                                    sb.Append(Environment.NewLine);
                                    validTrigs++;
                                }
                                if (validTrigs + validTimers >= 16)
                                {
                                    MacroToFile(fileCount, category, sb.ToString(), invalid, validTimers, validTrigs, notifyTray);
                                    fileCount++;
                                    sb.Clear();
                                    invalid = 0;
                                    validTimers = 0;
                                    validTrigs = 0;
                                }
                                // find timers that are activated by a trigger
                                List<TimerData> timers = TriggerTree.FindTimers(trigger);
                                foreach (TimerData timer in timers)
                                {
                                    if (!categoryTimers.Contains(timer))
                                    {
                                        if (!Macros.IsInvalidMacroTimer(timer))
                                        {
                                            sb.Append(sayCmd);
                                            sb.Append(SpellTimerToMacro(timer));
                                            sb.Append(Environment.NewLine);
                                            validTimers++;
                                            if (validTrigs + validTimers >= 16)
                                            {
                                                //tooLong = true;
                                                MacroToFile(fileCount, category, sb.ToString(), invalid, validTimers, validTrigs, notifyTray);
                                                fileCount++;
                                                sb.Clear();
                                                invalid = 0;
                                                validTimers = 0;
                                                validTrigs = 0;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (validTrigs > 0 || validTimers > 0)
                        {
                            MacroToFile(fileCount, category, sb.ToString(), invalid, validTimers, validTrigs, notifyTray);
                            fileCount++;
                        }
                    }
                    catch (Exception x)
                    {
                        SimpleMessageBox.Show(ActGlobals.oFormActMain, "Macro file error:\n" + x.Message);
                    }
                }
            }
            return fileCount;
        }

        public static void MacroToFile(int fileCount, string category, string content, int invalid, int validTimers, int validTrigs, bool notifyTray = true)
        {
            string fileName = TriggerTree.doFileName;
            if (fileCount > 0)
                fileName = Path.GetFileNameWithoutExtension(TriggerTree.doFileName) + fileCount.ToString() + Path.GetExtension(TriggerTree.doFileName);
            if (ActGlobals.oFormActMain.SendToMacroFile(fileName, content, string.Empty))
            {
                if(notifyTray)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(string.IsNullOrEmpty(category) ? string.Empty : string.Format("For category\n'{0}'\n", category));
                    sb.Append("Wrote ");
                    sb.Append(validTrigs > 0 ? string.Format("{0} trigger{1}", validTrigs, validTrigs > 1 ? "s" : string.Empty) : string.Empty);
                    sb.Append(validTrigs > 0 && validTimers > 0 ? " and " : string.Empty);
                    sb.Append(validTimers > 0 ? string.Format("{0} timer{1}", validTimers, validTimers > 1 ? "s" : string.Empty) : string.Empty);
                    sb.Append(invalid > 0 ? string.Format("\n\nCould not write {0} item{1}.", invalid, invalid > 1 ? "s" : string.Empty) : string.Empty);
                    sb.Append(string.Format("\n\nIn EQII chat, enter:\n/do_file_commands {0}", fileName));

                    TraySlider traySlider = new TraySlider();
                    traySlider.ButtonLayout = TraySlider.ButtonLayoutEnum.OneButton;
                    traySlider.ShowTraySlider(sb.ToString(), "Wrote Category Macro");
                }
            }
        }

    }
}
