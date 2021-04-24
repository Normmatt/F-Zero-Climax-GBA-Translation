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

namespace ProfileTranslatorTool
{
    public partial class Form1 : Form
    {
        private GBAROM ROM;
        private ProfileList profileList = new ProfileList();

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

        private const int maxWidth = 108;

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

            g2 = pictureBox4.CreateGraphics();
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
            output += String.Format("TextNL equ 0x0A") + Environment.NewLine;
            output += String.Format("TextEnd equ .byte 0x0") + Environment.NewLine;
            output += Environment.NewLine;
            output += String.Format(".org 0x08CF9E54") + Environment.NewLine;
            output += String.Format("ProfileInfoList:") + Environment.NewLine;

            for (int i = 0; i < profileList.Profiles.Count; i++)
            {
                output += String.Format("\t; {0}", profileList.Profiles[i].Name) + Environment.NewLine;
                output += String.Format("\t.byte 0x{0:X02} ; BloodType", profileList.Profiles[i].BloodType) + Environment.NewLine;
                output += String.Format("\t.byte 0x{0:X02} ; Age", profileList.Profiles[i].Age) + Environment.NewLine;
                output += String.Format("\t.byte 0x{0:X02} ; BirthMonth", profileList.Profiles[i].BirthMonth) + Environment.NewLine;
                output += String.Format("\t.byte 0x{0:X02} ; BirthDay", profileList.Profiles[i].BirthDay) + Environment.NewLine;
                output += String.Format("\t.word Profile{0}_CharacterProfile", i + 1) + Environment.NewLine;
                output += String.Format("\t.word Profile{0}_VehicleProfile", i + 1) + Environment.NewLine;
                output += Environment.NewLine;
            }

            output += Environment.NewLine;

            output += Environment.NewLine;
            output += String.Format(".org 0x08DF0000") + Environment.NewLine;
            output += Environment.NewLine;

            for (int i = 0; i < profileList.Profiles.Count; i++)
            {
                output += String.Format("\t.include \"profile_{0}/script.asm\"", i + 1) + Environment.NewLine;
            }

            output += Environment.NewLine;
            output += ".close";
            output += Environment.NewLine;
            output += " ; make sure to leave an empty line at the end";

            return output;
        }

        private string MakeScriptString(int id)
        {
            var oldText = profileList.Profiles[id].CharacterProfile.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            var newText = FormatString(oldText).Replace("\"", "\\\"").Replace("\r", "").Replace("\n", "\",TextNL,\"").Replace(",\"\"", "");

            var oldText2 = profileList.Profiles[id].VehicleProfile.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            var newText2 = FormatString(oldText2).Replace("\"", "\\\"").Replace("\r", "").Replace("\n", "\",TextNL,\"").Replace(",\"\"", "");

            var output = " ; F-Zero Climax Translation by Normmatt" + Environment.NewLine;

            output += Environment.NewLine;
            output += String.Format(".align 4") + Environment.NewLine;

            output += String.Format("Profile{0}_CharacterProfile:", id + 1) + Environment.NewLine;
            output += String.Format("\t.sjis \"{0}\"", newText) + Environment.NewLine;

            output += Environment.NewLine;
            output += String.Format(".align 4") + Environment.NewLine;
            output += String.Format("Profile{0}_VehicleProfile:", id + 1) + Environment.NewLine;
            output += String.Format("\t.sjis \"{0}\"", newText2) + Environment.NewLine;

            output += Environment.NewLine;
            output += Environment.NewLine;
            output += " ; make sure to leave an empty line at the end";

            return output;
        }

