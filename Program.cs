
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
