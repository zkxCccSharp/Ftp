// SMEE.AOI.FTP.ProcessStarter.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "CLogUtil.h"
#include "DataModels.h"
#include "CArgsParser.h"
#include "CProcessStartUtil.h"

/*
* ����Ĳ������ֱ���
* ������Exe�ļ�ȫ����
* �û�����
* ���롢
* �û�������
* �������̷�ʽ
* (�ݲ�֧�������в���)
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