        private void UpdateListBox()
        {
            listBox1.Items.Clear();
            foreach (var profile in profileList.Profiles)
            {
                listBox1.Items.Add(profile.Name);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //profileList.Profiles.Add(new Profile() { Title = textBox1.Text, Text = textBox2.Text});
            //profileList.Save("story.xml");
            //UpdateListBox();
            //listBox1.SelectedIndex = profileList.Stories.Count - 1;
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

            profileList.Load("profiles.xml");

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

            textBox2.Text = profileList.Profiles[listBox1.SelectedIndex].CharacterProfile;
            textBox3.Text = profileList.Profiles[listBox1.SelectedIndex].VehicleProfile;

            Color[] palette = new Color[16];
            int palIndex = 14;
            
            //title pointer list = 0x00CF7BAC (4bpp)
            LoadPalFile("obj.pal");
            for (int j = 0; j < palette.Length; j++)
                palette[j] = PALfilePalette[j + 16 * palIndex];

            //pictureBox1.Image = title;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            vScrollBar1.Value = vScrollBar1.Minimum;
            vScrollBar2.Value = vScrollBar2.Minimum;
            textBox2.Text = FormatString(textBox2.Text);
            textBox3.Text = FormatString(textBox3.Text);
            profileList.Profiles[listBox1.SelectedIndex].CharacterProfile = textBox2.Text;
            profileList.Profiles[listBox1.SelectedIndex].VehicleProfile = textBox3.Text;
            profileList.Save("profiles.xml");
        }

        private string FormatString(string oldText)
        {
            //oldText = oldText.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
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

                //If an unknown character (sjis) appears just return original string
                if (oldText[i] > 0x80)
                    return oldText;

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
            var charHeight = 14;
            var maxHeight = 18 * charHeight;

            g.Clear(Color.FromArgb(14*8,12*8,11*8));
            g.ResetTransform();
            g.ScaleTransform(multiplier, multiplier);

            g.DrawLine(Pens.Magenta, maxWidth, 0, maxWidth, maxHeight);

            g.DrawLine(Pens.Magenta, 0, maxHeight, maxWidth, maxHeight);

            for (int index = start; index < textBox2.Lines.Length; index++)
            {
                foreach (var c in textBox2.Lines[index])
                {
                    var width = 16;
                    if(c < 0x80)
                        width = widthTable[c - 32];
                    var sRect = new Rectangle(0, (c - 32)*16, width, 16);
                    var dRect = new Rectangle(x, y, width, 16);
                    g.DrawImage(font, dRect, sRect, GraphicsUnit.Pixel);
                    x += width;
                    //Game bugs out if curWidth is not half word aligned
                    x += x & 1;
                }
                x = 0;
                y += charHeight;
            }
        }

        private void DrawString2(int start = 0)
        {
            int x = 0;
            int y = 0;
            int multiplier = 1;
            var charHeight = 14;
            var maxHeight = 18 * charHeight;

            g2.Clear(Color.FromArgb(14 * 8, 12 * 8, 11 * 8));
            g2.ResetTransform();
            g2.ScaleTransform(multiplier, multiplier);

            g2.DrawLine(Pens.Magenta, maxWidth, 0, maxWidth, maxHeight);

            g2.DrawLine(Pens.Magenta, 0, maxHeight, maxWidth, maxHeight);

            for (int index = start; index < textBox3.Lines.Length; index++)
            {
                foreach (var c in textBox3.Lines[index])
                {
                    var width = 16;
                    if (c < 0x80)
                        width = widthTable[c - 32];
                    var sRect = new Rectangle(0, (c - 32) * 16, width, 16);
                    var dRect = new Rectangle(x, y, width, 16);
                    g2.DrawImage(font, dRect, sRect, GraphicsUnit.Pixel);
                    x += width;
                    //Game bugs out if curWidth is not half word aligned
                    x += x & 1;
                }
                x = 0;
                y += charHeight;
            }
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
            //DrawString2(profileList.Stories[id].Title);

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
            DirectoryInfo di = Directory.CreateDirectory("..\\..\\asm\\script\\profiles");
            File.WriteAllText(di.FullName + "\\profiles.asm", MakeStoryFileString());
            for (int i = 0; i < profileList.Profiles.Count; i++)
            {
                DirectoryInfo sdi = di.CreateSubdirectory("profile_" + (i + 1));

                File.WriteAllText(sdi.FullName + "\\script.asm", MakeScriptString(i));
            }
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            DrawString(vScrollBar1.Value);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox2.Text = FormatString(textBox2.Text);
            textBox3.Text = FormatString(textBox3.Text);
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            string[] names = new string[]
            {
                "Captain Falcon", //07
                "Dr. Stewart", //03
                "Pico", //06
                "Samurai Gorox", //05
                "Jody Summer", //02
                "Mighty Gazzele", //01
                "Baba", //04
                "Octoman", //08
                "Clash", //29
                "Ead", //09
                "Bio Rex", //15
                "Billy", //11
                "Silver Neelsen", //23
                "Gomar & Shioh", //22
                "John Tanaka", //26
                "Mrs Arrow", //21
                "Blood Falcon", //25
                "Jack Levin", //14
                "James McCloud", //10
                "Zoda", //13
                "Michael Chain", //24
                "Super Arrow", //20
                "Kate Alen", //12
                "Roger Buster", //28
                "Leon", //19
                "Draq", //27
                "Beastman", //18
                "Antonio Guster", //17
                "Black Shadow", //30
                "The Skull", //16
                "Ryu Suzaku", //00
                "Lisa Brilliant", //33
                "Lucy Liberty", //31
                "Miss Killer", //32
                "Dark Soldier", //34
                "Berserker", //77
                "Clank Hughes", //00
                "(unused)", //Never used in game
                "Hyper Zoda", //13
            };

            var profileBasePtr = 0xCF9E54;

            profileList.Profiles.Clear();
            for (int i=0; i<39; i++)
            {
                var profilePtr = profileBasePtr + 12 * i;
                var profile = new Profile();

                profile.BloodType = ROM.GetU8(profilePtr + 0); //Too Lazy to decode this to a string
                profile.Age = ROM.GetU8(profilePtr + 1);
                profile.BirthMonth = ROM.GetU8(profilePtr + 2);
                profile.BirthDay = ROM.GetU8(profilePtr + 3);
                profile.CharacterProfile = ROM.GetSjisString((int)ROM.GetU32(profilePtr + 4) - 0x08000000);
                profile.VehicleProfile = ROM.GetSjisString((int)ROM.GetU32(profilePtr + 8) - 0x08000000);
                profile.Name = names[i];

                profileList.Profiles.Add(profile);
            }

            profileList.Save("profiles.xml");
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            //Apparently saving to XML replaces \r\n with just \n so replace it here so preview works correctly
            textBox3.Text = Regex.Replace(textBox3.Text, "(?<!\r)\n", "\r\n");
            DrawString2();
            if (textBox3.Lines.Length >= 25)
            {
                vScrollBar2.Maximum = textBox3.Lines.Length - 25; //25 visible lines
                vScrollBar2.Value = vScrollBar2.Minimum;
            }
            else
            {
                vScrollBar2.Maximum = 0;
                vScrollBar2.Value = 0;
            }
        }
    }
}
