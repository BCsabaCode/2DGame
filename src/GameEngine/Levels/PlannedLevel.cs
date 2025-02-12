using System;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.Storage;

using GameEngine.Entities.Mobs;

namespace GameEngine.Levels
{
    class PlannedLevel : Level
    {

        public PlannedLevel(string path, string sector_data)
        {
            //mapLoading = new Action(async () => await Load_level(path, sector_data));
            Load_level(path, sector_data);
        }

        async void Load_level(string path, string sector_data)
        {
            byte[] bytes;
            int mapWidth, mapHeight;
            try
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(Environment.CurrentDirectory + path);
                IRandomAccessStream stream = await RandomAccessStreamReference.CreateFromFile(file).OpenReadAsync();
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                PixelDataProvider pixelData = await decoder.GetPixelDataAsync();
                bytes = pixelData.DetachPixelData();

                mapWidth = Convert.ToInt32(decoder.PixelWidth);
                mapHeight = Convert.ToInt32(decoder.PixelHeight);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }


            int[,] floor = new int[mapWidth, mapHeight];

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                { 
                    floor.SetValue(-BitConverter.ToInt32(bytes, (x + y * mapWidth) * sizeof (int)), x, y);
                }
            }

            map = new Map(mapWidth, mapHeight, floor, sector_data);

            AddEntity(new Dummy(0.0f, 0.0f));
        }
    }
}
