#include "stdafx.h"
#include "CProcessStartUtil.h"
#include "CLogUtil.h"

#define NEW_PROCESS_DEFAULT 0
#define NEW_PROCESS_CREATEPROCESS 1
#define NEW_PROCESS_SHELLEXECUTE 2
#define NEW_PROCESS_SHELLEXECUTEEX 3
#define NEW_PROCESS_CREATEPROCESSASUSER 4

void CProcessStartUtil::StartProcess(const ProcessInitParam& initParam)
{
	switch (initParam.startType)
	{
	case NEW_PROCESS_CREATEPROCESS:
		ExecuteCreateProcess(initParam);
		break;
	case NEW_PROCESS_SHELLEXECUTE:
		ExecuteShellExecute(initParam);
		break;
	case NEW_PROCESS_SHELLEXECUTEEX:
		ExecuteShellExecuteEx(initParam);
		break;
	case NEW_PROCESS_CREATEPROCESSASUSER:
		ExecuteCreateProcessAsUser(initParam);
		break;
	default:
		DefaultExecute(initParam);
		break;
	}
}

void CProcessStartUtil::ExecuteShellExecute(const ProcessInitParam& initParam)
{
	HRESULT hResult = ::CoInitializeEx(
		NULL,
		COINIT_APARTMENTTHREADED | COINIT_DISABLE_OLE1DDE);
	LogCoInitializeError(hResult);
	HINSTANCE hInstance = ::ShellExecuteW(
		NULL,
		L"open",
		initParam.exeFileName.c_str(),
		NULL,
		NULL,
		0);
	int iInstance = (int)hInstance;
	if (iInstance > 32) return;
	LogShellExecuteError(iInstance);
}

void CProcessStartUtil::ExecuteCreateProcess(const ProcessInitParam& initParam)
{
	wchar_t strCommandLine[MAX_PATH];
	CopyString(strCommandLine, MAX_PATH, initParam.exeFileName);
	STARTUPINFO si; PROCESS_INFORMATION pi;
	PrepareParams(si, pi);
	BOOL bResult = ::CreateProcessW(
		NULL,
		strCommandLine,
		NULL,
		NULL,
		FALSE,
		NORMAL_PRIORITY_CLASS,
		NULL,
		NULL,
		&si,
		&pi);
	if (bResult != TRUE)
	{
		CLogUtil::PrintAPIError(L"CreateProcessW", ::GetLastError());
	}
	::CloseHandle(pi.hProcess);
	::CloseHandle(pi.hThread);
}

void CProcessStartUtil::ExecuteCreateProcessWithToken(const ProcessInitParam& initParam)
{
	HANDLE hToken;
	BOOL bResult = ::LogonUserW(
		initParam.user.c_str(),
		initParam.domain.c_str(),
		initParam.password.c_str(),
		LOGON32_LOGON_INTERACTIVE,
		LOGON32_PROVIDER_DEFAULT,
		&hToken);
	if (bResult == TRUE)
	{
		wchar_t strCommandLine[MAX_PATH];
		CopyString(strCommandLine, MAX_PATH, initParam.exeFileName);
		STARTUPINFO si; PROCESS_INFORMATION pi;
		PrepareParams(si, pi);
		bResult = ::CreateProcessWithTokenW(
			hToken,
			LOGON_WITH_PROFILE,
			NULL,
			strCommandLine,
			NORMAL_PRIORITY_CLASS | CREATE_UNICODE_ENVIRONMENT,
			NULL,
			NULL,
			&si,
			&pi);
		if (bResult != TRUE)
		{
			CLogUtil::PrintAPIError(L"CreateProcessWithTokenW", ::GetLastError());
		}
		::CloseHandle(pi.hProcess);
		::CloseHandle(pi.hThread);
	}
	else
	{
		CLogUtil::PrintAPIError(L"LogonUserW", ::GetLastError());
	}
	::CloseHandle(hToken);
}

void CProcessStartUtil::ExecuteCreateProcessAsUser(const ProcessInitParam& initParam)
{
	HANDLE hToken;
	BOOL bResult = ::LogonUserW(
		initParam.user.c_str(),
		initParam.domain.c_str(),
		initParam.password.c_str(),
		LOGON32_LOGON_INTERACTIVE,
		LOGON32_PROVIDER_DEFAULT,
		&hToken);
	if (bResult == TRUE)
	{
		wchar_t strCommandLine[MAX_PATH];
		CopyString(strCommandLine, MAX_PATH, initParam.exeFileName);
		STARTUPINFO si; PROCESS_INFORMATION pi;
		PrepareParams(si, pi);
		bResult = ::CreateProcessAsUserW(
			hToken,
			NULL,
			strCommandLine,
			NULL,
			NULL,
			FALSE,
			NORMAL_PRIORITY_CLASS | CREATE_UNICODE_ENVIRONMENT,
			NULL,
			NULL,
			&si,
			&pi);
		if (bResult != TRUE)
		{
			CLogUtil::PrintAPIError(L"CreateProcessAsUserW", ::GetLastError());
		}
		::CloseHandle(pi.hProcess);
		::CloseHandle(pi.hThread);
	}
	else
	{
		CLogUtil::PrintAPIError(L"LogonUserW", ::GetLastError());
	}
	::CloseHandle(hToken);
}

