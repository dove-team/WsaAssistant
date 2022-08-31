﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace WsaAssistant.Libs
{
    public sealed class Command
    {
        private static Command instance;
        public static Command Instance
        {
            get
            {
                if (instance == null)
                    instance = new Command();
                return instance;
            }
        }
        private Command() { }
        public bool Shell(string cmd, out string message,bool isfile = false)
        {
            try
            {
                LogManager.Instance.LogInfo("shell:" + cmd);
                var process = new Process();
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.FileName = @"powershell.exe";
                if(isfile)
                {
                    process.StartInfo.Arguments = $"-File \"{cmd}\"";
                }
                else
                { 
                    process.StartInfo.Arguments = $" {cmd}";
                }
                process.Start();
                message = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                process.Close();
                process.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return false;
        }
        public bool Excute(string cmd, out string message)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.UseShellExecute = false;
                process.Start();
                process.StandardInput.WriteLine($"{cmd}&exit");
                process.StandardInput.AutoFlush = true;
                message = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                process.Close();
                process.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return false;
        }
        public bool Excute(IEnumerable<string> cmds, out string message)
        {
            try
            {
                var total = cmds.Count();
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();
                for (var idx = 0; idx < total; idx++)
                {
                    var cmd = cmds.ElementAt(idx);
                    if (idx == total - 1)
                        process.StandardInput.WriteLine(cmd + "&exit");
                    else
                        process.StandardInput.WriteLine(cmd);
                }
                process.StandardInput.AutoFlush = true;
                message = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                process.Close();
                process.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return false;
        }
    }
}