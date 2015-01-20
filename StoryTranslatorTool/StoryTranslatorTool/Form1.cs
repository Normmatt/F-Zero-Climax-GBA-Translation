using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Nintenlord.GBA;
using Nintenlord.GBA.Compressions;

namespace StoryTranslatorTool
{
    public partial class Form1 : Form
    {
        private GBAROM ROM;
        private StoryList storyList = new StoryList();

        private byte[] rawGraphics; //raw tileset graphics
        private Bitmap mapBitmap;
        private Graphics mapGraphics;

        private Color[] PALfilePalette;
        private Color[] GrayScalePalette;
        private Color[] WhiteToBlackPalette;

        private byte[] widthTable;
        private Image font;
        private Image font2;
        private Bitmap title;
        private Bitmap title_output;

        private Graphics g;
        private Graphics g2;

        private const int maxWidth = 177;

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        public void SaveBitmap(string path)
        {
            ImageFormat im;
            switch (Path.GetExtension(path).ToUpper())
            {
                case ".PNG":
                    im = ImageFormat.Png;
                    break;
                case ".BMP":
                    im = ImageFormat.Bmp;
                    break;
                case ".GIF":
                    im = ImageFormat.Gif;
                    break;
                default:
                    MessageBox.Show("Wrong image format.");
                    return;
            }
            mapBitmap.Save(path, im);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        public void LoadPalFile(string path)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(path));
            br.BaseStream.Position = 0x18;
            byte[] data = new byte[br.BaseStream.Length - br.BaseStream.Position];
            data = br.ReadBytes(data.Length);
            br.Close();

            PALfilePalette = new Color[data.Length / 4];
            for (int i = 0; i < data.Length; i += 4)
            {
                PALfilePalette[i / 4] = Color.FromArgb(data[i], data[i + 1], data[i + 2]);
            }
        }

        public void LoadRawPalFile(int offset)
        {
            byte[] rawPal = ROM.GetData(offset, 0x1FF);

            PALfilePalette = new Color[0x100];

            Color[] temp = GBAGraphics.toPalette(rawPal, 0, 0x100);
            temp.CopyTo(PALfilePalette, 0x00);

            for (int i = 0; i < 0x100; i += 16)
            {
                PALfilePalette[i] = Color.Transparent;
            }
        }

