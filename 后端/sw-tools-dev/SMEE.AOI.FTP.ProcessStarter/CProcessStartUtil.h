#pragma once

#include "DataModels.h"

class CProcessStartUtil
{
public:
	static void StartProcess(const ProcessInitParam& initParam);
    static void ExecuteCreateProcess(const ProcessInitParam& initParam);
	static void ExecuteCreateProcessWithToken(const ProcessInitParam& initParam);
	static void ExecuteShellExecute(const ProcessInitParam& initParam);
	static void ExecuteCreateProcessAsUser(const ProcessInitParam& initParam);
	static void ExecuteShellExecuteEx(const ProcessInitParam& initParam);
	static void DefaultExecute(const ProcessInitParam& initParam);
private:
	static void LogShellExecuteError(int iInstance);
	static void LogCoInitializeError(HRESULT hResult);
	static void CopyString(wchar_t* const target,
		int targetLen, const std::wstring& source);
	static void PrepareParams(STARTUPINFO& si, PROCESS_INFORMATION& pi);
};

