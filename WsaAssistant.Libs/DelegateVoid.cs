using System;

namespace WsaAssistant.Libs
{
    public delegate void BooleanHandler(object sender, bool state);
    public delegate void ProgressHandler(string progressPercentage);
    public delegate void ProgressCompleteHandler(object sender, bool hasError, Uri downloadUrl, string filePath);
}