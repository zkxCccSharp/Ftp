#include "stdafx.h"
#include "CArgsParser.h"
#include "CLogUtil.h"

/*
* 传入的参数，分别是
* 待启动Exe文件全名、
* 用户名、
* 密码、
* 用户所在域、
* 启动进程方式
* (暂不支持命令行参数)
*/

// 如果有5个参数，则视为顺序输入了每一个参数
// 如果有6个参数，则忽略第一个参数
// 如果参数个数不是5或者6，报错

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
