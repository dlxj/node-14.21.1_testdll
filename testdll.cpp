
#include <iostream>

#include "windows.h"

int main()
{
    HINSTANCE   ghDLL = NULL;
    ghDLL = LoadLibrary("D:\\GitHub\\node-14.21.1\\out\\Debug\\node.dll");

    typedef int (_cdecl* FunctionPtr) (int argc, wchar_t* wargv[]);

    FunctionPtr wmain;

    wmain = (FunctionPtr)GetProcAddress(ghDLL, "wmain");

    int argc = 2;

    wchar_t* wargv[] = {
      (wchar_t*)L"C:\\projects\\edge-js\\tools\\build\\node-14.21.1\\out\\Debug\\node2.exe",
      //(wchar_t*)L"C:\\projects\\edge-js\\tools\\build\\node-14.21.1\\out\\Debug\\pmserver\\server.js",
      (wchar_t*)L"D:\\GitHub\\echodict\\pmserver\\server.js"
    };

    wmain(argc, wargv);

    std::cout << "Hello World!\n";
}
