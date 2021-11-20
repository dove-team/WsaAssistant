﻿namespace WSATools.Libs
{
    public delegate void CloseHandler(object sender, bool? result);
    public delegate void BooleanHandler(object sender, bool state);
    public delegate void ProgressHandler(string progressPercentage);
    public delegate void ProgressCompleteHandler(object sender, bool hasError, string filePath);
}