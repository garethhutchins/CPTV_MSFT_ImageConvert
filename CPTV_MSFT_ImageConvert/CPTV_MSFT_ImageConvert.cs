using System;
using System.Drawing;
using Emc.InputAccel.CaptureClient;
using System.IO;

//Written by Gareth as just an example of how 3rd party image libs can be used.

namespace CPTV_MSFT_ImageConvert
{
    public class CPTV_MSFT_ImageConvert : CustomCodeModule
    {
        public override void ExecuteTask(IClientTask task, IBatchContext batchContext)
        {
            //First Get the name of the current Step
            String StepName = task.BatchNode.StepData.StepName;
            //Now get the inputfiles for each page
            if (task.BatchNode.RootLevel > 0)
            {
                foreach (IBatchNode p in task.BatchNode.GetDescendantNodes(0))
                {
                    ConvertImage(p);
                }
            }
            else
            {
                IBatchNode p = task.BatchNode;
                ConvertImage(p);
            }

            //Now end
            task.CompleteTask();
        }

        private void ConvertImage(IBatchNode p)
        {
            //Get the Data from the file
            Stream s = new MemoryStream();
            s = p.NodeData.ValueSet.ReadFile("InputImage").ReadData();
            //Get Image data from the stream
            Image i = Image.FromStream(s);
            //Now write the data back to a stream
            Stream ns = new MemoryStream();
            i.Save(ns, System.Drawing.Imaging.ImageFormat.Jpeg);
            //Now save it back as a Stage File
            byte[] fileData;
            ns.Position = 0;
            using (var streamReader = new MemoryStream())
            {
                ns.CopyTo(streamReader);
                fileData = streamReader.ToArray();
            }
            p.NodeData.ValueSet.WriteFileData("OutputImage", fileData, "Jpeg");
        }

        public override void StartModule(ICodeModuleStartInfo startInfo)
        {
            //Do nothing
        }

    }
}