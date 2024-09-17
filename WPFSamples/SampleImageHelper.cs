using System.Collections.ObjectModel;
using System.IO;
using System.Security;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using WPFSamples.Controls.PixelLab.Common;

namespace WPFSamples;

public class SampleImageHelper
{
    public IEnumerable<string> ImagePaths => GetPicturePaths();

    public IList<BitmapImage> BitmapImages => GetBitmapImages().ToArray();

    public IList<BitmapImage> BitmapImages6 => GetBitmapImages(6).ToArray();

    private static IEnumerable<string> GetPicturePaths(int maxCount = -1)
    {
        maxCount = maxCount < 0 ? MaxImageReturnCount : Math.Min(maxCount, MaxImageReturnCount);

        string[]? commandLineArgs = null;
        try
        {
            commandLineArgs = Environment.GetCommandLineArgs();
        }
        catch (NotSupportedException)
        {
        }
        catch (SecurityException)
        {
        } // In an XBap

        IEnumerable<string>? picturePaths = null;
        if (commandLineArgs != null)
        {
            picturePaths = commandLineArgs
                .Select(GetPicturePaths)
                .FirstOrDefault(paths => paths.Any());
        }

        picturePaths ??= DefaultPicturePaths
            .Select(GetPicturePaths)
            .FirstOrDefault(value => value.Any());

        return picturePaths.EmptyIfNull().Take(maxCount);
    }

    private static IEnumerable<BitmapImage> GetBitmapImages(int maxCount = -1)
    {
        return from path in GetPicturePaths(maxCount)
            select new BitmapImage(new Uri(path));
    }

    #region impl

    private static IEnumerable<string> GetPicturePaths(string sourceDirectory)
    {
        if (string.IsNullOrEmpty(sourceDirectory)) return [];
        try
        {
            DirectoryInfo di = new(sourceDirectory);
            if (di.Exists)
            {
                return di
                    .EnumerateFiles(DefaultImageSearchPattern, SearchOption.AllDirectories)
                    .Select(fi => fi.FullName);
            }
        }
        catch (IOException)
        {
        }
        catch (ArgumentException)
        {
        }
        catch (SecurityException)
        {
        }

        return [];
    }

    private static readonly ReadOnlyCollection<string> DefaultPicturePaths = GetDefaultPicturePaths();

    private static ReadOnlyCollection<string> GetDefaultPicturePaths()
    {
        if (BrowserInteropHelper.IsBrowserHosted)
        {
            return Array.Empty<string>().ToReadOnlyCollection();
        }

        return new[]
        {
            @"C:\Users\Public\Pictures\Sample Pictures\",
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Spotlight")
        }.ToReadOnlyCollection();
    }

    private const string DefaultImageSearchPattern = @"*.jpg";
    private const int MaxImageReturnCount = 50;

    #endregion
}