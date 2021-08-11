using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace MicroscopeGUI.Helper
{
    public class Metadata
    {
        public string[] GetTags(string filename)
        {
            // open a filestream for the file we wish to look at
            using (Stream fs = File.Open(filename, FileMode.Open, FileAccess.ReadWrite))
            {
                // create a decoder to parse the file
                BitmapDecoder decoder = BitmapDecoder.Create(fs, BitmapCreateOptions.None, BitmapCacheOption.Default);
                // grab the bitmap frame, which contains the metadata
                BitmapFrame frame = decoder.Frames[0];
                // get the metadata as BitmapMetadata
                BitmapMetadata metadata = frame.Metadata as BitmapMetadata;
                // close the stream before returning
                fs.Close();

                // return a null array if keywords don't exist.  otherwise, return a string array
                if (metadata != null && metadata.Keywords != null){
                    return metadata.Keywords.ToArray();
                }else
                {
                    return null;
                }
            }
        }

        public static void AddTags(string filename, string[] tags)
        {
            // open a filestream for the file we wish to look at
            using (Stream fs = File.Open(filename, FileMode.Open, FileAccess.ReadWrite))
            {
                // create a decoder to parse the file
                BitmapDecoder decoder = BitmapDecoder.Create(fs, BitmapCreateOptions.None, BitmapCacheOption.Default);
                // grab the bitmap frame, which contains the metadata
                BitmapFrame frame = decoder.Frames[0];
                // get the metadata as BitmapMetadata
                BitmapMetadata metadata = frame.Metadata as BitmapMetadata;
                // instantiate InPlaceBitmapMetadataWriter to write the metadata to the file
                InPlaceBitmapMetadataWriter writer = frame.CreateInPlaceBitmapMetadataWriter();
                string[] keys;
                if (metadata.Keywords != null)      // tags exist - include them when saving
                {
                    // build the complete list of tags - new and old
                    keys = new string[metadata.Keywords.Count + tags.Length];
                    int i = 0;
                    foreach (string keyword in metadata.Keywords)
                    {
                        keys[i] = keyword;
                        i++;
                    }
                    foreach (string tag in tags)
                    {
                        keys[i] = tag;
                        i++;
                    }

                    // associate the tags with the writer
                    // the type of variable to pass (here, an array of strings) depends on
                    // which metadata property you are using.  Since we are modifying the
                    // Keywords property, we use the array.  If you use the author property,
                    // it will simply be a string.
                    writer.SetQuery("System.Keywords", keys);
                }
                else     // no old tags - just use the new ones
                {
                    keys = tags;
                    // associate the tags with the writer
                    // the type of variable to pass (here, an array of strings) depends on
                    // which metadata property you are using.  Since we are modifying the
                    // Keywords property, we use the array.  If you use the author property,
                    // it will simply be a string.
                    writer.SetQuery("System.Keywords", tags);
                }

                // try to save the metadata to the file
                if (!writer.TrySave())
                {
                    // if it fails, there is no room for the metadata to be written to.
                    // we must add room to the file using SetUpMetadataOnImage (defined below)
                    SetUpMetadataOnImage(filename, keys);
                }
            }
        }

        private static void SetUpMetadataOnImage(string filename, string[] tags)
        {
            // padding amount, using 2Kb.  don't need much here; metadata is rather small
            uint paddingAmount = 2048;

            // open image file to read
            using (Stream file = File.Open(filename, FileMode.Open, FileAccess.Read))
            {
                // create the decoder for the original file.  The BitmapCreateOptions and BitmapCacheOption denote
                // a lossless transocde.  We want to preserve the pixels and cache it on load.  Otherwise, we will lose
                // quality or even not have the file ready when we save, resulting in 0b of data written
                BitmapDecoder original = BitmapDecoder.Create(file, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
                // create an encoder for the output file
                JpegBitmapEncoder output = new JpegBitmapEncoder();

                // add padding and tags to the file, as well as clone the data to another object
                if (original.Frames[0] != null && original.Frames[0].Metadata != null)
                {
                    // Because the file is in use, the BitmapMetadata object is frozen.
                    // So, we clone the object and add in the padding.
                    BitmapFrame frameCopy = (BitmapFrame)original.Frames[0].Clone();
                    BitmapMetadata metadata = original.Frames[0].Metadata.Clone() as BitmapMetadata;

                    // we use the same method described in AddTags() as saving tags to save an amount of padding
                    metadata.SetQuery("/app1/ifd/PaddingSchema:Padding", paddingAmount);
                    metadata.SetQuery("/app1/ifd/exif/PaddingSchema:Padding", paddingAmount);
                    metadata.SetQuery("/xmp/PaddingSchema:Padding", paddingAmount);
                    // we add the tags we want as well.  Again, using the same method described above
                    metadata.SetQuery("System.Keywords", tags);

                    // finally, we create a new frame that has all of this new metadata, along with the data that was in the original message
                    output.Frames.Add(BitmapFrame.Create(frameCopy, frameCopy.Thumbnail, metadata, frameCopy.ColorContexts));
                    output.Frames.Add(BitmapFrame.Create(frameCopy, frameCopy.Thumbnail, metadata, original.Frames[0].ColorContexts));
                    file.Close();  // close the file to ready for overwrite
                }
                // finally, save the new file over the old file
                using (Stream outputFile = File.Open(filename, FileMode.Create, FileAccess.Write))
                {
                    output.Save(outputFile);
                }
            }
        }


    }
}
