using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Diagnostics;
using System.IO;

namespace WSATools.Untils
{
    public sealed class Zipper
    {
        public static bool UnZip(string zipFileName, string targetDirectory)
        {
            try
            {
                var zip = new FastZip();
                zip.ExtractZip(zipFileName, targetDirectory, "");
                File.Delete(zipFileName);
                return Adb.Instance.HasBrige;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return false;
        }
    }
}