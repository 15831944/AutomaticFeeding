﻿//  ************************************************************************************
//   解决方案：NJIS.FPZWS.LineControl.Drilling
//   项目名称：NJIS.PLC.Communication
//   文 件 名：CommunicationProtocol.cs
//   创建时间：2018-11-08 16:16
//   作    者：
//   说    明：
//   修改时间：2018-11-08 16:16
//   修 改 人：
//  Copyright © 2017 广州宁基智能系统有限公司. 版权所有
//  *************************************************************************************

using System;
using System.Text;
using NJIS.PLC.Communication.BasicFramework;
using NJIS.PLC.Communication.Core.Security;

namespace NJIS.PLC.Communication.Core.Net
{
    /*******************************************************************************
     * 
     *    一个通信辅助类，对通信中的数据做了信号区分
     *
     *    Communications helper classes, the data signal in a communications distinction
     * 
     *******************************************************************************/


    /// <summary>
    ///     用于本程序集访问通信的暗号说明
    /// </summary>
    internal class NjisProtocol
    {
        /// <summary>
        ///     规定所有的网络传输指令头都为32字节
        /// </summary>
        internal const int HeadByteLength = 32;

        /// <summary>
        ///     所有网络通信中的缓冲池数据信息
        /// </summary>
        internal const int ProtocolBufferSize = 1024;


        /// <summary>
        ///     用于心跳程序的暗号信息
        /// </summary>
        internal const int ProtocolCheckSecends = 1;

        /// <summary>
        ///     客户端退出消息
        /// </summary>
        internal const int ProtocolClientQuit = 2;

        /// <summary>
        ///     因为客户端达到上限而拒绝登录
        /// </summary>
        internal const int ProtocolClientRefuseLogin = 3;

        /// <summary>
        ///     允许客户端登录到服务器
        /// </summary>
        internal const int ProtocolClientAllowLogin = 4;


        /// <summary>
        ///     说明发送的只是文本信息
        /// </summary>
        internal const int ProtocolUserString = 1001;

        /// <summary>
        ///     发送的数据就是普通的字节数组
        /// </summary>
        internal const int ProtocolUserBytes = 1002;

        /// <summary>
        ///     发送的数据就是普通的图片数据
        /// </summary>
        internal const int ProtocolUserBitmap = 1003;

        /// <summary>
        ///     发送的数据是一条异常的数据，字符串为异常消息
        /// </summary>
        internal const int ProtocolUserException = 1004;


        /// <summary>
        ///     请求文件下载的暗号
        /// </summary>
        internal const int ProtocolFileDownload = 2001;

        /// <summary>
        ///     请求文件上传的暗号
        /// </summary>
        internal const int ProtocolFileUpload = 2002;

        /// <summary>
        ///     请求删除文件的暗号
        /// </summary>
        internal const int ProtocolFileDelete = 2003;

        /// <summary>
        ///     文件校验成功
        /// </summary>
        internal const int ProtocolFileCheckRight = 2004;

        /// <summary>
        ///     文件校验失败
        /// </summary>
        internal const int ProtocolFileCheckError = 2005;

        /// <summary>
        ///     文件保存失败
        /// </summary>
        internal const int ProtocolFileSaveError = 2006;

        /// <summary>
        ///     请求文件列表的暗号
        /// </summary>
        internal const int ProtocolFileDirectoryFiles = 2007;

        /// <summary>
        ///     请求子文件的列表暗号
        /// </summary>
        internal const int ProtocolFileDirectories = 2008;

        /// <summary>
        ///     进度返回暗号
        /// </summary>
        internal const int ProtocolProgressReport = 2009;


        /// <summary>
        ///     不压缩数据字节
        /// </summary>
        internal const int ProtocolNoZipped = 3001;

        /// <summary>
        ///     压缩数据字节
        /// </summary>
        internal const int ProtocolZipped = 3002;


        /// <summary>
        ///     生成终极传送指令的方法，所有的数据均通过该方法出来
        /// </summary>
        /// <param name="command">命令头</param>
        /// <param name="customer">自用自定义</param>
        /// <param name="token">令牌</param>
        /// <param name="data">字节数据</param>
        /// <returns>包装后的数据信息</returns>
        internal static byte[] CommandBytes(int command, int customer, Guid token, byte[] data)
        {
            byte[] temp = null;
            var zipped = ProtocolNoZipped;
            var sendLength = 0;
            if (data == null)
            {
                temp = new byte[HeadByteLength];
            }
            else
            {
                // 加密
                data = NjisSecurity.ByteEncrypt(data);
                if (data.Length > 102400)
                {
                    // 100K以上的数据，进行数据压缩
                    data = SoftZipped.CompressBytes(data);
                    zipped = ProtocolZipped;
                }

                temp = new byte[HeadByteLength + data.Length];
                sendLength = data.Length;
            }

            BitConverter.GetBytes(command).CopyTo(temp, 0);
            BitConverter.GetBytes(customer).CopyTo(temp, 4);
            BitConverter.GetBytes(zipped).CopyTo(temp, 8);
            token.ToByteArray().CopyTo(temp, 12);
            BitConverter.GetBytes(sendLength).CopyTo(temp, 28);
            if (sendLength > 0)
            {
                Array.Copy(data, 0, temp, 32, sendLength);
            }

            return temp;
        }


        /// <summary>
        ///     解析接收到数据，先解压缩后进行解密
        /// </summary>
        /// <param name="head">指令头</param>
        /// <param name="content">指令的内容</param>
        /// <return>真实的数据内容</return>
        internal static byte[] CommandAnalysis(byte[] head, byte[] content)
        {
            if (content != null)
            {
                var zipped = BitConverter.ToInt32(head, 8);
                // 先进行解压
                if (zipped == ProtocolZipped)
                {
                    content = SoftZipped.Decompress(content);
                }

                // 进行解密
                return NjisSecurity.ByteDecrypt(content);
            }

            return null;
        }


        /// <summary>
        ///     获取发送字节数据的实际数据，带指令头
        /// </summary>
        /// <param name="customer">用户数据</param>
        /// <param name="token">令牌</param>
        /// <param name="data">字节信息</param>
        /// <returns>包装后的指令信息</returns>
        internal static byte[] CommandBytes(int customer, Guid token, byte[] data)
        {
            return CommandBytes(ProtocolUserBytes, customer, token, data);
        }


        /// <summary>
        ///     获取发送字节数据的实际数据，带指令头
        /// </summary>
        /// <param name="customer">用户数据</param>
        /// <param name="token">令牌</param>
        /// <param name="data">字符串数据信息</param>
        /// <returns>包装后的指令信息</returns>
        internal static byte[] CommandBytes(int customer, Guid token, string data)
        {
            if (data == null) return CommandBytes(ProtocolUserString, customer, token, null);
            return CommandBytes(ProtocolUserString, customer, token, Encoding.Unicode.GetBytes(data));
        }
    }
}