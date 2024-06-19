#include "stdafx.h"
#include "CArgsParser.h"
#include "CLogUtil.h"

/*
* ����Ĳ������ֱ���
* ������Exe�ļ�ȫ����
* �û�����
* ���롢
* �û�������
* �������̷�ʽ
* (�ݲ�֧�������в���)
*/

// �����5������������Ϊ˳��������ÿһ������
// �����6������������Ե�һ������
// ���������������5����6������

bool CArgsParser::TryParseArgs(int count, TCHAR* args[], ProcessInitParam& param)
{
	try
	{
		if (count == 5)
		{
			param.exeFileName = args[0];
			param.user = args[1];
			param.password = args[2];
			param.domain = args[3];
			param.startType = std::stoi(args[4]);
			return true;
		}
		else if (count == 6)
		{
			param.exeFileName = args[1];
			param.user = args[2];
			param.password = args[3];
			param.domain = args[4];
			param.startType = std::stoi(args[5]);
			return true;
		}
		else
		{
			CLogUtil::PrintError(L"args Count Error!");
			return false;
		}
	}
	catch (const std::exception& ex)
	{
		CLogUtil::PrintError(L"args Convert Error!");
		CLogUtil::PrintError(ex.what());
		return false;
	}
}
