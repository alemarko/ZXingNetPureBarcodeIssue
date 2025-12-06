using System;
using System.IO;
using ZXing;

internal class Program
{
	public static void Main(string[] args)
	{
		var text = "ABC-123-abc";

		var pngBytesSystemDrawing = CreateCode128PngUsingSystemDrawing(text);
		var outFilePathSystemDrawing = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "bc128-net9-SystemDrawing.png");
		File.WriteAllBytes(outFilePathSystemDrawing, pngBytesSystemDrawing);

		var pngBytesImageSharp = CreateCode128PngUsingImageSharp(text);
		var outFilePathImageSharp = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "bc128-net9-ImageSharp.png");
		File.WriteAllBytes(outFilePathImageSharp, pngBytesImageSharp);

		var pngBytesSkiaSharp = CreateCode128PngUsingSkiaSharp(text);
		var outFilePathSkiaSharp = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "bc128-net9-SkiaSharp.png");
		File.WriteAllBytes(outFilePathSkiaSharp, pngBytesSkiaSharp);
	}

	private static (BarcodeFormat Format, ZXing.Common.EncodingOptions Options) GetBarcodeFormatAndOptions =>
		(BarcodeFormat.CODE_128, new ZXing.OneD.Code128EncodingOptions
		{
			ForceCodesetB = false,
			PureBarcode = false,
			Width = 20,
			Height = 30,
			Margin = 0
		});

	public static byte[] CreateCode128PngUsingSystemDrawing(string text)
	{
		var writer = new BarcodeWriterGeneric();
		(writer.Format, writer.Options) = GetBarcodeFormatAndOptions;

		using var bitmap = writer.WriteAsBitmap(text);
		using var ms = new MemoryStream();
		bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
		return ms.ToArray();
	}

	public static byte[] CreateCode128PngUsingSkiaSharp(string text)
	{
		var writer = new ZXing.SkiaSharp.BarcodeWriter();
		(writer.Format, writer.Options) = GetBarcodeFormatAndOptions;

		var skBitmap = writer.Write(text);
		return skBitmap.Encode(SkiaSharp.SKEncodedImageFormat.Png, quality: 100).ToArray();
	}

	public static byte[] CreateCode128PngUsingImageSharp(string text)
	{
		var writer = new BarcodeWriterPixelData();
		(writer.Format, writer.Options) = GetBarcodeFormatAndOptions;

		var pixelData = writer.Write(text);
		using var qrImage = SixLabors.ImageSharp.Image.LoadPixelData<SixLabors.ImageSharp.PixelFormats.Rgba32>(
			pixelData.Pixels, pixelData.Width, pixelData.Height);
		using var ms = new MemoryStream();
		SixLabors.ImageSharp.ImageExtensions.SaveAsPng(qrImage, ms);
		return ms.ToArray();
	}
}