        private new void CreateGraphics()
        {
            g = pictureBox1.CreateGraphics();
            g.SmoothingMode = SmoothingMode.None;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            g2 = Graphics.FromImage(title);
            g2.SmoothingMode = SmoothingMode.None;
            g2.InterpolationMode = InterpolationMode.NearestNeighbor;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private string MakeStoryFileString()
        {
            var output = " ; F-Zero Climax Translation by Normmatt" + Environment.NewLine;

            output += Environment.NewLine;
            output += String.Format(".gba\t\t\t\t; Set the architecture to GBA") + Environment.NewLine;
            output += String.Format(".open \"rom/output.gba\",0x08000000\t; Open input.gba for output.") + Environment.NewLine;
            output += String.Format("\t\t\t\t\t\t; 0x08000000 will be used as the") + Environment.NewLine;
            output += String.Format("\t\t\t\t\t\t; header size") + Environment.NewLine;
            output += Environment.NewLine;
            output += String.Format(".relativeinclude on") + Environment.NewLine;
            output += Environment.NewLine;
            output += String.Format(";Equs for Scripting and control codes") + Environment.NewLine;
            output += String.Format("TextNL equ 0x0D") + Environment.NewLine;
            output += String.Format("TextEnd equ .byte 0x0") + Environment.NewLine;
            output += Environment.NewLine;
            output += String.Format(".org 0x08CF7BAC") + Environment.NewLine;
            output += String.Format("StoryEpisodeTitlePointerList:") + Environment.NewLine;

            for (int i = 0; i < storyList.Stories.Count; i++)
            {
                output += String.Format("\t.word StoryEpisode{0}_Title", i + 1) + Environment.NewLine;
            }

            output += Environment.NewLine;
            output += String.Format(".org 0x08CF9D5C") + Environment.NewLine;
            output += String.Format("StoryEpisodePointerList:") + Environment.NewLine;

            for (int i = 0; i < storyList.Stories.Count; i++)
            {
                output += String.Format("\t.word StoryEpisode{0}PointerList", i + 1) + Environment.NewLine;
            }

            output += Environment.NewLine;
            output += String.Format(".org 0x08DD0000") + Environment.NewLine;
            output += Environment.NewLine;
            output += String.Format("EOF:") + Environment.NewLine;
            output += String.Format(".byte 0x82,0x64,0x82,0x6E,0x82,0x65,0,0 ;EOF in SJIS") + Environment.NewLine;
            output += Environment.NewLine;

            for (int i = 0; i < storyList.Stories.Count; i++)
            {
                output += String.Format("\t.include episode_{0}/script.asm", i + 1) + Environment.NewLine;
            }

            for (int i = 0; i < storyList.Stories.Count; i++)
            {
                output += Environment.NewLine;
                output += String.Format(".align 4") + Environment.NewLine;
                output += String.Format("StoryEpisode{0}_Title:", i + 1) + Environment.NewLine;
                output += String.Format("\t.incbin episode_{0}/title_lz.bin", i + 1) + Environment.NewLine;
            }

            output += Environment.NewLine;
            output += ".close";
            output += Environment.NewLine;
            output += " ; make sure to leave an empty line at the end";

            return output;
        }

        private string MakeScriptString(int id)
        {
            var originalText = textBox2.Text;

            var oldText = storyList.Stories[id].Text.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);

            var newText = FormatString(oldText);

            textBox2.Text = newText;

            var output = " ; F-Zero Climax Translation by Normmatt" + Environment.NewLine;

            output += Environment.NewLine;
            output += String.Format(".align 4") + Environment.NewLine;
            output += String.Format("StoryEpisode{0}PointerList:", id+1) + Environment.NewLine;

            for (int i = 0; i < textBox2.Lines.Length; i++)
            {
                output += String.Format("\t.word Episode{0}_Line{1}", id+1, i + 1) + Environment.NewLine;
            }

            output += "\t.word EOF" + Environment.NewLine;
            output += "\t.word EOF" + Environment.NewLine;
            output += Environment.NewLine;

            for (int i = 0; i < textBox2.Lines.Length; i++)
            {
                output += String.Format("Episode{0}_Line{1}:", id+1, i + 1) + Environment.NewLine;
                output += String.Format("\t.ascii \"{0}\"", textBox2.Lines[i].Replace("\"", "\\\"")) + Environment.NewLine;
                output += "\tTextEnd" + Environment.NewLine;
            }

            output += Environment.NewLine;
            output += Environment.NewLine;
            output += " ; make sure to leave an empty line at the end";

            textBox2.Text = originalText;

            return output;
        }

