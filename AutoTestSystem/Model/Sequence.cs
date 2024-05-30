using System;
using System.Collections.Generic;
using System.Threading;

namespace AutoTestSystem.Model
{
    [Serializable]
    public class Sequence
    {
        public string SeqName = null;
        public bool IsTest = true;                                //!是否测试
        public bool IsTestFinished = false;                       //!测试完成标志
        public bool TestResult = true;                            //!测试结果
        public int TotalNumber = 0;                               //!测试大项item总数量
        public int TestSerial = 0;                                //!测试大项在所有中的序列号
        public string TestVersion = null;                         //!测试程序版本
        public string SystemName = null;                          //!测试系统名称 SystemName
        public List<Items> SeqItems = new List<Items>();
        public string start_time = null;
        public string finish_time = null;
        
        public void Clear()
        {
            //IsTest = true;
            IsTestFinished = false;
            TestResult = true;
            start_time = null;
            finish_time = null;
        }
    }

    [Serializable]
    public class Items
    {
        public int testNumber = 0;                                //!当前测试项序列号
        public bool tResult = true;                               //!测试项测试结果
        public bool isTest = true;                                //!是否测试,不测试的跳过
        public int startIndex = 0;                                //!需要执行的step index
        public DateTime start_time_json = new DateTime();               //!测试项的开始时间
        public DateTime start_time = new DateTime();
        //public DateTime EndTime = new DateTime();        //!测试项的结束时间

        public string ItemName = null;   //当前测试step名字
        public string ErrorCode = null;  //测试错误码
        public string RetryTimes = null; //测试失败retry次数
        public string TimeOut = null;    //测试步骤超时时间
        public string SubStr1 = null;    //截取字符串 如截取abc中的b SubStr1=a，SubStr2=c
        public string SubStr2 = null;
        public string IfElse = null;     //测试步骤结果是否做为if条件，决定else步骤是否执行
        public string For = null;        //循环测试for(6)开始6次循环，ENDFOR结束
        public string Mode = null;       //机种，根据机种决定哪些用例不跑，哪些用例需要跑
        public string ComdOrParam = null;   //发送的测试命令
        public string ExpectStr = null;  //期待的提示符，用来判断反馈是不是结束了
        public string CheckStr1 = null;  //检查反馈是否包含CheckStr1
        public string CheckStr2 = null;  //检查反馈是否包含CheckStr2
        public string Limit_max = null;  //最小限值
        public string testValue = null;  //测试得到的值
        public string Limit_min = null;  //最大限值
        public string ElapsedTime = null; //测试步骤耗时
        public string ErrorDetails = null; //测试错误码详细描述
        public string Unit = null;       //测试值单位
        public string MES_var = null;    //上传MES信息的变量名字
        public string ByPassFail = null;     //手动人为控制测试结果 1=pass，0||空=fail
        public string FTC = null;        //失败继续 fail to continue。1=继续，0||空=不继续
        public string TestKeyword = null;  //测试步骤对应的关键字，执行对应关键字下的代码段         
        public string Spec = null;        //测试定义的Spec值
        public string error_code = "";
        public string Json = null;
        public string EeroName;
        public string NoContain;
        public string SpecStatic = null;
        public void Clear()
        {
            tResult = true;
            isTest = true;
            startIndex = 0;
            testValue = null;
            ElapsedTime = null;
            start_time_json = new DateTime();
            start_time = new DateTime();
        }


        public void CopyFrom(Items model) {


            this.testNumber = model.testNumber;                                //!当前测试项序列号
            this.tResult = model.tResult;                               //!测试项测试结果
            this.isTest = model.isTest;                                //!是否测试,不测试的跳过
            this.startIndex = model.startIndex;                                //!需要执行的step index
            this.start_time_json = model.start_time_json;               //!测试项的开始时间
            this.start_time = model.start_time;

            this.ItemName = model.ItemName;   //当前测试step名字
            this.ErrorCode = model.ErrorCode;  //测试错误码
            this.RetryTimes = model.RetryTimes; //测试失败retry次数
            this.TimeOut = model.TimeOut;    //测试步骤超时时间
            this.SubStr1 = model.SubStr1;    //截取字符串 如截取abc中的b SubStr1=a，SubStr2=c
            this.SubStr2 = model.SubStr2;
            this.IfElse = model.IfElse;     //测试步骤结果是否做为if条件，决定else步骤是否执行
            this.For = model.For;        //循环测试for(6)开始6次循环，ENDFOR结束
            this.Mode = model.Mode;       //机种，根据机种决定哪些用例不跑，哪些用例需要跑
            this.ComdOrParam = model.ComdOrParam;   //发送的测试命令
            this.ExpectStr = model.ExpectStr;  //期待的提示符，用来判断反馈是不是结束了
            this.CheckStr1 = model.CheckStr1;  //检查反馈是否包含CheckStr1
            this.CheckStr2 = model.CheckStr2;  //检查反馈是否包含CheckStr2
            this.Limit_max = model.Limit_max;  //最小限值
            this.testValue = model.testValue;  //测试得到的值
            this.Limit_min = model.Limit_min;  //最大限值
            this.ElapsedTime = model.ElapsedTime; //测试步骤耗时
            this.ErrorDetails = model.ErrorDetails; //测试错误码详细描述
            this.Unit = model.Unit;       //测试值单位
            this.MES_var = model.MES_var;    //上传MES信息的变量名字
            this.ByPassFail = model.ByPassFail;     //手动人为控制测试结果 1=pass，0||空=fail
            this.FTC = model.FTC;        //失败继续 fail to continue。1=继续，0||空=不继续
            this.TestKeyword = model.TestKeyword;  //测试步骤对应的关键字，执行对应关键字下的代码段         
            this.Spec = model.Spec;        //测试定义的Spec值
            this.error_code = model.error_code;
            this.Json = model.Json;
            this.EeroName = model.EeroName;
            this.NoContain = model.NoContain;
            this.SpecStatic = model.SpecStatic;


    }







    }

    public class MesInfo
    {
        public string serial;
        public string Fix_Num;
        public string start_time;
        public string finish_time;
        public string test_time;
        public string status;
        public string error_code;
    }
}