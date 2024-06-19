#pragma once

#include "DataModels.h"

class CArgsParser
{
public:
	static bool TryParseArgs(int count, TCHAR* args[], ProcessInitParam& param);
};

