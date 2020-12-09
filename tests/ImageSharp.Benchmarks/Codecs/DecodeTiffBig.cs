// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.IO;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;

using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Tests;

using SDImage = System.Drawing.Image;
using SDSize = System.Drawing.Size;

namespace SixLabors.ImageSharp.Benchmarks.Codecs
{
    [Config(typeof(DecodeTiffBig.Config.LongClr))]
    public class DecodeTiffBig : BenchmarkBase
    {
        private class Config : SixLabors.ImageSharp.Benchmarks.Config
        {
            public class LongClr : Config
            {
                public LongClr()
                {
                    this.AddJob(
                        Job.Default.WithRuntime(ClrRuntime.Net472).WithLaunchCount(1).WithWarmupCount(3).WithIterationCount(5),
                        Job.Default.WithRuntime(CoreRuntime.Core31).WithLaunchCount(1).WithWarmupCount(3).WithIterationCount(5),
                        Job.Default.WithRuntime(CoreRuntime.Core21).WithLaunchCount(1).WithWarmupCount(3).WithIterationCount(5));

                    this.SummaryStyle = SummaryStyle.Default.WithMaxParameterColumnWidth(60);
                }
            }
        }

        private string prevImage = null;

        private byte[] data;

        private string TestImageFullPath => Path.Combine(TestEnvironment.InputImagesDirectoryFullPath, Path.Combine(TestImages.Tiff.Benchmark_Path, this.TestImage));

        [Params(
            TestImages.Tiff.Benchmark_BwFax3,
            //// TestImages.Tiff.Benchmark_RgbFax4,
            TestImages.Tiff.Benchmark_BwRle,
            TestImages.Tiff.Benchmark_GrayscaleUncompressed,
            TestImages.Tiff.Benchmark_PaletteUncompressed,
            TestImages.Tiff.Benchmark_RgbDeflate,
            TestImages.Tiff.Benchmark_RgbLzw,
            TestImages.Tiff.Benchmark_RgbPackbits,
            TestImages.Tiff.Benchmark_RgbUncompressed)]
        public string TestImage { get; set; }

        [IterationSetup]
        public void ReadImages()
        {
            if (this.prevImage != this.TestImage)
            {
                this.data = File.ReadAllBytes(this.TestImageFullPath);
                this.prevImage = this.TestImage;
            }
        }

        [Benchmark(Baseline = true, Description = "System.Drawing Tiff")]
        public SDSize TiffSystemDrawing()
        {
            using (var memoryStream = new MemoryStream(this.data))
            using (var image = SDImage.FromStream(memoryStream))
            {
                return image.Size;
            }
        }

        [Benchmark(Description = "ImageSharp Tiff")]
        public Size TiffCore()
        {
            Configuration config = Configuration.Default.Clone();
            config.StreamProcessingBufferSize = 1024 * 64;

            config.ImageFormatsManager.AddImageFormat(Formats.Experimental.Tiff.TiffFormat.Instance);
            config.ImageFormatsManager.AddImageFormatDetector(new Formats.Experimental.Tiff.TiffImageFormatDetector());
            config.ImageFormatsManager.SetDecoder(Formats.Experimental.Tiff.TiffFormat.Instance, new Formats.Experimental.Tiff.TiffDecoder());

            using (var ms = new MemoryStream(this.data))
            using (var image = Image.Load<Rgba32>(config, ms))
            {
                return image.Size();
            }
        }
    }
}
