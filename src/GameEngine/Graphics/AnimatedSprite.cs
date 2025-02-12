using System;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.Storage;
using System.Text.RegularExpressions;
using Windows.Graphics.DirectX;
using System.Collections.Generic;

using GameEngine.Interfaces;
using Windows.Devices.Geolocation;
using System.Diagnostics;

namespace GameEngine.Graphics
{
    class AnimatedSprite : IUpdateable
    {
        private static Dictionary<string, AnimatedSprite> _animatedSprites = new Dictionary<string, AnimatedSprite>();
        private static CanvasAnimatedControl _canvas = null;
        private static List<IUpdateable> _updateList = new List<IUpdateable>();

        CanvasBitmap[] _bitmaps;
        int _length;
        int _index = 0;
        int _rate;
        int _timeMax;
        int _time = 0;

        private AnimatedSprite(string name, int lenght, int rate, CanvasBitmap[] bitmaps)
        {
            _length = lenght - 1;
            _bitmaps = bitmaps;
            _rate = rate;

            _timeMax = rate * lenght;

            _updateList.Add(this);
        }

        public CanvasBitmap GetSprite()
        {
            return _bitmaps[_index];
        }

        public void Update()
        {
            if (_rate == 0) return;
            _time++;
            if (_time % _rate == 0)
            {
                if (_index >= _length)
                {
                    _index = 0;
                }
                else
                {
                    _index++;
                }
            }
            if (_time == _timeMax)
            {
                _time = 0;
            }
        }

        public static List<IUpdateable> GetUpdateables()
        {
            return _updateList;
        }

        public static void Init(CanvasAnimatedControl canvas)
        {
            _canvas = canvas;
        }

        public static AnimatedSprite GetAnimatedSprite(string name)
        {
            if (_animatedSprites.ContainsKey(name))
            {
                return _animatedSprites[name];
            }
            else
            {
                throw new ArgumentException("Missing AnimatedSprite: " + name);
            }
        }

        public static async Task<bool> LoadSheet(string spriteSheetDataPath, string spriteSheetPath)
        {
            if (_canvas == null) return false;
            //Load whole sheet
            byte[] bytes;
            int sheetWidth;
            int sheetHeight;
            try
            {
                StorageFile sheet_file = await StorageFile.GetFileFromPathAsync(Environment.CurrentDirectory + spriteSheetPath);
                IRandomAccessStream stream = await RandomAccessStreamReference.CreateFromFile(sheet_file).OpenReadAsync();
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                PixelDataProvider pixelData = await decoder.GetPixelDataAsync();
                bytes = pixelData.DetachPixelData();
                sheetWidth = Convert.ToInt32(decoder.PixelWidth);
                sheetHeight = Convert.ToInt32(decoder.PixelHeight);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            //Load sheet data
            string data;
            try
            {
                StorageFile data_file = await StorageFile.GetFileFromPathAsync(Environment.CurrentDirectory + spriteSheetDataPath);
                data = await FileIO.ReadTextAsync(data_file);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            //Process sheet data
            string name;
            int rate, length;
            int x, y, xAbs, yAbs;
            byte[] bitmap_bytes;
            int WDS;
            int HDS;
        //Sheet data proccesing
        MatchCollection matches = Regex.Matches(data, @"(\w+) (\d+) (\d+) (\d+) (\d+) (\d+) (\d+)");
            foreach (Match match in matches)
            {
                //AnimatedSprite data
                name = match.Groups[1].Value;
                length = int.Parse(match.Groups[2].Value);
                rate = int.Parse(match.Groups[3].Value);
                //Dissection start point
                x = int.Parse(match.Groups[4].Value);
                y = int.Parse(match.Groups[5].Value);

                WDS = int.Parse(match.Groups[6].Value);
                HDS = int.Parse(match.Groups[7].Value);

                bitmap_bytes = new byte[WDS * HDS * 4];

                CanvasBitmap[] bitmaps = new CanvasBitmap[length];

                int ySave = y;
                for (int i = 0; i < length;i++)
                {
                    y = ySave + i * HDS;
                   //Out of bound check
                    if ((x + WDS) > sheetWidth || (y + HDS) > sheetHeight || x < 0 || y < 0) continue;

                    for (int by = 0; by < HDS; by++)
                    {
                        yAbs = by + y;
                        for (int bx = 0; bx < WDS * 4; bx++)
                        {
                            xAbs = bx + x * 4;
                            bitmap_bytes[bx + by * WDS * 4] = bytes[xAbs + yAbs * sheetWidth * 4];
                        }
                    }
                    CanvasBitmap bitmap = CanvasBitmap.CreateFromBytes(_canvas, bitmap_bytes, WDS, HDS, DirectXPixelFormat.R8G8B8A8UIntNormalized);

                    if (bitmap == null) throw new Exception("Couldn't create bitmap: " + name);

                    bitmaps[i] = bitmap;
                }

                if (_animatedSprites.ContainsKey(name)) return false;
                _animatedSprites.Add(name, new AnimatedSprite(name, length, rate, bitmaps));
            }

            return true;
        }
    }
}
