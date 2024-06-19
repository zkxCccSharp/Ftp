#include "stdafx.h"
#include "CLogUtil.h"

void CLogUtil::PrintInputArgs(int count, TCHAR* args[])
{
	// Console 准备
	std::locale loc("chs");
	std::wcout.imbue(loc);
	// 文件 准备
	std::wofstream ofs;
	ofs.open("ProcessStarter.log", std::ios::trunc);
	WriteALine(ofs, L"args:");
	for (int i = 0; i < count; ++i)
	{
		WriteALine(ofs, args[i]);
	}
	WriteALine(ofs, L"args End.");
	ofs.close();
}

void CLogUtil::PrintAPIError(const std::wstring& funcName, DWORD errCode)
{
	std::wstring msg;
	msg = funcName + L" Return [" + std::to_wstring(errCode) + L"]!";
	PrintError(msg);
}

void CLogUtil::PrintError(const std::string& errMsg)
{
	std::wstring wmsg;
	ConvertToWString(errMsg, wmsg);
	PrintError(wmsg);
}

void CLogUtil::PrintError(const std::wstring& errMsg)
{
	// Console 准备
	std::locale loc("chs");
	std::wcout.imbue(loc);
	// 文件 准备
	std::wofstream ofs;
	ofs.open("ProcessStarter.log", std::ios::app);
	WriteALine(ofs, errMsg);
	ofs.close();
}

void CLogUtil::ConvertToWString(const std::string& s, std::wstring& outS)
{
	int len = ::MultiByteToWideChar(CP_ACP, 0, s.c_str(), s.size(), NULL, 0) + 1;
	wchar_t* buf = new wchar_t[len];
	::MultiByteToWideChar(CP_ACP, 0, s.c_str(), s.size(), buf, len - 1);
	outS = buf;
	delete[] buf;
}

void CLogUtil::WriteALine(std::wofstream& ofs, const std::wstring& msg)
{
	std::wcout << msg << std::endl;
	ofs << msg << std::endl;
}