        private void UpdateListBox()
        {
            listBox1.Items.Clear();
            foreach (var story in storyList.Stories)
            {
                listBox1.Items.Add(story.Title);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            storyList.Stories.Add(new Story() { Title = textBox1.Text, Text = textBox2.Text});
            storyList.Save("story.xml");
            UpdateListBox();
            listBox1.SelectedIndex = storyList.Stories.Count - 1;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ROM = new GBAROM();
            ROM.OpenROM("..\\..\\rom\\input.gba");

            GrayScalePalette = new Color[0x100];
            WhiteToBlackPalette = new Color[0x100];

            for (int x = 0; x < 0x10; x++)
            {
                for (int y = 0; y < 0x10; y++)
                {
                    int value = x * 0x10 + y;
                    int value2 = ((0x10 - x) * 0x10 + (0x10 - y)) & 0xFF;

                    GrayScalePalette[x + y * 0x10] = Color.FromArgb(value, value, value);

                    WhiteToBlackPalette[x + y * 0x10] = Color.FromArgb(value2, value2, value2);
                }
            }

            storyList.Load("story.xml");

            UpdateListBox();

            int empty;
            Color[] palette = new Color[16];
            int palIndex = 14;

            LoadPalFile("obj.pal");
            for (int j = 0; j < palette.Length; j++)
                palette[j] = PALfilePalette[j + 16 * palIndex];

            rawGraphics = ROM.GetData(0x37CBCC, 0x8000);

            palette[0] = Color.Transparent;
            palette[3] = palette[6];

            font2 = GBAGraphics.ToBitmap(rawGraphics, rawGraphics.Length, 0, palette, 16, GraphicsMode.Tile4bit, out empty);

            palIndex = 15;
            LoadPalFile("bg.pal");
            for (int j = 0; j < palette.Length; j++)
                palette[j] = PALfilePalette[j + 16 * palIndex];

            palette[0] = Color.Transparent;
            font = GBAGraphics.ToBitmap(rawGraphics, rawGraphics.Length, 0, palette, 16, GraphicsMode.Tile4bit, out empty);

            widthTable = File.ReadAllBytes("widthTable.bin");

            title = new Bitmap(256, 16);
            title_output = new Bitmap(32, 112);

            CreateGraphics();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1) return;

            int empty;

            textBox1.Text = storyList.Stories[listBox1.SelectedIndex].Title;
            textBox2.Text = storyList.Stories[listBox1.SelectedIndex].Text;

            Color[] palette = new Color[16];
            int palIndex = 14;
            
            //title pointer list = 0x00CF7BAC (4bpp)
            LoadPalFile("obj.pal");
            //rawGraphics = ROM.DecompressLZ77CompressedData((int)(ROM.GetU32(0x00CF7BAC + (listBox1.SelectedIndex << 2)) - 0x08000000));
            for (int j = 0; j < palette.Length; j++)
                palette[j] = PALfilePalette[j + 16 * palIndex];
            //bg pointer list = 0x00CF779C (8bpp)
            //LoadPalFile("bg.pal");
            //rawGraphics = ROM.DecompressLZ77CompressedData((int)(ROM.GetU32(0x00CF779C + (listBox1.SelectedIndex << 2)) - 0x08000000));

            //pictureBox1.Image = title;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //vScrollBar1.Value = vScrollBar1.Minimum;
            //textBox2.Text = FormatString(textBox2.Text);
            storyList.Stories[listBox1.SelectedIndex].Title = textBox1.Text;
            storyList.Stories[listBox1.SelectedIndex].Text = textBox2.Text;
            storyList.Save("story.xml");

            for (int i = 0; i < storyList.Stories.Count; i++ )
            {
                listBox1.Items[i] = storyList.Stories[i].Title;
            }
        }

        private string FormatString(string oldText)
        {
            oldText = oldText.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            var curWidth = 0;
            var replacements = new List<int>();
            var newString = "";
            for (var i = 0; i < oldText.Length; i++)
            {
                if (oldText[i] == '\r' || oldText[i] == '\n')
                {
                    curWidth = 0;
                    continue;
                }
                var width = widthTable[oldText[i] - 32];
                curWidth += width;

                //Game bugs out if curWidth is not half word aligned
                curWidth += curWidth & 1;

                if (curWidth > maxWidth)
                {
                    var lastSpace = oldText.LastIndexOf(" ", i);
                    replacements.Add(lastSpace);
                    curWidth = 0;

                    for (int j = lastSpace + 1; j < i + 1; j++)
                    {
                        curWidth += widthTable[oldText[j] - 32];
                    }
                }
            }

            for (var i = 0; i < oldText.Length; i++)
            {
                if (replacements.Contains(i))
                {
                    newString += Environment.NewLine;
                }
                else
                {
                    newString += oldText[i];
                }
            }
            return newString;
        }

