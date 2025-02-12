using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.Storage;
using Windows.Media.Render;

namespace GameEngine.Sounds
{
    static class Sound
    {
        static AudioGraph graph;
        static AudioDeviceOutputNode output;
        static Dictionary<string, AudioFileInputNode> inputs = new Dictionary<string, AudioFileInputNode>();
        static bool initialized = false;
        //Check: https://docs.microsoft.com/en-us/windows/uwp/gaming/working-with-audio-in-your-directx-game
        public static async Task InitSound()
        {
            AudioGraphSettings settings = new AudioGraphSettings(AudioRenderCategory.Media);
            CreateAudioGraphResult result = await AudioGraph.CreateAsync(settings);

            if (result.Status == AudioGraphCreationStatus.Success)
            {
                graph = result.Graph;

                //Create output device for audio playing
                CreateAudioDeviceOutputNodeResult deviceOutputNodeResult = await graph.CreateDeviceOutputNodeAsync();

                //Check for succes
                if (deviceOutputNodeResult.Status == AudioDeviceNodeCreationStatus.Success)
                {
                    output = deviceOutputNodeResult.DeviceOutputNode;
                    graph.ResetAllNodes();

                    graph.Start();
                    initialized = true;
                }
            }
        }

        public static async Task LoadSound(string sound)
        {
            if (initialized)
            {
                //Stop graph for loading new sound
                graph.Stop();

                StorageFile soundFile = await StorageFile.GetFileFromPathAsync(Environment.CurrentDirectory + @"\resources\sounds\" + sound);
                CreateAudioFileInputNodeResult fileInputResult = await graph.CreateFileInputNodeAsync(soundFile);

                if (fileInputResult.Status == AudioFileNodeCreationStatus.Success)
                {
                    inputs.Add(soundFile.Name, fileInputResult.FileInputNode);
                    fileInputResult.FileInputNode.Stop();
                    fileInputResult.FileInputNode.AddOutgoingConnection(output);
                    graph.Start();
                }
            }
        }

        public static void PlaySound(string sound)
        {
            if (initialized)
            {
                AudioFileInputNode node;
                if (!inputs.TryGetValue(sound, out node)) throw new ArgumentException("There is no loaded sound with name: " + sound);
                node.Reset();
                node.Start();
            }
        }
    }
}
