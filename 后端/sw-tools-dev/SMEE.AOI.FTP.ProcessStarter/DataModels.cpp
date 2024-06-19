#include "stdafx.h"
#include "DataModels.h"

ProcessInitParam::ProcessInitParam() : exeFileName(), user(), password(), domain()
{
	startType = 0;
}

ProcessInitParam::~ProcessInitParam() { }
