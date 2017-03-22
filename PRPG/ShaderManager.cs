/*
    HotSwap shader sytem for MonoGame

    HotSwap code only exists for debug builds
    Edit paths to match your project
    Construct in your Initialize method
    Add shaders in LoadContent (or whenever)
    Call CheckForChanges in Update() or periodically however you like
    mgcb.exe usually located in C:\Program Files (x86)\MSBuild\MonoGame\v3.0\Tools
*/
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using System;
using System.IO;
using System.Diagnostics;
using System.Text;

namespace PRPG
{


    public class ShaderManager {

#if DEBUG
        ContentManager TempContent;
        DateTime LastUpdate;
        const string mgcbPathExe = "c:/projects/prpgcs/Content/MGCB/mgcb.exe";
        const string contentPath = "c:/projects/prpgcs/Content";
#endif

        public Dictionary<string, Effect> Shaders;
        ContentManager Content;
        GraphicsDevice Device;

        public ShaderManager(ContentManager content, GraphicsDevice device) {
            Content = content;
            Device = device;
            Shaders = new Dictionary<string, Effect>();
#if DEBUG
            TempContent = new ContentManager(Content.ServiceProvider, Content.RootDirectory);
            LastUpdate = DateTime.Now;
#endif
        }

#if DEBUG
        public void CheckForChanges() {
            var files = Directory.GetFiles(contentPath, "*.fx");
            foreach (var file in files) {
                var t = File.GetLastWriteTime(file);
                if (t > LastUpdate) {
                    ShaderChanged(file);
                    LastUpdate = t;
                }
            }

        }
        public void ShaderChanged(string path) {
            string name = Path.GetFileNameWithoutExtension(path);
            Process pProcess = new Process {
                StartInfo =
                       {
                            FileName = mgcbPathExe,
                            Arguments = "/platform:DesktopGL /config: /profile:Reach /compress:False /importer:EffectImporter /processor:EffectProcessor /processorParam:DebugMode=Auto /build:"+name+".fx",
                            CreateNoWindow = true,
                            WorkingDirectory = contentPath,
                            UseShellExecute = false,
                            RedirectStandardError = true,
                            RedirectStandardOutput = true
                        }
            };

            //Get program output
            string stdError = null;
            StringBuilder stdOutput = new StringBuilder();
            pProcess.OutputDataReceived += (sender, args) => stdOutput.Append(args.Data);

            try {
                pProcess.Start();
                pProcess.BeginOutputReadLine();
                stdError = pProcess.StandardError.ReadToEnd();
                pProcess.WaitForExit();

                string builtPath = contentPath+"/" + name + ".xnb";
                string movePath = "C:/projects/prpgcs/PRPG/bin/DesktopGL/AnyCPU/Debug/Content/" + name + ".xnb";
                File.Copy(builtPath, movePath, true);

                ContentManager newTemp = new ContentManager(TempContent.ServiceProvider, TempContent.RootDirectory);
                var newShaders = new Dictionary<string, Effect>();
                foreach (var shaderName in Shaders.Keys) {
                    var effect = newTemp.Load<Effect>(shaderName);
                    newShaders.Add(shaderName.ToLower(), effect);
                }

                TempContent.Unload();
                TempContent.Dispose();
                TempContent = newTemp;
                Shaders = newShaders;


            }
            catch (Exception e) {
                //todo log
            }
            finally {

            }


        }
#endif

        public Effect GetShader(string name) {            
            return Shaders[name.ToLower()];
        }

        public void AddShader(string name) {
            if (!Shaders.ContainsKey(name.ToLower())) {
                var shader = Content.Load<Effect>(name);
                Shaders.Add(name.ToLower(), shader);
            }
        }

    }
}