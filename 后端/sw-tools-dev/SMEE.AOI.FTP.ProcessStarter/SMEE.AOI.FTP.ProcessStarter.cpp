// SMEE.AOI.FTP.ProcessStarter.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "CLogUtil.h"
#include "DataModels.h"
#include "CArgsParser.h"
#include "CProcessStartUtil.h"

/*
* 传入的参数，分别是
* 待启动Exe文件全名、
* 用户名、
* 密码、
* 用户所在域、
* 启动进程方式
* (暂不支持命令行参数)
*/
void _tmain(int argCount, TCHAR* aver[])
{
    int nArgCount = 0;
    PWSTR* ppArg = ::CommandLineToArgvW(GetCommandLineW(), &nArgCount);
    CLogUtil::PrintInputArgs(nArgCount, ppArg);
    ProcessInitParam initParam;
    if (!CArgsParser::TryParseArgs(nArgCount, ppArg, initParam))
    {
        CLogUtil::PrintError(L"Parse Input Args Failed!");
    }
    else
    {
        CProcessStartUtil::StartProcess(initParam);
    }
    ::HeapFree(::GetProcessHeap(), 0, ppArg);
}

