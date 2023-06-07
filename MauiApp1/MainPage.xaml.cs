using System.Text.RegularExpressions;
using Microsoft.JSInterop;
using Path = System.IO.Path;

namespace MauiApp1;

public partial class MainPage : ContentPage {
	
	public MainPage() {
		InitializeComponent();
	}

	[JSInvokable]
	public static async Task<string> DataUrlToFile(string base64, string fileName) {
		Match match = Regex.Match(base64, @"data:(?<mediatype>.+?)?(?<isBase64>;base64)?,(?<data>.+)");
		
		string path = null;
#if ANDROID
		if (Android.OS.Environment.IsExternalStorageEmulated)
			path = Android.App.Application.Context!.GetExternalFilesDir(Android.OS.Environment.DirectoryDownloads)!.AbsolutePath;
#endif
		if (path == null)
			path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
		
		if (string.IsNullOrEmpty(fileName))
			fileName = "file";
		path = Path.Combine(path, fileName);

		string data = match.Groups["data"].Value;
		if (match.Groups["isBase64"].Success) {
			byte[] bytes = Convert.FromBase64String(data);
			await File.WriteAllBytesAsync(path, bytes);
		}
		else {
			data = Uri.UnescapeDataString(data);
			await File.WriteAllTextAsync(path, data);
		}
		return $"FileContent: '{await File.ReadAllTextAsync(path)}', FilePath: ${path}";
	}
}