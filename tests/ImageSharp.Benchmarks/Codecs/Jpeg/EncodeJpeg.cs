// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.Drawing.Imaging;
using System.IO;
using BenchmarkDotNet.Attributes;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Tests;
using SDImage = System.Drawing.Image;

namespace SixLabors.ImageSharp.Benchmarks.Codecs.Jpeg
{
    public class EncodeJpeg
    {
        private const string TestImage = TestImages.Jpeg.BenchmarkSuite.Jpeg420Exif_MidSizeYCbCr;
        private const int EncodingQuality = 100;

        // GDI+ uses 4:1:1 subsampling - https://stackoverflow.com/questions/745610/how-to-disable-subsampling-with-net-gdi
        // ImageSharp lowest subsampling is 4:2:0 which is an okay approximation
        private const JpegSubsample EncodingSubsampling = JpegSubsample.Ratio420;

        // System.Drawing
        private SDImage bmpDrawing;
        private Stream bmpStream;
        private ImageCodecInfo jpegCodec;
        private EncoderParameters encoderParameters;

        // ImageSharp
        private Image<Rgba32> bmpCore;
        private JpegEncoder encoder;

        private MemoryStream destinationStream;

        [GlobalSetup]
        public void ReadImages()
        {
            if (this.bmpStream == null)
            {
                this.bmpStream = File.OpenRead(Path.Combine(TestEnvironment.InputImagesDirectoryFullPath, TestImage));

                this.bmpCore = Image.Load<Rgba32>(this.bmpStream);
                this.bmpCore.Metadata.ExifProfile = null;
                this.encoder = new JpegEncoder { Quality = EncodingQuality, Subsample = EncodingSubsampling };

                this.bmpStream.Position = 0;
                this.bmpDrawing = SDImage.FromStream(this.bmpStream);
                this.jpegCodec = GetEncoder(ImageFormat.Jpeg);
                this.encoderParameters = new EncoderParameters(1);
                // Quality cast to long is necessary
                this.encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, (long)EncodingQuality);

                this.destinationStream = new MemoryStream();
            }
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            this.bmpStream.Dispose();
            this.bmpStream = null;
            this.bmpCore.Dispose();
            this.bmpDrawing.Dispose();
        }

        [Benchmark(Baseline = true, Description = "System.Drawing Jpeg")]
        public void JpegSystemDrawing()
        {
            this.bmpDrawing.Save(this.destinationStream, this.jpegCodec, this.encoderParameters);
            this.destinationStream.Seek(0, SeekOrigin.Begin);
        }

        [Benchmark(Description = "ImageSharp Jpeg")]
        public void JpegCore()
        {
            this.bmpCore.SaveAsJpeg(this.destinationStream, this.encoder);
            this.destinationStream.Seek(0, SeekOrigin.Begin);
        }

        // https://docs.microsoft.com/en-us/dotnet/api/system.drawing.imaging.encoderparameter?redirectedfrom=MSDN&view=net-5.0
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}

/*
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
Intel Core i7-6700K CPU 4.00GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=6.0.100-preview.3.21202.5
  [Host]     : .NET Core 3.1.13 (CoreCLR 4.700.21.11102, CoreFX 4.700.21.11602), X64 RyuJIT
  DefaultJob : .NET Core 3.1.13 (CoreCLR 4.700.21.11102, CoreFX 4.700.21.11602), X64 RyuJIT


|                Method |     Mean |    Error |   StdDev | Ratio | RatioSD |
|---------------------- |---------:|---------:|---------:|------:|--------:|
| 'System.Drawing Jpeg' | 39.54 ms | 0.269 ms | 0.225 ms |  1.00 |    0.00 |
|     'ImageSharp Jpeg' | 47.25 ms | 0.937 ms | 1.219 ms |  1.20 |    0.02 |
*/
