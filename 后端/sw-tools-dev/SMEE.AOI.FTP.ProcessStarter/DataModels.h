#pragma once

#include <string>

class ProcessInitParam
{
public:
	ProcessInitParam();
	~ProcessInitParam();
public:
	std::wstring exeFileName;
	std::wstring user;
	std::wstring password;
	std::wstring domain;
	int startType;
};
