#pragma once

#include <string>
#include <fstream>

class CLogUtil
{
public:
	static void PrintInputArgs(int count, TCHAR* args[]);
	static void PrintError(const std::wstring& errMsg);
	static void PrintAPIError(const std::wstring& funcName, DWORD errCode);
	static void PrintError(const std::string& errMsg);
private:
	static void WriteALine(std::wofstream& ofs, const std::wstring& msg);
	static void ConvertToWString(const std::string& s, std::wstring& outS);
};

