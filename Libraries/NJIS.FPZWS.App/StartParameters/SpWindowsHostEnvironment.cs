﻿// ************************************************************************************
//  解决方案：NJIS.FPZWS.PDD.DataGenerate
//  项目名称：NJIS.FPZWS.PDD
//  文 件 名：SpWindowsHostEnvironment.cs
//  创建时间：2016-11-11 12:50
//  作    者：
//  说    明：
//  修改时间：2017-07-10 12:07
//  修 改 人：
// Copyright © 2017 广州宁基智能系统有限公司. 版权所有
// *************************************************************************************

#region

using System;
using System.Configuration.Install;
using Topshelf;
using Topshelf.HostConfigurators;
using Topshelf.Runtime;
using Topshelf.Runtime.Windows;

#endregion

namespace NJIS.FPZWS.App.StartParameters
{
    public class SpWindowsHostEnvironment : HostEnvironment
    {
        private readonly WindowsHostEnvironment _environement;
        private readonly HostConfigurator _hostConfigurator;

        public SpWindowsHostEnvironment(HostConfigurator configurator)
        {
            _hostConfigurator = configurator;
            _environement = new WindowsHostEnvironment(configurator);
        }

        public bool IsServiceInstalled(string serviceName)
        {
            return _environement.IsServiceInstalled(serviceName);
        }

        public bool IsServiceStopped(string serviceName)
        {
            return _environement.IsServiceStopped(serviceName);
        }

        public void StartService(string serviceName, TimeSpan startTimeOut)
        {
            _environement.StartService(serviceName, startTimeOut);
        }

        public void StopService(string serviceName, TimeSpan stopTimeOut)
        {
            _environement.StopService(serviceName, stopTimeOut);
        }

        public string CommandLine
        {
            get { return _environement.CommandLine; }
        }

        public bool IsAdministrator
        {
            get { return _environement.IsAdministrator; }
        }

        public bool IsRunningAsAService
        {
            get { return _environement.IsRunningAsAService; }
        }

        public bool RunAsAdministrator()
        {
            return _environement.RunAsAdministrator();
        }

        public Host CreateServiceHost(HostSettings settings, ServiceHandle serviceHandle)
        {
            return _environement.CreateServiceHost(settings, serviceHandle);
        }

        public void SendServiceCommand(string serviceName, int command)
        {
            _environement.SendServiceCommand(serviceName, command);
        }

        public void InstallService(InstallHostSettings settings, Action beforeInstall, Action afterInstall,
            Action beforeRollback, Action afterRollback)
        {
            using (var installer = new SpHostServiceInstaller(settings, _hostConfigurator))
            {
                Action<InstallEventArgs> before = x =>
                {
                    if (beforeInstall != null)
                        beforeInstall();
                };

                Action<InstallEventArgs> after = x =>
                {
                    if (afterInstall != null)
                        afterInstall();
                };

                Action<InstallEventArgs> before2 = x =>
                {
                    if (beforeRollback != null)
                        beforeRollback();
                };

                Action<InstallEventArgs> after2 = x =>
                {
                    if (afterRollback != null)
                        afterRollback();
                };

                installer.InstallService(before, after, before2, after2);
            }
        }

        public void UninstallService(HostSettings settings, Action beforeUninstall, Action afterUninstall)
        {
            _environement.UninstallService(settings, beforeUninstall, afterUninstall);
        }
    }
}