void CProcessStartUtil::ExecuteShellExecuteEx(const ProcessInitParam& initParam)
{
	SHELLEXECUTEINFOW sei;
	ZeroMemory(&sei, sizeof(sei));
	sei.cbSize = sizeof(sei);
	sei.lpVerb = L"runas";
	sei.lpFile = initParam.exeFileName.c_str();
	BOOL bResult = ::ShellExecuteExW(&sei);
	if (bResult != TRUE)
	{
		CLogUtil::PrintAPIError(L"ShellExecuteExW", ::GetLastError());
	}
	::CloseHandle(sei.hProcess);
}

void CProcessStartUtil::DefaultExecute(const ProcessInitParam& initParam)
{
	ExecuteShellExecuteEx(initParam);
}

void CProcessStartUtil::LogCoInitializeError(HRESULT hResult)
{
	switch (hResult)
	{
	case E_INVALIDARG:
		CLogUtil::PrintError(L"E_INVALIDARG!");
		break;
	case E_OUTOFMEMORY:
		CLogUtil::PrintError(L"E_OUTOFMEMORY!");
		break;
	case E_UNEXPECTED:
		CLogUtil::PrintError(L"E_UNEXPECTED!");
		break;
	case S_OK:
	case S_FALSE:
	case RPC_E_CHANGED_MODE:
		break;
	default:
		CLogUtil::PrintError(L"Undefined CoInitialize Return ERROR!");
		break;
	}
}

void CProcessStartUtil::LogShellExecuteError(int iInstance)
{
	switch (iInstance)
	{
	case 0:
		CLogUtil::PrintError(L"OS is Out Of Memary!");
		break;
	case ERROR_FILE_NOT_FOUND:
		CLogUtil::PrintError(L"ERROR_FILE_NOT_FOUND!");
		break;
	case ERROR_PATH_NOT_FOUND:
		CLogUtil::PrintError(L"ERROR_PATH_NOT_FOUND!");
		break;
	case ERROR_BAD_FORMAT:
		CLogUtil::PrintError(L"ERROR_BAD_FORMAT! exe File Invalid!");
		break;
	case SE_ERR_ACCESSDENIED:
		CLogUtil::PrintError(L"SE_ERR_ACCESSDENIED!");
		break;
	case SE_ERR_ASSOCINCOMPLETE:
		CLogUtil::PrintError(L"SE_ERR_ASSOCINCOMPLETE! File NOT Completed!");
		break;
	case SE_ERR_DDEBUSY:
		CLogUtil::PrintError(L"SE_ERR_DDEBUSY!");
		break;
	case SE_ERR_DDEFAIL:
		CLogUtil::PrintError(L"SE_ERR_DDEFAIL!");
		break;
	case SE_ERR_DDETIMEOUT:
		CLogUtil::PrintError(L"SE_ERR_DDETIMEOUT!");
		break;
	case SE_ERR_DLLNOTFOUND:
		CLogUtil::PrintError(L"SE_ERR_DLLNOTFOUND!");
		break;
	case SE_ERR_NOASSOC:
		CLogUtil::PrintError(L"SE_ERR_NOASSOC!");
		break;
	case SE_ERR_OOM:
		CLogUtil::PrintError(L"SE_ERR_OOM!");
		break;
	case SE_ERR_SHARE:
		CLogUtil::PrintError(L"SE_ERR_SHARE!");
		break;
	default:
		CLogUtil::PrintError(L"Undefined ShellExecute Return ERROR!");
		break;
	}
}

void CProcessStartUtil::CopyString(wchar_t* const target,
	int targetLen, const std::wstring& source)
{
	::memset(target, 0, targetLen * sizeof(wchar_t));
	::wcscpy_s(target, targetLen - 1, source.c_str());
}

void CProcessStartUtil::PrepareParams(STARTUPINFO& si, PROCESS_INFORMATION& pi)
{
	ZeroMemory(&si, sizeof(si));
	ZeroMemory(&pi, sizeof(pi));
	si.cb = sizeof(si);
}
