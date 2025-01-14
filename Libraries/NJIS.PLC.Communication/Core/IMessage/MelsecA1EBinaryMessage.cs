﻿//  ************************************************************************************
//   解决方案：NJIS.FPZWS.LineControl.Drilling
//   项目名称：NJIS.PLC.Communication
//   文 件 名：MelsecA1EBinaryMessage.cs
//   创建时间：2018-11-08 16:16
//   作    者：
//   说    明：
//   修改时间：2018-11-08 16:16
//   修 改 人：
//  Copyright © 2017 广州宁基智能系统有限公司. 版权所有
//  *************************************************************************************

namespace NJIS.PLC.Communication.Core.IMessage
{
    /// <summary>
    ///     三菱的A兼容1E帧协议解析规则
    /// </summary>
    public class MelsecA1EBinaryMessage : INetMessage
    {
        /// <summary>
        ///     消息头的指令长度
        /// </summary>
        public int ProtocolHeadBytesLength => 2;


        /// <summary>
        ///     从当前的头子节文件中提取出接下来需要接收的数据长度
        /// </summary>
        /// <returns>返回接下来的数据内容长度</returns>
        public int GetContentLengthByHeadBytes()
        {
            var contentLength = 0;

            if (HeadBytes[1] == 0x5B)
            {
                contentLength = 2; //结束代码 + 0x00
                return contentLength;
            }

            var length = SendBytes[10] % 2 == 0 ? SendBytes[10] : SendBytes[10] + 1;
            switch (HeadBytes[0])
            {
                case 0x80: //位单位成批读出后，回复副标题
                    contentLength = length / 2;
                    break;
                case 0x81: //字单位成批读出后，回复副标题
                    contentLength = SendBytes[10] * 2;
                    break;
                case 0x82: //位单位成批写入后，回复副标题
                    break;
                case 0x83: //字单位成批写入后，回复副标题
                    break;
            }

            //在A兼容1E协议中，写入值后，若不发生异常，只返回副标题 + 结束代码(0x00)
            //这已经在协议头部读取过了，后面要读取的长度为0（contentLength=0）
            return contentLength;
        }


        /// <summary>
        ///     检查头子节的合法性
        /// </summary>
        /// <param name="token">特殊的令牌，有些特殊消息的验证</param>
        /// <returns></returns>
        public bool CheckHeadBytesLegal(byte[] token)
        {
            if (HeadBytes != null)
            {
                if (HeadBytes[0] - SendBytes[0] == 0x80)
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        ///     获取头子节里的消息标识
        /// </summary>
        /// <returns></returns>
        public int GetHeadBytesIdentity()
        {
            return 0;
        }


        /// <summary>
        ///     消息头字节
        /// </summary>
        public byte[] HeadBytes { get; set; }


        /// <summary>
        ///     消息内容字节
        /// </summary>
        public byte[] ContentBytes { get; set; }


        /// <summary>
        ///     发送的字节信息
        /// </summary>
        public byte[] SendBytes { get; set; }
    }
}