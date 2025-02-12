using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Graphics.DirectX;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

using GameEngine.Levels;

namespace GameEngine.Graphics
{
    public class Sprite
    {
        private static Dictionary<int, Sprite> sprites = new Dictionary<int, Sprite>();
        private static Dictionary<string, int> table = new Dictionary<string, int>();

        private static CanvasAnimatedControl canvas = null;

        int _width, _height;
        bool square;
        CanvasBitmap _sprite;

        private Sprite(int width, int height, CanvasBitmap sprite)
        {
            _width = width;
            _height = height;
            if (_width == _height) square = true;
            else square = false;
            _sprite = sprite;
        }

        public bool IsSquare()
        {
            return square;
        }

        public int GetWidth()
        {
            return _width;
        }

        public int GetHeight()
        {
            return _height;
        }

        public CanvasBitmap GetBitmap()
        {
            return _sprite;
        }

        public static void Init(CanvasAnimatedControl canvas)
        {
            Sprite.canvas = canvas;
        }

        public static Sprite GetSprite(int id)
        {
            if (!sprites.TryGetValue(id, out Sprite sprite)) return null;
            return sprite;
        }

        public static Sprite GetSprite(string name)
        {
            if (!table.TryGetValue(name, out int id)) throw new ArgumentException("Cannot find name: " + name);
            if (!sprites.TryGetValue(id, out Sprite sprite)) throw new ArgumentException("Cannot find sprite with name: " + name + " and ID: " + id);
            return sprite;
        }

        public static int GetSpriteID(string name)
        {
            if (!table.TryGetValue(name, out int id)) throw new ArgumentException("Cannot find name: " + name);
            return id;        
        }

        public static CanvasBitmap CreateBitmapFromBytes(byte[] data, int width, int height)
        {
            if (canvas == null) return null;

            return CanvasBitmap.CreateFromBytes(canvas, data, width, height, DirectXPixelFormat.R8G8B8A8UIntNormalized);
        }

        public static bool CreateSpriteFromColor(string name,int id,int width, int height, byte red, byte green, byte blue)
        {
            if (canvas == null) return false;

            byte[] bitmap_bytes = new byte[width * height * 4];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width * 4; x+=4) //Increase by 4 because each color is 4 byte
                {
                    bitmap_bytes[x + 0 + y * width * 4] = red;
                    bitmap_bytes[x + 1 + y * width * 4] = green;
                    bitmap_bytes[x + 2 + y * width * 4] = blue;
                    bitmap_bytes[x + 3 + y * width * 4] = 0xFF; //Alpha
                }
            }
            CanvasBitmap bitmap = CanvasBitmap.CreateFromBytes(canvas, bitmap_bytes, width, height, DirectXPixelFormat.R8G8B8A8UIntNormalized);
            Sprite sprite = new Sprite(width, height, bitmap);

            sprites.Add(id, sprite);
            table.Add(name, id);

            if (sprites.ContainsKey(id))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Sprite betoltes
        /// </summary>
        /// <param name="spriteSheetDataPath">sheet_data helye</param>
        /// <param name="spriteSheetPath">Spritesheet helye</param>
        /// <returns></returns>
        public static async Task<bool> LoadSheet(string spriteSheetDataPath, string spriteSheetPath)
        {
            if (canvas == null) return false;
            //Load whole sheet
            byte[] bytes;
            int sheetWidth;
            int sheetHeight;
            try {
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

            int x, y, id, xAbs, yAbs, w, h;
            bool solid, penetrateable;
            string name;
            //Sheet data proccesing
            MatchCollection matches = Regex.Matches(data, @"(\w+) (\d+) (\d+) (\d+) (\d+) (\d+) (\d) (\d)");
            foreach (Match match in matches)
            {
                name = match.Groups[1].Value;
                x = int.Parse(match.Groups[2].Value);
                y = int.Parse(match.Groups[3].Value);
                w = int.Parse(match.Groups[4].Value);
                h = int.Parse(match.Groups[5].Value);
                id = int.Parse(match.Groups[6].Value);
                byte[] bitmap_bytes = new byte[w * h * 4];

                //Out of bound check
                if (x * w > sheetWidth || y * h > sheetHeight || x < 0 || y < 0) continue;

                for (int by = 0; by < h; by++)
                {
                    yAbs = by + y * h;
                    for (int bx = 0; bx < w * 4; bx++)
                    {
                        xAbs = bx + x * w * 4;
                        bitmap_bytes[bx + by * w * 4] = bytes[xAbs + yAbs * sheetWidth * 4];
                    }
                }

                CanvasBitmap bitmap = CanvasBitmap.CreateFromBytes(canvas, bitmap_bytes, w, h, DirectXPixelFormat.R8G8B8A8UIntNormalized);

                if (bitmap == null) return false;

                Sprite sprite = new Sprite(w, h, bitmap);
                sprites.Add(id, sprite);
                table.Add(name, id);
                
                //Data for tiles
                solid = 1 == int.Parse(match.Groups[7].Value);
                penetrateable = 1 == int.Parse(match.Groups[8].Value);
     
                //Create tiles based on sprites
                new Tile(solid, penetrateable, id);

                if (sprites.ContainsKey(id))
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }
}
