//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace NJIS.FPZWS.LineControl.Cutting.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class CuttingFeedBack
    {
        public long LineId { get; set; }
        public string PartId { get; set; }
        public string BatchName { get; set; }
        public string DeviceName { get; set; }
        public long DevicePPCD_ID { get; set; }
        public System.DateTime CreatedTime { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }
        public float Thickness { get; set; }
    }
}