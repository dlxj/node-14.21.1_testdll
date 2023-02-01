using System.Runtime.InteropServices;

using System.Threading;
using RestSharp;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;

public static class JObjectExtensions
{
    public static IDictionary<string, object> ToDictionary(this JObject @object)
    {
        var result = @object.ToObject<Dictionary<string, object>>();

        var JObjectKeys = (from r in result
                           let key = r.Key
                           let value = r.Value
                           where value.GetType() == typeof(JObject)
                           select key).ToList();

        var JArrayKeys = (from r in result
                          let key = r.Key
                          let value = r.Value
                          where value.GetType() == typeof(JArray)
                          select key).ToList();

        JArrayKeys.ForEach(key => result[key] = ((JArray)result[key]).Values().Select(x => ((JValue)x).Value).ToArray());
        JObjectKeys.ForEach(key => result[key] = ToDictionary(result[key] as JObject));

        return result;
    }
}

public class NodeVM
{
    // 定义给 C# 的传参，全部参数都定义在这个结构数组里
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct wmain_params
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public string[] wargv;  // 大小固定为 2 个元素
    }
    [DllImport("D:\\GitHub\\node-14.21.1\\out\\Debug\\node.dll", EntryPoint = "wmain", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern int wmain(int argc, ref wmain_params wargv);

    private static bool isInited = false;

    public static void init()  // 调用 node.dll ，启动 server.js ，上面有一个运行任意 nodejs 代码的 vm  
    {
        if (!isInited)
        {

            ThreadPool.QueueUserWorkItem(s =>
            {
                wmain_params pms = new wmain_params();
                pms.wargv = new string[] {
                    "C:\\projects\\edge-js\\tools\\build\\node-14.21.1\\out\\Debug\\node2.exe",
                    //"C:\\projects\\edge-js\\tools\\build\\node-14.21.1\\out\\Debug\\pmserver\\server.js",
                    "D:\\GitHub\\echodict\\pmserver\\server.js"
                    };

                wmain(2, ref pms);

            });

            isInited = true;

            Thread.Sleep(3000);
        }
    }

    /*
            string code = @"
                console.log(fs)  // fs 是事先 import 好的模块，这里可以直接用  所有可用参数都在这里展开了：  ...params
                console.log(franc)
                console.log('hello, from vm')
                return callback({ msg:'hi,,,' }) // 约定最后以 callback 返回值
            ";

            string imports = @"
                [""fs"", {""franc"":[""franc"", ""francAll""]}  ]
            ";
            // fs = import('fs')
            // let { franc, francAll } = import('franc')

            JToken ret = NodeVM.runvm(code, imports);
            string msg = ret["msg"].ToString();
     */
    public static JToken runvm(string code, string imports)
    {
        var url = "http://127.0.0.1:8880/vm/vmrun";
        var client = new RestClient(url);
        var request = new RestRequest();
        request.Method = Method.Post;
        request.Timeout = 5000;
        request.AddHeader("content-type", "application/x-www-form-urlencoded;charset=UTF-8");
        request.AddParameter("application/x-www-form-urlencoded", $"code={WebUtility.UrlEncode(code)}&imports={WebUtility.UrlEncode(imports)}", ParameterType.RequestBody);

        var response = client.Execute(request);
        string jsonstr = response.Content;


        JObject obj = JObject.Parse(jsonstr);
        int status = obj.Value<int>("status");
        if (status != 200)
        {
            throw new Exception($"接口请求失败: {url}");
        }

        //var m = obj["data"]["msg"].ToString();

        //Dictionary<string, object> d = (Dictionary<string, object>)obj.ToDictionary();

        //var data = obj.Value("data");

        var data = obj["data"];

        return data;

    }

}

/*
 
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            NodeVM.init();  // 只需要初始化一次 // 如果需要在 pmserver 里调用，注释掉这一行，就会调用 pmserver 的接口 

            string code = @"
                console.log(fs)  // fs 是事先 import 好的模块，这里可以直接用  所有可用参数都在这里展开了：  ...params
                console.log(franc)
                console.log('hello, from vm')
                return callback({ msg:'hi,,,' }) // 约定最后以 callback 返回值
            ";

            string imports = @"
                [""fs"", {""franc"":[""franc"", ""francAll""]}  ]
            ";
            // fs = import('fs')
            // let { franc, francAll } = import('franc')

            JToken ret = NodeVM.runvm(code, imports);
            string msg = ret["msg"].ToString();
            
        }
    }
}
 
 */