        private void DrawString(int start=0)
        {
            int x = 0;
            int y = 0;
            int multiplier = 1;

            g.Clear(Color.FromArgb(14*8,12*8,11*8));
            g.ResetTransform();
            g.ScaleTransform(multiplier, multiplier);

            g.DrawLine(Pens.Magenta, maxWidth, 0, maxWidth, pictureBox1.Height);

            for (int index = start; index < textBox2.Lines.Length; index++)
            {
                foreach (var c in textBox2.Lines[index])
                {
                    var width = widthTable[c - 32];
                    var sRect = new Rectangle(0, (c - 32)*16, width, 16);
                    var dRect = new Rectangle(x, y, width, 16);
                    g.DrawImage(font, dRect, sRect, GraphicsUnit.Pixel);
                    x += width;
                    //Game bugs out if curWidth is not half word aligned
                    x += x & 1;
                }
                x = 0;
                y += 16;
            }
        }

        private void DrawString2(String input)
        {
            int x = 0;

            g2.Clear(Color.Black);

            g2.DrawLine(Pens.Magenta, 241, 0, 241, pictureBox2.Height);

            foreach (var c in input)
            {
                var width = widthTable[c - 32];
                var sRect = new Rectangle(0, (c - 32) * 16, width, 16);
                var dRect = new Rectangle(x, 0, width, 16);
                g2.DrawImage(font2, dRect, sRect, GraphicsUnit.Pixel);
                x += width;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            DrawString2(textBox1.Text);

            Graphics g3 = Graphics.FromImage(title_output);
            for (int i = 0; i < 7; i++)
            {
                var sRect = new Rectangle(i * 32, 0, 32, 16);
                var dRect = new Rectangle(0, i * 16, 32, 16);
                g3.DrawImage(title, dRect, sRect, GraphicsUnit.Pixel);
            }

            pictureBox2.Image = title;
            pictureBox3.Image = title_output;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            //Apparently saving to XML replaces \r\n with just \n so replace it here so preview works correctly
            textBox2.Text = Regex.Replace(textBox2.Text, "(?<!\r)\n", "\r\n");
            DrawString();
            if (textBox2.Lines.Length >= 25)
            {
                vScrollBar1.Maximum = textBox2.Lines.Length - 25; //25 visible lines
                vScrollBar1.Value = vScrollBar1.Minimum;
            }
            else
            {
                vScrollBar1.Maximum = 0;
                vScrollBar1.Value = 0;
            }
        }

        private void WriteTitleToLZ(string filename, int id)
        {
            DrawString2(storyList.Stories[id].Title);

            Graphics g3 = Graphics.FromImage(title_output);
            for (int i = 0; i < 7; i++)
            {
                var sRect = new Rectangle(i * 32, 0, 32, 16);
                var dRect = new Rectangle(0, i * 16, 32, 16);
                g3.DrawImage(title, dRect, sRect, GraphicsUnit.Pixel);
            }

            Color[] palette = new Color[16];
            int palIndex = 14;
            LoadPalFile("obj.pal");
            for (int j = 0; j < palette.Length; j++)
                palette[j] = PALfilePalette[j + 16 * palIndex];

            byte[] raw = GBAGraphics.ToGBARaw(title_output, palette, GraphicsMode.Tile4bit);
            byte[] lzcomped = LZ77.Compress(raw);

            File.WriteAllBytes(filename, lzcomped);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Color[] palette = new Color[16];
            int palIndex = 14;
            LoadPalFile("obj.pal");
            for (int j = 0; j < palette.Length; j++)
                palette[j] = PALfilePalette[j + 16 * palIndex];

            byte[] raw = GBAGraphics.ToGBARaw(title_output, palette, GraphicsMode.Tile4bit);

            File.WriteAllBytes("test.bin",raw);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DirectoryInfo di = Directory.CreateDirectory("..\\..\\asm\\script\\story");
            File.WriteAllText(di.FullName + "\\story.asm", MakeStoryFileString());
            for (int i = 0; i < storyList.Stories.Count; i++)
            {
                DirectoryInfo sdi = di.CreateSubdirectory("episode_" + (i + 1));

                File.WriteAllText(sdi.FullName + "\\script.asm", MakeScriptString(i));

                WriteTitleToLZ(sdi.FullName + "\\title_lz.bin", i);
            }
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            DrawString(vScrollBar1.Value);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox2.Text = FormatString(textBox2.Text);
        }
    }
}
